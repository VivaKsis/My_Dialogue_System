using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Graph", "ExitGraph", "flowNodeColor", 0, NodeAttributes.NodeType.Normal)]
	public class ExitGraph : Node
	{
		[SerializeField]
		public string outputName = "ExitGraph";
		
		[SerializeField]
		public string outputGuid;
		
		bool isActive = false;
		int activeFrameCount = 0;
		int currentFrameCount = 0;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("exitIcon.png");
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
			
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			
			if (_node.graphOwner.subGraphNode != null)
			{
				_node.graphOwner.subGraphNode.RegisterExitNode(_node);
			}
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}

	
		public override void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();
			GUILayout.Label("output name:");
			outputName = GUILayout.TextField(outputName);
	
			if (EditorGUI.EndChangeCheck ()) 
			{

				for (int o = 0; o < graphOwner.subGraphNode.outputNodes.Count; o ++)
				{
					if (graphOwner.subGraphNode.outputNodes[o].guid == outputGuid)
					{
						graphOwner.subGraphNode.outputNodes[o].id = outputName;
					}
				}
			}
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			activeFrameCount = Time.frameCount;
			isActive = true;
			
			// Call execute next just for fading out node higlighting in editor
			ExecuteNext(0, _flowReactor);
		}
		
		#if UNITY_EDITOR
		public override void OnDelete()
		{
			if (node.graphOwner != null)
			{
				if (node.graphOwner.subGraphNode != null)
				{
					node.graphOwner.subGraphNode.UnRegister(outputGuid);
				}
			}
		}
		#endif
		
		
		public override void OnUpdate(FlowReactorComponent _flowReactor)
		{
			if (!graphOwner.isActive)
				return;
				
			currentFrameCount = Time.frameCount;
		
			if (currentFrameCount > activeFrameCount + 1 && isActive)
			{
					
				if (graphOwner.subGraphNodeInstance != null)
				{
					graphOwner.subGraphNodeInstance.ExecuteOutput(guid, _flowReactor);
				}
				else
				{
					if (graphOwner.subGraphNode != null)
					{
						graphOwner.subGraphNode.ExecuteOutput(guid, _flowReactor);
					}
				}
				isActive = false;
				graphOwner.isActive = false;
			}
		}
	}
}