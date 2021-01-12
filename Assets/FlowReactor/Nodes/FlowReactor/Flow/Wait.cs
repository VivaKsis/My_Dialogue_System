using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Flow", "Wait", "flowNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class Wait : Node
	{
		public FRFloat delay;
		[HelpBox("Set to true if the countdown should be restarted when node is being called before time ran up.", HelpBox.MessageType.info)]
		public FRBoolean restartTime;
		
		float currentStartTime;
		bool isActive = false;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("waitIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
			
			if (Application.isPlaying && isActive)
			{
				EditorGUI.ProgressBar(new Rect(nodeRect.x + 20, nodeRect.y + 30, nodeRect.width - 40, 10),(Time.time - currentStartTime) / delay.Value, "");
			}
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			isActive = false;
			currentStartTime = 0f;
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			if (restartTime.Value)
			{
				currentStartTime = Time.time;
				isActive = true;
			}
			else
			{
				if (!isActive)
				{
					currentStartTime = Time.time;
					isActive = true;
				}
			}
		
		}
		
		public override void OnUpdate(FlowReactorComponent _flowReactor)
		{
			if (!graphOwner.isActive)
				return;
				
			if (isActive)
			{
				if (Time.time > currentStartTime + delay.Value)
				{
					isActive = false;
					runParallel = false;
					ExecuteNext(0, _flowReactor);
				}
			}
		}
	}
}