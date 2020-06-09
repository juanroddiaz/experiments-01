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
    private float _radius = 2.0f;
    [SerializeField]
    private float _angleSpeed = 10.0f;
    [Header("Orientation")]
    [SerializeField]
    private float _orientationSpeed = 45.0f;
    [Header("Attack")]
    [SerializeField]
    private FieldOfViewSight _attackFovLogic;
    [SerializeField]
    private ShootingLogic _shootingLogic;

    private Transform _ownerTransform;
    private Transform _target;
    private Vector3 _initialPosition = Vector3.zero;
    private Vector3 _calculatedPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
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
        // TODO: smooth orbital using target position
        _calculatedPosition = _targetPosition;
        transform.position = _calculatedPosition;

        UpdateOrientation();
        if (_attackFovLogic.CheckTargetOnSight())
        {
            _shootingLogic.TryAttack(_attackFovLogic.Target);
        }
    }

    public void Attack(Transform target)
    {
        _target = target;
        _attackFovLogic.Target = target;
    }

    public void OutOfRange(Transform target)
    {
        // todo: list of targets
        _target = null;
        _attackFovLogic.Target = null;
    }

    private void CalculateVerticalMovement()
    {
        _targetPosition.y = Mathf.Sin(Time.time * _verticalFrecuency) * _verticalAmplitude;
        _targetPosition.y = _ownerTransform.position.y + _initialPosition.y;
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
        _targetPosition.x = Mathf.Cos(yAxisAngle) * _radius + _ownerTransform.position.x;
        _targetPosition.z = Mathf.Sin(yAxisAngle) * _radius + _ownerTransform.position.z;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_targetPosition, Vector3.one * 0.5f);
    }
}
