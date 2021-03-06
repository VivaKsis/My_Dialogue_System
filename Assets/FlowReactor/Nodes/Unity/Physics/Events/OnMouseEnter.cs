﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Physics/Events", "Called when the mouse enters the Collider.", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnMouseEnter : Node
	{
		public FRGameObject collider;
	
		FlowReactorComponent flowReactor;

		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("eventIcon.png");

			// If hide input is true
			// no other node can be connected to this node
			hideInput = true;
			
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
			
			if (collider.Value != null)
			{
				var _go = collider.Value;
					
				if (_go != null)
				{
					var _component = _go.GetComponent<MouseColliderListener>();
					if (_component != null)
					{
						_component.onMouseEnterNodes.Add(this);
					}
					else
					{
						_go.AddComponent<MouseColliderListener>().onMouseEnterNodes.Add(this);
					}
				}
			}
		}
		
		public void OnEnter()
		{
			ExecuteNext(0, flowReactor);
		}

	}
}
