using TMPro;
using UnityEngine;

public class GameplayTextLogic : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private bool _anchoredToTarget = true;
    [SerializeField]
    private Vector3 _localPositionOffset;

    private Transform _targetTransform;
    private bool _hasText = false;

    public void Initialize(Transform target)
    {
        _targetTransform = target;
        _hasText = _text != null;
        UpdatePosition();
    }

    public void UpdateValue(float value)
    {
        if (_hasText)
        {
            _text.text = value.ToString("0.0");
        }
        
        if (_anchoredToTarget)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(_targetTransform.position);
        transform.localPosition += _localPositionOffset;
    }

    public void OnFinish()
    {
        Destroy(gameObject);
    }
}
