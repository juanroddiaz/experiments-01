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
    private float _verticalFrecuency = 1.0f;
    [Header("X/Z Movement")]
    [SerializeField]
    private float _angleSpeed = 10.0f;
    [Header("Orientation")]
    [SerializeField]
    private float _orientationSpeed = 45.0f;

    private Transform _ownerTransform;
    private Transform _target;
    private Vector3 _initialPosition = Vector3.zero;
    private Vector3 _calculatedPosition = Vector3.zero;
    private bool _initialized = false;

    public void Initialize(PodControllerData data)
    {
        _ownerTransform = data.Owner;
        _initialPosition = transform.position;
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized)
        {
            return;
        }

        CalculateVerticalMovement();
        CalculateHorizontalMovement();
        transform.position = _calculatedPosition + _ownerTransform.position + _initialPosition;

        UpdateOrientation();
    }

    public void Attack(Transform target)
    {
        _target = target;
    }

    public void OutOfRange(Transform target)
    {
        // todo: list of targets
        _target = null;
    }

    private void CalculateVerticalMovement()
    {
        _calculatedPosition.y = Mathf.Sin(Time.time * _verticalFrecuency) * _verticalAmplitude;
    }

    private void CalculateHorizontalMovement()
    {
        var dir = _ownerTransform.forward;
        float yAxisAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (yAxisAngle < 0)
        {
            yAxisAngle += 360;
        }
        yAxisAngle += 180.0f;
        yAxisAngle *= Mathf.Deg2Rad;
        _calculatedPosition.x = Mathf.Cos(yAxisAngle) * 1.5f;
        _calculatedPosition.z = Mathf.Sin(yAxisAngle) * 1.5f;
    }

    private void UpdateOrientation()
    {
        Quaternion lookRotation = Quaternion.identity;
        bool hasTarget = _target != null;
        if (hasTarget)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            lookRotation = Quaternion.LookRotation(_ownerTransform.forward.normalized);

        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                lookRotation, _orientationSpeed * Time.deltaTime);
    }
}
