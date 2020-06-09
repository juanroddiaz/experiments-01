using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewSight : MonoBehaviour
{
    [SerializeField]
    private float _sightAngle = 10.0f;

    public Transform Target;

    // Update is called once per frame
    public bool CheckTargetOnSight()
    {
        if (Target == null)
        {
            return false;
        }

        float angle = Vector3.Angle(Target.position - transform.position, transform.forward);
        //Debug.Log("angle: " + angle.ToString());
        if (angle >= _sightAngle)
        {
            return false;
        }

        return true;
    }
}
