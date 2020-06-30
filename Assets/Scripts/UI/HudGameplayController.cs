using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudGameplayController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField]
    private JoystickLogic _joystick;
    [Header("Panels")]
    [SerializeField]
    private GameObject _controlsPanel;
    [SerializeField]
    private GameObject _pausePanel;
    [SerializeField]
    private GameObject _gameplayPanel;

    private SceneManager _sceneManager;

    public void Initialize(SceneManager manager)
    {
        _sceneManager = manager;
        OnUnpause();
    }

    public JoystickLogic GetJoystick()
    {
        return _joystick;
    }

    public void OnPause()
    {
        _sceneManager.TogglePause(true);
        _pausePanel.SetActive(true);
        _controlsPanel.SetActive(false);
    }

    public void OnUnpause()
    {
        _sceneManager.TogglePause(false);
        _pausePanel.SetActive(false);
        _controlsPanel.SetActive(true);
    }

    public void OnQuitGame()
    {
        _sceneManager.OnQuit();
    }

    public void AddToGameplayUI(Transform t)
    {
        t.SetParent(_gameplayPanel.transform);
        t.localScale = Vector3.one;
    }
}
