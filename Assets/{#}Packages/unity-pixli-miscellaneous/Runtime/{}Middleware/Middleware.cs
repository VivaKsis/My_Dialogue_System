using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Middleware<T, TResult> : IMiddleware<T, TResult>
{
	public abstract TResult Operate(T value);
}
