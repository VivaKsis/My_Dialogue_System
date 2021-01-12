using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Graph" , "Runs a sub-graph as an instance" , "subGraphNodeColor" , 1, NodeAttributes.NodeType.SubGraphInstance)]
	public class SubGraphInstance : Node
	{
		public int selected = 0;
		public Graph graphInstance;
		public Graph graphCopy;
		
		
		#if UNITY_EDITOR
		double clickTime;
		double doubleClickTime = 0.3f;
	
	
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			icon = EditorHelpers.LoadIcon("subGraphIcon.png");
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 200, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(graphInstance != null ? graphInstance.name + " (Instance)" : "SubGraphInstance", _id, _graph, _editorSkin);
	
			
			if (nodeRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				if ((EditorApplication.timeSinceStartup - clickTime) < doubleClickTime && graphInstance != null)
				{
					// Double click on node
					OpenSubGraph();
					
					Event.current.Use();
				}
				
	
				clickTime = EditorApplication.timeSinceStartup;
			}
		
		}
	
		// Draw custom node inspector
		public override void DrawCustomInspector()
		{
			if (graphCopy != null)
			{
				GUILayout.Label(graphCopy.isActive.ToString());
			}
			// Get all sub graphs 
			List<Graph> subs = new List<Graph>();
			for (int s = 0; s < rootGraph.subGraphs.Count; s ++)
			{
				if (rootGraph.subGraphs[s] == null)
					continue;
					
				if (rootGraph.subGraphs[s] != graphOwner && !rootGraph.subGraphs[s].isCopy)
				{
					subs.Add(rootGraph.subGraphs[s]);
				}
			}
			
			if (subs.Count > 0)
			{
				using (new GUILayout.VerticalScope("Box"))
				{
					GUILayout.Label("Available sub-graphs");
					for (int i = 0; i < subs.Count; i ++)
					{
						if (subs[i] == null)
							continue;
						
						
						if (GUILayout.Button(subs[i].name))
						{
							selected = i;
							graphInstance = subs[i];
							CheckOutputs();
						}
					}
				}
			}
			
			if ( graphInstance != null)
			{
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
				{		
					GUILayout.Label("Selected: " + graphInstance.name);
					
					if (GUILayout.Button("Open"))
					{
						OpenSubGraph();
					}
							
				}
			}
		}
		
		
		void CheckOutputs()
		{
			if (graphInstance == null)
				return;

		
			for (int j = 0; j < outputNodes.Count; j ++)
			{
				if (outputNodes[j].outputNode != null)
				{
					outputNodes[j].outputNode.RemoveInputNode(this);	
				}
			}
			
			outputNodes = new List<Node.NodeOutput>();
			
			
			for (int s = 0; s < graphInstance.nodes.Count; s ++)
			{
				if (graphInstance.nodes[s].GetType() == typeof (ExitGraph))
				{
					var _exists = false;
				
					for (int o = 0; o < outputNodes.Count; o ++)
					{
						if (graphInstance.nodes[s].guid == outputNodes[o].subGraphExitguid)
						{
							_exists = true;
						}
					}
					
					if (!_exists)
					{
						var _exitNode = graphInstance.nodes[s] as ExitGraph;
						
						// Add new output
						outputNodes.Add(new NodeOutput(_exitNode.outputName,_exitNode.guid, true));
					
					}
				}
			}
			

			nodeRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 60 + (outputNodes.Count * 20));
		}
	
		
		void OpenSubGraph()
		{
			rootGraph.currentGraph = graphInstance;
							
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
							
			rootGraph.parentLevels.Add(new Graph.ParentLevels(graphInstance.name, graphInstance));
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// make a copy of the original graph
			if (graphInstance == null)
				return;
				
			var _copy = graphInstance.Copy(rootGraph, this, null);	
			graphCopy = _copy as Graph;
			
			// Add to subgraphs
			rootGraph.subGraphs.Add(graphCopy);
		}
		
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			graphCopy.isActive = true;
			
			for (int n = 0; n < graphCopy.nodes.Count; n ++)
			{
				if (graphCopy.nodes[n].GetType() == typeof(OnEnterGraph))
				{
					graphCopy.nodes[n].OnExecute(_flowReactor);
				}
			}
		}
		
		
		// Called by an exit graph node
		public void ExecuteOutput(string _guid, FlowReactorComponent _flowReactor)
		{
			graphCopy.isActive = false;
			// fade out highlight
			#if UNITY_EDITOR
			highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
			#endif
			
			for (int i = 0; i < outputNodes.Count; i ++)
			{
				if (outputNodes[i].subGraphExitguid == _guid && outputNodes[i].outputNode != null)
				{
					outputNodes[i].outputNode.branchNode = branchNode;
					outputNodes[i].outputNode.OnExecute(_flowReactor);
				}
			}
		}
	}
}