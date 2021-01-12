using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Flow" , "Stops all running coroutine nodes in this graph" , "actionNodeColor" , new string[]{""} , NodeAttributes.NodeType.Normal )]
	public class StopAllCoroutines : Node
	{

		// Editor node methods
		#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = true;

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
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
			
			foreach (var routine in _flowReactor.executingCoroutines.Keys)
			{
				_flowReactor.StopCoroutine(_flowReactor.executingCoroutines[routine].coroutine);
				_flowReactor.executingCoroutines[routine].node.highlightStartTime = 0f;			
			}
			
			_flowReactor.executingCoroutines = new Dictionary<int, FlowReactorComponent.CoroutineNodes>();
			
			ExecuteNext(0, _flowReactor);
		}
	}
}