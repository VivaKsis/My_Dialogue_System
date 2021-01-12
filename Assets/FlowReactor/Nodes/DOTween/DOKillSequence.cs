﻿#if FLOWREACTOR_DOTWEEN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

using DG.Tweening;

namespace FlowReactor.Nodes.DOTween
{
	[NodeAttributes( "DOTween" , "Kills a DOTween sequence by its tweenID" , "actionNodeColor" , 1 )]
	public class DOKillSequence : Node
	{
		public FRString tweenID;


		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// special case if node shouldn't have an input
			hideInput = false;
			icon = EditorHelpers.LoadIcon("doTweenIcon.png");
			
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
			DG.Tweening.DOTween.Kill(tweenID.Value);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
#endif