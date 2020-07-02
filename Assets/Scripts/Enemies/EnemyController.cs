using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameplayTextLogic _countdownLabel;
    [SerializeField]
    private CombatFlyingTextLogic  _combatNumber;
    [SerializeField]
    private float _attackTimer = 3.0f;
    [SerializeField]
    private TimeAreaLogic _timeSphere;

    private GameplayTextLogic _currentCountdownLabel;
    private EnemyManager _manager;
    private bool _combatNumberDirectionToggle;

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
                SpawnCombatNumber("TIME SPHERE", CombatMessageType.Normal);
                Vector3 timeSpherePos = transform.position;
                timeSpherePos.y = 0.0f;
                var timeSphere = ObjectPoolController.Instance.Spawn(_timeSphere.gameObject, timeSpherePos, Quaternion.identity);
                countDown += _attackTimer;
            }
            yield return null;
        }
    }

    private void SpawnCombatNumber(string value, CombatMessageType type)
    {
        GameObject combatNumberObj = ObjectPoolController.Instance.Spawn(_combatNumber.gameObject, Vector3.zero, Quaternion.identity);
        _manager.AddToGameplayUI(combatNumberObj.transform);
        var combatNumberLogic = combatNumberObj.GetComponent<CombatFlyingTextLogic>();
        float clipLength = combatNumberLogic.Initialize(value, transform, type, _combatNumberDirectionToggle);
        _combatNumberDirectionToggle = !_combatNumberDirectionToggle;
        ObjectPoolController.Instance.RecycleAfter(combatNumberObj, clipLength);
    }
}
