using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "GameConfig/Character Config")]
public class CharacterConfig : ScriptableObject
{
    public int DefaultModelIdx = 0;
    public float ReachMelee = 1.0f;
    public float ReachDodge = 1.5f;
    public float ReachDistanceAttack = 4.0f;
    public float MovementMaxSpeed = 5.0f;
}