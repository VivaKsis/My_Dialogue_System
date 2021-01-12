using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ObjectPool;

public class ObjectPoolInitializer : MonoBehaviour
{
	[SerializeField] private PoolData[] _initializationPoolData;
	public PoolData[] _InitializationPoolData => this._initializationPoolData;

	private void Initialize(PoolData[] initializationData)
	{
		for (int a = 0; a < initializationData.Length; a++)
		{
			Pool pool = new Pool(
				originalInstance: initializationData[a]._OriginalInstance,
				initialCapacity: initializationData[a]._InitialPoolCapacity,
				dontDestroyOnLoad: initializationData[a]._DontDestroyOnLoad
			);

			//Debug.Log(this._instanceId_pool[initializationData[a]._OriginalInstance.GetInstanceID()]);
		}
	}

	private void Start()
	{
#if UNITY_EDITOR
		if (ObjectPool.Instance == null)
			Debug.LogError("Object pool instance is null. Make sure that `ObjectPool` is created before this instance.");
#endif

		this.Initialize(initializationData: this._initializationPoolData);
	}
}