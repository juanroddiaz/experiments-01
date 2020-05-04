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


        var reachData = new CharacterReachData
        {
            OnMeeleTrigger = new CollisionTriggerData
            {
            },
            OnDodgeTrigger = new CollisionTriggerData
            {
            },
            OnDistanceTrigger = new CollisionTriggerData
            {
                TriggerEnterAction = OnDistanceRangeEnter,
                TriggerExitAction = OnDistanceRangeExit
            }
        };
        _reachLogic.Initialize(reachData);
    }

    public bool CheckCanMove()
    {
        return true;
    }

    public void OnClickEvent()
    {
        Debug.Log("OnClick event!");
    }

    public void OnMeleeRangeEnter(Transform t)
    {
        Debug.Log("Melee enter!");
    }

    public void OnMeleeRangeExit(Transform t)
    {
        Debug.Log("Melee exit!");
    }

    public void OnDodgeRangeEnter(Transform t)
    {
        Debug.Log("Dodge enter!");
    }

    public void OnDodgeRangeExit(Transform t)
    {
        Debug.Log("Dodge exit!");
    }

    public void OnDistanceRangeEnter(Transform t)
    {
        Debug.Log("Distance enter!");
        _podController.Attack(t);
    }

    public void OnDistanceRangeExit(Transform t)
    {
        Debug.Log("Distance exit!");
        _podController.OutOfRange(t);
    }
}
