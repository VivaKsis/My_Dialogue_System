using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Indicator<TValue, TOnChangeEvent> : MonoBehaviour
	where TOnChangeEvent : UnityEvent<TValue>
{
	[SerializeField] protected TValue minValue;
	public TValue _MinValue => this.minValue;

	[SerializeField] protected TValue maxValue;
	public TValue _MaxValue => this.maxValue;

	[SerializeField] protected TOnChangeEvent _onValueChange;
	public TOnChangeEvent _OnValueChange => this._onValueChange;

	protected abstract TValue Clamp(TValue value, TValue min, TValue max);

	[SerializeField] private TValue _value;
	public TValue Value
	{
		get => this._value;
		set
		{
			this._value = this.Clamp(value: value, min: this.minValue, max: this.maxValue);

			this._OnValueChange.Invoke(arg0: this._value);
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		this._value = this.Clamp(value: this._value, min: this.minValue, max: this.maxValue);
	}
#endif
}
