using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Events", "On Start", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnStart : Node
	{
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			icon = EditorHelpers.LoadIcon("startIcon.png");	
			
			// If hide input is true
			// no other node can be connected to this node
			hideInput = true;
			
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnGraphStart(FlowReactorComponent _flowReactor)
		{
			ExecuteNext(0, _flowReactor);
		}
	}
}