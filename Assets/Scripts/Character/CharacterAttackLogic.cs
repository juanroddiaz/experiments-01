using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterAttackType
{
    None,
    Melee,
    Dodge,
    Distance
}

[System.Serializable]
public class AttackArea
{
    public GameObject ColliderObject;
    public float ColliderActivationTime = 0.2f;
    public FieldOfViewSight FovSightLogic;
}

public class CharacterAttackData
{
    public CharacterAnimationLogic AnimationLogic;
}

public class CharacterAttackLogic : MonoBehaviour
{
    [SerializeField]
    private AttackArea _meleeAttack;

    private CharacterAnimationLogic _animationLogic;
    private CharacterAttackType _currentAttackType = CharacterAttackType.None;

    public bool IsAttacking {get; private set;}
    
    public void Initialize(CharacterAttackData data)
    {
        _animationLogic = data.AnimationLogic;
    }

    public bool TryToMeleeAttack()
    {
        if (_meleeAttack.FovSightLogic.CheckTargetOnSight())
        {
            //Debug.Log("HIT!! " + _meleeFovLogic.Target.name);
            _animationLogic.TogglePunchAnim(true);
            IsAttacking = true;
            return true;
        }

        return false;
    }

    public void SetAttackTarget(CharacterAttackType type, Transform target)
    {
        switch (type)
        {
            case CharacterAttackType.Melee:
                _meleeAttack.FovSightLogic.Target = target;
                break;
            case CharacterAttackType.Dodge:
            case CharacterAttackType.Distance:
                break;
            case CharacterAttackType.None:
                break;
        }

        _currentAttackType = type;
    }

    public Transform GetCurrentTarget()
    {
        switch(_currentAttackType)
        {
            case CharacterAttackType.Melee:
                return _meleeAttack.FovSightLogic.Target;
            case CharacterAttackType.Dodge:
            case CharacterAttackType.Distance:
                break;
            case CharacterAttackType.None:
                break;
        }

        return null;
    }

    public void StopAllAttacks()
    {
        StopAttack(CharacterAttackType.Melee);
        StopAttack(CharacterAttackType.Dodge);
        StopAttack(CharacterAttackType.Distance);
        _currentAttackType = CharacterAttackType.None;
    }

    public void StopAttack(CharacterAttackType type)
    {
        switch (type)
        {
            case CharacterAttackType.Melee:
                _meleeAttack.FovSightLogic.Target = null;
                _animationLogic.TogglePunchAnim(false);
                break;
            case CharacterAttackType.Dodge:
            case CharacterAttackType.Distance:
                break;
            case CharacterAttackType.None:
                break;
        }

        IsAttacking = false;
    }
}
