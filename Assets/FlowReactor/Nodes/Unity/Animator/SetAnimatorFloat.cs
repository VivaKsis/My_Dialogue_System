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
	[NodeAttributes( "Unity/Animator" , "Set float parameter to an animator object" , "actionNodeColor" , 1 )]
	public class SetAnimatorFloat : Node
	{
		public FRAnimator animator;
		
		[Title("Animator float name:")]
		public FRString floatName;
		[Title("Set to:")]
		public FRFloat setFloat;
		
		int floatNameHash;

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

		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			floatNameHash = UnityEngine.Animator.StringToHash(floatName.Value);
		}
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			animator.Value.SetFloat(floatNameHash, setFloat.Value);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}