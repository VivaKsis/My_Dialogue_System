using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventInt : UnityEvent<int> { }

public class DiscreteIndicator : Indicator<int, UnityEventInt>
{
	protected override int Clamp(int value, int min, int max) => Mathf.Clamp(value: value, min: min, max: max);
}
