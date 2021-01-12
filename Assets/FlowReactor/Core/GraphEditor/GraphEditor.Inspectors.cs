//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.EventSystem;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public partial class GraphEditor :  EditorWindow
	{
		//////////////// INSPECTORS ////////////
		////////////////////////////////////////
		
		Vector2 blackboardScrollPos;
		Vector2 eventboardScrollPos;
		Vector2 nodeInspectorScrollPosition;
		
		bool foldoutNodeInfo = false;
		bool editNodeTitle = false;
		string newNodeTitle = "";
		
		
		/// <summary>
		/// Custom inspector
		/// </summary>
		void CustomInspector(int _selectedInspector)
		{
	
			if (_selectedInspector >= rootGraph.inspectors.Count)
				return;
	
			Event current = Event.current;
			switch(current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (DragAndDrop.objectReferences.Length > 0)
					{
						if (DragAndDrop.objectReferences[0] != null)
						{
							if ( PrefabUtility.GetPrefabAssetType(DragAndDrop.objectReferences[0]) == PrefabAssetType.NotAPrefab)
							{				
								FRVariableGUIUtility.sceneRefObjectDragging = true;
							}
							else
							{
								FRVariableGUIUtility.sceneRefObjectDragging = false;
							}
						}
					}
					break;
				case EventType.DragExited:
					FRVariableGUIUtility.sceneRefObjectDragging = false;
					break;
			}
	
			using (new GUILayout.HorizontalScope("toolbar"))
			{ 
	
				if (_selectedInspector > 0)
				{
					if (GUILayout.Button("x", "toolbarButton"))
					{
						rootGraph.inspectors.RemoveAt(_selectedInspector);
					}
				}
				else
				{
					if (GUILayout.Button("+", "toolbarButton"))
					{
						rootGraph.inspectors.Add(new Graph.GraphInspectors(0, 200f));
					}
				}
			
				if (_selectedInspector < rootGraph.inspectors.Count)
				{
					var _t = rootGraph.inspectors[_selectedInspector].selectedTab;
					_t = GUILayout.SelectionGrid(_t, inspectorPanelOptions, 7, GUI.skin.GetStyle("toolbarButton"));
				
					rootGraph.inspectors[_selectedInspector].selectedTab = _t;		
				}  
			}
			
			if (_selectedInspector >= rootGraph.inspectors.Count)
			{
				return;
			}
		
			switch (rootGraph.inspectors[_selectedInspector].selectedTab)
			{ 
				case 0:
					if (!EditorApplication.isCompiling)
					{	
						#if !FLOWREACTOR_DEBUG
						GUI.enabled = !rootGraph.currentGraph.isCopy;
						#endif

						NodeInspector();

						GUI.enabled = true;
						
						Repaint();
					}
						
					break;
				case 1: 

					BlackboardInspector();
					Repaint();
					
					break;
				case 2:
					
					EventsInspector();
					Repaint();
					break;
				case 3:
					if (!EditorApplication.isCompiling)
					{
						// show graph inspector
						GraphInspector();
					}
					break;
				case 4:
					
					// show settings
					SettingsInspector();
					settings.currentlySelectedGraph = rootGraph;
					break;
					
				case 5:
						
					// show help / about
					HelpInspector();
					break;
			}
		
			
			//// splitt view
			//splitterRect = new Rect(rootGraph.tmpInspectorWidth - 5, -1, 10, position.height);
			//EditorGUIUtility.AddCursorRect(splitterRect,MouseCursor.ResizeHorizontal);

			//using (var areaScope = new GUILayout.AreaScope(splitterRect))
			//{
			//	if (GUILayout.Button("","TextArea", GUILayout.Width(6), GUILayout.MaxWidth(6), GUILayout.MinWidth(6), GUILayout.ExpandHeight(true))){}
			//}
		}
	
		void NodeInspector()
		{
			if (rootGraph.currentGraph == null)
				return;

			if (rootGraph.selectedNode != null) // && rootGraph.selectedNodes != null && rootGraph.selectedNodes.Keys.Count > 0)
			{
				DrawInspectorForNode(rootGraph.selectedNode, 0);
			}
		}
		
		void DrawInspectorForNode(Node _node, int _nodeIndex)
		{
			if (editorSkin == null)
			{
				LoadEditorResources();
			}


			if (_node != null)
			{
	
				if (nodeInspector != null)
				{
					// when comment node is selected do not use scroll view
					// it somehow eats all user input from the canvas. Therefore delete button would not work
					if (_node.nodeData.nodeType == NodeAttributes.NodeType.Comment)
					{
					
					}
					else
					{
					
						using (var _scrollView = new EditorGUILayout.ScrollViewScope(nodeInspectorScrollPosition))
						{
			
							nodeInspectorScrollPosition = _scrollView.scrollPosition;
							
							using (new GUILayout.HorizontalScope("ToolbarButton"))
							{
								foldoutNodeInfo = EditorGUILayout.Foldout(foldoutNodeInfo, _node.nodeData.title);
							}
							if (foldoutNodeInfo)
							{
								using (new GUILayout.VerticalScope("Box"))
								{
								
								
									GUILayout.Label("Node type", GUILayout.Width(100));
									GUILayout.Label(_node.nodeData.typeName,"TextArea");
									
									
									GUILayout.Label("Title");
								
									using (new GUILayout.HorizontalScope())
									{
									
										
										if (!editNodeTitle)
										{
											GUI.enabled = editNodeTitle;
											_node.nodeData.title = GUILayout.TextField(_node.nodeData.title);
											GUI.enabled = true;
											
											if (GUILayout.Button(editIcon, GUILayout.Width(20), GUILayout.Height(20)))
											{
												editNodeTitle = true;
												newNodeTitle = _node.nodeData.title;
											}
											
										}
										else
										{
											newNodeTitle = GUILayout.TextField(newNodeTitle);
											
											if (!string.IsNullOrEmpty(newNodeTitle))
											{
												if (GUILayout.Button(okIcon, GUILayout.Width(20), GUILayout.Height(20)))
												{
													if (rootGraph.exposedNodeVariables.ContainsKey(_node.nodeData.title))
													{
														// Update node title and exposed variables
														if (!rootGraph.exposedNodeVariables.ContainsKey(newNodeTitle))
														{
															rootGraph.exposedNodeVariables.UpdateKey(_node.nodeData.title, newNodeTitle);
													
															_node.nodeData.title = newNodeTitle;
															
															List<FRVariable> _nodeVariables = new List<FRVariable>();
															FlowReactor.Editor.GetAvailableVariableTypes.GetAllFRVariablesOnNode(_node, out _nodeVariables);
															for (int v = 0; v < _nodeVariables.Count; v ++)
															{
																if (_nodeVariables[v].type == FRVariable.VariableType.exposed)
																{
																	_nodeVariables[v].exposedNodeName = newNodeTitle;
																}
															}
													
															editNodeTitle = false;
														}
														else
														{
															if (_node.nodeData.title != newNodeTitle)
															{
																EditorUtility.DisplayDialog("Node title already exists", "Title: " + newNodeTitle + " already exists!" + "\nThis node has exposed variables, therefore the new title must be unique to make sure it does not collide with another exposed node with the same title.", "ok");
																editNodeTitle = false;
															}
															else
															{
																editNodeTitle = false;
															}
														}
													}
													else
													{
														_node.nodeData.title = newNodeTitle;
													}
													
													editNodeTitle = false;
												}
											}
											
											if (GUILayout.Button(cancelIcon, GUILayout.Width(20), GUILayout.Height(20)))
											{
												editNodeTitle = false;
											}
										}
									}
									
								
									
									GUILayout.Label("Description");
									_node.nodeData.description = GUILayout.TextArea(_node.nodeData.description, GUILayout.Height(50));
									
									if (GUILayout.Button("Clear"))
									{
										_node.nodeData.description = "";
									}
								}
									
							}
							
							// Default node inspector
							#if FLOWREACTOR_DEBUG
							// Always draw default inspector in debug mode
							_node.disableDefaultInspector = false;
							GUI.color = Color.yellow;
							GUILayout.Label("DEBUG MODE:");
							GUI.color = Color.white;
							#endif
							if (!_node.disableDefaultInspector)
							{
								EditorHelpers.DrawUILine();
								using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
								{
									NodeDefaultInspector.DrawDefaultInspectorWithoutScriptField(nodeInspector);
									//nodeInspector.DrawDefaultInspector();	
									
								}		
							}
							
							
							
							//if (nodeVariableFields == null || nodeVariableFields.Length == 0)
							//{
							//	GetAvailableNodeVariables(_node);
							//}
						
							//for (int f = 0; f < nodeVariableFields.Length; f ++)
							//{
							//	if (nodeVariableFields[f].FieldType == typeof(FlowReactor.Nodes.Modules.FRNodeEventDispatch))
							//	{
							//		GUILayout.Label("EVENTBOARD");
							//		var _module = nodeVariableFields[f].GetValue(_node) as FlowReactor.Nodes.Modules.FRNodeEventDispatch;
							//		//_eventBoard = EditorGUILayout.ObjectField(_eventBoard, typeof(EventBoard), false) as EventBoard;
									
							//		var _lr = GUILayoutUtility.GetLastRect();
									
							//		//nodeVariableFields[f].SetValue(_node, (object)_eventBoard);
							//		FRNodeEventDispatcherEditor.DrawEditor(_module, _node.rootGraph, _lr);
							//	}
							//}
							
							
							if (!_node.disableDrawCustomInspector)
							{
								EditorHelpers.DrawUILine();
								using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
								{	
									if (_node != null)
									{
										_node.DrawCustomInspector();
									}
								}
							}
						
						
						
							if (!_node.disableVariableInspector)
							{
								EditorHelpers.DrawUILine();
								using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
								{
									// Get all available variable types
									if (variableLocalTypes.Keys.Count == 0 || variableLocalTypes == null)
									{
										GetAvailableVariableTypes.ReturnExistingTypesOfType<FRVariable>(out variableLocalTypes);
									}
									
		
									GUILayout.Label("Variables:", "boldLabel");
								
								
									_node.DrawNodeVariables(true);
									
									
								}
							}
							
							GUILayout.Space(25);
							
				
		
							//using (new GUILayout.HorizontalScope("toolbar"))
							//{ 
							//	selectedInspectorPanelNode = GUILayout.SelectionGrid(selectedInspectorPanelNode, inspectorPanelOptions, 7, GUI.skin.GetStyle("toolbarButton"));
							//}  
						}		
					}
				}
				
			
			}
		}
		
		
		void GetAvailableNodeVariables(Node _node)
		{
			if (_node == null)
				return;
				
			nodeVariableFields = new FieldInfo[]{};		
			nodeVariableFields = _node.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}
		
		void GraphInspector()
		{
			
			using (new GUILayout.HorizontalScope("toolbar"))
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_restore_a"), "toolbarButton", GUILayout.Width (20)))
				{
					var window = CreateInstance<GraphExplorerWindow>();
					window.OpenExplorer(window, rootGraph, settings, editorSkin, this);
				}
			}
				
			if (graphExplorer == null)
			{
				graphExplorer = new GraphExplorer(rootGraph, settings, editorSkin, this);
			}

			graphExplorer.DrawExplorer();
			
			Repaint();
		}
	
	
		void BlackboardInspector()
		{
		
			// Drag and drop 
			DropAreaGUI();
			GUI.depth = 0;
			
			
			using (var blackboardScrollView = new EditorGUILayout.ScrollViewScope(blackboardScrollPos))
			{
				blackboardScrollPos = blackboardScrollView.scrollPosition;
				
				//if (rootGraph.blackboards == null || rootGraph.blackboards.Keys.Count == 0)
				//	return;

				var _bbList = rootGraph.blackboards.Keys.ToList();
				for (int i = 0; i < _bbList.Count; i ++)
				{
				
					if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseUp)
					{
						if (rootGraph.blackboards[_bbList[i]].blackboard != null)
						{
							rootGraph.blackboards[_bbList[i]].blackboard.dragVariableFieldSize = false;
						}
					}
				
					using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine"))) //GUILayout.BeginVertical(editorSkin.GetStyle("BoxLine"));
					{
						using (new GUILayout.HorizontalScope())
						{
							if (GUILayout.Button(new GUIContent(rootGraph.blackboards[_bbList[i]].blackboard != null ? "  " + rootGraph.blackboards[_bbList[i]].blackboard.name : "", blackBoardIcon), FlowReactorEditorStyles.boardElement, GUILayout.Height(30)))
							{
								rootGraph.blackboards[_bbList[i]].foldout = !rootGraph.blackboards[_bbList[i]].foldout;	
							}
							
							if (GUILayout.Button("x", GUILayout.Width(30),  GUILayout.Height(30)))
							{
								var _bbName =  rootGraph.blackboards[_bbList[i]].blackboard == null ? "" : rootGraph.blackboards[_bbList[i]].blackboard.name;
								if (  EditorUtility.DisplayDialog("Remove Blackboard", "Are you sure you want to remove blackboard: " + _bbName + " from this graph", "yes", "no"))
								{
									if (rootGraph.blackboards[_bbList[i]].blackboard != null)
									{
										rootGraph.blackboards[_bbList[i]].blackboard.RemoveAllConnectedNode(rootGraph);
									}
									
									rootGraph.blackboards.Remove(_bbList[i]);
								}
							}
						}
						
						if (!rootGraph.blackboards.ContainsKey(_bbList[i]))
							return;
						
						if (rootGraph.blackboards[_bbList[i]].foldout)
						{
						
							using ( new GUILayout.HorizontalScope())
							{
							
									
								if (rootGraph.blackboards[_bbList[i]].blackboardEditor == null && rootGraph.blackboards[_bbList[i]].blackboard != null)
								{
									rootGraph.blackboards[_bbList[i]].blackboardEditor = UnityEditor.Editor.CreateEditor(rootGraph.blackboards[_bbList[i]].blackboard) as BlackBoardEditor;
									rootGraph.blackboards[_bbList[i]].blackboardEditor.SetupList();
								}
								
								rootGraph.blackboards[_bbList[i]].blackboard = (FlowReactor.BlackboardSystem.BlackBoard)EditorGUILayout.ObjectField(rootGraph.blackboards[_bbList[i]].blackboard, typeof(FlowReactor.BlackboardSystem.BlackBoard), false);				
						
								// Update blackboard if user has selected a different blackboard asset
								if (rootGraph.blackboards[_bbList[i]].blackboard != rootGraph.blackboards[_bbList[i]].lastBlackboard)
								{
							
									rootGraph.blackboards[_bbList[i]].lastBlackboard = rootGraph.blackboards[_bbList[i]].blackboard;
								
									rootGraph.blackboards[_bbList[i]].blackboardEditor = UnityEditor.Editor.CreateEditor(rootGraph.blackboards[_bbList[i]].blackboard) as BlackBoardEditor;
									rootGraph.blackboards[_bbList[i]].blackboardEditor.SetupList();
								}
								
								//if (GUILayout.Button("New Blackboard"))
								//{
								//	//var _newBB = rootGraph.blackboards[_bbList[i]].blackboardEditor.CreateNewBlackboard();
								//	var _newBB = BlackBoardEditor.CreateNewBlackboard();
								//	if (_newBB != null)
								//	{
								//		rootGraph.blackboards[_bbList[i]].blackboard = _newBB;
								//	}
								//}
							}
							
							
							// DRAW BLACKBOARD VARIABLE LIST
							if (rootGraph.blackboards[_bbList[i]].blackboard != null)
							{
							
								//if (rootGraph.blackboards[_bbList[i]].blackboardEditor == null)
								//{
								//	rootGraph.blackboards[_bbList[i]].blackboardEditor = UnityEditor.Editor.CreateEditor(rootGraph.blackboards[_bbList[i]].blackboard) as BlackBoardEditor;
								//	rootGraph.blackboards[_bbList[i]].blackboardEditor.SetupList();
								//}
								if (rootGraph.blackboards[_bbList[i]].blackboardEditor != null)
								{
									rootGraph.blackboards[_bbList[i]].blackboardEditor.DrawList(this, _bbList[i]);
								}
							}
						}
					}
					
					EditorHelpers.DrawUILine();

				}	
			
	
				GUILayout.Space(5);
				
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(50);
					if (GUILayout.Button("Add Blackboard", GUILayout.Width (rootGraph.tmpInspectorWidth - 100), GUILayout.Height (25)))
					{
						rootGraph.blackboards.Add(Guid.NewGuid(), new Graph.Blackboards());	
					}
				}
		
				//GUILayout.Space(25);
			
			}
		
		}
		

		
		void EventsInspector()
		{
			// Drag and drop 
			DropAreaGUI();
			
			using (var eventboardScrollView = new EditorGUILayout.ScrollViewScope(eventboardScrollPos))
			{
				eventboardScrollPos = eventboardScrollView.scrollPosition;
				
		
				var _ebList = rootGraph.eventboards.Keys.ToList();
				for (int i = 0; i < _ebList.Count; i ++)
				{
				
					using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
					{
	
						using (new GUILayout.HorizontalScope())
						{
							if (GUILayout.Button(new GUIContent(rootGraph.eventboards[_ebList[i]].eventboard != null ? "  " + rootGraph.eventboards[_ebList[i]].eventboard.name : "", eventBoardIcon), FlowReactorEditorStyles.boardElement, GUILayout.Height(30)))
							{
								rootGraph.eventboards[_ebList[i]].foldout = !rootGraph.eventboards[_ebList[i]].foldout;	
							}
								
							if (GUILayout.Button("x", GUILayout.Width(30),  GUILayout.Height(30)))
							{
								var _ebName =  rootGraph.eventboards[_ebList[i]].eventboard == null ? "" : rootGraph.eventboards[_ebList[i]].eventboard.name;
								
								if (EditorUtility.DisplayDialog("Remove Eventboard", "Are you sure you want to remove Eventboard: " + _ebName + " from this graph?" + "\n" + "Removing Eventboard will disconnect all event nodes in this graph from this eventboard.", "yes", "no"))
								{
									if (rootGraph.eventboards[_ebList[i]].eventboard != null)
									{
										rootGraph.eventboards[_ebList[i]].eventboard.DisconnectNodes(rootGraph);
									}
									
									rootGraph.eventboards.Remove(_ebList[i]);
								}
							}
						}
						
						if (!rootGraph.eventboards.ContainsKey(_ebList[i]))
							return;
							
						if (rootGraph.eventboards[_ebList[i]].foldout)
						{
							
							using ( new GUILayout.HorizontalScope())
							{
								if (rootGraph.eventboards[_ebList[i]].eventboardEditor == null)
								{
									rootGraph.eventboards[_ebList[i]].eventboardEditor = UnityEditor.Editor.CreateEditor(rootGraph.eventboards[_ebList[i]].eventboard) as EventBoardEditor;
								}
								
								
								rootGraph.eventboards[_ebList[i]].eventboard = (EventBoard)EditorGUILayout.ObjectField(rootGraph.eventboards[_ebList[i]].eventboard, typeof(EventBoard), false);				
							
								//if (GUILayout.Button("New Eventboard"))
								//{
								//	rootGraph.eventboards[_ebList[i]].eventboard = FlowReactor.Editor.EventBoardEditor.CreateNewEventboard();
								//}
							}
								
							if (rootGraph.eventboards[_ebList[i]].eventboard != null)
							{	
									
							
								
								rootGraph.eventboards[_ebList[i]].eventboardEditor.DrawDefaultGUI(this, rootGraph.eventboards[_ebList[i]].eventboard);
							}
						}
					}
					
					EditorHelpers.DrawUILine();
				}
				
				GUILayout.Space(5);
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(50);
					if (GUILayout.Button("Add Eventboard", GUILayout.Width (rootGraph.tmpInspectorWidth - 100), GUILayout.Height (25)))
					{
						rootGraph.eventboards.Add(Guid.NewGuid(), new Graph.Eventboards());	
					}
				}
				
				//GUILayout.Space(25);
			}
		}
		
		
		void DropAreaGUI()
		{
			GUI.depth = 1;
			Event _evt = Event.current;
			Rect _dropArea = new Rect(0, 0, position.width, position.height);
		
			if (isDragging)
			{
				GUI.color = Color.green;
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Box (_dropArea, "", "Label");
			GUI.color = Color.white;
			switch (_evt.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!_dropArea.Contains(_evt.mousePosition))
				{
					isDragging = false;
					return;
				}
				else
				{
					isDragging = true;
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if (_evt.type == EventType.DragPerform)
					{
						isDragging = false;
						DragAndDrop.AcceptDrag();
						foreach (System.Object _dobj in DragAndDrop.objectReferences)
						{
						
							if (_dobj.GetType() == typeof(FlowReactor.BlackboardSystem.BlackBoard))
							{
								var _bb = _dobj as FlowReactor.BlackboardSystem.BlackBoard;
								var _tmpBB =  new Graph.Blackboards();
								_tmpBB.blackboard = _bb;
								rootGraph.blackboards.Add(Guid.NewGuid(),_tmpBB);	
							
								Repaint();
								Event.current.Use();
								
								EditorUtility.SetDirty(rootGraph);
							}
							
							if (_dobj.GetType() == typeof(EventBoard))
							{
								var _ev = _dobj as EventBoard;
								var _tmpEB = new Graph.Eventboards();
								_tmpEB.eventboard = _ev;
								
								rootGraph.eventboards.Add(Guid.NewGuid(), _tmpEB);	
								Repaint();
								Event.current.Use();
								
								EditorUtility.SetDirty(rootGraph);
							}
						}
					}
				}
				break;
			}	
		}
		
		
		void SettingsInspector()
		{
			FREditorSettingsGUI.Draw(settings);
		}
	
		void HelpInspector()
		{	
			FlowReactorWelcomeWindow.DrawGUI(logo, settings);
		}
	}
}
#endif