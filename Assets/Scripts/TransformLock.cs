using UnityEngine;

public class TransformLock : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    void Update()
    {
        transform.position = target.position;
    }
}
