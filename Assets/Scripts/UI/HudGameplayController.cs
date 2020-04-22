using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudGameplayController : MonoBehaviour
{
    [SerializeField]
    private JoystickLogic _joystick;

    public void Initialize()
    {
    }

    public JoystickLogic GetJoystick()
    {
        return _joystick;
    }
}
