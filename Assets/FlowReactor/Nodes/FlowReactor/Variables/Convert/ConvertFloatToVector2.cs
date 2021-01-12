﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes( "FlowReactor/Variables/Convert" , "Convert float value to vector2" , "actionNodeColor" , 1 )]
	public class ConvertFloatToVector2 : Node
	{
		
		public FRFloat valueX;
		public FRFloat valueY;
		
		public FRVector2 toVector2;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("convertIcon.png");
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
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
			toVector2.Value = new Vector2(valueX.Value, valueY.Value);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}