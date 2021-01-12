using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultEventsDispatcher : MonoBehaviour
{
	[SerializeField] private UnityEvent _onAwake;
	public UnityEvent _OnAwake => this._onAwake;

	private void Awake() => this._onAwake.Invoke();

	[SerializeField] private UnityEvent _onStart;
	public UnityEvent _OnStart => this._onStart;

	private void Start() => this._onStart.Invoke();

	[SerializeField] private UnityEvent _onUpdate;
	public UnityEvent _OnUpdate => this._onUpdate;

	private void Update() => this._onUpdate.Invoke();
}