﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private CharacterController _character;
    [SerializeField]
    private HudGameplayController _hud;
    [SerializeField]
    private EnemyManager _enemyManager;
    [SerializeField]
    private CameraShakeController _cameraShake;
    [SerializeField]
    private ChronosTimeManager _timeManager;

    public HudGameplayController Hud => _hud;
    public CameraShakeController CameraShake => _cameraShake;

    private void Awake()
    {
        _hud.Initialize(this);
        _character.Initialize(_hud.GetJoystick());
        _enemyManager.Initialize(this);
    }

    public void TogglePause(bool toggle)
    {
        // cheap but effective
        Time.timeScale = toggle ? 0.0f : 1.0f;
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
