using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour, IPoolable
{
	public GameObject GameObject => this.gameObject;
	public ObjectPool.Pool Pool { get; set; }

	public void OnRelease()
	{
		this.gameObject.SetActive(value: false);
	}

	public void OnAquire()
	{
		this.gameObject.SetActive(value: true);
	}
}
