using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.NodeUtilityModules
{
	public interface INodeControllable
	{
		// Automatically called on node initialization
		void OnNodeInitialize(Node _node);
		// Automatically called on node execution
		void OnNodeExecute();
		// Automatically called on node stop execution
		void OnNodeStopExecute();
		// Method for manual controllable method calls
		void OnNode(Node _node, params object[] _parameters);
	}
}