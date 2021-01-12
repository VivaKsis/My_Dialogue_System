using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipeline<T, TResult> : IPipeline<T, TResult>
{
	private List<Middleware<T, TResult>> _middlewares = new List<Middleware<T, TResult>>();

	public void Add(Middleware<T, TResult> middleware) => this._middlewares.Add(item: middleware);
	public void Remove(Middleware<T, TResult> middleware) => this._middlewares.Remove(item: middleware);

	public TResult Execute(T value)
	{
		TResult result = default;

		for (int a = 0; a < this._middlewares.Count; a++)
		{
			result = this._middlewares[a].Operate(value: value);
		}

		return result;
	}
}