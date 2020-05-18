using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum CameraShakeType
{
    None = 0,
    Strong,
    Mild,
    Extreme,
}

[System.Serializable]
public class CameraShakeEntry
{
    public CameraShakeType Type;
    public CinemachineImpulseSource Source;
}

public class CameraShakeController : MonoBehaviour
{
    [SerializeField]
    private List<CameraShakeEntry> _cameraShakeEntries = new List<CameraShakeEntry>();

    private Vector3 _steadyCameraForward = Vector3.zero;

    private void Awake()
    {
        _steadyCameraForward = Camera.main.transform.forward;
    }

    public Vector3 GetSteadyCameraForward()
    {
        return _steadyCameraForward;
    }

    public void GenerateImpulse(CameraShakeType type)
    {
        if (type == CameraShakeType.None)
        {
            return;
        }

        CameraShakeEntry entry = _cameraShakeEntries.Find(e => e.Type == type);
        if (entry != null)
        {
            //Debug.Log("CameraShakeType " + type.ToString());
            entry.Source.GenerateImpulse();
            return;
        }

        Debug.LogError("CameraShakeType " + type.ToString() + " doesn't exist in controller's entries! Check CameraShakeController object!");
    }
}