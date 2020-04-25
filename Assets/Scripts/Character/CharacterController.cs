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
    private CharacterReachLogic _reachLogic;
    [SerializeField]
    private Transform _modelContainer;
    [SerializeField]
    private Transform _podContainer;

    private CharacterAnimationLogic _animationLogic;
    private PodController _podController;

    public void Initialize(JoystickLogic joystick)
    {
        var model = _modelContainer.GetChild(0);
        _animationLogic = model.GetComponent<CharacterAnimationLogic>();

        // subscribe to joystick
        var movementData = new CharacterMovementData
        {
            Joystick = joystick,
            MovementMaxSpeed = _config.MovementMaxSpeed,
            OnCanMoveCheck = CheckCanMove,
            JoystickData = new JoystickEventData(),
            CharacterRigidbody = GetComponent<Rigidbody>(),
            AnimationLogic = _animationLogic
        };

        _movementLogic.Initialize(this, movementData);

        var pod = _podContainer.GetChild(0);
        _podController = pod.GetComponent<PodController>();
        var podData = new PodControllerData
        {
            Owner = transform
        };
        _podController.Initialize(podData);
    }

    public bool CheckCanMove()
    {
        return true;
    }

    public void OnClickEvent()
    {
        Debug.Log("OnClick event!");
    }
}
