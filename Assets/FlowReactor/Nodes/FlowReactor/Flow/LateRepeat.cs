using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Flow", "Late Repeat", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class LateRepeat : Node
	{
		public FRInt repeatCount = new FRInt(0);
		public FRInt waitForFrameCount = new FRInt(0);
		
		bool isActive = false;
		int activeFrameCount = 0;
		int currentFrameCount = 0;
		int currentRepeatCount = 0;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("nextFrameIcon.png");	
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "Repeat";
			outputNodes[1].id = "Finished";
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			currentRepeatCount = 0;
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			activeFrameCount = Time.frameCount;
			isActive = true;
		}
		
		public override void OnLateUpdate(FlowReactorComponent _flowReactor)
		{
			if (!graphOwner.isActive)
				return;
				
			currentFrameCount = Time.frameCount;
		
			if (currentFrameCount > activeFrameCount + waitForFrameCount.Value && isActive && (currentRepeatCount < repeatCount.Value || repeatCount.Value <= 0))
			{
				isActive = false;
				currentRepeatCount ++;
				runParallel = false;
				ExecuteNext(0, _flowReactor);
			}
			
			if (currentRepeatCount >= repeatCount.Value && repeatCount.Value > 0)
			{
				currentRepeatCount = 0;
				runParallel = false;
				ExecuteNext(1, _flowReactor);
			}
		}
	}
}