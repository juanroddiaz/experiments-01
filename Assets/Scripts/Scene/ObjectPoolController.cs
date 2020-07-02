using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<int, ObjectPoolInstance> PooledInstances;
    public Transform Parent;

    public ObjectPoolInstance GetUnusedInstance()
    {
        var entry = PooledInstances.Values.ToList().Find(i => !i.IsUsed);
        if (entry == null)
        {
            return null;
        }
        entry.IsUsed = true;
        return entry;
    }

    public void ReturnInstance(GameObject obj)
    {
        ObjectPoolInstance entry = null;
        if (PooledInstances.TryGetValue(obj.GetInstanceID(), out entry))
        {
            entry.IsUsed = false;
            entry.Instance.SetActive(false);
            entry.PooleableInstance.OnAfterRecycle();
            ResetTransform(entry.Instance.transform);
            return;
        }

        Debug.LogError("No instance found for " + obj.name);
    }

    public bool IsSpawned(GameObject obj)
    {
        ObjectPoolInstance entry = null;
        if (PooledInstances.TryGetValue(obj.GetInstanceID(), out entry))
        {
            return entry.IsUsed;
        }

        Debug.LogError("No instance found for " + obj.name);
        return false;
    }

    private void ResetTransform(Transform transform)
    {
        transform.SetParent(Parent, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = PrefabObject.transform.localScale;
    }
}

public class ObjectPoolController : MonoBehaviour
{
    [SerializeField]
    private List<ObjectPoolEntry> _poolEntries;

    private List<ObjectPoolGroup> _poolGroup;
    private static string _cloneSuffix = "(Clone)";

    public static ObjectPoolController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _poolGroup = new List<ObjectPoolGroup>();
        foreach(var e in _poolEntries)
        {
            var groupContainer = new GameObject(e.PrefabObject.name);
            groupContainer.transform.parent = transform;
            var group = new ObjectPoolGroup
            {
                PrefabObject = e.PrefabObject,
                PooledInstances = new Dictionary<int, ObjectPoolInstance>(),
                Parent = groupContainer.transform
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
        var go = Instantiate(group.PrefabObject, transform, false);
        go.SetActive(false);
        go.name += "_" + group.PooledInstances.Count;
        go.transform.parent = group.Parent;
        var iPooleable = go.GetComponent(typeof(IPooleableObject)) as IPooleableObject;
        if (iPooleable == null)
        {
            Debug.LogError("IPooleableObject interface not found in " + go.name);
            return null;
        }

        var instance = new ObjectPoolInstance
        {
            IsUsed = false,
            Instance = go,
            PooleableInstance = iPooleable
        };

        group.PooledInstances[instance.Instance.GetInstanceID()] = instance;
        return instance;
    }

    public GameObject Spawn(GameObject prefabObject, Vector3 position, Quaternion rotation)
    {
        var group = _poolGroup.Find(g => g.PrefabObject.name == prefabObject.name);
        if (group == null)
        {
            Debug.LogError("Group not found! Name: " + prefabObject.name);
            return null;
        }

        ObjectPoolInstance instance = group.GetUnusedInstance();
        if (instance == null)
        {
            instance = CreateInstanceInGroup(group);
        }

        instance.Instance.SetActive(true);
        instance.Instance.transform.position = position;
        instance.Instance.transform.rotation = rotation;
        instance.PooleableInstance.OnSpawn();
        return instance.Instance;
    }

    public void Recycle(GameObject obj)
    {
        var name = obj.name;
        if (name.Contains(_cloneSuffix))
        {
            name = name.Substring(0, name.LastIndexOf(_cloneSuffix));
        }

        var group = _poolGroup.Find(g => g.PrefabObject.name == name);
        if (group != null)
        {
            group.ReturnInstance(obj);
            return;
        }

        Debug.LogError("Trying to recycle " + name + " and group not found!");
    }

    public void RecycleAfter(GameObject obj, float delayTime)
    {
        Instance.StartCoroutine(RecycleCoroutine(obj, delayTime));
    }

    private IEnumerator RecycleCoroutine(GameObject obj, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);

        Recycle(obj);
    }

    public bool IsSpawned(GameObject obj)
    {
        var pool = _poolGroup.Find(g => g.PrefabObject.name == obj.name);
        return pool != null && pool.IsSpawned(obj);
    }
}
