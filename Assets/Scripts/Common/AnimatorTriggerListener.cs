using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTriggerData
{
    public Action<string> OnAttack;
}

public class AnimatorTriggerListener : MonoBehaviour
{
    private AnimatorTriggerData _data;
    private bool _initialized = false;

    public void Initialize(AnimatorTriggerData data)
    {
        _data = data;
        _initialized = true;
    }

    public void OnAttackEvent(string name)
    {
        Debug.Log("OnAttackEvent: " + name);
        if(_initialized)
        {
            _data.OnAttack?.Invoke(name);
        }
    }
}
