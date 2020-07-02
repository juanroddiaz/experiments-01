using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooleableObject
{
    void OnSpawn();
    void OnAfterRecycle();
}
