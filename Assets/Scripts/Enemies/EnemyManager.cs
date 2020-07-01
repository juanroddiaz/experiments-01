using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyController> _enemies;
    private SceneManager _sceneManager;

    public void Initialize(SceneManager sceneManager)
    {
        _sceneManager = sceneManager;
        _enemies = new List<EnemyController>(GetComponentsInChildren<EnemyController>());
        Debug.Log("Enemies found: " + _enemies.Count.ToString());
        foreach(var e in _enemies)
        {
            e.Initialize(this);
        }
    }

    public CameraShakeController GetCameraShakeController()
    {
        return _sceneManager.CameraShake;
    }

    public void AddToGameplayUI(Transform t)
    {
        _sceneManager.Hud.AddToGameplayUI(t);
    }
}
