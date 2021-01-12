using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Gets added by the HandIK node

namespace FlowReactor.Nodes
{
	public class AnimatorIKUpdate : MonoBehaviour
	{
		
		public List<FlowReactor.Nodes.HandIK> nodes = new List<FlowReactor.Nodes.HandIK>();
		
		public void Awake()
		{
			nodes = new List<FlowReactor.Nodes.HandIK>();
		}
		
		public void Register(FlowReactor.Nodes.HandIK _node)
		{
			nodes.Add(_node);
		}
		
		public void OnAnimatorIK()
		{
			for (int i = 0; i < nodes.Count; i ++)
			{
				if (nodes[i] != null)
				{
					nodes[i].OnIKUpdate();
				}
			}
		}
	}
}
