using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Physics" , "Adds a torque to the rigidbody." , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class AddTorque : Node
	{
	
		public FRGameObject gameObject;
		public FRVector3 directionForce;
		public ForceMode forceMode;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("rotateIcon.png");
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = false;
			disableVariableInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}	
	
		public override void DrawCustomInspector()
		{
			GUILayout.Label("gameObject:");
			FRVariableGUIUtility.DrawVariable(gameObject, this, false, editorSkin);
			GUILayout.Label("direction force:");
			FRVariableGUIUtility.DrawVariable(directionForce, this, false, editorSkin);
			GUILayout.Label("force mode:");
			forceMode = (ForceMode)EditorGUILayout.EnumPopup(forceMode);
		}
		#endif
	
	
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			Rigidbody rb = gameObject.Value.GetComponent<Rigidbody>();
			
			rb.AddTorque(directionForce.Value.x, directionForce.Value.y, directionForce.Value.z, forceMode);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
