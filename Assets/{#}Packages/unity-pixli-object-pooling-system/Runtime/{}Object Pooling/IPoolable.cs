using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectPool;

public interface IPoolable
{
	GameObject GameObject { get; }
	Pool Pool { get; set; }

	void OnRelease();
	void OnAquire();
}
