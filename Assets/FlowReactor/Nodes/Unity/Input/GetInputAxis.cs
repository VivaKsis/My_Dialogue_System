using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Input", "GetInputAxis", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class GetInputAxis : Node
	{
	
		public FRString axis;	
		[Title("Store to:")]
		public FRFloat result;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("axisIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif

		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
	
			GetAxis(_flowReactor);
			
			ExecuteNext(0, _flowReactor);
		}
		
		void GetAxis(FlowReactorComponent _flowReactor)
		{
			result.Value = Input.GetAxis(axis.Value);
		}
	}
}