using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Animator" , "Sets the look at position." , "actionNodeColor" , 1 )]
	public class SetAnimatorLookAtPosition : Node
	{
		public FRAnimator animator;
		
		[HelpBox("If look at game object is null, then the node will use the look at position instead.", HelpBox.MessageType.info)]
		[Title("Look At:")]
		public FRGameObject lookAtGameObject;
		public FRVector3 lookAtPosition;

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
			if (lookAtGameObject.Value == null )
			{
				animator.Value.SetLookAtPosition(lookAtPosition.Value);
			}
			else
			{
				animator.Value.SetLookAtPosition(lookAtGameObject.Value.transform.position);
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}