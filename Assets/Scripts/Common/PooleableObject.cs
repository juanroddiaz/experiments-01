using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooleableObject
{
    void SetPool(ObjectPoolController pool);
    void OnSpawn();
    void OnRecycle();
}
