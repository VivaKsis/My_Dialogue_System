using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Compare", "Compares object with tag", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class CompareTag : Node
	{
		
		public FRGameObject gameObject;
		[Title("Has tag:")]
		public FRString tag;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("debugIcon.png");
			
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			disableVariableInspector = false;
			
			outputNodes[0].id = "True";
			outputNodes[1].id = "False";
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif

		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			if(gameObject.Value.tag == tag.Value)
			{
				ExecuteNext(0, _flowReactor);
			}
			else
			{
				ExecuteNext(1, _flowReactor);
			}
		}
	}
}