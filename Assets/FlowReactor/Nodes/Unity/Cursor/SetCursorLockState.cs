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
	[NodeAttributes( "Unity/Cursor" , "Determines whether the hardware pointer is locked to the center of the view, constrained to the window, or not constrained at all." , "actionNodeColor" , 1 )]
	public class SetCursorLockState : Node
	{
		public CursorLockMode lockMode;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			icon = EditorHelpers.LoadIcon("cursorIcon.png");
		
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = false;
			disableVariableInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Lock mode");
			lockMode = (CursorLockMode) EditorGUILayout.EnumPopup(lockMode);
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
			Cursor.lockState = lockMode;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}