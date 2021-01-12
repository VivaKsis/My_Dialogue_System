using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.NodeUtilityModules
{
	[NodeModule("Allows to control a MonoBehaviour script which implements an INodeControllable interface.")]
	public class FRNodeControllable : FRNodeUtilityModule
	{
	
		public FRNodeControllable() {}
		
		/// <summary>
		/// Manually call the INodeControllable method OnNodeInitialize
		/// </summary>
		/// <param name="_flowReactor"></param>
		/// <param name="_node"></param>
		public void CallInitializeNode(FlowReactorComponent _flowReactor, Node _node)
		{
			_flowReactor.NodeControllable_OnNodeInitialization(_node);
		}
		
		/// <summary>
		/// Manually call the INodeControllable method OnNodeExecute
		/// </summary>
		/// <param name="_flowReactor"></param>
		/// <param name="_node"></param>
		public void CallStartExecuteNode(FlowReactorComponent _flowReactor, Node _node)
		{
			_flowReactor.NodeControllable_OnNodeExecute(_node);
		}
		
		/// <summary>
		/// Manually call the INodeControllable method OnNodeStopExecute
		/// </summary>
		/// <param name="_flowReactor"></param>
		/// <param name="_node"></param>
		public void CallStopExecuteNode(FlowReactorComponent _flowReactor, Node _node)
		{
			_flowReactor.NodeControllable_OnNodeStopExecute(_node);
		}
		
		/// <summary>
		/// Manually call the INodeControllable method OnNode
		/// </summary>
		/// <param name="_flowReactor"></param>
		/// <param name="_node"></param>
		public void CallOnNode(FlowReactorComponent _flowReactor, Node _node, params object[] _parameters)
		{
			_flowReactor.NodeControllable_OnNode(_node, _parameters);
		}
	}
}