using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using FlowReactor.EventSystem;

namespace FlowReactor.Nodes.Examples
{
	[NodeAttributes( "FlowReactor/Examples" , "This is an example node to show how to use the EventDispatcher module to raise a FlowReactor event with parameters" , "actionNodeColor" , new string[]{""} , NodeAttributes.NodeType.Normal )]
	public class Example_EventDispatcherNode : Node
	{
		// Eventboard reference for the event dispatcher module 
		public FlowReactor.EventSystem.EventBoard eventBoard;
		
		// Modules
		FRNodeEventDispatcher moduleNodeEventDispatcher = new FRNodeEventDispatcher();
		
		
		// Editor node methods
		#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = false;

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		
		// Draw custom node inspector
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Eventboard:");
			eventBoard = EditorGUILayout.ObjectField(eventBoard, typeof(EventBoard), false) as EventBoard;
		}
		
		#endif


		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
	
			////////////////////////
			moduleNodeEventDispatcher.RaiseEvent(eventBoard, "TestEvent", "this is a string", 66.6f);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}