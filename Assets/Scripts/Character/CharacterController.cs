using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private CharacterConfig _config;
    [SerializeField]
    private CharacterMovementLogic _movementLogic;
    [SerializeField]
    private CharacterAnimationLogic _animationLogic;

    private void Start()
    {
        // subscribe to joystick
        // read config
        // select models
    }
}
