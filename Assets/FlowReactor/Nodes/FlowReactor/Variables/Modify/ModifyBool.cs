using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Modify", "ModifyBool", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class ModifyBool : Node
	{
		
		public FRBoolean value;
		public FRBoolean setTo;
		
		
		[SerializeField]
		int selectedOperator;
		
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
		#endif
		
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			value.Value = setTo.Value;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}