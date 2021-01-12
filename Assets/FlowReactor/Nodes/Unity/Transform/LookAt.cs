using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Transform", "LookAt", 1, NodeAttributes.NodeType.Normal)]
	public class LookAt : Node
	{
		
		public FRGameObject fromGameObject;
		public FRGameObject targetGameObject;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("lookIcon.png");
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
			
			fromGameObject.Value.transform.LookAt(targetGameObject.Value.transform); 
			
			
			ExecuteNext(0, _flowReactor);
		}
	}
}