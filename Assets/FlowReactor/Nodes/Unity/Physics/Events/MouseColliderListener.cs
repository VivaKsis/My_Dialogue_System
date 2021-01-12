using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowReactor.Nodes.Unity
{
	public class MouseColliderListener : MonoBehaviour
	{
		public List<OnMouseDown> onMouseDownNodes = new List<OnMouseDown>();
		public List<OnMouseUp> onMouseUpNodes = new List<OnMouseUp>();
		public List<OnMouseOver> onMouseOverNodes = new List<OnMouseOver>();
		public List<OnMouseDrag> onMouseDragNodes = new List<OnMouseDrag>();
		public List<OnMouseEnter> onMouseEnterNodes = new List<OnMouseEnter>();
		public List<OnMouseExit> onMouseExitNodes = new List<OnMouseExit>();
		
		void OnMouseDown()
		{
			for (int i = 0; i < onMouseDownNodes.Count; i ++)
			{
				if (onMouseDownNodes[i] != null)
				{
					onMouseDownNodes[i].OnDown();
				}
			}
		}
		
		void OnMouseUp()
		{
			for (int i = 0; i < onMouseUpNodes.Count; i ++)
			{
				if (onMouseUpNodes[i] != null)
				{
					onMouseUpNodes[i].OnUp();
				}
			}
		}
		
		void OnMouseDrag()
		{
			for (int i = 0; i < onMouseDragNodes.Count; i ++)
			{
				if (onMouseDragNodes[i] != null)
				{
					onMouseDragNodes[i].OnDrag();
				}
			}
		}
		
		void OnMouseEnter()
		{
			for (int i = 0; i < onMouseEnterNodes.Count; i ++)
			{
				if (onMouseEnterNodes[i] != null)
				{
					onMouseEnterNodes[i].OnEnter();
				}
			}
		}
		
		void OnMouseExit()
		{
			for (int i = 0; i < onMouseExitNodes.Count; i ++)
			{
				if (onMouseExitNodes[i] != null)
				{
					onMouseExitNodes[i].OnExit();
				}
			}
		}
		
		void OnMouseOver()
		{
			for (int i = 0; i < onMouseOverNodes.Count; i ++)
			{
				if (onMouseOverNodes[i] != null)
				{
					onMouseOverNodes[i].OnOver();
				}
			}
		}
	}
}
