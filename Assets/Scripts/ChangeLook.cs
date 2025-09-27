using System;
using Unity.Cinemachine;
using UnityEngine;

public class ChangeLook:  MonoBehaviour
{
    [SerializeField] private CinemachineRotationComposer  rotationComposer;

    private void OnEnable()
    {
        rotationComposer.TargetOffset = new Vector3(0f, 2f, 0f);
    }

    private void OnDisable()
    {
        rotationComposer.TargetOffset = Vector3.zero;
    }
}