﻿using System;
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
    public CharacterAnimationLogic AnimationLogic;
}

public class CharacterMovementLogic : MonoBehaviour
{
    [SerializeField]
    private float _minimumJoystickMagnitude = 0.05f;

    private bool _isMoving = false;
    private JoystickLogic _joystick;
    private JoystickData _moveData;
    private CanMoveCheck _canMoveCheck;
    private float _maxSpeed = 0.0f;
    private Rigidbody _characterRigidbody;
    private CharacterAnimationLogic _animationLogic;
    private Action OnDisconnectEvents = null;

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
        _animationLogic = data.AnimationLogic;
        OnDisconnectEvents = _joystick.DisconnectEvents;
        _initialized = true;
    }

    public void OnMoveStart(JoystickData data)
    {
        OnMovementDataReceived(data);
    }

    public void OnMoveEvent(JoystickData data)
    {
        OnMovementDataReceived(data);
    }

    private void OnMovementDataReceived(JoystickData data)
    {
        _isMoving = _canMoveCheck.Invoke();
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
        _animationLogic.ToggleMovementAnim(false);
    }

    private void FixedUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        if (_isMoving)
        {
            if (_moveData.Direction.magnitude > _minimumJoystickMagnitude)
            {
                _orientation.y = Mathf.Atan2(_moveData.Horizontal, _moveData.Vertical) * 180 / Mathf.PI;
                transform.eulerAngles = _orientation;

                Vector3 direction = Vector3.zero;
                direction.x = _moveData.Direction.x;
                direction.z = _moveData.Direction.y;

                _animationLogic.ToggleMovementAnim(true);
                _characterRigidbody.MovePosition(transform.position + direction.normalized * _maxSpeed * Time.fixedDeltaTime);
            }            
        }

        _characterRigidbody.velocity = Vector3.zero;
    }

    private void OnDestroy()
    {
        OnDisconnectEvents?.Invoke();
    }
}
