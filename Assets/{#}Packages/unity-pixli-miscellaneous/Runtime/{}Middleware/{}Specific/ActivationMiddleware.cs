using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationMiddleware : Middleware<GameObject, GameObject>
{
	private readonly bool _active;

	public override GameObject Operate(GameObject value)
	{
		value.SetActive(value: this._active);

		return value;
	}

	public ActivationMiddleware(bool active) => this._active = active;
}
