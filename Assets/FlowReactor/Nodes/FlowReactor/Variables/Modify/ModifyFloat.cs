using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Modify", "ModifyFloat", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class ModifyFloat : Node
	{
		
		public FRFloat value;
		public FRFloat modifyWith;
		
		string[] operators = new string[]{"equal", "add", "subtract", "multiply", "divide"};
		
		[SerializeField]
		int selectedOperator = 0;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("editIcon.png");
			
			disableDefaultInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Operator:");
			
			
			using (new GUILayout.HorizontalScope("Box"))
			{
				GUILayout.Label(value.Value.ToString());
				selectedOperator = EditorGUILayout.Popup(selectedOperator, operators, GUILayout.Width(100));
				GUILayout.Label(modifyWith.Value.ToString());
			}
			
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			switch (selectedOperator)
			{
				// =
				case 0:
				
					value.Value = modifyWith.Value;
					
					break;
				// +
				case 1:
				
					value.Value += modifyWith.Value;
					
					break;
				// -
				case 2:
				
					value.Value -= modifyWith.Value;
					
					break;
				// *
				case 3:
				
					value.Value *= modifyWith.Value;
				
					break;
				// /
				case 4:
					
					value.Value /= modifyWith.Value;
					
					break;
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}