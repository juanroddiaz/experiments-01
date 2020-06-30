//-----------------------------------------------------------------------
// CombatFloatingNumberLogic.cs
//
// Copyright 2020 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public enum CombatMessageType
{
    Normal = 0,
    CritHit,
    Healing,
    Lethal,
}

[System.Serializable]
public class CombatMessageColors
{
    public CombatMessageType Type;
    public Color MessageColor;
    public Sprite Icon;
}

public class CombatFlyingTextLogic : MonoBehaviour, IPooleableObject
{
    [SerializeField] private TextMeshProUGUI _valueTxt;
    [SerializeField] private Animation _animation;
    [SerializeField] private Image _icon;
    [SerializeField] private Vector2 _randomPositionOffset;
    [Header("Message configuration")]
    [SerializeField] private List<CombatMessageColors> _messageColorConfig;

    private List<string> _animNames = new List<string>();
    private Transform _targetTransform;

    public float Initialize(string value, Transform target, CombatMessageType type, bool toTheRight)
    {
        if(_animNames.Count == 0)
        {
            foreach(AnimationState a in _animation)
            {
                _animNames.Add(a.name);
            }
        }

        if(_animation.GetClipCount() != 3 || _animNames.Count != 3)
        {
            Debug.LogError("CombatFlyingText must have 3 anims!!");
            return 0.0f;
        }

        _targetTransform = target;
        UpdatePosition();

        _icon.gameObject.SetActive(false);
        _valueTxt.text = value;
        _valueTxt.color = _messageColorConfig[(int)type].MessageColor;
        int animIdx = -1;
        switch(type)
        {
            case CombatMessageType.Healing:
                animIdx = 2;
                _icon.gameObject.SetActive(true);
                _icon.sprite = _messageColorConfig[(int)type].Icon;
                break;
            case CombatMessageType.Normal:
            case CombatMessageType.Lethal:
            case CombatMessageType.CritHit:
                transform.position += new Vector3(Random.Range(-_randomPositionOffset.x, _randomPositionOffset.x),
                    Random.Range(-_randomPositionOffset.y, _randomPositionOffset.y),
                    0.0f);
                animIdx = toTheRight ? 0 : 1;
                break;
        }

        _animation.Play(_animNames[animIdx]);

        return _animation.GetClip(_animNames[animIdx]).length;
    }

    private void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(_targetTransform.position);
    }

    void Update()
    {
        UpdatePosition();
    }

    public void OnSpawn()
    { }

    public void SetPool(ObjectPoolController pool)
    {
    }

    public void OnAfterRecycle()
    {
    }
}
