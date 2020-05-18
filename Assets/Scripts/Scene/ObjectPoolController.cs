using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolEntry
{
    public int PrefabDefaultInstances = 5;
    public GameObject PrefabObject;
}

public class ObjectPoolInstance
{
    public bool IsUsed = false;
    public GameObject Instance;
}

public class ObjectPoolGroup
{
    public GameObject PrefabObject;
    public List<ObjectPoolInstance> PooledInstances;
}

public class ObjectPoolController : MonoBehaviour
{
    [SerializeField]
    private List<ObjectPoolEntry> _poolEntries;

    private List<ObjectPoolGroup> _poolGroup;

    private void Awake()
    {
        _poolGroup = new List<ObjectPoolGroup>();
        foreach(var e in _poolEntries)
        {
            var group = new ObjectPoolGroup
            {
                PrefabObject = e.PrefabObject,
                PooledInstances = new List<ObjectPoolInstance>()
            };

            for (int i = 0; i < e.PrefabDefaultInstances; i++)
            {
                var go = Instantiate(group.PrefabObject, transform, true);
                go.SetActive(false);
                group.PooledInstances.Add(new ObjectPoolInstance
                {
                    IsUsed = false,
                    Instance = go
                });                
            }

            _poolGroup.Add(group);
        }
    }
}
