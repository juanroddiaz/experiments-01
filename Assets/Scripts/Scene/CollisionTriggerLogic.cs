using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerLogic : MonoBehaviour
{
    private Action _onTriggerEnterAction;
    private Action _onTriggerExitAction;

    public void Initialize(Action action)
    {
        _onTriggerEnterAction = action;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger enter: other " + other.name + ", collider: " + gameObject.name);
        _onTriggerEnterAction?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("On trigger exit: other " + other.name + ", collider: " + gameObject.name);
        _onTriggerExitAction?.Invoke();
    }
}
