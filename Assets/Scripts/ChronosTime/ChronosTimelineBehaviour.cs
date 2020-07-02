using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chronos.Timeline))]
public class ChronosTimelineBehaviour : MonoBehaviour
{
    private Chronos.Timeline _chronosTimeline = null;

    public Chronos.Timeline ChronosTime
    {
        get
        {
            if (_chronosTimeline == null)
            {
                _chronosTimeline = GetComponent<Chronos.Timeline>();
            }
            return _chronosTimeline;
        }
    }
}
