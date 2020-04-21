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
}

public class CharacterMovementLogic : MonoBehaviour
{
    private bool _isMoving = false;
    private JoystickLogic _joystick;
    private JoystickData _moveData;
    private CanMoveCheck _canMoveCheck;
    private float _maxSpeed = 0.0f;

    public void Initialize(CharacterMovementData data)
    {
        _joystick = data.Joystick;
        _joystick.InitializeEvents(data.JoystickData);
        _canMoveCheck = data.OnCanMoveCheck;
        _maxSpeed = data.MovementMaxSpeed;
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
}
