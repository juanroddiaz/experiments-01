using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewSight : MonoBehaviour
{
    [SerializeField]
    private float _sightAngle = 10.0f;

    private Transform _target = null;

    public void UpdateTarget(Transform newTarget = null)
    {
        _target = newTarget;
    }

    // Update is called once per frame
    public bool CheckTargetOnSight()
    {
        if (_target == null)
        {
            return false;
        }

        float angle = Vector3.Angle(_target.position - transform.position, transform.forward);
        //Debug.Log("angle: " + angle.ToString());
        if (angle >= _sightAngle)
        {
            return false;
        }

        return true;
    }
}
