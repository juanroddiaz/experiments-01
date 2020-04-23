using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    Idle = 0,
    Move,
    Dodge,
    DashAttack,
}

public class CharacterAnimationLogic : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    static readonly int _punch = Animator.StringToHash("Punch");
    static readonly int _walk = Animator.StringToHash("Walk");
    static readonly int _run = Animator.StringToHash("Run");
    static readonly int _dashAttack = Animator.StringToHash("DashAttack");
    static readonly int _dodge = Animator.StringToHash("Dodge");

    private AnimState _state;

    public void ToggleMovementAnim(bool toggle)
    {
        _animator.SetBool(_run, toggle);
    }

    public void TogglePunchAnim(bool toggle)
    {
        _animator.SetBool(_punch, toggle);
    }

    public void ToggleDashAttackAnim(bool toggle)
    {
        _animator.SetBool(_dashAttack, toggle);
    }

    public void TriggerDodge()
    {
        _animator.SetTrigger(_dodge);
    }

    public void OnIdleEvent()
    {
        _state = AnimState.Idle;
    }
}
