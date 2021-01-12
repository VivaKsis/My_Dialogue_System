using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "Internal" , "Description" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Coroutine )]
	public class CoroutineNode : Node
	{
		public IEnumerator coroutine;
		public virtual IEnumerator OnExecuteCoroutine(FlowReactorComponent _flowReactor) { yield break; }
		public virtual IEnumerator OnExecuteCoroutine<T>(T _params, FlowReactorComponent _flowReactor) { yield break; }
		
	}
}