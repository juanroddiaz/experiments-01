using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterReachData
{
    public CollisionTriggerData OnMeeleTrigger;
    public CollisionTriggerData OnDodgeTrigger;
    public CollisionTriggerData OnDistanceTrigger;
}

public class CharacterReachLogic : MonoBehaviour
{
    [SerializeField]
    private CollisionTriggerLogic _meleeTrigger;
    [SerializeField]
    private Projector _meleeProjector;
    [SerializeField]
    private CollisionTriggerLogic _dodgeTrigger;
    [SerializeField]
    private Projector _dodgeProjector;
    [SerializeField]
    private CollisionTriggerLogic _distanceTrigger;
    [SerializeField]
    private Projector _distanceProjector;

    public void Initialize(CharacterReachData data)
    {
        _meleeTrigger.Initialize(data.OnMeeleTrigger);
        _dodgeTrigger.Initialize(data.OnDodgeTrigger);
        _distanceTrigger.Initialize(data.OnDistanceTrigger);
    }
}
