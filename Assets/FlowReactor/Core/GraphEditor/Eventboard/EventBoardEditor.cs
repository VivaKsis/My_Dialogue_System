//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.EventSystem;

namespace FlowReactor.Editor
{
	[CustomEditor(typeof(EventBoard))]
	public class EventBoardEditor : UnityEditor.Editor
	{
		public EventBoard eventBoard;
		
		public static string newEventName = "";
		static GUISkin editorSkin;
		static Dictionary<string, System.Type> variableSceneTypes = new Dictionary<string, System.Type>();
		
		FREditorSettings settings;
		
		public class GenericMenuData
		{
			public EventBoard currentEventBoard;
			public Type type;
			public string typeName;
			public int selected;
			
			public GenericMenuData (){}
		}
		
		public void OnEnable()
		{
			if (target == null)
				return;
				
			if (variableSceneTypes.Keys.Count == 0 || variableSceneTypes == null) 
			{
				GetAvailableVariableTypes.GetFlowReactorVariableTypes(out variableSceneTypes);
			}
				
			eventBoard = (EventBoard)target;
		}
		
		public override void OnInspectorGUI()
		{
			using (new GUILayout.VerticalScope())
			{
				DrawDefaultGUI(null, eventBoard);
			}
		}
		
	
		public void DrawDefaultGUI(GraphEditor _editor, EventBoard _eventBoard)
		{
			if (editorSkin  == null)
			{
				editorSkin = EditorHelpers.LoadSkin();
			}
			
			if (settings == null)
			{
				settings = FREditorSettings.GetOrCreateSettings();
			}
			
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
					
				GUILayout.Label("Event name:");
				
				using (new GUILayout.HorizontalScope())
				{
					newEventName = GUILayout.TextField(newEventName);
					
					if (GUILayout.Button("Add Event"))
					{
						if (_eventBoard.events == null)
						{
							_eventBoard.events = new FlowReactor.OrderedDictionary.OrderedDictionary<Guid, EventBoard.Event>();
						}
						
						_eventBoard.AddNewEvent(newEventName);	
						newEventName = "";
					}
				}
			}
			
			if (_eventBoard.events == null)
				return;
			if (_eventBoard.events.Keys.Count == 0)
				return;
				
			var _events = _eventBoard.events.Keys.ToList();
			var _statsFoldout = new List<bool>( new bool[_events.Count]);
			
		
		
			GUILayout.Label("Events:", "boldLabel");
			
			EditorHelpers.DrawUILine();
			
			try{
			for(int e = 0; e < _events.Count; e ++)
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
					GUILayout.Space (10);
					_eventBoard.events[_events[e]].foldout = EditorGUILayout.Foldout(_eventBoard.events[_events[e]].foldout, _eventBoard.events[_events[e]].name);
				
					if (e > 0)
					{
						if (GUILayout.Button("▲", "toolbarButton", GUILayout.Width(20)))
						{
							var _e = _eventBoard.events[_events[e]];
							_eventBoard.events.RemoveAt(e);
							_eventBoard.events.Insert(e - 1, _events[e], _e);
						}
					}
					
					if (e < _events.Count - 1)
					{
						if (GUILayout.Button("▼", "toolbarButton", GUILayout.Width(20)))
						{
							var _e = _eventBoard.events[_events[e]];
							_eventBoard.events.RemoveAt(e);
							_eventBoard.events.Insert(e + 1, _events[e], _e);
						}
					}
					
					if (GUILayout.Button("x", "toolbarButton", GUILayout.Width(20)))
					{
						_eventBoard.events.Remove(_events[e]);
					}
				}
				
				if (_eventBoard.events[_events[e]].foldout)
				{
					using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
					{
						var _noCallEventNodes = false;
						var _noEventListenerNodes = false;
						if (_eventBoard.events[_events[e]].callEventNodes != null)
						{
							if (_eventBoard.events[_events[e]].callEventNodes.Count == 0)
							{
								_noCallEventNodes = true;
							}
						}
						else
						{
							_noCallEventNodes = true;
						}
						
						if (_eventBoard.events[_events[e]].listenerEventNodes != null)
						{
							if (_eventBoard.events[_events[e]].listenerEventNodes.Count == 0)
							{
								_noEventListenerNodes = true;
							}
						}
						else
						{
							_noEventListenerNodes = true;
						}
						
						if (_noCallEventNodes && _noEventListenerNodes)
						{
							EditorGUILayout.HelpBox("Event is not being used in any graph", MessageType.Warning);
						}
						
						using (new GUILayout.HorizontalScope()) //editorSkin.GetStyle("Box")))
						{
							//GUILayout.Label("Event:");
							_eventBoard.events[_events[e]].name = GUILayout.TextField(_eventBoard.events[_events[e]].name);
							
							if (Application.isPlaying)
							{
								if (GUILayout.Button("Call Event", "miniButton"))
								{
									_eventBoard.events[_events[e]].Raise(null);
								}
							}
							
							//if (GUILayout.Button("x", GUILayout.Width(20)))
							//{
							//	_eventBoard.events.Remove(_events[e]);
							//}
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Space(20);
							
							using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
							{
													
								if (GUILayout.Button("Add Parameter", "miniButton"))
								{
									if (_eventBoard.events[_events[e]].parameters == null)
									{
										_eventBoard.events[_events[e]].parameters = new FlowReactor.OrderedDictionary.OrderedDictionary<Guid, FRVariable>();
									}
		
									var menu = new GenericMenu();
									
									foreach(var key in variableSceneTypes.Keys)
									{
										var _data = new GenericMenuData();
										_data.type = variableSceneTypes[key];
										_data.selected = e;
										_data.typeName = key;
										_data.currentEventBoard = _eventBoard;
										menu.AddItem(new GUIContent(key), false, AddVariableCallback, _data);
									}
									
									menu.ShowAsContext();
								}
								
								if (_eventBoard.events[_events[e]].parameters != null)
								{
									foreach (var p in _eventBoard.events[_events[e]].parameters.Keys)
									{
										using (new GUILayout.HorizontalScope())
										{
											
											GUILayout.Label(_eventBoard.events[_events[e]].parameters[p].typeName, GUILayout.MaxWidth(100));
											
											_eventBoard.events[_events[e]].parameters[p].name = GUILayout.TextField(_eventBoard.events[_events[e]].parameters[p].name);
											
										
											
											if (GUILayout.Button("x", "miniButton", GUILayout.Width(20)))
											{
												_eventBoard.events[_events[e]].parameters.Remove(p);
											}
										}
									}
								}
							}
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Space(20);
							_eventBoard.events[_events[e]].connectedNodeFoldout = EditorGUILayout.Foldout(	_eventBoard.events[_events[e]].connectedNodeFoldout, "Connected nodes");
						}
						if (_eventBoard.events[_events[e]].connectedNodeFoldout )
						{
							using (new GUILayout.HorizontalScope())
							{
								GUILayout.Space(20);
								DrawMap(_editor, _eventBoard, _events[e]);
							}
						}
						
					}
				}
				
				//if (Application.isPlaying && _eventBoard.events[_events[e]].eventListeners != null)
				//{
				//	_statsFoldout[e] = EditorGUILayout.Toggle("Stats", _statsFoldout[e]);
				//	if (_statsFoldout[e])
				//	{
				//		using (new GUILayout.VerticalScope())
				//		{
				//			GUILayout.Label("Listeners registered to " + _eventBoard.events[_events[e]].name + " :", "boldLabel");
								
				//			if (_eventBoard.events[_events[e]].eventListeners != null)
				//			{
				//				GUILayout.Label("Listener count: " + _eventBoard.events[_events[e]].eventListeners.Count.ToString());
				//			}
								
				//			for (int l = 0; l < _eventBoard.events[_events[e]].eventListeners.Count; l ++)
				//			{
				//				//GUILayout.Label(l.ToString() + " : " + eventBoard.events[_events[e]].eventListeners[l]);
				//				GUILayout.Label(l.ToString() + " : " + _eventBoard.events[_events[e]].eventListeners[l].graphOwner.name + " - " + _eventBoard.events[_events[e]].eventListeners[l].name);
				//			}
								
				//			if (_eventBoard.events[_events[e]].eventComponentListeners != null)
				//			{	
				//				GUILayout.Label("-----");
				//				GUILayout.Label("Scene listeners", "boldLabel");
			
				//				GUILayout.Label("Listener count: " + _eventBoard.events[_events[e]].eventComponentListeners.Count.ToString());
							
								
				//				for (int l = 0; l < _eventBoard.events[_events[e]].eventComponentListeners.Count; l ++)
				//				{
				//					GUILayout.Label(l.ToString() + " : " + _eventBoard.events[_events[e]].eventComponentListeners[l].name);
				//				}
				//			}
								
				//		}	
				//	}
				//}
			}
			}
			catch
			{
				
			}
			
			EditorUtility.SetDirty (_eventBoard);
		}
		
		// Display all registered call event nodes and event listener nodes
		public void DrawMap(GraphEditor _editor, EventBoard _eventBoard, Guid _event)
		{
			#if FLOWREACTOR_DEBUG
			GUI.contentColor = Color.yellow;
			if (GUILayout.Button("clear"))
			{
				_eventBoard.events[_event].callEventNodes = new List<FlowReactor.Nodes.Node>();
				_eventBoard.events[_event].listenerEventNodes = new List<FlowReactor.Nodes.Node>();
			}
			GUI.contentColor = Color.white;
			#endif
			
		
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
			{
			
				//GUILayout.Label("Call event nodes:");
			
				if (_eventBoard.events[_event].callEventNodes != null)
				{
					
					using (new GUILayout.VerticalScope())
					{
						for (int i = 0; i < _eventBoard.events[_event].callEventNodes.Count; i ++)
						{
						
							if (_editor != null)
							{
						
								if (_editor.rootGraph == _eventBoard.events[_event].callEventNodes[i].node.rootGraph)
								{
									using (new GUILayout.HorizontalScope())
									{
										GUI.contentColor = _eventBoard.events[_event].callEventNodes[i].node.color;
									
										if (GUILayout.Button(_eventBoard.events[_event].callEventNodes[i].node.name + " : " + _eventBoard.events[_event].callEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].callEventNodes[i].node.graphOwner.name, FlowReactorEditorStyles.graphExplorerButton, GUILayout.ExpandWidth(true)))
										{		
											_editor.GotoNode(_eventBoard.events[_event].callEventNodes[i].node);
										}
										
										GUI.contentColor = Color.white;
									}
								}
								else
								{
									GUI.contentColor = _eventBoard.events[_event].callEventNodes[i].node.color;
									GUILayout.Label(eventBoard.events[_event].callEventNodes[i].node.name + " : " + _eventBoard.events[_event].callEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].callEventNodes[i].node.graphOwner.name);
									GUI.contentColor = Color.white;
								}	
							}
							else
							{
								if (_eventBoard.events[_event].callEventNodes[i] == null)
								{
									// graph doesn't exist
									_eventBoard.events[_event].callEventNodes.RemoveAt(i);
								}
								else
								{
									GUI.contentColor = _eventBoard.events[_event].callEventNodes[i].node.color;
									GUILayout.Label(_eventBoard.events[_event].callEventNodes[i].node.name + " : " + _eventBoard.events[_event].callEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].callEventNodes[i].node.graphOwner.name);
									GUI.contentColor = Color.white;
								}	 
							}
							
						
						}				
					}
				}
			
			//}
			
			//GUILayout.Space(5);
			
			//GUI.color = settings.GetColor("eventNodeColor");
			//using (new GUILayout.VerticalScope(editorSkin.GetStyle("FRVariable")))
			//{
				//GUI.color = Color.white;
				//GUILayout.Label("Event listener nodes:");
				if ( _eventBoard.events[_event].listenerEventNodes != null)
				{
					
					using (new GUILayout.VerticalScope())
					{

						for (int i = 0; i < _eventBoard.events[_event].listenerEventNodes.Count; i ++)
						{
							if (_editor != null)
							{
								if (_editor.rootGraph == _eventBoard.events[_event].listenerEventNodes[i].node.rootGraph)
								{
									using (new GUILayout.HorizontalScope())
									{
										GUI.contentColor = _eventBoard.events[_event].listenerEventNodes[i].node.color;
									
										if (GUILayout.Button(_eventBoard.events[_event].listenerEventNodes[i].node.name + " : " + _eventBoard.events[_event].listenerEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].listenerEventNodes[i].node.graphOwner.name, FlowReactorEditorStyles.graphExplorerButton, GUILayout.ExpandWidth(true)))
										{
											_editor.GotoNode(_eventBoard.events[_event].listenerEventNodes[i].node);
										}
										GUI.contentColor = Color.white;
									}
								}
								else
								{
									GUI.contentColor = _eventBoard.events[_event].listenerEventNodes[i].node.color;
									GUILayout.Label(_eventBoard.events[_event].listenerEventNodes[i].node.name + " : "  + _eventBoard.events[_event].listenerEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].listenerEventNodes[i].node.graphOwner.name);
									GUI.contentColor = Color.white;
								}
							}
							else
							{
								if (_eventBoard.events[_event].listenerEventNodes[i] == null)
								{
									// graph doesn't exist
									_eventBoard.events[_event].listenerEventNodes.RemoveAt(i);
								}
								else
								{
									GUI.contentColor = _eventBoard.events[_event].listenerEventNodes[i].node.color;
									GUILayout.Label(_eventBoard.events[_event].listenerEventNodes[i].node.name + " : " + _eventBoard.events[_event].listenerEventNodes[i].node.rootGraph.name + " / " + _eventBoard.events[_event].listenerEventNodes[i].node.graphOwner.name);
									GUI.contentColor = Color.white;
								}
							}
						}
					}
				}		
			}
		}
		
		public void DrawNodeInspector(EventBoard _eventBoard, string _currentSelected, out string _selectedID, out int _selectedIDInt)
		{
			if (editorSkin == null)
			{
				editorSkin = EditorHelpers.LoadSkin();
			}
			
			_selectedID = _currentSelected;
			
			
			var _index = 0;
			foreach(var e in _eventBoard.events.Keys)
			{
				if (!string.IsNullOrEmpty(_currentSelected))
				{
					if (e == Guid.Parse(_currentSelected))
					{
						
						break;
					}
				}
				
				_index ++;
			}
			
			_selectedIDInt = _index; //_currentSelectedIDInt;
		
			Guid _tmp = Guid.Empty;
			
			if (!string.IsNullOrEmpty(_currentSelected))
			{
				_tmp = Guid.Parse(_currentSelected);
			}
			
			if (_eventBoard != null && _eventBoard.events != null && _eventBoard.events.Keys.Count > 0)
			{
				using ( new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
				{
					GUILayout.Label("Select event:", "boldLabel");
				
					var _ebKeys = _eventBoard.events.Keys.Select(x => x.ToString()).ToArray();
					var _ebNames = _eventBoard.events.Values.Select(x => x.name != null ? x.name.ToString() : "empty").ToArray();
					
					_selectedIDInt = EditorGUILayout.Popup(_selectedIDInt, _ebNames);
					
					if (_selectedIDInt >= 0 && _selectedIDInt < _ebKeys.Length)
					{
						_selectedID = _ebKeys[_selectedIDInt];
					}
				}
			}
		}
		
		void AddVariableCallback(object obj)
		{
			var selected = (GenericMenuData)obj;
			var _events = selected.currentEventBoard.events.Keys.ToList();
			if (selected.currentEventBoard.events[_events[selected.selected]].parameters == null)
			{
				selected.currentEventBoard.events[_events[selected.selected]].parameters = new OrderedDictionary.OrderedDictionary<Guid, FRVariable>();
			}
	
			var _v = (FRVariable)Activator.CreateInstance(Type.GetType(selected.type.ToString()));
			_v.name = "My" + Type.GetType(selected.type.ToString()).Name.ToString();
			_v.typeName = selected.typeName;
			selected.currentEventBoard.events[_events[selected.selected]].parameters.Add(Guid.NewGuid(), _v);
			
		}
		
		public static EventBoard CreateNewEventboard()
		{
			
			var _path = EditorUtility.SaveFilePanel("Create new eventboard", Application.dataPath, "eventboard", "asset");
			
			if (string.IsNullOrEmpty(_path))
				return null;
				
			var _name = System.IO.Path.GetFileName(_path);
			
			if (_path.StartsWith(Application.dataPath)) {
				_path =  "Assets" + _path.Substring(Application.dataPath.Length);
			}
			
			EventBoard ebasset = ScriptableObject.CreateInstance<EventBoard>();

			AssetDatabase.CreateAsset(ebasset, _path);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = ebasset;
			
			return ebasset;
		}
	}
}
#endif