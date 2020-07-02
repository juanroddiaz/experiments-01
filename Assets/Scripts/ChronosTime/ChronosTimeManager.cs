using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronosTimeManager : MonoBehaviour
{
    [SerializeField]
    private string _playerClockName = "Player";
    [SerializeField]
    private string _slowMoClockName = "SlowMo";

    private Chronos.Clock _playerClock;
    private Chronos.Clock _slowMoClock;

    private void Start()
    {
        _playerClock = Chronos.Timekeeper.instance.Clock(_playerClockName);
        _slowMoClock = Chronos.Timekeeper.instance.Clock(_slowMoClockName);
    }
}
