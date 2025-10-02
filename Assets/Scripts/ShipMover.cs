using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class ShipMover : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline;
    [SerializeField] private bool loop = true;
    
    public event Action<float> OnProgressChanged;
    public event Action OnRaceEnding;

    [Header("Движение")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float currentSpeed = 4f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 3f;
    private float targetSpeed = 0f;
    [SerializeField] private float defaultSpeed = 4f;

    [Header("Вращение")]
    [SerializeField] private float rotationSpeed = 3f;
    private Quaternion targetRotation;
    
    [Header("Крен")]
    [SerializeField] private float bankAngle = 30f;
    [SerializeField] private float bankDeadZone = 0.1f;
    [SerializeField] private float bankSmooth = 5f;
    [SerializeField] private float bankReturnSpeed = 2f;
    private float currentBank = 0f;
    private float bankVelocity = 0f;         // минимальный угол, ниже которого крен обнуляется

    [Header("Progress")]
    [SerializeField] [Range(0f, 1f)] private float normalizedPosition = 0f;
    public float NormalizedPosition 
    { 
        get => normalizedPosition; 
        set => normalizedPosition = Mathf.Clamp01(value); 
    }
    
    private bool boosting = false;
    private bool isStaring = false;

    public void StartingRace()
    {
        NormalizedPosition = 0f;
        maxSpeed = defaultSpeed;
        currentSpeed = 0f;
        boosting = false;
        isStaring = true;
    }
    
    public void EndingRace()
    {
        isStaring = false;
        StopAllCoroutines();
        NormalizedPosition = 0f;
        maxSpeed = defaultSpeed;
    }
    
     void Update()
    {
        if (spline == null) return;
        if (!isStaring) return;

        HandleMovement();
        HandleRotation();
        HandleBanking();
    }

    private void HandleMovement()
    {
        targetSpeed = isStaring ? maxSpeed : 0f;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 
            (currentSpeed < targetSpeed ? acceleration : deceleration) * Time.deltaTime);
        
        if (currentSpeed <= 0.01f) return;

        // Движение вдоль кривой с плавным ускорением
        float splineLength = spline.CalculateLength();
        float deltaT = (currentSpeed / splineLength) * Time.deltaTime;
        
        float previousPosition = NormalizedPosition;
        NormalizedPosition += deltaT;

        if (NormalizedPosition >= 1f)
        {
            NormalizedPosition = 1f;
            OnRaceEnding?.Invoke();
            return;
        }

        // Вызов события только при реальном изменении позиции
        if (!Mathf.Approximately(previousPosition, NormalizedPosition))
        {
            OnProgressChanged?.Invoke(NormalizedPosition);
        }

        // Плавное обновление позиции
        Vector3 targetPos = spline.EvaluatePosition(NormalizedPosition);
        transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (spline == null) return;

        Vector3 tangent = spline.EvaluateTangent(NormalizedPosition);
        if (tangent.sqrMagnitude > 0.001f)
        {
            tangent.Normalize();
            
            // Плавное определение целевого вращения
            targetRotation = Quaternion.LookRotation(tangent, Vector3.up);
            
            // Используем Slerp для плавного вращения
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleBanking()
    {
        if (spline == null) return;

        Vector3 tangent = spline.EvaluateTangent(NormalizedPosition);
        Vector3 localTangent = transform.InverseTransformDirection(tangent);
        
        float side = -localTangent.x;
        float targetBank = 0f;

        // Учет мертвой зоны и плавное определение целевого крена
        if (Mathf.Abs(side) > bankDeadZone)
        {
            targetBank = Mathf.Clamp(side * bankAngle, -bankAngle, bankAngle);
        }
        else
        {
            // Плавный возврат к нулю при маленьких значениях
            targetBank = 0f;
        }

        // Двойное сглаживание: SmoothDamp + дополнительный Lerp для большей плавности
        float smoothedTargetBank = Mathf.SmoothDamp(
            currentBank, 
            targetBank, 
            ref bankVelocity, 
            1f / bankSmooth
        );

        // Дополнительное сглаживание для возврата в нулевое положение
        if (Mathf.Abs(targetBank) < 1f)
        {
            smoothedTargetBank = Mathf.Lerp(
                currentBank, 
                targetBank, 
                bankReturnSpeed * Time.deltaTime
            );
        }

        currentBank = smoothedTargetBank;

        // Применяем крен к текущему вращению
        if (Mathf.Abs(currentBank) > 0.01f)
        {
            transform.rotation *= Quaternion.Euler(0, 0, currentBank * Time.deltaTime * 5f);
        }
    }

    
    public void BoostToEnd(float multiplier)
    {
        StopAllCoroutines();
        maxSpeed = defaultSpeed * multiplier;
        boosting = true;
    }
    
    public void Boost(BoostConfig config)
    {
        if (!boosting) StartCoroutine(BoostRoutine(config.duration, config.multiplier));
    }

    private IEnumerator BoostRoutine(float time, float multiplier)
    {
        boosting = true;
        maxSpeed = defaultSpeed * multiplier;
        yield return new WaitForSeconds(time);
        maxSpeed = defaultSpeed;
        boosting = false;
    }
}
