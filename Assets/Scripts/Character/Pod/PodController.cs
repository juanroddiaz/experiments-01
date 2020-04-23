using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodControllerData
{
    public Transform Owner;
}

public class PodController : MonoBehaviour
{
    [Header("Y Movement")]
    [SerializeField]
    private float _verticalAmplitude = 0.25f;
    [SerializeField]
    private float _frecuency = 1.0f;
    [Header("X/Z Movement")]

    private Vector3 _initialPosition = Vector3.zero;

    public void Initialize(PodControllerData data)
    {

    }

    private void Update()
    {
        ApplyVerticalMovement();
    }

    public void Attack()
    {
    }

    private void ApplyVerticalMovement()
    {
    }

    private void ApplyHorizontalMovement()
    {
    }
}
