using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Compare", "CompareBool", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class CompareBool : Node
	{
		
		public FRBoolean compare;
		public FRBoolean with;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("debugIcon.png");

			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
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
			
			if (compare.Value == with.Value)
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