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
	[NodeAttributes( "Dialogue Management" , "Triggers when an answer is chosen" , "eventNodeColor" , new string[]{""} , NodeAttributes.NodeType.Event )]
	public class OnAnswerChosen : Node
	{
		[HideInNode]
		public FRInt Index;

		private FlowReactorComponent flowReactor;

		// Editor node methods
#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = false;
			disableVariableInspector = false;
			disableDrawCustomInspector = false;

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
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;

			EventManager.Instance.onGoToAnswer += AnswerChosen;

			// Do not remove this line
			base.OnInitialize(_flowReactor);
		}

		public void AnswerChosen(int index)
		{
			EventManager.Instance.onGoToAnswer -= AnswerChosen;

			Index.Value = index;
			ExecuteNext(0, flowReactor);
		}
	}
}