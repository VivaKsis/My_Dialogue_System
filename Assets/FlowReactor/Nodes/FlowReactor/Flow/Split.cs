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
	[NodeAttributes( "FlowReactor/Flow" , "splits the current flow into two." , "flowNodeColor" , 2 , NodeAttributes.NodeType.Normal )]
	public class Split : Node
	{

		// Editor node methods
		#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("splitIcon.png");
			
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif


		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////	
			ExecuteNext(0, _flowReactor);
			ExecuteNext(1, _flowReactor);
		
		}
		
	}
}