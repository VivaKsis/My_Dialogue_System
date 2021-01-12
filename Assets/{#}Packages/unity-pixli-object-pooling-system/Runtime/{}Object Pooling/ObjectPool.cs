using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ObjectPool : MonoBehaviourSingleton<ObjectPool>
{
	[Serializable]
	public struct PoolData
	{
		[SerializeField] private GameObject _originalInstance;
		public GameObject _OriginalInstance => this._originalInstance;

		[SerializeField] private int _initialPoolCapacity;
		public int _InitialPoolCapacity => this._initialPoolCapacity;

		[SerializeField] private bool _dontDestroyOnLoad;
		public bool _DontDestroyOnLoad => this._dontDestroyOnLoad;

		public PoolData(GameObject originalInstance, int initialPoolCapacity, bool dontDestroyOnLoad)
		{
			this._originalInstance = originalInstance;
			this._initialPoolCapacity = initialPoolCapacity;
			this._dontDestroyOnLoad = dontDestroyOnLoad;
		}
	}

	public class Pool
	{
		public GameObject OriginalInstance { get; }

		public int InitialCapacity { get; }
		public int InstanceId { get; }
		public bool DontDestroyOnLoad { get; }

		private List<IPoolable> _poolables;
		private List<IPoolable> _aquiredPoolables;
		private Queue<IPoolable> _releasedPoolables;

		public Pipeline<GameObject, GameObject> ReleasePipeline { get; private set; } = new Pipeline<GameObject, GameObject>();
		public Pipeline<GameObject, GameObject> AcquirePipeline { get; private set; } = new Pipeline<GameObject, GameObject>();

		/// <summary>
		/// Populates poolables queue with new instances of `_originalInstance`.
		/// </summary>
		/// <param name="quantity">Quantity of new instances.</param>
		private void Populate(int quantity, bool dontDestroyOnLoad)
		{
			for (int b = 0; b < quantity; b++)
			{
				IPoolable poolable = Object.Instantiate(original: this.OriginalInstance).GetComponent<IPoolable>();

				if (dontDestroyOnLoad)
					Object.DontDestroyOnLoad(target: poolable.GameObject);

				poolable.Pool = this;

				poolable.OnRelease();

				this.ReleasePipeline.Execute(value: poolable.GameObject);

				this._poolables.Add(item: poolable);
				this._releasedPoolables.Enqueue(item: poolable);
			}
		}

		/// <summary>
		/// "Releases"[adds object back to the queue] object back to the pool.
		/// Calls `OnRelease` of the `poolable`.
		/// </summary>
		/// <param name="poolable"></param>
		internal void Release(IPoolable poolable)
		{
			poolable.OnRelease();

			this.ReleasePipeline.Execute(value: poolable.GameObject);


			this._aquiredPoolables.Remove(item: poolable);
			this._releasedPoolables.Enqueue(item: poolable);
		}

		/// <summary>
		/// "Aquires"[dequeues object from the queue] object from the pool.
		/// Calls `OnAquire` of the `poolable`.
		/// </summary>
		/// <returns></returns>
		internal IPoolable Aquire()
		{
			if (this._releasedPoolables.Count == 0)
				this.Populate(quantity: this.InitialCapacity, dontDestroyOnLoad: this.DontDestroyOnLoad);

			IPoolable poolable = this._releasedPoolables.Dequeue();

			poolable.OnAquire();

			this.AcquirePipeline.Execute(value: poolable.GameObject);

			this._aquiredPoolables.Add(item: poolable);

			return poolable;
		}

		private void Configure()
		{
			this.ReleasePipeline.Add(
				middleware: new ActivationMiddleware(active: false)
			);
			this.ReleasePipeline.Add(
				middleware: new PositionMiddleware(position: Vector3.zero)
			);

			this.AcquirePipeline.Add(
				middleware: new ActivationMiddleware(active: true)
			);
		}

		private void Destroy()
		{
			this.OriginalInstance.GetComponent<IPoolable>().Pool = null;

			ObjectPool.Instance._instanceId_pool.Remove(key: this.InstanceId);

			SceneManager.sceneLoaded -= this.OnSceneLoaded;
			SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
			SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
#if UNITY_DEBUG
			int iteration = this._unityDebugSceneLoadedIteration++;

			Debug.Log($"[Loaded] Invoker: {this.InstanceId}: name - {this.OriginalInstance}");
			foreach (KeyValuePair<int, Pool> item in ObjectPool.Instance._instanceId_pool)
			{
				Debug.Log($"[{iteration}] OnSceneLoaded[{scene.path}]: key[{item.Key}]: OriginalInstance[{item.Value.OriginalInstance}] & InstanceId[{item.Value.InstanceId}]");
			}
#endif
		}

		//TODO: make it work with additively added scenes workflow.
		private void OnSceneUnloaded(Scene scene)
		{
#if UNITY_DEBUG
			int iteration = this._unityDebugSceneUnloadedIteration++;

			Debug.Log($"[Unloaded] Invoker: {this.InstanceId}: name - {this.OriginalInstance}");
			foreach (KeyValuePair<int, Pool> item in ObjectPool.Instance._instanceId_pool)
			{
				Debug.Log($"[{iteration}] OnSceneUnloaded[{scene.path}]: key[{item.Key}]: OriginalInstance[{item.Value.OriginalInstance}] & InstanceId[{item.Value.InstanceId}]");
			}
#endif

			if (!this.DontDestroyOnLoad)
				this.Destroy();
			else
			{
				for (int a = 0; a < this._aquiredPoolables.Count; a++)
				{
					this.Release(
						poolable: this._aquiredPoolables[a]
					);
				}
			}
		}

		private void OnActiveSceneChanged(Scene oldScene, Scene activeScene)
		{
#if UNITY_DEBUG
			int iteration = this._unityDebugActiveSceneChangedIteration++;

			Debug.Log($"[{iteration}] Active Scene changed!!! For instance - {this.OriginalInstance}.\nOld: {oldScene.path} Active: {activeScene.path}.");
#endif

			//? That is not true, they keep references and you don't have to resubscribe.
			////? Once the scene has been unloaded event is unloaded along with it. Aka scene instance was deleted and event along with it.
			////SceneManager.sceneLoaded += this.OnSceneLoaded;

			////? `SceneManager.sceneUnloaded` is expendable event because it belongs to the scene that is being unloaded.
			////SceneManager.sceneUnloaded += this.OnSceneUnloaded;
		}

		//TODO: what if you want a part to be deleted on load and some part not.
		public Pool(GameObject originalInstance, int initialCapacity, bool dontDestroyOnLoad)
		{
			this.OriginalInstance = originalInstance;

			this.InitialCapacity = initialCapacity;
			this.InstanceId = this.OriginalInstance.GetInstanceID();
			this.DontDestroyOnLoad = dontDestroyOnLoad;

			if (ObjectPool.Instance._instanceId_pool.ContainsKey(key: this.InstanceId))
				return;

			this.Configure();

			IPoolable originalPoolable = this.OriginalInstance.GetComponent<IPoolable>();
#if UNITY_EDITOR
			if (originalPoolable == null)
				Debug.LogError("Object must implement `IPoolable` in order to be added to the ObjectPool.");
#endif
			originalPoolable.Pool = this;

			//Debug.Log(originalPoolable.GameObject.GetInstanceID());

			this._poolables = new List<IPoolable>(capacity: this.InitialCapacity);
			this._aquiredPoolables = new List<IPoolable>(capacity: this.InitialCapacity);
			this._releasedPoolables = new Queue<IPoolable>(capacity: this.InitialCapacity);

			this.Populate(quantity: this.InitialCapacity, dontDestroyOnLoad: this.DontDestroyOnLoad);

			ObjectPool.Instance._instanceId_pool.Add(
				key: this.InstanceId,
				value: this
			);

			SceneManager.sceneLoaded += this.OnSceneLoaded;
			SceneManager.sceneUnloaded += this.OnSceneUnloaded;
			SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		}

		public Pool(PoolData poolData)
			: this(
				originalInstance: poolData._OriginalInstance,
				initialCapacity: poolData._InitialPoolCapacity,
				dontDestroyOnLoad: poolData._DontDestroyOnLoad)
		{ }

		//public Pool(GameObject originalInstance)
		//	: this(
		//		originalInstance: originalInstance,
		//		initialCapacity: ObjectPool.Instance._defaultInitialPoolCapacity,
		//		dontDestroyOnLoad: true)
		//{}

#if UNITY_DEBUG
		private int _unityDebugSceneLoadedIteration;
		private int _unityDebugSceneUnloadedIteration;
		private int _unityDebugActiveSceneChangedIteration;
#endif
#if UNITY_EDITOR
#endif
	}

	[SerializeField] private int _defaultInitialPoolCapacity = 8;
	public int _DefaultInitialPoolSize => this._defaultInitialPoolCapacity;

	/// <summary>
	/// "Releases" object back to its respective pool.
	/// </summary>
	/// <param name="poolable">Object to release back to the pool.</param>
	public void Release(IPoolable poolable) => poolable.Pool.Release(poolable: poolable);

	/// <summary>
	/// "Releases" object back to its respective pool.
	/// </summary>
	/// <param name="gameObject">GameObject to release back to the pool.</param>
	public void Release(GameObject gameObject)
	{
		IPoolable poolable = gameObject.GetComponent<IPoolable>();

		poolable.Pool.Release(poolable: poolable);
	}

	/// <summary>
	/// "Aquires" object from its respective pool.
	/// Creates a new `Pool` and takes `poolable.GameObject` as its original instance in case the `Pool` is null.
	/// Pools using original instance id.
	/// Recommended to pass `poolable` that already has a pool unless you want to create a new one and use this one as original instance.
	/// </summary>
	/// <param name="poolable">Original instance or poolable with pool.</param>
	/// <returns>GameObject instance of aquired poolable.</returns>
	public GameObject Aquire(IPoolable poolable)
	{
#if UNITY_EDITOR
		if (poolable.Pool == null)
		{
			if (this._editorOnlyAddPoolsAutomatically)
			{
				new Pool(
					originalInstance: poolable.GameObject,
					initialCapacity: this._defaultInitialPoolCapacity,
					dontDestroyOnLoad: false
				);
			}
			else
			{
				//const char backspace = '\b';

				Debug.LogError(
	message: $@"IPoolable - {poolable.GameObject} InstanceId: {poolable.GameObject.GetInstanceID()} doesn't have a Pool instance. Make sure you added pool and it has instances related to this poolable."
				);
			}
		}
#endif

		return poolable.Pool.Aquire().GameObject;
	}

	/// <summary>
	/// "Aquires" object from its respective pool.
	/// Creates a new `Pool` and takes `gameObject` as its original instance in case the `Pool` is null.
	/// Pools using original instance id.
	/// Recommended to pass `poolable` that already has a pool unless you want to create a new one and use this one as original instance.
	/// </summary>
	/// <param name="gameObject">Original instance or poolable with pool.</param>
	/// <returns>GameObject instance of aquired poolable.</returns>
	public GameObject Aquire(GameObject gameObject) => this.Aquire(poolable: gameObject.GetComponent<IPoolable>());

	/// <summary>
	/// "Aquires" component from its respective pool and.
	/// Creates a new `Pool` and takes `poolable.GameObject` as its original instance in case the `Pool` is null.
	/// Pools using original instance id.
	/// Recommended to pass `poolable` that already has a pool unless you want to create a new one and use this one as original instance.
	/// </summary>
	/// <typeparam name="TObject">Component type.</typeparam>
	/// <param name="poolable">Original instance or poolable with pool.</param>
	/// <returns>Component instance of aquired poolable.</returns>
	public TObject Aquire<TObject>(IPoolable poolable) => this.Aquire(poolable: poolable).GetComponent<TObject>();

	/// <summary>
	/// "Aquires" component from its respective pool and.
	/// Creates a new `Pool` and takes `poolable.GameObject` as its original instance in case the `Pool` is null.
	/// Pools using original instance id.
	/// Recommended to pass `poolable` that already has a pool unless you want to create a new one and use this one as original instance.
	/// </summary>
	/// <typeparam name="TObject">Component type.</typeparam>
	/// <param name="gameObject">Original instance or poolable with pool.</param>
	/// <returns>Component instance of aquired poolable.</returns>
	public TObject Aquire<TObject>(GameObject gameObject) => this.Aquire(gameObject: gameObject).GetComponent<TObject>();

	private Dictionary<int, Pool> _instanceId_pool;

	//TODO: summary.
	public bool HasPool(IPoolable poolable) => this._instanceId_pool.ContainsKey(poolable.GameObject.GetInstanceID());

	//TODO: fix summary.
	/// <summary>
	/// 
	/// Pool can also be added this way: `new ObjectPool.Pool(poolData: new ObjectPool.PoolData(...))`.
	/// </summary>
	/// <param name="poolData"></param>
	public void AddPool(PoolData poolData) => new Pool(poolData: poolData);

	protected override void Awake()
	{
		base.Awake();

		this._instanceId_pool = new Dictionary<int, Pool>();
	}

#if UNITY_DEBUG
#endif
#if UNITY_EDITOR
	[Space]
	[Header("Editor Only")]
	[SerializeField] private bool _editorOnlyAddPoolsAutomatically;
	public bool _EditorOnlyAddPoolsAutomatically => this._editorOnlyAddPoolsAutomatically;
#endif
}
