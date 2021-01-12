using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionalLayout : MonoBehaviour
{
	//TODO: Finish summaries for the whole struct.
	/// <summary>
	/// 
	/// </summary>
	private struct PositionalPoint
	{
		[SerializeField] private RectTransform _point;
		public RectTransform _Point => this._point;
		
		[SerializeField] private RectTransform _child;
		public RectTransform _Child => this._child;

		public void AcceptChild(RectTransform rectTransform)
		{
			this._child = rectTransform;

			this._child.SetParent(this._point);
		}

		public void RejectChild()
		{
			if (this._child != null)
			{
				this._child.SetParent(null);

				this._child = null;
			}
#if UNITY_DEBUG
			else
				Debug.LogWarning("[Attempt to reject child.] No child is present.");
#endif
		}
	}

	[SerializeField] private PositionalPoint[] _positionalPoints;
	//public PositionalPoint[] _PositionalPoints => this._positionalPoints;

	//TODO: Finish summary.
	/// <summary>
	/// 
	/// </summary>
	/// <param name="rectTransform"></param>
	/// <returns>Whether item was successfully pushed onto the stack or not.</returns>
	public bool Push(RectTransform rectTransform)
	{
		for (int a = 0; a < this._positionalPoints.Length; a++)
		{
			if (this._positionalPoints[a]._Child == null)
			{
				this._positionalPoints[a].AcceptChild(rectTransform);

				return true;
			}
		}

#if UNITY_DEBUG
		Debug.LogWarning("[Attempt to push item to position.] There are no positional points left to place an item.");
#endif

		return false;
	}

	//TODO: Finish summary.
	/// <summary>
	/// 
	/// </summary>
	/// <returns>Whether item was successfully popped from the stack or not.</returns>
	public bool Pop()
	{
		for (int a = this._positionalPoints.Length - 1; a >= 0; a--)
		{
			if (this._positionalPoints[a]._Child != null)
			{
				this._positionalPoints[a].RejectChild();

				return true;
			}
		}

#if UNITY_DEBUG
		Debug.LogWarning("[Attempt to pop item from position.] All positional points are free.");
#endif

		return false;
	}
}
