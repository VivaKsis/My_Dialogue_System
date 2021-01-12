//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
#if FLOWREACTOR_DATABOX
using Databox;
#endif

namespace FlowReactor.Editor
{
	public class PopUps
	{
		
		public class GenericStringPopup : PopupWindowContent
		{
			public int _selected = 0;
			string[] options;
			
			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, 200);
			}
		
			public override void OnGUI(Rect rect)
			{	
				for(int e = 0; e < options.Length; e ++)
				{
					if (GUILayout.Button(options[e]))
					{
						_selected = e;	
						editorWindow.Close();
					}
				}
			}
			
			public override void OnOpen()	{	}
		
			public override void OnClose()	{	}
			
			public GenericStringPopup(string[] _options, out int _selectedEventboard)
			{
				options = _options;
				_selectedEventboard = _selected;
			}
		}
		
		public class PopupShowStringList : PopupWindowContent
		{
			List<string> selections = new List<string>();
			string searchTypeString = "";
			Rect rect = new Rect(0,0,0,0);
			Vector2 scrollPosition = Vector2.zero;
			string selected  = "";
			FieldInfo field;
			FieldInfo field2;
			string field2Value = "";
			
			object targetObject;
			
			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, 200);
			}
		
			public override void OnGUI(Rect rect)
			{	
				GUILayout.Label("Table", EditorStyles.boldLabel);
			
				
				using (new GUILayout.HorizontalScope())
				{
					GUI.SetNextControlName ("Filter");
					searchTypeString = GUILayout.TextField(searchTypeString, "SearchTextField");
					
					if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
					{
						searchTypeString = "";
					}
				}
			
				var _index = 0;
				
				using (var scrollView = new GUILayout.ScrollViewScope(scrollPosition))
				{
					scrollPosition = scrollView.scrollPosition;
				
					foreach(var entry in selections)
					{
						//Debug.Log(searchTypeString);
						if (entry.ToLower().Contains(searchTypeString.ToLower()) || string.IsNullOrEmpty(searchTypeString))
						{
							using (new GUILayout.HorizontalScope())
							{
								if (GUILayout.Button(entry.ToString()))
								{
									//DataboxEditor.duplicateToTable = entry.ToString();
									searchTypeString = "";
									editorWindow.Close();
									selected = entry.ToString();
									
									field.SetValue(targetObject, entry.ToString());
									
									if (field2 != null)
									{
										field2.SetValue(targetObject, field2Value);
									}
								}
							}
						}
						
						_index ++;
					}
				}
			}
		
			public override void OnOpen()
			{
				//Debug.Log("Popup opened: " + this);
				EditorGUI.FocusTextInControl ("Filter");
			}
		
			public override void OnClose()
			{
				//Debug.Log("Popup closed: " + this);
			
			}
			
		
			public PopupShowStringList(List<string> _tables, Rect _rect, object _targetObject, FieldInfo _field)
			{
				selections = new List<string>(_tables);
				rect = _rect;
				field = _field;
				targetObject = _targetObject;
			}
			
			public PopupShowStringList(List<string> _tables, Rect _rect, object _targetObject, FieldInfo _field, FieldInfo _field2)
			{
				selections = new List<string>(_tables);
				rect = _rect;
				field = _field;
				field2 = _field2;
				field2Value = _field2.GetValue(_targetObject) as string;
				targetObject = _targetObject;
			}
		}

		public class PopupShowPossibleBlackboardVariables : PopupWindowContent
		{
			Dictionary<string, Guid> selections = new Dictionary<string, Guid>();
			string searchTypeString = "";
			Rect rect = new Rect(0,0,0,0);
			Vector2 scrollPosition = Vector2.zero;
			string selected  = "";
			int selectedBB;
			Dictionary<Guid, Graph.Blackboards> blackboards = new Dictionary<Guid, Graph.Blackboards>();
			string newVariableName = "";
			
			FieldInfo selectedBlackboardGuid;
			FieldInfo selectedVariableGuid;
			FieldInfo selectedName;
			FieldInfo variableField;
			FRVariable variable;
			Graph rootGraph;
			FlowReactor.Nodes.Node node;
			
			string field2Value = "";
			object targetObject;
			
			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, 400);
			}
		
			public override void OnGUI(Rect rect)
			{	
				
				var bb = blackboards.Keys.Select(x => x.ToString()).ToArray();
				var bbName = blackboards.Values.Select(x => x.blackboard != null ? x.blackboard.name.ToString() : "empty").ToArray();
				
				GUILayout.Label("Blackboards:", "boldLabel");
				selectedBB = EditorGUILayout.Popup(selectedBB, bbName);
				
				if (blackboards.Keys.Count == 0)
				{	
					EditorGUILayout.HelpBox("Please add a blackboard", MessageType.Info);
					return;
				}
				
				if (blackboards[Guid.Parse(bb[selectedBB])].blackboard != null)
				{

					selectedBlackboardGuid.SetValue(targetObject, Guid.Parse(bb[selectedBB]));
					
					// find all compatible override variables of type field.fieldType
					var _compatibleTypes = new Dictionary<string, Guid>();
					
					//if (GUILayout.Button("EXPOSE"))
					//{
					//	rootGraph.exposedNodeVariables = new Dictionary<FlowReactor.Nodes.Node, Graph.ExposedVariables>();
						
					
					//	variable.type = FRVariable.VariableType.exposed;
					//	variable.graph = node.graphOwner;
					//	variable.nodeOwner = node;
					
					
					//	if (rootGraph.exposedNodeVariables == null)
					//	{
					//		rootGraph.exposedNodeVariables = new Dictionary<FlowReactor.Nodes.Node, Graph.ExposedVariables>();
					//	}
						
					//	rootGraph.exposedNodeVariables.Add(node, new Graph.ExposedVariables(variable));
					//}
					
					using (new GUILayout.VerticalScope("Box"))
					{
						GUILayout.Label("name:");
						newVariableName = EditorGUILayout.TextField(newVariableName);
						if (GUILayout.Button("create new variable"))
						{
					
							var _v = (FRVariable)Activator.CreateInstance(variable.GetType());
							
							if (string.IsNullOrEmpty(newVariableName))
							{
								_v.name = "My" + variable.GetType().Name.ToString();
							}
							else
							{
								_v.name = newVariableName;
							}
							_v.type = FRVariable.VariableType.blackboard;
							_v.blackboardGuid = Guid.Parse(bb[selectedBB]).ToString();
							_v.variableGuid = Guid.NewGuid().ToString();
							_v.assignedBlackboard = blackboards[Guid.Parse(bb[selectedBB])].blackboard;
							_v.graph = rootGraph;
							_v.sceneReferenceOnly = variable.sceneReferenceOnly;
							
							
							variable.type = FRVariable.VariableType.blackboard;
							variable.blackboardGuid = Guid.Parse(bb[selectedBB]).ToString();
							variable.variableGuid = _v.variableGuid;
							variable.assignedBlackboard = blackboards[Guid.Parse(bb[selectedBB])].blackboard;
							variable.graph = rootGraph;
												
					
							rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables.Add(Guid.Parse(_v.variableGuid), _v);
							
							rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
							foreach(var key in rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables.Keys)
							{
								rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
							}
							
							if (rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[Guid.Parse(_v.variableGuid)].connectedNodes == null)
							{
								rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[Guid.Parse(_v.variableGuid)].connectedNodes = new List<FlowReactor.Nodes.Node>();
							}
												
							rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[Guid.Parse(_v.variableGuid)].connectedNodes.Add(node);
							
						
						
							rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
							foreach(var key in rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables.Keys)
							{
								rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
							}
			
							EditorUtility.SetDirty(rootGraph.blackboards[Guid.Parse(bb[selectedBB])].blackboard);
							
							
							
							editorWindow.Close();
						}
					}
					
					foreach (var bbKey in blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables.Keys)
					{
						if (variableField != null)
						{
							if (blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].GetType() == variableField.FieldType)
							{							
								var _name = blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].name;
								if (string.IsNullOrEmpty(blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].name) || _compatibleTypes.ContainsKey(_name))
								{
									_name = bbKey.ToString();
								}
								_compatibleTypes.Add(_name, bbKey);
							}
							
						}
						else
						{
						
							if (blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].GetType() == variable.GetType())
							{							
								var _name = blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].name;
								if (string.IsNullOrEmpty(blackboards[Guid.Parse(bb[selectedBB])].blackboard.variables[bbKey].name) || _compatibleTypes.ContainsKey(_name))
								{
									_name = bbKey.ToString();
								}
								_compatibleTypes.Add(_name, bbKey);
							}
						}
					}
					
					
					selections = new Dictionary<string, Guid>(_compatibleTypes);
					
					GUILayout.Label("Variables:", "boldLabel");
					
					using (new GUILayout.HorizontalScope())
					{
						GUI.SetNextControlName ("Filter");
						searchTypeString = GUILayout.TextField(searchTypeString, "SearchTextField");
						
						if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
						{
							searchTypeString = "";
						}
					}
				
					var _index = 0;
					
					using (var scrollView = new GUILayout.ScrollViewScope(scrollPosition))
					{
						scrollPosition = scrollView.scrollPosition;
					
						foreach(var entry in selections.Keys)
						{
							//Debug.Log(searchTypeString);
							if (entry.ToLower().Contains(searchTypeString.ToLower()) || string.IsNullOrEmpty(searchTypeString))
							{
								using (new GUILayout.HorizontalScope())
								{
									if (GUILayout.Button(entry.ToString()))
									{
										//DataboxEditor.duplicateToTable = entry.ToString();
										searchTypeString = "";
										editorWindow.Close();
										selected = entry.ToString();
										
										selectedVariableGuid.SetValue(targetObject, selections[entry]); //entry.ToString());
										
										if (selectedName != null)
										{
											selectedName.SetValue(targetObject, field2Value);
										}
										
										if (variableField == null)
										{
											variable.type = FRVariable.VariableType.blackboard;
											variable.blackboardGuid = Guid.Parse(bb[selectedBB]).ToString();
											variable.variableGuid = selections[entry].ToString();
											variable.assignedBlackboard = blackboards[Guid.Parse(bb[selectedBB])].blackboard;
											variable.graph = rootGraph;
											
											if (rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].connectedNodes == null)
											{
												rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].connectedNodes = new List<FlowReactor.Nodes.Node>();
											}
											
											rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].connectedNodes.Add(node);
											
										
											//if (rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].sceneReferenceOnly)
											//{
											//	EditorUtility.DisplayDialog("Scene only variable", "You are connecting a node variable with a blackboard variable which already has a scene only variable connection. You can only assign a scene object (Blackboard scene override) to this variable.", "Ok");
											//}
											
											// set scene reference only. If a connected variable has a scene reference only attribute set the blackboard variable to scene ref only as well.
											// even though it might be connected with other non scene ref only variables. (scene ref always overrides others)
											if (!rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].sceneReferenceOnly && variable.sceneReferenceOnly)
											{
												rootGraph.blackboards[Guid.Parse(variable.blackboardGuid)].blackboard.variables[Guid.Parse(variable.variableGuid)].sceneReferenceOnly = variable.sceneReferenceOnly;
											}
											
											
				
										}
									}
								}
							}
							
							_index ++;
						}
					}
				}
			}
		
			public override void OnOpen()
			{
				EditorGUI.FocusTextInControl ("Filter");
			}
		
			public override void OnClose(){}
			
		
			//public PopupShowStringList(List<string> _tables, Rect _rect, object _targetObject, FieldInfo _field)
			//{
			//	selections = new List<string>(_tables);
			//	rect = _rect;
			//	field = _field;
			//	targetObject = _targetObject;
			//}
			
			public PopupShowPossibleBlackboardVariables(Dictionary<Guid, Graph.Blackboards> _blackboards, Rect _rect, object _targetObject, FieldInfo _variableField, FieldInfo _selectedBlackboardGuid, FieldInfo _selectedVariableGuid, FieldInfo _selectedName)
			{
				blackboards = _blackboards;
				variableField = _variableField;
				rect = _rect;
				selectedBlackboardGuid = _selectedBlackboardGuid;
				selectedVariableGuid = _selectedVariableGuid;
				selectedName = _selectedName;
				field2Value = _selectedName.GetValue(_targetObject) as string;
				targetObject = _targetObject;
			}
			
			public PopupShowPossibleBlackboardVariables(Dictionary<Guid, Graph.Blackboards> _blackboards, Rect _rect, object _targetObject, FRVariable _variable, FlowReactor.Nodes.Node _node, FieldInfo _selectedBlackboardGuid, FieldInfo _selectedVariableGuid, FieldInfo _selectedName)
			{
			
				//Debug.Log("open " + _variable.name + " " + _node.nodeData.title);
				blackboards = _blackboards;
				rect = _rect;
				variable = _variable;
				selectedBlackboardGuid = _selectedBlackboardGuid;
				selectedVariableGuid = _selectedVariableGuid;
				selectedName = _selectedName;
				field2Value = _selectedName.GetValue(_targetObject) as string;
				targetObject = _targetObject;
				rootGraph = _node.rootGraph;
				node = _node;
			}
		}

		
		#if FLOWREACTOR_DATABOX
		public class PopupDataboxVariables : PopupWindowContent
		{
			DataboxObjectManager databoxManager;
			FRVariable variable;
		
			int _so = 0;
			int _t = 0;
			int _e = 0;
			int _v = 0;
		
			string[] _objects;
			string[] _tables;
			string[] _entries;
			string[] _values;
		
		
			public override Vector2 GetWindowSize()
			{
				return new Vector2(220, 200);
			}
		
			public override void OnGUI(Rect rect)
			{	
				
				bool _noObjects = false;

				using (new GUILayout.HorizontalScope())
				{
					using (new GUILayout.VerticalScope())
					{
						_objects = (from o in  databoxManager.dataObjects
							select o.id.ToString()).ToArray();
					
						if (_objects.Length > 0)
						{
							GUILayout.Label("Databox object:");
							_so = EditorGUILayout.Popup("", _so, _objects);
						
							_tables = (from t in  databoxManager.dataObjects[_so].dataObject.DB
								select t.Key.ToString()).ToArray();
					
							if (_tables.Length == 0)
								return;
						
							GUILayout.Label("Table:");
							_t = EditorGUILayout.Popup("", _t, _tables); 
						
						
							_entries = (from e in  databoxManager.dataObjects[_so].dataObject.DB[_t].entries
								select e.Key.ToString()).ToArray();
							
							if (_entries.Length == 0)
								return;
							
							GUILayout.Label("Entry:");
							_e = EditorGUILayout.Popup("", _e, _entries); 
						
							_values = (from v in  databoxManager.dataObjects[_so].dataObject.DB[_t].entries[_e].data
								select v.Key.ToString()).ToArray();
						
							if (_values.Length == 0)
								return;
							
							GUILayout.Label("Value:");
							_v = EditorGUILayout.Popup("", _v, _values); 
						}
						else
						{
							_noObjects = true;
						}
					}
				}
			
				if (!_noObjects)
				{
					if (GUILayout.Button("Assign", GUILayout.Height(40)))
					{
						variable.useDatabox = true;
						variable.databoxID = _objects[_so];
						variable.tableID = _tables[_t];
						variable.entryID = _entries[_e];
						variable.valueID = _values[_v];
					
						editorWindow.Close();
					}
				}
			}
		
			public override void OnOpen()
			{
		
			}
		
			public override void OnClose()
			{
		
			}
		
			public PopupDataboxVariables(DataboxObjectManager _databoxManager, FRVariable _variable)//(FlowReactorDataboxAddon _addon, DataboxObjectManager _databoxManager, SONodeVariable _variable)
			{
				databoxManager = _databoxManager;
				variable = _variable;
			}
		}
		#endif
	}
}	
#endif