using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Graph/Events", "OnEnterGraph", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnEnterGraph : Node
	{
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			icon = EditorHelpers.LoadIcon("portalIcon.png");

			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			disableVariableInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			ExecuteNext(0, _flowReactor);
		}
	}
}
