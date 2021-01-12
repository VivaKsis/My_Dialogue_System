using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Transform" , "Applies a rotation of eulerAngles to gameObject" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class Rotate : Node
	{
		
		public FRGameObject gameObject;
		public FRFloat xAxis;
		public FRFloat yAxis;
		public FRFloat zAxis;
		public FRFloat speed;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("rotateIcon.png");
	
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
			gameObject.Value.transform.Rotate(new Vector3(xAxis.Value, yAxis.Value, zAxis.Value) * speed.Value * Time.deltaTime);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}