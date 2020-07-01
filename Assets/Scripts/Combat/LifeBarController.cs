//-----------------------------------------------------------------------
// LifeBarController.cs
//
// Copyright 2020 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]

public class LifeBarController : MonoBehaviour, IPooleableObject
{
    [SerializeField] private Transform _foregroundPivot;
    [SerializeField] private GameObject _backgroundObj;
    [SerializeField] private TextMeshProUGUI _valueTxt;

    [SerializeField] private bool _hideIfEmpty;
    [SerializeField] private bool _hideIfFull;
    [SerializeField] private bool _showValue = true;
    [SerializeField]
    private Vector3 _localPositionOffset;

    private Transform _targetTransform;
    private bool _initialized = false;

    void Awake()
    {
        _valueTxt.enabled = _showValue;
    }

    public void Initialize(Transform target)
    {
        _targetTransform = target;
        _initialized = true;
    }

    public void UpdateValue(int currentValue, int maxValue)
    {
        currentValue = Math.Max(currentValue, 0);

        var percent = currentValue / (float)maxValue;
        _foregroundPivot.localScale = new Vector3(percent, 1, 1);

        _valueTxt.text = currentValue.ToString();

        var hide = (_hideIfEmpty && currentValue == 0) || (_hideIfFull && currentValue == maxValue);
        _backgroundObj.SetActive(!hide);
        _valueTxt.gameObject.SetActive(!hide);
    }

    void Update()
    {
        if(_initialized)
        {
            transform.position = Camera.main.WorldToScreenPoint(_targetTransform.position);
        }        
    }

    public void OnSpawn()
    {
    }

    public void SetPool(ObjectPoolController pool)
    {
    }

    public void OnAfterRecycle()
    {
    }
}
