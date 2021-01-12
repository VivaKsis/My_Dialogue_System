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
	[NodeAttributes( "Unity/Input/Events" , "Listen to mouse input" , "eventNodeColor" , 1 ,  NodeAttributes.NodeType.Event)]
	public class OnMouseInput : Node
	{
		
		public enum ButtonState
		{
			pressed,
			down,
			up
		}
		
		public enum MouseButton
		{
			left,
			right,
			middle
		}
		
		public MouseButton selectedMouseButton;
		public ButtonState selectedButtonState;

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
			GUILayout.Label("Mouse button:");
			selectedMouseButton = (MouseButton)EditorGUILayout.EnumPopup(selectedMouseButton);
			GUILayout.Label("Mouse button state:");
			selectedButtonState = (ButtonState)EditorGUILayout.EnumPopup(selectedButtonState);
			
			outputNodes[0].id = selectedMouseButton.ToString() + " " + selectedButtonState.ToString();
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;
			
			switch (selectedButtonState)
			{
			case ButtonState.pressed:
				FlowReactorKeyInputListener.Instance.RegisterMousePressed(this);
				break;
			case ButtonState.down:
				FlowReactorKeyInputListener.Instance.RegisterMouseDown(this);
				break;
			case ButtonState.up:
				FlowReactorKeyInputListener.Instance.RegisterMouseUp(this);
				break;
			}
		}
		
		
		public void OnMouse(int _button)
		{
			switch(_button)
			{
			case 0:
				if (selectedMouseButton == MouseButton.left)
				{
					ExecuteNext(0, flowReactor);
				}
				break;
			case 1:
				if (selectedMouseButton == MouseButton.right)
				{
					ExecuteNext(0, flowReactor);
				}
				break;
			case 2:
				if (selectedMouseButton == MouseButton.middle)
				{
					ExecuteNext(0, flowReactor);
				}
				break;
			}
		}
	
	}
}