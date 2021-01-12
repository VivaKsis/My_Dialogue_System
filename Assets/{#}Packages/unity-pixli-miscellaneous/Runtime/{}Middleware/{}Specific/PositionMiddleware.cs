using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMiddleware : Middleware<GameObject, GameObject>
{
	private readonly Vector3 _position;

	public override GameObject Operate(GameObject value)
	{
		value.transform.position = this._position;

		return value;
	}

	public PositionMiddleware(Vector3 position) => this._position = position;
}
