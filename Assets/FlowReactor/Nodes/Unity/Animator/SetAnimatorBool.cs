﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Animator" , "Set bool parameter to an animator object" , "actionNodeColor" , 1 )]
	public class SetAnimatorBool : Node
	{
		public FRAnimator animator;
		
		[Title("Animator bool name:")]
		public FRString boolName;
		[Title("Set to:")]
		public FRBoolean value;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
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
			if (animator.Value.GetBool(boolName.Value) != value.Value)
			{
				animator.Value.SetBool(boolName.Value, value.Value);
			}

			
			ExecuteNext(0, _flowReactor);
		}
	}
}