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
	[NodeAttributes( "Unity/Input/Events" , "Listen to a specific key input" , "eventNodeColor" , 1 , NodeAttributes.NodeType.Event)]
	public class OnKeyInput : Node
	{
		public KeyCode key;
		
		public enum KeyState
		{
			pressed,
			down,
			up
		}
		
		public KeyState selectedKeyState;
		
		FlowReactorComponent flowReactor;
		
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		
		// Draw custom node inspector
		public override void DrawCustomInspector()
		{
			GUILayout.Label("KeyCode:");
			key = (KeyCode) EditorGUILayout.EnumPopup(key);
			GUILayout.Label("KeyState:");
			selectedKeyState = (KeyState) EditorGUILayout.EnumPopup(selectedKeyState);
			
		
			outputNodes[0].id = key.ToString();
			
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;
			
			// Register to Key input listener
			switch (selectedKeyState)
			{
				case KeyState.pressed:
					FlowReactorKeyInputListener.Instance.RegisterKeyPressed(this);
					break;
				case KeyState.down:
					FlowReactorKeyInputListener.Instance.RegisterKeyDown(this);
					break;
				case KeyState.up:
					FlowReactorKeyInputListener.Instance.RegisterKeyUp(this);
					break;
			}
		}
		
		// Called by the Key input listener singleton
		public void OnKey(KeyCode _keyCode)
		{
			if (_keyCode == key)
			{
				ExecuteNext(0, flowReactor);
			}
		}
		
	}
}