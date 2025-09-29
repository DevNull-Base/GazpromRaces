using UnityEngine;
using UnityEngine.Splines;

public class NpcMover : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline;       
    [Range(0f, 1f)]
    public float normalizedPosition = 0; 

    [Header("Движение")]
    public float moveSpeed = 10f;       
    public float rotationSpeed = 5f;    
    public bool loop = true;            

    void Update()
    {
        if (spline == null)
            return;
        
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

        // Позиция на сплайне
        Vector3 targetPos = spline.EvaluatePosition(normalizedPosition);
        Vector3 tangent = ((Vector3)spline.EvaluateTangent(normalizedPosition)).normalized;
        
        transform.position = targetPos;
        
        if (tangent != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(tangent, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}