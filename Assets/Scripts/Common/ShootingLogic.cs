using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class ShootingLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject _missileObj;
    [SerializeField]
    private float _fireRatio = 0.5f;

    private float _fireCooldown = 0.0f;
    private bool _readyToAttack = false;

    private void Start()
    {
        _fireCooldown = _fireRatio;
        _readyToAttack = true;
    }

    public void TryAttack()
    {
        if (_readyToAttack)
        {
            _fireCooldown = 0.0f;
            _readyToAttack = false;
            Instantiate(_missileObj, transform.position, transform.rotation);
        }
    }

    private void Update()
    {
        if (_readyToAttack)
        {
            return;
        }

        if (_fireCooldown < _fireRatio)
        {
            _fireCooldown += Time.deltaTime;
            if (_fireCooldown >= _fireRatio)
            {
                _readyToAttack = true;
            }
        }
    }
}
