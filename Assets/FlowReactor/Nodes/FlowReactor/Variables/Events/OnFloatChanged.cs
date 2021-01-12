using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Events", "On float changed", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnFloatChanged : Node
	{
	
		public FRFloat variableFloat;
		
		FlowReactorComponent flowReactor;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("eventIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		#endif

		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;
			
			if (variableFloat.assignedBlackboard != null)
			{
				var _f =variableFloat.assignedBlackboard.GetData<FRFloat>(Guid.Parse(variableFloat.variableGuid));
				_f.OnValueChanged += ValueChanged;
			}
			else
			{
				variableFloat.OnValueChanged += ValueChanged;
			}
		}
		
		public override void OnNodeDisable(FlowReactorComponent _flowReactor)
		{
			if (variableFloat.assignedBlackboard != null)
			{
				var _f = variableFloat.assignedBlackboard.GetData<FRFloat>(Guid.Parse(variableFloat.variableGuid));
				_f.OnValueChanged -= ValueChanged;
			}
			else
			{
				variableFloat.OnValueChanged -= ValueChanged;
			}
		}
		
		
		public void ValueChanged(FRVariable _var)
		{

			ExecuteNext(0, flowReactor);
		}
	}
}
