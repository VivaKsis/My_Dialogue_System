﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Character" , "Rotates the player object based on input axis" , "actionNodeColor" , 1 )]
	public class RotatePlayer : Node
	{
		public FRGameObject player;
		public FRFloat horizontalAxis;
		public FRFloat verticalAxis;
		public FRFloat smooth;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
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
			Quaternion _rotate = Quaternion.identity;
			Vector3 move = new Vector3(horizontalAxis.Value, 0, verticalAxis.Value).normalized;
			
			if (move != Vector3.zero)
			{
				_rotate = Quaternion.LookRotation(move);
				player.Value.transform.rotation = Quaternion.Slerp(player.Value.transform.rotation, _rotate, Time.deltaTime * smooth.Value);
			}
		    
			
			ExecuteNext(0, _flowReactor);
		}
	}
}