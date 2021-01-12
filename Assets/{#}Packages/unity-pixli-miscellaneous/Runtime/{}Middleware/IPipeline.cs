using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPipeline<in T, out TResult>
{
	TResult Execute(T value);
}
