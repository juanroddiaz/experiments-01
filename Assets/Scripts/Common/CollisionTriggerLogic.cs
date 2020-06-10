using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerData
{
    public Action<Transform> TriggerEnterAction;
    public Action<Transform> TriggerExitAction;
}

public class CollisionTriggerLogic : MonoBehaviour
{
    private CollisionTriggerData _data;
    private bool _initialized = false;

    public void Initialize(CollisionTriggerData data)
    {
        _data = data;
        _initialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger enter: other " + other.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.TriggerEnterAction?.Invoke(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("On trigger exit: other " + other.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.TriggerExitAction?.Invoke(other.transform);
        }
    }
}
