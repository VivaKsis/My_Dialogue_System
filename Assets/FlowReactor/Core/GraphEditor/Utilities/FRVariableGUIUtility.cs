//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public static class FRVariableGUIUtility
	{
		// set by the FlowReactorHierarchyEditor.cs
		// if user drags a scene object reference we lock all object fields
		public static bool sceneRefObjectDragging;
		public static GameObject sceneRefObject;
		
	
		public static string selectedFieldName;
		public static Guid selectedBlackboardGuid;
		public static Guid selectedVariableGuid;
		
		static Texture2D bbIcon;
		
		static bool isDragging;
		static bool showError;
		
		static Color colOrange = new Color(255f/255f, 100f/255f, 0f/255f);
		
		public static void DrawVariableAttributesNode(List<Node.NodeVariableGroups.NodeVariableAttribute> _attributes)
		{
			var _isHidden = false;
			DrawVariableAttributes( _attributes, false, out _isHidden);
		}
		
		public static void DrawVariableAttributesNode(List<Node.NodeVariableGroups.NodeVariableAttribute> _attributes, out bool _isHidden)
		{
			DrawVariableAttributes( _attributes, false, out _isHidden);
		}
		
		public static void DrawVariableAttributesInspector(List<Node.NodeVariableGroups.NodeVariableAttribute> _attributes)
		{
			var _isHidden = false;
			DrawVariableAttributes( _attributes, true, out _isHidden);
		}
		
		public static void DrawVariableAttributesInspector(List<Node.NodeVariableGroups.NodeVariableAttribute> _attributes, out bool _isHidden)
		{
			DrawVariableAttributes( _attributes, true, out _isHidden);
		}
		
		static void DrawVariableAttributes(List<Node.NodeVariableGroups.NodeVariableAttribute> _attributes, bool _inspectorAttribute, out bool _isHidden)
		{

			_isHidden = false;
			

			if (_attributes == null || _attributes.Count == 0)
			{
				return;
			}
				
			for (int a = 0; a < _attributes.Count; a ++)
			{
				// TITLE
				if (_attributes[a].GetType() == typeof (Node.NodeVariableGroups.NodeVariableTitleAttribute))
				{
					var _a = _attributes[a] as Node.NodeVariableGroups.NodeVariableTitleAttribute;
					GUILayout.Label(_a.attributeTitle, "boldLabel");
					EditorHelpers.DrawUILine();
					
				}
				
				// HELPBOX
				if (_attributes[a].GetType() == typeof (Node.NodeVariableGroups.NodeVariableHelpBoxAttribute) && _inspectorAttribute)
				{
					var _a = _attributes[a] as Node.NodeVariableGroups.NodeVariableHelpBoxAttribute;
					MessageType _mt = MessageType.None;
					switch(_a.attributeMessageType)
					{
					case HelpBox.MessageType.info:
						_mt = MessageType.Info;
						break;
					case HelpBox.MessageType.warning:
						_mt = MessageType.Warning;
						break;
					}
					EditorGUILayout.HelpBox(_a.attributeMessage, _mt);
				}
				
				// Open Url
				if (_attributes[a].GetType() == typeof (Node.NodeVariableGroups.NodeVariableOpenURLAttribute))
				{
					var _a = _attributes[a] as Node.NodeVariableGroups.NodeVariableOpenURLAttribute;
					if (GUILayout.Button(_a.attributeUrlTitle))
					{
						Application.OpenURL(_a.attributeUrl);
					}
				}
				
				// HIDE
				if (_attributes[a].GetType() == typeof(Hide))
				{
					if (_inspectorAttribute)
					{
						_isHidden = true;
					}
				}
				if (_attributes[a].GetType() == typeof(HideInInspector))
				{
					if (!_inspectorAttribute)
					{
						_isHidden = true;
					}
				}
			}
				
		}
		
		public static float GetVariableAttributeHeight(FieldInfo _fieldInfo)
		{
			float _returnHeight = 0f;
			var _attributes = _fieldInfo.GetCustomAttributes(false);
			if (_attributes.Length > 0)
			{
				for (int a = 0; a < _attributes.Length; a ++)
				{
					// TITLE
					if (_attributes[a].GetType() == typeof(Title))
					{
						_returnHeight += 22;
					}
					// URL
					if (_attributes[a].GetType() == typeof(OpenURL))
					{
						_returnHeight += 22;
					}
					if (_attributes[a].GetType() == typeof(HideInNode))
					{
						_returnHeight = 0f;
					}
				}
			}
			
			return _returnHeight;
		}
		
		
		
		public static void DrawVariable(FRVariable _variable, FlowReactor.Nodes.Node _node, bool _onlyBlackboard, GUISkin _editorSkin)
		{
			DrawVariable("", _variable, _node, _onlyBlackboard, _editorSkin);
		}
		
		public static void DrawVariable(string _variableName, FRVariable _variable, FlowReactor.Nodes.Node _node, bool _onlyBlackboard, GUISkin _editorSkin)
		{

			var _rootGraph = _node.rootGraph;
			//var _returnHeight = 22f;
			
			if (_variable == null)
			{
				return;
			}
			

			if (_variable.type == FRVariable.VariableType.local)
			{
				GUI.color = _node.color;
				
				using (new GUILayout.HorizontalScope())//_editorSkin.GetStyle("FRVariable")))
				{
					
					GUILayout.Label("",_editorSkin.GetStyle("LineVariables"));
					GUI.color = Color.white;
					
					if (!string.IsNullOrEmpty(_variableName))
					{
						GUILayout.Label(_variableName);
					}
					
					if (!_onlyBlackboard)
					{
					
						// Get Custom attributes for FRVariables and pass them to the variable Draw() method
						FieldInfo[] _f = _node.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						object[] _attributes = null;
						
						for (int f = 0;f < _f.Length; f++)
						{
							if (_f[f].GetValue(_node) == _variable)
							{
							
								_attributes = _f[f].GetCustomAttributes(false);
								//if (_attribute != null)
								//{
								//	Debug.Log("Found attribute " + _f[f].Name + " "  + _attribute.Length);
								//}
							}
						}
						
					
						GUI.enabled = !sceneRefObjectDragging;
						
						if (_attributes != null)
						{
							var _att = _attributes.FirstOrDefault( x => x.GetType() == typeof(SceneObjectOnly));
							if (_att != null)
							{
								_variable.sceneReferenceOnly = true;
					
								GUI.enabled = false;
								GUILayout.Label(new GUIContent("scene only", EditorGUIUtility.IconContent("d_SceneViewOrtho").image), "ObjectField");			
								GUI.enabled = true;
							}
							else
							{
								_variable.sceneReferenceOnly = false;
								_variable.Draw(false, _attributes);
							}
						}
						else
						{
							_variable.Draw(false, _attributes);
						}
						GUI.enabled = true;
						
						
						var _lr = GUILayoutUtility.GetLastRect();

						
						// DropArea for when user drags a blackboard variable to this node variable field
						DropAreaGUI(_lr, _variable, _node, _editorSkin);
						
					}
					else
					{
						GUILayout.Label("", GUILayout.Width(100));
					}
					

					var _dropdownRect = GUILayoutUtility.GetLastRect();
				
					
					GUI.enabled = _rootGraph.blackboards.Keys.Count > 0 ? true : false;
					GUI.enabled = !Application.isPlaying;
					GUI.SetNextControlName("NodeClear");
				
					//GUILayout.Label("F");
					//var _dropdownRect = GUILayoutUtility.GetLastRect();
					
					if (GUILayout.Button("", _editorSkin.GetStyle("VariableMenuButton"), GUILayout.Width(20)))
					{
						//VariableContextMenu(_dropdownRect, _rootGraph, _node, _variable);
						PopupWindow.Show(_dropdownRect, new VariableFieldPopup(_dropdownRect, _rootGraph, _node, _variable, _variableName));
					}
					
					//if (EditorGUILayout.DropdownButton(new GUIContent("", "connect to blackboard"), FocusType.Keyboard, _editorSkin.GetStyle("ConnectBlackboardButton"),  GUILayout.Width(20)))
					//{	
					//	//selectedFieldName = _variable.name;
					//	FieldInfo fieldSelectedBlackboardGuid = typeof(FRVariableGUIUtility).GetField("selectedBlackboardGuid");
					//	FieldInfo fieldSelectedVariableGuid = typeof(FRVariableGUIUtility).GetField("selectedVariableGuid");
					//	FieldInfo fieldSelectedName = typeof(FRVariableGUIUtility).GetField("selectedFieldName");
					
					//	if (fieldSelectedVariableGuid != null && fieldSelectedName != null && fieldSelectedBlackboardGuid != null)
					//	{			
					//		PopupWindow.Show(_dropdownRect, new PopUps.PopupShowPossibleBlackboardVariables(_rootGraph.blackboards, _dropdownRect, typeof(FlowReactor.Editor.FRVariableGUIUtility), _variable, _node, fieldSelectedBlackboardGuid, fieldSelectedVariableGuid, fieldSelectedName));
					//	}
					//}
																
					GUI.enabled = true;
										
				}
			}
			else if(_variable.type == FRVariable.VariableType.blackboard)
			{
				GUI.color = Color.black;

				using (new GUILayout.HorizontalScope())//_editorSkin.GetStyle("FRVariable")))
				{			
					
					if (!_rootGraph.blackboards.ContainsKey(Guid.Parse(_variable.blackboardGuid)))
					{
						// user has remove blackboard from graph
						// reset variable connection
						_variable.type = FRVariable.VariableType.local;
						_variable.blackboardGuid = "";
						_variable.variableGuid = "";
						_variable.assignedBlackboard = null;
						_variable.graph = null;
					}
					else
					{
						if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables.ContainsKey(Guid.Parse(_variable.variableGuid)))
						{
						
							var _bbPath = _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.name  + 
								" / " + _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].name;
							var _bbVarName = _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].name;
									
							GUILayout.Label("",_editorSkin.GetStyle("LineVariables"));
							GUI.color = Color.white;
							
							GUI.enabled = false;
							
							if (!string.IsNullOrEmpty(_variableName))
							{
								GUILayout.Label(new GUIContent(_variableName, "connected to: " + _bbPath));
							}
							
							if (_rootGraph.blackboards.ContainsKey(Guid.Parse(_variable.blackboardGuid)))
							{
								try
								{
									var _varName = _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].name;
								
									GUILayout.FlexibleSpace();
									
									var _var = _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)];
									//if (_var.GetType() == typeof(FRGameObject))
									//{
										//var _g = _var as FRGameObject;
										//if (_g.Value == null)
										//{
										//	GUILayout.Label(_bbPath);
										//}
										//else
										//{
										if (_var.sceneReferenceOnly)
										{
											GUI.enabled = false;
											GUILayout.Label(" = " + _bbPath);
											//GUILayout.Label(new GUIContent("scene only", EditorGUIUtility.IconContent("d_SceneViewOrtho").image), "ObjectField");			
											GUI.enabled = true;
										}
										else
										{
											GUI.enabled = false; //!sceneRefObjectDragging;
											GUILayout.Label(" = " + _bbVarName);
											_var.Draw(false, null);
											GUI.enabled = true;
										}
									//}
									//else
									//{
									//	GUI.enabled = !sceneRefObjectDragging;
									//	_var.Draw(false, null);
									//	GUI.enabled = true;
									//}
									//GUILayout.Label(_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.name  + " / " + _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].name);	
									var _lrect = GUILayoutUtility.GetLastRect();
									GUI.Label(_lrect, new GUIContent("", "connected to: " + _bbPath));
								
								}
								catch
								{
									GUILayout.Label(_variable.name);
									GUI.color = Color.red;
									GUILayout.Label( " : (Variable id doesn't exist)");
									GUI.color = Color.white;
								
								}
							
							}
							else
							{
								using (new GUILayout.HorizontalScope())
								{
									GUILayout.Label(_variable.name);
									GUI.color = Color.red;
									GUILayout.Label( " : (Blackboard id doesn't exist)");
									GUI.color = Color.white;
		
								}
							}
							
							GUILayout.FlexibleSpace();
	
							GUI.enabled = true;
							// or else show button to switch back to local node variable
							if (GUILayout.Button(new GUIContent("", "Disconnect from blackboard"), _editorSkin.GetStyle("DisconnectBlackboardButton"), GUILayout.Width(20)))
							{
								// remove connection from blackboard
								if (_rootGraph.blackboards.ContainsKey(Guid.Parse(_variable.blackboardGuid)))
								{
									if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables.ContainsKey(Guid.Parse(_variable.variableGuid)))
									{
										if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes != null)
										{
											_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes.Remove(_node);
										}
									}
								}
								
								if (_variable.sceneReferenceOnly)
								{
									bool _hasOtherSceneRefs = false;
									for (int n = 0; n < _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes.Count; n ++)
									{
										if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes[n] != _node)
										{
									
											List<FRVariable> _allNodeVariables;
											FlowReactor.Editor.GetAvailableVariableTypes.GetAllFRVariablesOnNode(_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes[n], out _allNodeVariables);
										
											for (int i = 0; i < _allNodeVariables.Count; i ++)
											{
												if (_allNodeVariables[i].sceneReferenceOnly)
												{
													_hasOtherSceneRefs = true;
												}
											}
											
										
										}
									}
									
									if (!_hasOtherSceneRefs)
									{
										_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].sceneReferenceOnly = false;
									}
								}
								
								_variable.type = FRVariable.VariableType.local;
								_variable.blackboardGuid = "";
								_variable.variableGuid = "";
								_variable.assignedBlackboard = null;
								_variable.graph = null;
																					
							}
						}
						else
						{
							_variable.type = FRVariable.VariableType.local;
							_variable.blackboardGuid = "";
							_variable.variableGuid = "";
							_variable.assignedBlackboard = null;
							_variable.graph = null;
						}
					}
				}
			}	
			else if (_variable.type == FRVariable.VariableType.exposed)
			{
				using (new GUILayout.HorizontalScope())
				{
					GUI.color = colOrange;
					GUILayout.Label("",_editorSkin.GetStyle("LineVariables"));
					GUI.color = Color.white;
					
					if (!string.IsNullOrEmpty(_variableName))
					{
						GUILayout.Label(new GUIContent(_variableName + " = " + _variable.exposedName, "exposed as: " + _variable.exposedName));
					}
					
					if (GUILayout.Button("", _editorSkin.GetStyle("VariableExposedButton"), GUILayout.Width(20)))
					{
						UnExposeVariable(_variable);
					}
				}
			}
		}
		

		
		//public static void DrawVariableW(FieldInfo _field, FRVariable _variable, FlowReactor.Nodes.Node _node, GUISkin _editorSkin)
		//{
		//	var _rootGraph = _node.rootGraph;
			
		//	if (_variable.type == FRVariable.VariableType.local)
		//	{
		//		GUI.color = new Color(100f / 255f, 140f / 255f, 120f / 255f);
		//		using (new GUILayout.HorizontalScope(_editorSkin.GetStyle("FRVariable")))
		//		{
		//			GUI.color = Color.white;
					
		//			_variable.Draw(false, null);
			
		//			var _dropdownRect = GUILayoutUtility.GetLastRect();
		//			var field = _field;
		//			GUI.enabled = _rootGraph.blackboards.Keys.Count > 0 ? true : false;
		//			GUI.enabled = !Application.isPlaying;									
		//			if (EditorGUILayout.DropdownButton(new GUIContent("Blackboard"), FocusType.Keyboard, "miniButton",  GUILayout.Width(80)))
		//			{	
		//				selectedFieldName = _variable.name;
		//				FieldInfo fieldSelectedBlackboardGuid = typeof(FRVariableGUIUtility).GetField("selectedBlackboardGuid");
		//				FieldInfo fieldSelectedVariableGuid = typeof(FRVariableGUIUtility).GetField("selectedVariableGuid");
		//				FieldInfo fieldSelectedName = typeof(FRVariableGUIUtility).GetField("selectedFieldName");
												
		//				if (fieldSelectedVariableGuid != null && fieldSelectedName != null && fieldSelectedBlackboardGuid != null)
		//				{			
		//					PopupWindow.Show(_dropdownRect, new PopUps.PopupShowPossibleBlackboardVariables(_rootGraph.blackboards, _dropdownRect, typeof(FlowReactor.Editor.FRVariableGUIUtility), field, fieldSelectedBlackboardGuid, fieldSelectedVariableGuid, fieldSelectedName));
		//				}
		//			}
																
		//			GUI.enabled = true;
										
		//			if ( !string.IsNullOrEmpty(selectedBlackboardGuid.ToString()) && !string.IsNullOrEmpty(selectedVariableGuid.ToString())) // && field.Name == selectedFieldName)
		//			{
																	
		//				FRVariable _selectedVar = null;
		//				if (_rootGraph.blackboards.ContainsKey(selectedBlackboardGuid))
		//				{		
		//					if(_rootGraph.blackboards[selectedBlackboardGuid].blackboard.variables.TryGetValue(selectedVariableGuid, out _selectedVar))
		//					{
								
		//						if (_rootGraph.blackboards[selectedBlackboardGuid].blackboard.variables[selectedVariableGuid].connectedNodes == null)
		//						{
		//							_rootGraph.blackboards[selectedBlackboardGuid].blackboard.variables[selectedVariableGuid].connectedNodes = new List<FlowReactor.Nodes.Node>();
		//						}
											
		//						_rootGraph.blackboards[selectedBlackboardGuid].blackboard.variables[selectedVariableGuid].connectedNodes.Add(_node);
																		
								
		//						_variable.type = FRVariable.VariableType.blackboard;
		//						_variable.blackboardGuid = selectedBlackboardGuid.ToString();
		//						_variable.variableGuid = selectedVariableGuid.ToString();
		//						_variable.assignedBlackboard = _rootGraph.blackboards[selectedBlackboardGuid].blackboard;
		//						_variable.graph = _rootGraph;
		//					}
		//				}
																	
		//				selectedBlackboardGuid = Guid.Empty;
		//				selectedVariableGuid = Guid.Empty;
		//				selectedFieldName = "-";
																	
																
		//			}
		//		}
		//	}
		//	else if(_variable.type == FRVariable.VariableType.blackboard)
		//	{
		//		GUI.color = Color.black;
		//		using (new GUILayout.HorizontalScope(_editorSkin.GetStyle("FRVariable")))
		//		{
		//			GUI.color = Color.white;
												
		//			if (_rootGraph.blackboards.ContainsKey(Guid.Parse((_variable.blackboardGuid))))
		//			{
		//				GUILayout.Label(_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.name + " / " + _rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].name);	
		//			}
		//			else
		//			{
		//				GUI.color = Color.red;
		//				GUILayout.Label("Blackboard id does not exist");
		//				GUI.color = Color.white;
		//			}
					
		//			GUI.enabled = !Application.isPlaying;
					
		//			// or else show button to switch back to local node variable
		//			if (GUILayout.Button("Disconnect", "miniButton", GUILayout.Width(80)))
		//			{
		//				// remove connection from blackboard
		//				if (_rootGraph.blackboards.ContainsKey(Guid.Parse(_variable.blackboardGuid)))
		//				{
		//					if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables.ContainsKey(Guid.Parse(_variable.variableGuid)))
		//					{
		//						if (_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes != null)
		//						{
		//							_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].connectedNodes.Remove(_node);
		//						}
		//					}
		//				}
						
		//				_rootGraph.blackboards[Guid.Parse(_variable.blackboardGuid)].blackboard.variables[Guid.Parse(_variable.variableGuid)].sceneReferenceOnly = false;
						
		//				_variable.type = FRVariable.VariableType.local;
		//				_variable.blackboardGuid = "";
		//				_variable.variableGuid = "";
		//				_variable.assignedBlackboard = null;
		//				_variable.graph = null;
																			
		//			}
		//		}
				
		//		GUI.enabled = true;
		//	}		
									
		//}
		
	
		
		public static void DrawVariableNodeInput(string _variableName, FRVariable _variable, FlowReactor.Nodes.Node _node, bool _onlyBlackboard, GUISkin _editorSkin)
		{
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("o", GUILayout.Width(20))){}
				GUILayout.Label(_variableName);
			}
		}
		
		
		static void DropAreaGUI(Rect _rect, FRVariable _nodeVariable, Node _node, GUISkin _skin)
		{
			Rect _dropArea = new Rect(_rect.x - 2, _rect.y - 2, _rect.width + 4, _rect.height + 4);
			Event _evt = Event.current;
			
			if (_dropArea.Contains(_evt.mousePosition) && BlackBoardVariableDragProperties.isDragging)
			{
				if (BlackBoardVariableDragProperties.variable.GetType() == _nodeVariable.GetType())
				{
					GUI.color = Color.green;
				}
				else
				{
					GUI.color = Color.red;
				}
				
				GUI.Box (_dropArea, "", _skin.GetStyle("FRVariable"));
			}
			
			GUI.color = Color.white;
			
		
			
			if (_evt.type == EventType.MouseUp && _dropArea.Contains(_evt.mousePosition))
			{
				
				if (BlackBoardVariableDragProperties.variable != null && _nodeVariable != null)
				{
					if (BlackBoardVariableDragProperties.variable.GetType() == _nodeVariable.GetType())
					{
						
						var bb = _node.graphOwner.blackboards.Keys.Select(x => x.ToString()).ToArray();
						
						// Connect BB variable to node variable		
						_nodeVariable.type = FRVariable.VariableType.blackboard;
						_nodeVariable.variableGuid = BlackBoardVariableDragProperties.blackboardVariableGuid.ToString();
						_nodeVariable.blackboardGuid = BlackBoardVariableDragProperties.blackboardGuid.ToString();
						_nodeVariable.assignedBlackboard = _node.rootGraph.blackboards[BlackBoardVariableDragProperties.blackboardGuid].blackboard;
						_nodeVariable.graph = _node.rootGraph;
						
					
						if (!_node.rootGraph.blackboards[Guid.Parse(_nodeVariable.blackboardGuid)].blackboard.variables[Guid.Parse(_nodeVariable.variableGuid)].sceneReferenceOnly && _nodeVariable.sceneReferenceOnly)
						{
							_node.rootGraph.blackboards[Guid.Parse(_nodeVariable.blackboardGuid)].blackboard.variables[Guid.Parse(_nodeVariable.variableGuid)].sceneReferenceOnly = true;
						}
						
						if (BlackBoardVariableDragProperties.variable.connectedNodes == null)
						{
							BlackBoardVariableDragProperties.variable.connectedNodes = new List<FlowReactor.Nodes.Node>();
						}
													
						BlackBoardVariableDragProperties.variable.connectedNodes.Add(_node);
							
						BlackBoardVariableDragProperties.isDragging = false;
					
						GUI.FocusControl("");
						
						BlackBoardVariableDragProperties.editor.SetupListWithoutResLoading();
						
					}					
					else
					{
						BlackBoardVariableDragProperties.isDragging = false;
					}
				}
				else
				{
					BlackBoardVariableDragProperties.isDragging = false;
				}
				
				BlackBoardVariableDragProperties.variable = null;
				
				
				_evt.Use();
				
			}
		}
		
		
		public static void ExposeVariable(Graph _rootGraph, Node _node, FRVariable _variable, string _variableName)
		{
			_variable.type = FRVariable.VariableType.exposed;
			_variable.graph = _node.graphOwner;
			_variable.nodeOwner = _node;
			_variable.exposedNodeName = _node.nodeData.title;
			_variable.exposedName = _variableName;
			_variable.name = _variableName;
			
			//Debug.Log("expose variable node name: " + _node.nodeData.title);
				
			if (_rootGraph.exposedNodeVariables == null)
			{
				_rootGraph.exposedNodeVariables = new Dictionary<string, Graph.ExposedVariables>();
			}
				
				
			if (!_rootGraph.exposedNodeVariables.ContainsKey(_node.nodeData.title))
			{
				_rootGraph.exposedNodeVariables.Add(_node.nodeData.title, new Graph.ExposedVariables(_node, _variableName, _variable));
			}
			else
			{

				if (_rootGraph.exposedNodeVariables[_node.nodeData.title].node != _node)
				{
					// is it the same node?
					var _newNodeName = ReturnNewNodeName(_rootGraph.exposedNodeVariables, _node.nodeData.title);
					_node.nodeData.title = _newNodeName;
					
					_variable.exposedNodeName = _newNodeName;
					
					_rootGraph.exposedNodeVariables.Add(_node.nodeData.title, new Graph.ExposedVariables(_node, _variableName, _variable));
				}
				else
				{
					if (!_rootGraph.exposedNodeVariables[_node.nodeData.title].variables.ContainsKey(_variableName))
					{
						_rootGraph.exposedNodeVariables[_node.nodeData.title].variables.Add(_variableName, _variable); // = Graph.AddToVariablesWithUniqueExposedName(variable, rootGraph.exposedNodeVariables[node].variables);
					}
				}
			}
		}
		
		public static void UnExposeVariable(FRVariable _variable)
		{
			if (_variable.nodeOwner.rootGraph.exposedNodeVariables.ContainsKey(_variable.nodeOwner.nodeData.title))
			{
				if (_variable.nodeOwner.rootGraph.exposedNodeVariables[_variable.nodeOwner.nodeData.title].variables.ContainsKey(_variable.exposedName))
				{
					_variable.nodeOwner.rootGraph.exposedNodeVariables[_variable.nodeOwner.nodeData.title].variables.Remove(_variable.exposedName);
								
					if (_variable.nodeOwner.rootGraph.exposedNodeVariables[_variable.nodeOwner.nodeData.title].variables.Keys.Count == 0)
					{
						_variable.nodeOwner.rootGraph.exposedNodeVariables.Remove(_variable.nodeOwner.nodeData.title);
					}
								
					_variable.type = FRVariable.VariableType.local;
					_variable.graph = null;
					_variable.nodeOwner = null;						
				}
			}
		}
		
		static string ReturnNewNodeName(Dictionary<string, Graph.ExposedVariables> _exposedNodes, string _nodeName)
		{
			bool _foundName = false;
			string _name = "";
			int _nameCount = 1;
			while (!_foundName)
			{
			
				_name = _nodeName + "_" + _nameCount;
				
		
				if (!_exposedNodes.ContainsKey(_name))
				{
					_foundName = true;
				}
				else
				{
					//Debug.Log("key exists " + _name);
				}
		
				_nameCount ++;
			}
			
			return _name;
		}
		
		
		
		public class VariableFieldPopup : PopupWindowContent
		{
			Rect dropdownRect;
			Graph rootGraph;
			Node node;
			FRVariable variable;
			string variableName;
			
			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, 45);
			}
			
			public override void OnGUI(Rect rect)
			{	
				if (GUILayout.Button("Connect to Blackboard"))
				{
				
					//selectedFieldName = _variable.name;
					FieldInfo fieldSelectedBlackboardGuid = typeof(FRVariableGUIUtility).GetField("selectedBlackboardGuid");
					FieldInfo fieldSelectedVariableGuid = typeof(FRVariableGUIUtility).GetField("selectedVariableGuid");
					FieldInfo fieldSelectedName = typeof(FRVariableGUIUtility).GetField("selectedFieldName");
				
				
					if (fieldSelectedVariableGuid != null && fieldSelectedName != null && fieldSelectedBlackboardGuid != null)
					{	
						
						#if !UNITY_2020_1_OR_NEWER
						FlowReactor.Editor.EditorCoroutines.Execute(OpenBlackboardPopupDelayed());
						PopupWindow.Show(dropdownRect, new PopUps.PopupShowPossibleBlackboardVariables(rootGraph.blackboards, dropdownRect, typeof(FlowReactor.Editor.FRVariableGUIUtility), variable, node, fieldSelectedBlackboardGuid, fieldSelectedVariableGuid, fieldSelectedName));
						
						#else
						PopupWindow.Show(dropdownRect, new PopUps.PopupShowPossibleBlackboardVariables(rootGraph.blackboards, dropdownRect, typeof(FlowReactor.Editor.FRVariableGUIUtility), variable, node, fieldSelectedBlackboardGuid, fieldSelectedVariableGuid, fieldSelectedName));
						#endif
						
					}
					
				
				}
				if (GUILayout.Button("Expose to scene"))
				{
				
					ExposeVariable(rootGraph, node, variable, variableName);
		
					editorWindow.Close();
				}
				
				
			}

			// We have to delay this to make sure the VariableFieldPopup has been closed first
			IEnumerator OpenBlackboardPopupDelayed()
			{
			
				var startTime = EditorApplication.timeSinceStartup;
				while (EditorApplication.timeSinceStartup < startTime + 0.1f)
				{
					yield return 0f;
				}
				editorWindow.Close();
			}
			
			public VariableFieldPopup (Rect _dropdownRect, Graph _rootGraph, Node _node, FRVariable _variable, string _variableName)
			{
				dropdownRect = _dropdownRect;
				rootGraph = _rootGraph;
				node = _node;
				variable = _variable;
				variableName = _variableName;
			}
		
		}
	}
}

#endif