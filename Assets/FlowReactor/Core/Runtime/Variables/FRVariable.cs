//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	Base class for all FRVariable types used in FlowReactor
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if FLOWREACTOR_DATABOX
using Databox;
#endif

using FlowReactor.Nodes;
using FlowReactor.OrderedDictionary;

namespace FlowReactor
{

	[System.Serializable]
	public class FRVariable
	{ 
		public enum VariableType
		{
			local,
			blackboard,
			exposed
		}
		
		public VariableType type = VariableType.local;
		
		public bool isCopy;
		
		public Graph _graph;
		public Graph graph
		{
			get 
			{
				return _graph;
			}
			set
			{
				_graph = value;
			}
		}
		
		// Exposed
		public Node nodeOwner;
		public string exposedNodeName;
		public string exposedName;
		public bool editExposedName;
		
		// Blackboard
		public FlowReactor.BlackboardSystem.BlackBoard assignedBlackboard;
		public string blackboardGuid; // assigned blackboard guid
		public string variableGuid; // assigned variable guid
		
		// Databox
		public bool useDatabox = false;
		public string databoxID;
		public string tableID;
		public string entryID;
		public string valueID;
		
		public bool overrideVariable;
		public string name;
		public string typeName;
		
		public bool showError;
		
		// if true the variable should only accept scene ref objects.
		// wjem
		public bool sceneReferenceOnly;
		
		
		// used to draw variable in inspector
		public virtual void Draw(bool _allowSceneObject, object[] _attributes){}
		// used to draw variable in blackboard list
		public virtual void Draw(Rect rect){}

		// OBSOLETE!!!
		//public virtual void Draw(){}
		//public virtual void Draw(bool _allowSceneObject){}
		
		public virtual float DrawReturnHeight(){return 0f;}
		public virtual float GetGUIHeight(){return  19; }
		
		// Keep track of all nodes which are connected to this variable (for blackboard variables)
		public List<FlowReactor.Nodes.Node> connectedNodes;
		public bool showConnectedNodes;
		
		public delegate void ValueChanged(FRVariable _data);
		public ValueChanged OnValueChanged;
		
		
		public FRVariable()	{	}
	}
}