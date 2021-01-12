using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiddleware<in T, out TResult>
{
	TResult Operate(T value);
}
