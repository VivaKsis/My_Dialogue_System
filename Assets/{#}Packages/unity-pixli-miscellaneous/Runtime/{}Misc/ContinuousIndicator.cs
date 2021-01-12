using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//TODO: move it out.
[Serializable]
public class UnityEventFloat : UnityEvent<float> { }

public class ContinuousIndicator : Indicator<float, UnityEventFloat>
{
	protected override float Clamp(float value, float min, float max) => Mathf.Clamp(value: value, min: min, max: max);
}
