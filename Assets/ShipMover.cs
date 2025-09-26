using UnityEngine;
using UnityEngine.Splines;

public class ShipMover : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline;
    [Range(0f, 1f)]
    public float normalizedPosition = 0f;

    [Header("Движение")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public bool loop = true;

    [Header("Реализм")]
    public float bankAngle = 30f;       // максимальный угол крена
    public float bankSmooth = 2f;       // скорость выравнивания крена
    public float bankDeadZone = 0.05f;  // зона "прямо", где крен = 0
    public float bankMin = 1f;          // минимальный угол, ниже которого крен обнуляется

    private float currentBank = 0f;
    private float bankVelocity = 0f;    // для SmoothDamp

    void Update()
    {
        if (spline == null) return;

        // --- движение вдоль кривой ---
        float splineLength = spline.CalculateLength();
        float deltaT = (moveSpeed / splineLength) * Time.deltaTime;

        normalizedPosition += deltaT;
        if (loop)
        {
            if (normalizedPosition > 1f)
                normalizedPosition -= 1f;
        }
        else
        {
            normalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        Vector3 pos = spline.EvaluatePosition(normalizedPosition);
        Vector3 tangent = spline.EvaluateTangent(normalizedPosition);
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
}
