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
    public IPooleableObject PooleableInstance;
}

public class ObjectPoolGroup
{
    public GameObject PrefabObject;
    public List<ObjectPoolInstance> PooledInstances;

    public ObjectPoolInstance GetUnusedInstance()
    {
        var entry = PooledInstances.Find(i => !i.IsUsed);
        if (entry == null)
        {
            return null;
        }
        entry.IsUsed = true;
        return entry;
    }

    public void ReturnInstance(GameObject obj)
    {
        var entry = PooledInstances.Find(o => o.IsUsed && o.Instance == obj);
        if (entry == null)
        {
            return;
        }
        entry.IsUsed = false;
        entry.Instance.SetActive(false);
        entry.PooleableInstance.OnRecycle();
        return;
    }
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
                CreateInstanceInGroup(group);
            }

            _poolGroup.Add(group);
        }
    }

    private ObjectPoolInstance CreateInstanceInGroup(ObjectPoolGroup group)
    {
        var go = Instantiate(group.PrefabObject, transform, true);
        go.SetActive(false);
        var iPooleable = go.GetComponent(typeof(IPooleableObject)) as IPooleableObject;
        iPooleable.SetPool(this);
        var instance = new ObjectPoolInstance
        {
            IsUsed = false,
            Instance = go,
            PooleableInstance = iPooleable
        };

        group.PooledInstances.Add(instance);
        return instance;
    }

    public GameObject Spawn(GameObject prefabObject)
    {
        var group = _poolGroup.Find(g => g.PrefabObject.name == prefabObject.name);
        ObjectPoolInstance instance = null;
        if(group != null)
        {
            instance = group.GetUnusedInstance();
            if (instance == null)
            {
                instance = CreateInstanceInGroup(group);
            }
        }

        instance.PooleableInstance.OnSpawn();
        instance.Instance.SetActive(true);
        return instance.Instance;
    }

    public void Recycle(GameObject obj)
    {
        var group = _poolGroup.Find(g => g.PrefabObject.name == obj.name);
        if (group != null)
        {
            group.ReturnInstance(obj);
        }
    }
}
