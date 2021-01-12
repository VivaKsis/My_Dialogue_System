using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Modify", "ModifyGameObject", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class ModifyGameObject : Node
	{
		
		public FRGameObject value;
		public FRGameObject modifyWith;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("editIcon.png");
			
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		
		public override void DrawCustomInspector()
		{
				
			if (value != null && modifyWith != null)
				return;
				
			GUILayout.Label("Operator:");
		
			if (value.Value != null && modifyWith.Value != null)
			{
			
				using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
				{
					GUILayout.Label(value.Value.name.ToString() + " = " + modifyWith.Value.name.ToString());
				}
			
			}
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
		
			value.Value = modifyWith.Value;
		
			ExecuteNext(0, _flowReactor);
		}
	}
}