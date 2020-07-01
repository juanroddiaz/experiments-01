using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthOwner
{
    Enemy = 0,
    Player,
    Prop,
}

public class HealthLogicData
{
    public int MaxLife = 0;
    public Action<Vector3> OnDamage;
    public Action<Vector3> OnDeath;
    public HealthOwner Owner;
    public int SkinArmor = 0;
    public float DamageReductionFactor = 0.0f;
    public Transform UIParentTransform;
    public CameraShakeController CameraShake;
}

public class HealthLogic : MonoBehaviour
{
    [SerializeField] private GameObject _lifeBarPrefab;
    [SerializeField] private GameObject _combatFloatingTextPrefab;
    [SerializeField] private CameraShakeType _deathCameraShakeType = CameraShakeType.Mild;
    [SerializeField] private CameraShakeType _hurtCameraShakeType = CameraShakeType.Strong;
    [Header("Invincibility")]
    [SerializeField] private float _invincibleTime = 1.5f;
    [SerializeField] private GameObject _invincibleFx = null;
    [Header("Cheats")]
    [SerializeField] public bool _debugInvincible = false;

    public LifeBarController GetLifeBarController()
    {
        return _lifeBarController;
    }

    private LifeBarController _lifeBarController;
    private HealthLogicData _data;

    private int _currentDamage = 0;

    private GameObject _spawnedLifeBarGO;
    private List<GameObject> _spawnedCombatFloatingTextGameObjectList;
    private float _combatHealthAngle = 0f;
    private bool _combatHealthNumberToTheRight = true;
    private int _combatHealthMaxIncrement = 12;
    private bool _invincible = false;

    void Awake()
    {
        _spawnedCombatFloatingTextGameObjectList = new List<GameObject>();
    }

    public void Initialize(HealthLogicData data)
    {
        _spawnedLifeBarGO = ObjectPoolController.Instance.Spawn(_lifeBarPrefab, Vector3.zero, Quaternion.identity);
        _lifeBarController = _spawnedLifeBarGO.GetComponent<LifeBarController>();
        _data = data;

        _combatHealthAngle = 0f;
        SetFullLife();
        if (_invincibleFx != null)
        {
            _invincibleFx.SetActive(false);
        }
    }

    public int GetMaxLife()
    {
        return _data.MaxLife;
    }

    private float ComputeNewAngle()
    {
        float angle = _combatHealthAngle + (Mathf.PI * 2f / _combatHealthMaxIncrement);
        if (angle >= (2f * Mathf.PI))
            angle -= (2f * Mathf.PI);
        return angle;
    }

    public bool IsAlive()
    {
        return _currentDamage > 0;
    }

    public bool IsInvincible()
    {
        return _invincible;
    }

    public bool TakeDamage(Vector3 from, int damage)
    {
        if (!IsAlive())
        {
            return false;
        }

        if (_invincible)
        {
            Debug.Log("Entity is invincible!");
            return true;
        }

        bool itWasAlive = _currentDamage > 0;
        bool isDead = false;
        int calculatedDamage = CalculateDamage(damage);
        _currentDamage -= calculatedDamage;
        _lifeBarController.UpdateValue(_currentDamage, _data.MaxLife);
        CameraShakeType shakeType = _hurtCameraShakeType;
        CombatMessageType damageType = CombatMessageType.Normal;

        Vector3 damageForward = transform.position - from;

        if (itWasAlive)
        {
            isDead = _currentDamage <= 0;
            //TriggerEffect(damageForward.normalized, isDead, from);
            if (isDead)
            {
                _data.OnDeath?.Invoke(damageForward);
                damageType = CombatMessageType.Lethal;
                shakeType = _deathCameraShakeType;
            }
            else
            {
                _data.OnDamage?.Invoke(damageForward);
            }
        }

        SpawnCombatNumber(calculatedDamage.ToString(), damageType);
        _data.CameraShake.GenerateImpulse(shakeType);
        return !isDead;
    }

    private int CalculateDamage(int damage)
    {
#if ADMIN_PANEL
        if(_debugInvincible)
        {
            return 0;
        }
#endif
        int d = Mathf.CeilToInt(damage * (1.0f - _data.DamageReductionFactor));
        d -= _data.SkinArmor;
        return Mathf.Max(0, d);
    }

    public void Heal(int healing, bool incrementMaxLife = false)
    {
        _data.MaxLife += incrementMaxLife ? healing : 0;
        _currentDamage = Mathf.Min(_currentDamage + healing, _data.MaxLife);
        _lifeBarController.UpdateValue(_currentDamage, _data.MaxLife);
        var message = "+" + healing.ToString() + (incrementMaxLife ? " MAX!" : "");
        SpawnCombatNumber(message, CombatMessageType.Healing);
    }

    public void SetFullLife(bool invincible = false)
    {
        _lifeBarController.UpdateValue(_data.MaxLife, _data.MaxLife);
        _currentDamage = _data.MaxLife;
        if (invincible)
        {
            SetInvincibility();
        }
    }

    public void ToggleInvincibility(bool toggle)
    {
        _invincible = toggle;
    }

    public void SetInvincibility(float time = 0.0f)
    {
        if (_invincible)
        {
            return;
        }
        time = time > 0.0f ? time : _invincibleTime;
        StartCoroutine(ToggleInvincible(time));
    }

    private IEnumerator ToggleInvincible(float time)
    {
        _invincible = true;
        _invincibleFx.SetActive(true);
        float counter = 0.0f;
        while (counter < time)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        _invincibleFx.SetActive(false);
        _invincible = false;
        yield return null;
    }

    private void SpawnCombatNumber(string value, CombatMessageType type)
    {
        GameObject combatNumberObj = ObjectPoolController.Instance.Spawn(_combatFloatingTextPrefab, Vector3.zero, Quaternion.identity);

        _spawnedCombatFloatingTextGameObjectList.Add(combatNumberObj);
        var combatNumberLogic = combatNumberObj.GetComponent<CombatFlyingTextLogic>();
        float clipLength = combatNumberLogic.Initialize(value, transform, type, _combatHealthNumberToTheRight);
        _combatHealthAngle = ComputeNewAngle();
        _combatHealthNumberToTheRight = !_combatHealthNumberToTheRight;
        if (ObjectPoolController.Instance.IsSpawned(combatNumberObj))
        {
            ObjectPoolController.Instance.RecycleAfter(combatNumberObj, clipLength);
            return;
        };
    }   

    public void RecycleFeedback()
    {
        if (_spawnedLifeBarGO != null && ObjectPoolController.Instance.IsSpawned(_spawnedLifeBarGO))
        {
            ObjectPoolController.Instance.Recycle(_spawnedLifeBarGO);
        };

        foreach (var c in _spawnedCombatFloatingTextGameObjectList)
        {
            if (c != null && ObjectPoolController.Instance.IsSpawned(c))
            {
                ObjectPoolController.Instance.Recycle(c);
            };
        }
    }
}
