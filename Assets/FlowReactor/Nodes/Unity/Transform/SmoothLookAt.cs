using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Transform", "SmoothLookAt", 1, NodeAttributes.NodeType.Normal)]
	public class SmoothLookAt : Node
	{
		
		public FRGameObject gameObject;
		public FRGameObject targetGameObject;
		public FRFloat speed;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("smoothLookIcon.png");
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
			SmoothLook();
			
			ExecuteNext(0, _flowReactor);
		}
		
		void SmoothLook()
		{
			var targetRotation = Quaternion.LookRotation(targetGameObject.Value.transform.position - gameObject.Value.transform.position);
	       
			// Smoothly rotate towards the target point.
			gameObject.Value.transform.rotation = Quaternion.Slerp(gameObject.Value.transform.rotation, targetRotation, speed.Value * Time.deltaTime); 
		}
	}
}