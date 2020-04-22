using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private CharacterController _character;
    [SerializeField]
    private HudGameplayController _hud;

    private void Awake()
    {
        _hud.Initialize();
        _character.Initialize(_hud.GetJoystick());
    }
}
