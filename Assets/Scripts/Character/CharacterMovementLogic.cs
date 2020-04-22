using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool CanMoveCheck();

public class CharacterMovementData
{
    public JoystickLogic Joystick;
    public float MovementMaxSpeed;
    public CanMoveCheck OnCanMoveCheck;
    public JoystickEventData JoystickData;
    public Rigidbody CharacterRigidbody;
}

public class CharacterMovementLogic : MonoBehaviour
{
    private bool _isMoving = false;
    private JoystickLogic _joystick;
    private JoystickData _moveData;
    private CanMoveCheck _canMoveCheck;
    private float _maxSpeed = 0.0f;
    private Rigidbody _characterRigidbody;

    private Vector3 _orientation = Vector3.zero;
    private bool _initialized = false;

    public void Initialize(CharacterMovementData data)
    {
        _joystick = data.Joystick;
        data.JoystickData.OnTouchDown = OnMoveStart;
        data.JoystickData.OnTouchUp = OnMoveEnd;
        data.JoystickData.OnDrag = OnMoveEvent;
        _joystick.InitializeEvents(data.JoystickData);
        _canMoveCheck = data.OnCanMoveCheck;
        _maxSpeed = data.MovementMaxSpeed;
        _characterRigidbody = data.CharacterRigidbody;
        _initialized = true;
    }

    public void OnMoveStart(JoystickData data)
    {
        // check if can move before setting the flag
        _isMoving = _canMoveCheck.Invoke();
        if (!_isMoving)
        {
            return;
        }
        _moveData = data;
    }

    public void OnMoveEvent(JoystickData data)
    {
        if (!_isMoving)
        {
            return;
        }
        _moveData = data;
    }

    public void OnMoveEnd(JoystickData data)
    {
        StopMoving();
    }

    public void StopMoving()
    {
        _isMoving = false;
    }

    private void FixedUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        if (!_canMoveCheck.Invoke())
        {
            return;
        }

        if (_isMoving)
        {
            _orientation.y = Mathf.Atan2(_moveData.Horizontal, _moveData.Vertical) * 180 / Mathf.PI;
            transform.eulerAngles = _orientation;

            Vector3 direction = Vector3.zero;
            direction.x = _moveData.Direction.x;
            direction.z = _moveData.Direction.y;

            _characterRigidbody.MovePosition(transform.position + direction.normalized * _maxSpeed * Time.fixedDeltaTime);
        }

        _characterRigidbody.velocity = Vector3.zero;
    }
}
