using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Input", "GetInputButton", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class GetInputButton : Node
	{
	
		public FRString button;	
		[Title("Store to:")]
		public FRBoolean result;
		
		public enum InputType
		{
			hold,
			down,
			up
		}
		
		public InputType inputType;
		
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("buttonIcon.png");
			
			disableDefaultInspector = true;
			disableDrawCustomInspector = false;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		
		public override void DrawCustomInspector()
		{
			
			GUILayout.Label("input type");
			inputType = (InputType)EditorGUILayout.EnumPopup(inputType);
			
		}
		#endif	
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			GetButton(_flowReactor);
			
			ExecuteNext(0, _flowReactor);
		}
		
		
		void GetButton(FlowReactorComponent _flowReactor)
		{
		
			switch (inputType)
			{
				case InputType.hold:
					result.Value = Input.GetButton(button.Value);
					break;
				case InputType.down:
					result.Value = Input.GetButtonDown(button.Value);
					break;
				case InputType.up:
					result.Value = Input.GetButtonUp(button.Value);
					break;
			}
		}
	}
}