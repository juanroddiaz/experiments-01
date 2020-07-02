using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeAreaState
{
    None,
    Growing,
    Idle,
    Shrinking,
}

public class TimeAreaLogic : MonoBehaviour, IPooleableObject
{
    [SerializeField]
    private Vector3 _startSize = Vector3.one;
    [SerializeField]
    private Vector3 _endSize = Vector3.one;
    [SerializeField]
    private float _growthSpeed = 5.0f;
    [SerializeField]
    private float _idleTime = 0.5f;
    [SerializeField]
    private float _shrinkSpeed = 5.0f;

    private float _progressionTime = 0.0f;
    private TimeAreaState _state = TimeAreaState.None;

    public void OnSpawn()
    {
        transform.localScale = _startSize;
        _state = TimeAreaState.Growing;
        _progressionTime = 0.0f;
    }

    void Update()
    {
        if (_state == TimeAreaState.None)
        {
            return;
        }

        switch(_state)
        {
            case TimeAreaState.Growing:
                _progressionTime += Time.deltaTime * _growthSpeed;
                transform.localScale = Vector3.Lerp(_startSize, _endSize, _progressionTime);
                if (transform.localScale == _endSize)
                {
                    _state = TimeAreaState.Idle;
                    _progressionTime = 0.0f;
                }
                break;
            case TimeAreaState.Idle:
                _progressionTime += Time.deltaTime;
                if (_progressionTime >= _idleTime)
                {
                    _state = TimeAreaState.Shrinking;
                    _progressionTime = 0.0f;
                }
                break;
            case TimeAreaState.Shrinking:
                _progressionTime += Time.deltaTime * _shrinkSpeed;
                transform.localScale = Vector3.Lerp(_endSize, _startSize, _progressionTime);
                if (transform.localScale == _startSize)
                {
                    _state = TimeAreaState.None;
                    ObjectPoolController.Instance.Recycle(gameObject);
                }
                break;
        }
    }

    
    public void OnAfterRecycle()
    {
        
    }
}
