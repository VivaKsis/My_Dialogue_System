using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultEventsDispatcherWithDelay : MonoBehaviour
{
	[SerializeField] private UnityEvent _onStart;
	public UnityEvent _OnStart => this._onStart;

	[SerializeField] private float _delay = 2.0f;
	public float _Delay => this._delay;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this._delay);

		this._onStart.Invoke();
	}
}
