using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Debug", "Draw ray in forward direction", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class DebugDrawRay : Node
	{

		public FRVector3 fromVector3;
		public FRVector3 toVector3;
		
		public FRFloat distance;
		public FRColor rayColor;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("debugIcon.png");
	

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
	
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
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
			
			Vector3 _forward = Vector3.zero;

			_forward = (toVector3.Value - fromVector3.Value);
			
			Debug.DrawRay(fromVector3.Value, _forward, rayColor.Value);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}