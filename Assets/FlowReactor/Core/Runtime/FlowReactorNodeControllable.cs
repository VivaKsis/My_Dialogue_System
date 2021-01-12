using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

// A simple example of a INodeControllable Monobehaviour script.
// Make sure a node with an INodeControllable field is exists.
// In the FlowReactorComponent click on scan nodes and assign this script to the INodeControllable field.
// You can assign Unity Events in the inspector to call other MB methods
namespace FlowReactor
{
	
	public class FlowReactorNodeControllable : MonoBehaviour, INodeControllable
	{
		public UnityEvent onNodeInititalizeEvent;
		public UnityEvent onNodeExecuteEvent;
		public UnityEvent onNodeStopExecuteEvent;
		
		
		public void OnNodeInitialize(Node _node)
		{ 
			onNodeInititalizeEvent.Invoke();
		}
		
		public void OnNodeExecute()
		{
			onNodeExecuteEvent.Invoke();
		}
		
		public void OnNodeStopExecute()
		{
			onNodeStopExecuteEvent.Invoke();
		}
		
		public void OnNode(Node _node,  object[] _parameters){}
	}
}
