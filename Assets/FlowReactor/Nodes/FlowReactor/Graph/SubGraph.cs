using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("Internal", "Sub Graph", "subGraphNodeColor", 0, NodeAttributes.NodeType.SubGraph)]
	public class SubGraph : Node
	{
		
		public Graph subGraph;
		public string subGraphName;
		
		
		#if UNITY_EDITOR
		bool nodeExtractPreview = false;
		List<Node> nodesBeforeExtractionPreview = new List<Node>();

		double clickTime;
		double doubleClickTime = 0.3;
			
		
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("subGraphIcon.png");
			disableDefaultInspector = true;
			disableVariableInspector = true;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 200, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(subGraphName, _id, _graph, _editorSkin);

		
			subGraph.name = subGraphName;
			
	
			if (nodeRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				if ((EditorApplication.timeSinceStartup - clickTime) < doubleClickTime)
				{
					// Double click on node
					OpenSubGraph();
					
					Event.current.Use();
				}
				
	
				clickTime = EditorApplication.timeSinceStartup;
			}
			
		}
		
		
		void OpenSubGraph()
		{
			if (nodeExtractPreview)
			{
				DisableNodeExtractPreview();
			}
							
			rootGraph.currentGraph = subGraph;
			rootGraph.mouseClickInGroupNode = false;
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
							
			rootGraph.parentLevels.Add(new Graph.ParentLevels(subGraphName, subGraph));
		}
		
		void EnableNodeExtractPreview()
		{
			nodesBeforeExtractionPreview = new List<Node>(rootGraph.currentGraph.nodes);
				
			for (int r = 0; r < rootGraph.currentGraph.nodes.Count; r ++)
			{
				if (rootGraph.currentGraph.nodes[r].node != this)
				{
					rootGraph.currentGraph.nodes[r].guiDisabled = true;
				}
			}
			
			for (int n = 0; n < subGraph.nodes.Count; n ++)
			{
				rootGraph.currentGraph.nodes.Add(subGraph.nodes[n]);
			}
		}
		
		void DisableNodeExtractPreview()
		{
			rootGraph.currentGraph.nodes = new List<Node>(nodesBeforeExtractionPreview);
	
			
			for (int r = 0; r < rootGraph.currentGraph.nodes.Count; r ++)
			{
				rootGraph.currentGraph.nodes[r].guiDisabled = false;
			}
		}
		
	
		public override void MoveDelta(Vector2 _delta)
		{
			base.MoveDelta(_delta);
			
			if (!nodeExtractPreview)
				return;
			
			for (int n = 0; n < subGraph.nodes.Count; n ++)
			{
				subGraph.nodes[n].nodeRect.position += _delta;
			} 
		}
		
	
		public override void DrawCustomInspector()
		{
			 
			using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label("Sub graph name");
				subGraphName = GUILayout.TextField(subGraphName);
			}
			
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				using (new GUILayout.HorizontalScope())
				{
					if (!nodeExtractPreview)
					{
						if (GUILayout.Button("Preview nodes inside", GUILayout.Height(20)))
						{
							nodeExtractPreview = true;
							EnableNodeExtractPreview();
						}
					}
					else
					{
						if (GUILayout.Button("Close preview", GUILayout.Height(20)))
						{
							nodeExtractPreview = false;
							DisableNodeExtractPreview();
						}
					}
				}
				
				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Extract nodes", GUILayout.Height(20)))
					{
						EnableNodeExtractPreview();
						Extract();
					}		
				}
			
			
				if (GUILayout.Button("Rebuild outputs"))
				{
					RebuildOutputs();
				}
			
			}
		}
		
		
		void Extract()
		{
			for (int n = 0; n < subGraph.nodes.Count; n ++)
			{
				subGraph.nodes[n].guiDisabled = false;
				subGraph.nodes[n].graphOwner = rootGraph.currentGraph;
				subGraph.nodes[n].rootGraph = rootGraph;
			}
								
			for (int n = subGraph.nodes.Count - 1; n > -1; n --)
			{
				subGraph.nodes.RemoveAt(n);
			}
								
			for (int r = 0; r < rootGraph.currentGraph.nodes.Count; r ++)
			{
				rootGraph.currentGraph.nodes[r].guiDisabled = false;
			}
								
								
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
							
			DeleteSubGraph(subGraph);
		}
		
		
		void RebuildOutputs()
		{
		
			for (int j = 0; j < outputNodes.Count; j ++)
			{
				if (outputNodes[j].outputNode != null)
				{
					outputNodes[j].outputNode.RemoveInputNode(this);	
				}
			}
			
			outputNodes = new List<Node.NodeOutput>();
			
			
			for (int s = 0; s < subGraph.nodes.Count; s ++)
			{
				if (subGraph.nodes[s].GetType() == typeof (ExitGraph))
				{
					var _exists = false;
				
					for (int o = 0; o < outputNodes.Count; o ++)
					{
						if (subGraph.nodes[s].guid == outputNodes[o].subGraphExitguid)
						{
							_exists = true;
						}
					}
					
					if (!_exists)
					{
						var _exitNode = subGraph.nodes[s] as ExitGraph;
						
						// Add new output
						outputNodes.Add(new NodeOutput(_exitNode.outputName,_exitNode.guid, true));
					
					}
				}
			}
			

			nodeRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 60 + (outputNodes.Count * 20));
		}
		#endif
	
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			subGraph.isActive = false;
		}
	
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			subGraph.isActive = true;
		
			for (int n = 0; n < subGraph.nodes.Count; n ++)
			{
				if (subGraph.nodes[n].GetType() == typeof(OnEnterGraph))
				{
					subGraph.nodes[n].OnExecute(_flowReactor);
				}
			}
		}
		
		// Called by an exit graph node
		public void ExecuteOutput(string _guid, FlowReactorComponent _flowReactor)
		{
			subGraph.isActive = false;
			// fade out highlight
			#if UNITY_EDITOR
			highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
			#endif
			
			// Deactivate all subgraphs inside of this graph
			for (int s = 0; s < subGraph.subGraphs.Count; s ++)
			{
				subGraph.subGraphs[s].Deactivate();
			}
		
			for (int i = 0; i < outputNodes.Count; i ++)
			{
			
				if (outputNodes[i].subGraphExitguid == _guid && outputNodes[i].outputNode != null)
				{
					outputNodes[i].outputNode.branchNode = branchNode;
					outputNodes[i].outputNode.OnExecute(_flowReactor);
				}
			}
		}
		
		public void RegisterExitNode(Node _node)
		{
			var _exitNode = _node as ExitGraph;

			outputNodes.Add(new NodeOutput(_exitNode.outputName, _node.guid.ToString(), true));
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, nodeRect.height + 20);
		}
		
		#if UNITY_EDITOR
		public void UnRegister(string _guid)
		{
		
			for (int i = 0; i < outputNodes.Count; i ++)
			{
				if (outputNodes[i].guid == _guid)
				{
					if (outputNodes[i].outputNode != null)
					{
						outputNodes[i].outputNode.RemoveInputNode(this);
					}
					
					outputNodes.RemoveAt(i);
					nodeRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, nodeRect.height - 20);
				}
			}
		}
		#endif
		
	}
}