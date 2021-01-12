using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    #region Pool
    public GameObject GameObject => this.gameObject;
    public ObjectPool.Pool Pool { get; set; }

    public void OnRelease()
    {
        
    }

    public void OnAquire()
    {
    }
    #endregion
}
