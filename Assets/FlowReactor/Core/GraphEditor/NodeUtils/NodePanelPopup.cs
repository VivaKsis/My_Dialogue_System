//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public class NodePanelPopup
	{
		public class NodePanel : PopupWindowContent
		{
			GUISkin editorSkin;
			FREditorSettings settings;
			
			NodeCategoryTree collectedNodes;
			
			bool filterReset;
			string filter = "";
			string lastFilter = "";
			string description = "";
			
			Vector2 position = Vector2.zero;
			Rect rect = new Rect(0,0,0,0);
			Vector2 scrollPosition = Vector2.zero;
	
			int currentDepth = 0;
			string currentPath;
		
			string favoritePath;
			bool stopTraversing;
			Texture2D favoriteIcon;
			
			NodeCategoryTree currentCategory;
			
			Graph graph;
			GraphEditor graphEditor;
			EditorWindow wasFocusedWindow;
			bool createdNewNode = false;
			string firstNodeName = "";
			NodeCategoryTree.NodeData firstNodeData;
			Vector2 firstNodePosition;
			
			Node newNode = null;
			
			private static NodePanel current;
			
			
			public override Vector2 GetWindowSize()
			{
				return new Vector2(rect.width, rect.height);
			}
			
			//public void DrawPanelRect(Rect _graphRect)
			//{
			//	using (new GUILayout.AreaScope(new Rect(_graphRect.width - 200, 0, 200, _graphRect.height ), "Box"))
			//	{
			//		DrawInternal();
			//	}
			//}
		
			public override void OnGUI(Rect _rect)
			{	
				DrawInternal();
			}
			void DrawInternal()
			{
				GUILayout.Label("Nodes", FlowReactorEditorStyles.elementButtonBack, GUILayout.Height(20));
				
				GUILayout.Space(5);
				//if (currentCategory != null)
				//{
				//GUILayout.Label(currentPath + " " + currentDepth + " " + currentCategory.Path);
				//}
				using (new GUILayout.HorizontalScope())
				{
					GUI.SetNextControlName ("Filter");
					
					EditorGUI.FocusTextInControl ("Filter");
					filter = GUILayout.TextField(filter, "SearchTextField");
					
					if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
					{
						firstNodeName = "";
						filter = "";
						
						favoritePath = "";
						filterReset = false;
						currentDepth = 0;

						collectedNodes = CollectAvailableNodes.CollectNodes();
					}
					
					
					if (string.IsNullOrEmpty(filter) && !filterReset)
					{
						filterReset = true;
						firstNodeName = "";
						collectedNodes = CollectAvailableNodes.CollectNodes();
					}
					else if (!string.IsNullOrEmpty(filter) && filterReset)
					{
						filterReset = false;
					}
					
				}
				
				GUILayout.Space(5);
				

				if (string.IsNullOrEmpty(filter))
				{
					if (currentDepth - 1 >= 0)
					{
						
						// button back
						if (GUILayout.Button(currentCategory.Path, FlowReactorEditorStyles.elementButtonBack, GUILayout.Height(25)))
						{
							currentDepth --;
							currentPath = currentCategory.parentGraphTree.Path;
							currentCategory.FoldOut = false;
							currentCategory = currentCategory.parentGraphTree;	
							
							if (currentDepth == 0)
							{
								collectedNodes = CollectAvailableNodes.CollectNodes();
							}
						}
						
						
						var _l = GUILayoutUtility.GetLastRect();
						GUI.Label(new Rect(_l.x, _l.y, _l.width, _l.height), "<"); // NodePanelStyles.leftArrow.normal.background);
					
					}
				}
			
				using (var scrollView = new GUILayout.ScrollViewScope(scrollPosition))
				{		
					scrollPosition = scrollView.scrollPosition;
					
				
					Draw(collectedNodes);
					
				}
								
				GUILayout.Label(description, "textArea");
				
				editorWindow.Repaint();	
				
			}
			
			
			public void Draw(NodeCategoryTree tree)
			{
				if (stopTraversing)
					return;
				tree.Traverse(DrawGUI);
			}
		
			void DrawGUI(int depth, NodeCategoryTree tree)
			{
				if (depth == -1)
					return;
				

				// if search filter is empty draw categories and its nodes				
				if (string.IsNullOrEmpty(filter))
				{
					if (depth == currentDepth && currentPath == tree.parentGraphTree.Path) 
					{
						if (tree.Path.Contains("Internal"))
							return;
							
						DrawCategories(tree, depth);
					}			
					
					if (depth != currentDepth && currentPath != tree.parentGraphTree.Path)
					{				
					
						if (currentPath == tree.Path)
						{
						
							if (currentPath == favoritePath)
							{							
								tree.FoldOut = true;
								favoritePath = "";
							}
							
							if (tree.FoldOut)
							{
							
							
								for (int n = 0; n < tree.nodesInCategory.Count; n ++)
								{
								
										
									if (string.IsNullOrEmpty(firstNodeName))
									{
										
										firstNodeName = tree.nodesInCategory[n].title;
										firstNodePosition =  new Vector2(rect.x - (graph.showInspector ? graph.tmpInspectorWidth : 0), rect.y - rect.height);
									}
									
									using (new GUILayout.HorizontalScope())
									{
										
										//if (settings.favoredNodes == null)
										//{
										//	settings.favoredNodes = new List<NodeCategoryTree.NodeData>();
										//}
										//var _exists = false;
										//int _existIndex = 0;
										//for (int f = 0; f < settings.favoredNodes.Count; f ++)
										//{
										//	if (settings.favoredNodes[f].name == tree.nodesInCategory[n].name)
										//	{
										//		_exists = true;
										//		_existIndex = f;
										//		break;
										//	}
										//}
										
										//var _favSkin = "FavouritDeactive";
										//if (_exists)
										//{
										//	_favSkin = "FavouritActive";
										//}
										
										//if (GUILayout.Button("", editorSkin.GetStyle(_favSkin)))
										//{
										//	if (!_exists)
										//	{
										//		settings.favoredNodes.Add(tree.nodesInCategory[n]);
										//	}
										//	else
										//	{
										//		settings.favoredNodes.RemoveAt(_existIndex);
										//	}
											
										//	//favouritDepth = depth;
										//	favouritPath = tree.Path;
										//	//favouritTree = tree;

										//	if (tree.Path.Contains("Favourites"))
										//	{
										//		stopTraversing = true;
										//		UpdateFavourites();
										//	}
										
										//	currentPath = favouritPath;
											
										//}
										
										DrawFavoriteButton(tree, n);
										
										//Debug.Log("hello");
										if (GUILayout.Button(tree.nodesInCategory[n].title, FlowReactorEditorStyles.elementButton)) //NodePanelStyles.elementButton, GUILayout.Height(25)))
										{
										
											Vector2 _position;  
										
											_position = new Vector2(rect.x - (graph.showInspector ? graph.tmpInspectorWidth : 0), rect.y - (rect.height * graph.zoomFactor));
											#if FLOWREACTOR_DEBUG
											Debug.Log("create");
											#endif
	
			
											newNode = NodeCreator.CreateNode(tree.nodesInCategory[n], graph, graph.currentGraph, tree.nodesInCategory[n].nameSpace + "." + tree.nodesInCategory[n].typeName, _position);
										
											graph.nodeSelectionPanelOpen = false;
											createdNewNode = true;
											
											editorWindow.Close();
										}
									
									}
									
									var _lastRect = GUILayoutUtility.GetLastRect();
									if (_lastRect.Contains(Event.current.mousePosition))
									{
										description =  tree.nodesInCategory[n].description;
									}
								}
							}
						
						}
					}
			
				}	
				// if search filter is not null only show all nodes in one list
				else
				{
					tree.FoldOut = true;
					
					bool _found = false;
					for (int n = 0; n < tree.nodesInCategory.Count; n ++)
					{
						if (tree.Path.Contains("Internal"))
							return;
							
						if (tree.nodesInCategory[n].title.ToLower().Contains(filter.ToLower()))
						{
							_found = true;
						}
					}
					
					
					if (_found)
					{
						GUILayout.Label(tree.parentGraphTree.Path + " / " + tree.Path, "boldLabel");
						EditorHelpers.DrawUILine();
					}
					
					for (int n = 0; n < tree.nodesInCategory.Count; n ++)
					{
						if (tree.nodesInCategory[n].title.ToLower().Contains(filter.ToLower()))
						{
							if (filter != lastFilter)
							{
								firstNodeName = "";
							}
							if (string.IsNullOrEmpty(firstNodeName))
							{
							
								lastFilter = filter;
								firstNodeName = tree.nodesInCategory[n].title;
								firstNodeData = tree.nodesInCategory[n];
								firstNodePosition =  new Vector2(rect.x - (graph.showInspector ? graph.tmpInspectorWidth : 0), rect.y - rect.height);
							}
	
							
							using (new GUILayout.HorizontalScope())
							{
								DrawFavoriteButton(tree, n);
							
								
								//GUILayout.Label(tree.nodesInCategory[n].name);
								if (GUILayout.Button(tree.nodesInCategory[n].title, FlowReactorEditorStyles.elementButton))
								{
									Vector2 _position;  						
								
									//_position = new Vector2((rect.x / graph.zoomFactor) - (graph.showInspector ? (graph.tmpInspectorWidth / graph.zoomFactor): 0), (rect.y / graph.zoomFactor) - (rect.height / graph.zoomFactor)); // (rect.height / graph.zoomFactor));	
									_position = new Vector2((rect.x / graph.zoomFactor) - (graph.tmpInspectorWidth / graph.zoomFactor), (rect.y / graph.zoomFactor) - (rect.height / graph.zoomFactor) - 20); // (rect.height / graph.zoomFactor));	
									
									#if FLOWREACTOR_DEBUG
									Debug.Log("create " + _position);
									#endif
					
									newNode = NodeCreator.CreateNode(tree.nodesInCategory[n], graph, graph.currentGraph, tree.nodesInCategory[n].nameSpace + "." + tree.nodesInCategory[n].typeName, _position);
										         
									//frEditor.showNodePanelMouse = false;
									graph.nodeSelectionPanelOpen = false;
									createdNewNode = true;
									
									editorWindow.Close();
								}
								var _lastRect = GUILayoutUtility.GetLastRect();
								if (_lastRect.Contains(Event.current.mousePosition))
								{
									description =  tree.nodesInCategory[n].description;
								}
							}
						}
					}
				}
				
				
				var _e = Event.current;
				if (_e.isKey && _e.keyCode == KeyCode.Return && !string.IsNullOrEmpty(firstNodeName))
				{
					_e.Use();
				    
					newNode = NodeCreator.CreateNode(firstNodeData, graph, graph.currentGraph, firstNodeData.nameSpace + "." + firstNodeName, firstNodePosition);
					
				    graph.nodeSelectionPanelOpen = false;
					createdNewNode = true;
				
					firstNodeName = "";
			
					editorWindow.Close();
				}
				
			}	
			
			void DrawFavoriteButton(NodeCategoryTree tree, int nodeCategoryIndex )
			{
				if (settings.favoredNodes == null)
				{
					settings.favoredNodes = new List<NodeCategoryTree.NodeData>();
				}
				var _exists = false;
				int _existIndex = 0;
				for (int f = 0; f < settings.favoredNodes.Count; f ++)
				{
					if (settings.favoredNodes[f].title == tree.nodesInCategory[nodeCategoryIndex].title)
					{
						_exists = true;
						_existIndex = f;
						break;
					}
				}
										
				var _favSkin = "FavoritDeactive";
				if (_exists)
				{
					_favSkin = "FavoritActive";
				}
										
				if (GUILayout.Button("", editorSkin.GetStyle(_favSkin)))
				{
					if (!_exists)
					{
						settings.favoredNodes.Add(tree.nodesInCategory[nodeCategoryIndex]);
					}
					else
					{
						settings.favoredNodes.RemoveAt(_existIndex);
					}
											
					favoritePath = tree.Path;
		
					if (tree.Path.Contains("Favorites"))
					{
						stopTraversing = true;
						UpdateFavourites();
					}
										
					currentPath = favoritePath;
											
				}
			}
			
			void DrawCategories(NodeCategoryTree tree, int depth)
			{
				using (new GUILayout.HorizontalScope())
				{
					if (tree.Path.Contains("Favorites"))
					{		
						if (GUILayout.Button(new GUIContent(" " + tree.Path, favoriteIcon), FlowReactorEditorStyles.elementButtonBold, GUILayout.ExpandWidth(true), GUILayout.Height(25)))
						{	
	
							tree.FoldOut = !tree.FoldOut;
								
							currentDepth = depth + 1;
							currentPath = tree.Path;
							currentCategory = tree;
						}
					}
					else
					{
						if (GUILayout.Button(tree.Path, FlowReactorEditorStyles.elementButtonBold, GUILayout.ExpandWidth(true), GUILayout.Height(25)))
						{	
	
							tree.FoldOut = !tree.FoldOut;
								
							currentDepth = depth + 1;
							currentPath = tree.Path;
							currentCategory = tree;
						}
					}
					
						
					var _l = GUILayoutUtility.GetLastRect();
					GUI.Label(new Rect(_l.width - 20, _l.y, _l.width, _l.height), ">");
				}
			}
			
			void UpdateFavourites()
			{
				FlowReactor.Editor.EditorCoroutines.Execute(UpdateFavouritesIE());
			}
			
			IEnumerator UpdateFavouritesIE()
			{
				collectedNodes.categories.Remove("Favorites");
				
				var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
				List<CollectAvailableNodes.CollectedNodesData> coll = new List<CollectAvailableNodes.CollectedNodesData>();
				for (int f = 0; f < _settings.favoredNodes.Count; f ++)
				{
	
					coll.Insert(0, new CollectAvailableNodes.CollectedNodesData
					(
						//_settings.favoredNodes[f].name,
						_settings.favoredNodes[f].title,
						_settings.favoredNodes[f].typeName,
						_settings.favoredNodes[f].nameSpace,
						"Favorites",
						_settings.favoredNodes[f].description,
						_settings.favoredNodes[f].color,
						_settings.favoredNodes[f].outputSlotCount,
						_settings.favoredNodes[f].nodeOutputs,
						_settings.favoredNodes[f].nodeType
				
					));
				
				}
				
				
				for (int c = 0; c < coll.Count; c ++)
				{
					var _child = collectedNodes.BuildTree(coll[c].category, coll[c].name);
				
					_child.AddNode 
					(
						//coll[c].name,
						coll[c].title,
						coll[c].typeName,
						coll[c].nameSpace,
						coll[c].category,
						coll[c].description,
						coll[c].color,
						coll[c].outputCount,
						coll[c].nodeOutputs,
						coll[c].nodeType
					);
				}
				
				yield return new WaitForSeconds(0.2f);
				
				stopTraversing = false;
			}
			
			
			public override void OnOpen()
			{
				favoriteIcon = EditorHelpers.LoadIcon("favouritIcon.png");
				wasFocusedWindow = EditorWindow.focusedWindow;	
				graph.isMouseOverNode = false;
			
				
				EditorWindow.FocusWindowIfItsOpen(editorWindow != null ? editorWindow.GetType() : null);
				
				FocusEditor();			
			}
		
			public override void OnClose()
			{
				graphEditor.drawCurserHandle = false;
				graph.nodeSelectionPanelOpen = false;
				
				if (!createdNewNode)
				{
					graph.lastSelectedNodeIndex = -1;
				}
				
				graph.drawModeOn = false;
				graph.selectionBoxDragging = false;
				graph.globalMouseButton = -1;
				graph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;

				// connect created node with selected node
				if (newNode != null)
				{
					if (newNode.hideInput)
					{
						graph.lastSelectedNodeIndex = -1;
					}
					else
					{
						if (graph.lastSelectedNodeIndex > -1)
						{
							if (graph.currentGraph.nodes[graph.lastSelectedNodeIndex] != newNode)
							{
								graph.currentGraph.nodes[graph.lastSelectedNodeIndex].outputNodes[graph.lastSelectedOutput].outputNode
									= newNode;
									
								graph.currentGraph.nodes[graph.lastSelectedNodeIndex].outputNodes[graph.lastSelectedOutput].guid = newNode.guid;
											
								newNode.inputNodes.Add(new Node.InputNode(graph.currentGraph.nodes[graph.lastSelectedNodeIndex]));
							}
							
							for (int o = 0; o < graph.currentGraph.nodes[graph.lastSelectedNodeIndex].outputNodes.Count; o ++)
							{
								graph.currentGraph.nodes[graph.lastSelectedNodeIndex].outputNodes[o].endlessLoop = false;
							}
						}		
					
					
						graph.lastSelectedNodeIndex = -1;
					}
					
				
				}
				
				collectedNodes.ResetFoldout();
			}
			
			public NodePanel(Rect _rect, Graph _graph, GraphEditor _graphEditor, GUISkin _editorSkin, FREditorSettings _settings)
			{
				firstNodeName = "";
				current = this;
				//collectedNodes = _collectedNodes;
				rect = _rect;
				position = new Vector2(rect.x, rect.y);
				graph = _graph;
				graphEditor = _graphEditor;
				editorSkin = _editorSkin;
				settings = _settings;
			}
			
			public static void FocusEditor()
			{
				if ( current != null ) 
				{
					current.editorWindow.Focus();
					current.editorWindow.Repaint();
				}
			}
		}
	}
}
#endif