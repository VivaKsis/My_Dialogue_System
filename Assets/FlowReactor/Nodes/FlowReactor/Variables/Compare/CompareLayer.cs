using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Compare", "Compare gameobject with Layer", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class CompareLayer : Node
	{
		
		public FRGameObject gameObject;
		[Title("Has layer:")]
		public FRLayerMask layer;
		
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
			if(((1 << gameObject.Value.layer) & layer.Value) != 0)
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