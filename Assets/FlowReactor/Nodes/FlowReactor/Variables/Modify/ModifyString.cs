using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Modify", "ModifyString", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class ModifyString : Node
	{
		
		public FRString value;
		public FRString modifyWith;
		
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
			
			
			using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label(value.Value + " = " + modifyWith.Value);
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