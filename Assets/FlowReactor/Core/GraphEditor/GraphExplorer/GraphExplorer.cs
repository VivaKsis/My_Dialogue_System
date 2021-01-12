//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public class GraphExplorer
	{
		
		Graph rootGraph;
		FREditorSettings settings;
		GUISkin editorSkin;
		GraphEditor editor;
		
		Texture2D nodeRunningIcon;
		Texture2D nodeNotRunningIcon;
		Texture2D nodeErrorIcon;
		Texture2D groupIcon;
		
		Vector2 scrollPosition;
		
		string searchFilter;

		static string[] searchOptions = new string[] { "Nodes", "Sub-Graphs", "Groups"}; // index 1, 2, 4, Nothing = 0, Everything = -1, 3 = Nodes + Sub, 5 = Nodes + Groups, 6 = Subs + Groups
		
		int searchOption = -1;
		
		public GraphExplorer (Graph _graph, FREditorSettings _settings, GUISkin _skin, GraphEditor _editor)
		{
			LoadIcons();	
			
			rootGraph = _graph;
			settings = _settings;
			editorSkin = _skin;
			editor = _editor;
		}
		
		public void DrawExplorer()
		{
			if (rootGraph == null)
				return;
				
			using (new GUILayout.HorizontalScope("toolbar"))
			{
				searchFilter = GUILayout.TextField(searchFilter, "SearchTextField");
				if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
				{
					searchFilter = "";
				}
				
				searchOption = EditorGUILayout.MaskField("", searchOption, searchOptions, GUILayout.Width(100));
				
			
			}
			
			using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition)) //, GUILayout.Height(100)))
			{
				scrollPosition = scrollView.scrollPosition;
				GUI.color = settings.GetColor("subGraphNodeColor");
				GUILayout.Label(rootGraph.name, "boldLabel");
				GUI.color = Color.white;
				
				EditorHelpers.DrawUILine();
			
				if (searchOption != 0)
				{
					TraverseNodeTree(0, rootGraph);
				}
				
				GUILayout.Space(50);
			}
			
		}
		
		public void DrawExplorer(Graph _rootGraph, FREditorSettings _settings, GUISkin _editorSkin, GraphEditor _editor)
		{
			if (nodeRunningIcon == null)
			{
				LoadIcons();	
			}
			
			rootGraph = _rootGraph;
			settings = _settings;
			editorSkin = _editorSkin;
			editor = _editor;
		
			DrawExplorer();
		}
		
		public void TraverseNodeTree(int _level, Graph _graph)
		{		
			if (_level > 0)
			{
				if (searchOption == -1 || searchOption == 3 || searchOption == 6 || searchOption == 2)
				{
					using (new GUILayout.HorizontalScope(FlowReactorEditorStyles.graphExplorerButton))
					{
						
						GUI.color = settings.GetColor("subGraphNodeColor");
						if (Application.isPlaying)
						{
							_graph.foldout = EditorGUILayout.Foldout(_graph.foldout, new GUIContent(" " + _graph.name, _graph.isActive ? nodeRunningIcon : nodeNotRunningIcon));
						}
						else
						{
							_graph.foldout = EditorGUILayout.Foldout(_graph.foldout, new GUIContent(" " + _graph.name, nodeNotRunningIcon));
						}
						GUI.color = Color.white;

						//if (GUILayout.Button(">", GUILayout.Width(25), GUILayout.Height(18)))
						//{
						//	GotoNode(_graph, _graph.subGraphNode);
						//}
					}

				}
				
			}
			else
			{
				_graph.foldout = true;
			}
			
			if (searchOption > 0)
			{
				_graph.foldout = true;
			}
			
			if (searchOption == 2)
			{
				for (int s = 0; s < _graph.subGraphs.Count; s ++)
				{
					TraverseNodeTree(_level + 1, _graph.subGraphs[s]);
				}
				return;
			}
			
			if (_graph.foldout)
			{
				using (new EditorGUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					for (int n = 0; n < _graph.nodes.Count; n ++)
					{
						if (_graph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.SubGraph ||
							_graph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.SubGraphInstance ||
							_graph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Comment)
							continue;
						
						if (!string.IsNullOrEmpty(searchFilter))
						{
							if (_graph.nodes[n].nodeData.nodeType != NodeAttributes.NodeType.Group)
							{
								if (!_graph.nodes[n].name.ToLower().Contains(searchFilter.ToLower()))
								{
									continue;
								}
							}
							else
							{
						
								var _groupNode = _graph.nodes[n] as Group;
								if (!_groupNode.groupTitle.ToLower().Contains(searchFilter.ToLower()))
								{
									continue;
								}
								
							}
						}
					
					
						using (new GUILayout.HorizontalScope(FlowReactorEditorStyles.graphExplorerButton))
						{
							
							if (_graph.nodes[n].nodeData.nodeType != NodeAttributes.NodeType.Group && (searchOption == -1 || searchOption == 1 || searchOption == 3 || searchOption == 5))
							{
								GUI.contentColor = _graph.nodes[n].color;
								if (Application.isPlaying)
								{
									GUILayout.Label(new GUIContent(" " + _graph.nodes[n].name, _graph.nodes[n].isRunning ? nodeRunningIcon : nodeNotRunningIcon));
								}
								else
								{
									GUILayout.Label(new GUIContent(" " + _graph.nodes[n].name, nodeNotRunningIcon));
								}
								GUI.contentColor = Color.white;
							}
							else
							{
								if (searchOption == -1 || searchOption == 4 || searchOption == 4 || searchOption == 5 || searchOption == 6)
								{
									var _groupNode = _graph.nodes[n] as Group;
									if (_groupNode != null)
									{
										GUI.contentColor = _graph.nodes[n].groupColor;				
										GUILayout.Label(new GUIContent(" " + _groupNode.groupTitle, groupIcon));
										GUI.contentColor = Color.white;
									}
								}
							}
						
							//
							if (_graph.nodes[n].hasError && (searchOption == -1 || searchOption == 1 || searchOption == 3 || searchOption == 5))
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label(nodeErrorIcon);
							}
						}
						var _rect = GUILayoutUtility.GetLastRect();
						if (_rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
						{
							
							GotoNode(_graph.nodes[n].graphOwner, _graph.nodes[n]);
						}
					
					}
						
					for (int s = 0; s < _graph.subGraphs.Count; s ++)
					{
						TraverseNodeTree(_level + 1, _graph.subGraphs[s]);
					}
				}
			}	
			
		
		}
		
		void GotoNode(Graph _graphOwner, Node _node)
		{	
			// Rebuild graph levels and search for graph
			var _path = "";
			var _graphList = new List<Graph>();
			var _p = rootGraph.TraverseGraph(_graphOwner, "", out _path, _graphList, out _graphList);
						
			var _graphs = _path.Split("/"[0]);
							
			rootGraph.parentLevels = new List<Graph.ParentLevels>();
					
			var _index = 0;
							
			foreach (var p in _graphs)
			{
				rootGraph.parentLevels.Add(new Graph.ParentLevels(p, _graphList[_index]));
				_index ++;
			}

						
			rootGraph.isMouseOverNode = false;
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();

			rootGraph.currentGraph = _graphOwner;
							
			editor.FocusPosition(_node.nodeRect.center);
							
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			rootGraph.selectedNodes.Add(_node.GetInstanceID(), _node);
			
		}
	
		void LoadIcons()
		{
			nodeRunningIcon = EditorHelpers.LoadGraphic("nodeRunningIcon.png");
			nodeNotRunningIcon = EditorHelpers.LoadGraphic("nodeNotRunningIcon.png");
			nodeErrorIcon = EditorHelpers.LoadGraphic("errorIcon.png");
			groupIcon = EditorHelpers.LoadGraphic("groupIconSmall.png");
		}
	}
}
#endif