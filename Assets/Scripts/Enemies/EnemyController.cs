using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameplayTextLogic _countdownLabel;
    [SerializeField]
    private float _attackTimer = 3.0f;

    private GameplayTextLogic _currentCountdownLabel;
    private EnemyManager _manager;

    public void Initialize(EnemyManager manager)
    {
        _manager = manager;
        StartCoroutine(RunAttackCountdown());
    }

    private IEnumerator RunAttackCountdown()
    {
        _currentCountdownLabel = Instantiate(_countdownLabel);
        _manager.AddToGameplayUI(_currentCountdownLabel.transform);
        _currentCountdownLabel.Initialize(transform);

        float countDown = _attackTimer;
        while (countDown > 0.0f)
        {
            _currentCountdownLabel.UpdateValue(countDown);
            countDown -= Time.deltaTime;
            if (countDown <= 0.0f)
            {
                countDown += _attackTimer;
            }
            yield return null;
        }
    }
}
