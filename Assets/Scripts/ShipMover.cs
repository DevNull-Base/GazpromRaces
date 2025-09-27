using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class ShipMover : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline; 
    public float NormalizedPosition {get; private set;}
    public event Action<float> OnProgressChanged;
    public event Action OnRaceEnding;

    [Header("Движение")]
    public float baseSpeed = 5f;
    public float rotationSpeed = 5f;
    public bool loop = true;

    [Header("Крен")]
    public float bankAngle = 30f;       // максимальный угол крена
    public float bankSmooth = 2f;       // скорость выравнивания крена
    public float bankDeadZone = 0.05f;  // зона "прямо", где крен = 0
    public float bankMin = 1f;          // минимальный угол, ниже которого крен обнуляется

    private float currentBank = 0f;
    private float bankVelocity = 0f;    // для SmoothDamp

    private float currentSpeed = 0f;
    private bool boosting = false;
    private bool isStaring = false;
    

    public void StartingRace()
    {
        NormalizedPosition = 0f;
        currentSpeed = baseSpeed;
        isStaring = true;
    }
    
    public void EndingRace()
    {
        isStaring = false;
        StopAllCoroutines();
        NormalizedPosition = 0f;
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        if (!isStaring) return;
        
        if (spline == null) return;

        // --- движение вдоль кривой ---
        float splineLength = spline.CalculateLength();
        float deltaT = (currentSpeed / splineLength) * Time.deltaTime;
        
        NormalizedPosition += deltaT;

        if (NormalizedPosition > 1f)
        {
            OnRaceEnding?.Invoke();
            EndingRace();
        }
        
        
        OnProgressChanged?.Invoke(NormalizedPosition);
        NormalizedPosition = Mathf.Clamp01(NormalizedPosition);

        Vector3 pos = spline.EvaluatePosition(NormalizedPosition);
        Vector3 tangent = spline.EvaluateTangent(NormalizedPosition);
        tangent.Normalize();

        transform.position = pos;

        // --- ориентация вдоль касательной ---
        if (tangent != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(tangent, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // --- расчет крена ---
        Vector3 localTangent = transform.InverseTransformDirection(tangent);
        float side = -localTangent.x; // влево/вправо
        float targetBank = 0f;

        if (Mathf.Abs(side) > bankDeadZone)
        {
            targetBank = Mathf.Clamp(side * bankAngle, -bankAngle, bankAngle);
        }

        // Плавное сглаживание крена
        currentBank = Mathf.SmoothDamp(currentBank, targetBank, ref bankVelocity, 1f / bankSmooth);

        // Убираем мелкие колебания
        if (Mathf.Abs(currentBank) < bankMin)
            currentBank = 0f;

        // Применяем крен
        transform.rotation *= Quaternion.Euler(0, 0, currentBank);
    }
    
    public void BoostToEnd(float multiplier)
    {
        StopAllCoroutines();
        currentSpeed = baseSpeed * multiplier;
    }
    
    public void Boost(float duration, float multiplier)
    {
        if (!boosting) StartCoroutine(BoostRoutine(duration, multiplier));
    }

    private IEnumerator BoostRoutine(float time, float multiplier)
    {
        boosting = true;
        currentSpeed = baseSpeed * multiplier;
        yield return new WaitForSeconds(time);
        currentSpeed = baseSpeed;
        boosting = false;
    }
}
