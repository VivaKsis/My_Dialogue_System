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
using UnityEditorInternal;

using FlowReactor;
using FlowReactor.OdinSerializer;
using FlowReactor.OrderedDictionary;

#if FLOWREACTOR_DATABOX
using Databox;
#endif 

namespace FlowReactor.Editor
{
	[CustomEditor(typeof(FlowReactor.BlackboardSystem.BlackBoard))]
	public class BlackBoardEditor : UnityEditor.Editor
	{
		public FlowReactor.BlackboardSystem.BlackBoard data;

		UnityEditorInternal.ReorderableList reorderableList;
		
		static GUISkin editorSkin;
		
		int lineHeight = 22;
		
		int oldListIndex;
		int newListIndex;
		bool isDragging;
		int dragItemIndex;
		public bool globalDragLocked = false;
		
		Guid blackboardGuid;
		
		//float variableFieldWidth = 120;
		//public bool dragVariableFieldSize = false;
		
		GraphEditor graphEditor;
		Texture2D linkIcon;
		Texture2D dragIcon;
		
		static Dictionary<string, System.Type> variableSceneTypes = new Dictionary<string, System.Type>();
		
		public void OnEnable()
		{
			try
			{
				data = (FlowReactor.BlackboardSystem.BlackBoard)target;
		
				
				if (variableSceneTypes.Keys.Count == 0 || variableSceneTypes == null) 
				{
					GetAvailableVariableTypes.GetFlowReactorVariableTypes(out variableSceneTypes);
				}
			
				SetupList();
			
			}
			catch{}
		}
		
		public void SetupList()
		{
			data.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
			foreach(var key in data.variables.Keys)
			{
				data.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
			}
			
			reorderableList = new ReorderableList(
				serializedObject, serializedObject.FindProperty("tempVariablesList"), true, true, true, true
			);
			
			reorderableList.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
			reorderableList.drawHeaderCallback = DrawHeader;
			reorderableList.onAddDropdownCallback = AddDropdownCallback;
			reorderableList.onReorderCallbackWithDetails = OnReorder;
			reorderableList.onRemoveCallback = OnRemove;
			reorderableList.elementHeightCallback = OnHeightCallback;
			reorderableList.onMouseDragCallback = OnMouseDrag;
			reorderableList.onMouseUpCallback = OnMouseUp;
			reorderableList.onSelectCallback = OnSelect;
			
			//reorderableList.drawElementBackgroundCallback = DrawBackgroundList;
			
			linkIcon = EditorHelpers.LoadIcon("linkIcon.png");
			dragIcon = EditorHelpers.LoadGraphic("dragIcon.png");
		}
		
		// Called after user has dragged a variable from the blackboard to a node variable field.
		// We do this because there's no easy way of deselecting a selected reorderable list item.
		// So instead we rebuild the list.
		public void SetupListWithoutResLoading()
		{
			data.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
			foreach(var key in data.variables.Keys)
			{
				data.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
			}
			
			reorderableList = new ReorderableList(
				serializedObject, serializedObject.FindProperty("tempVariablesList"), true, true, true, true
			);
			
			
			reorderableList.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
			reorderableList.drawHeaderCallback = DrawHeader;
			reorderableList.onAddDropdownCallback = AddDropdownCallback;
			reorderableList.onReorderCallbackWithDetails = OnReorder;
			reorderableList.onRemoveCallback = OnRemove;
			reorderableList.elementHeightCallback = OnHeightCallback;
			reorderableList.onMouseDragCallback = OnMouseDrag;
			reorderableList.onMouseUpCallback = OnMouseUp;
			reorderableList.onSelectCallback = OnSelect;
		}
		
	
	
		void DrawHeader(Rect rect)
		{
			string name = "Variables";
			EditorGUI.LabelField(rect, name);
			
			if (GUI.Button(new Rect(rect.x + rect.width - 14, rect.y, 18, 18), linkIcon, "toolbarButton"))
			{
				// use first bool as option for all
				var _show = data.variables.First().Value.showConnectedNodes;
				
				foreach(var variable in data.variables.Keys)
				{
					data.variables[variable].showConnectedNodes = !_show;
				}
			}
		}
		
		//void DrawBackgroundList(Rect _rect, int _index, bool _isActive, bool _isFocused)
		//{
		//	if (!_isActive || !_isFocused)
		//	{
		//		//GUI.Box(new Rect(_rect.x, _rect.y - 3, _rect.width, _rect.height + 6), "", editorSkin.GetStyle("Box"));
		//	}
		//}
		
		void OnSelect(ReorderableList _list)
		{
			
		}
		
		void OnMouseDrag(ReorderableList _list)
		{
			globalDragLocked = true;
		}

		void OnMouseUp(ReorderableList _list)
		{
			globalDragLocked = false;
		}
		
		float OnHeightCallback(int index)
		{
			if (index >= data.variables.Count)
				return lineHeight;
				
			try
			{
				if (!data.variables[index].showConnectedNodes)
				{
					return data.variables[index].GetGUIHeight();
				}
				else
				{
					if ( data.variables[index].connectedNodes == null ||  data.variables[index].connectedNodes.Count == 0)
					{
						return data.variables[index].GetGUIHeight();
					}
					else
					{
						return  data.variables[index].GetGUIHeight() + ((data.variables[index].connectedNodes.Count) * lineHeight);
					}
				}
			}
			catch
			{

				if (!data.variables[index].showConnectedNodes)
				{	
					return lineHeight;
				}
				else if ( data.variables[index].connectedNodes == null ||  data.variables[index].connectedNodes.Count == 0)
				{
					return lineHeight;
				}
				else
				{
					return lineHeight + ((data.variables[index].connectedNodes.Count) * lineHeight);
				}
			}
		}
		
		void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
				
			if(index >= reorderableList.count)
				return;

			

			SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
		
			
			EditorGUI.BeginChangeCheck();
			
			var _width = rect.width - 68;
			#if FLOWREACTOR_DATABOX
			if (data.useDatabox)
			{
				_width = rect.width - 150;
			}
			#endif
				
			// Drag variable field size
			var splitterRect = new Rect(rect.x + data.variableFieldWidth + 21, rect.y + 1, 10, 20);
			var splitterRectVisual = new Rect(rect.x + data.variableFieldWidth + 21, rect.y + 1, 3, 20);
			EditorGUIUtility.AddCursorRect(splitterRect,MouseCursor.ResizeHorizontal);
			if (GUI.RepeatButton(splitterRectVisual, "", "TextArea"))
			{
				data.dragVariableFieldSize = true;
			}

			if (splitterRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
			{			
				data.dragVariableFieldSize = true;
			}
				
			if ( Event.current.type == EventType.MouseDrag && data.dragVariableFieldSize)
			{
				data.variableFieldWidth += Event.current.delta.x / data.variables.Count;
				
				if (data.variableFieldWidth < 10)
				{
					data.variableFieldWidth = 10;
				}
				if (_width - data.variableFieldWidth <= 20)
				{
					data.variableFieldWidth = _width - 50;
				} 
			}
				
			if (Event.current.type == EventType.MouseUp)
			{
				data.dragVariableFieldSize = false;
			}
			
			
			
			var _id = Guid.Parse(data.tempVariablesList[index].id);
			
			
			if (data.variables.ContainsKey(_id))
			{
				Event _evt = Event.current;
				DragItem(index, rect, data.variables[_id], _id, _evt);
				
				//if (data.variables[_id].sceneReferenceOnly)
				//{
				//	GUI.Label(new Rect(rect.x + 20, rect.y, 20, 20), EditorGUIUtility.IconContent("toggle on act"));
				//}
				
				data.variables[_id].name = GUI.TextField(new Rect(rect.x + 20, rect.y, data.variableFieldWidth, EditorGUIUtility.singleLineHeight), data.variables[_id].name);
			
				if (!data.variables[_id].useDatabox)
				{
				
					GUI.enabled = !FRVariableGUIUtility.sceneRefObjectDragging;
					if (data.variables[_id].sceneReferenceOnly)
					{
						GUI.enabled = false;
						GUI.Label(new Rect(rect.x + data.variableFieldWidth + 25, rect.y, _width - data.variableFieldWidth, EditorGUIUtility.singleLineHeight), new GUIContent("scene only", EditorGUIUtility.IconContent("d_SceneViewOrtho").image), "ObjectField");			
						GUI.enabled = true;
					}
					else
					{
						data.variables[_id].Draw(new Rect(rect.x + data.variableFieldWidth + 25, rect.y, _width - data.variableFieldWidth, EditorGUIUtility.singleLineHeight)); //rect.height - 2)); //EditorGUIUtility.singleLineHeight));
					}
					GUI.enabled = true;
							
					#if FLOWREACTOR_DATABOX
					if (data.useDatabox)
					{
						GUI.enabled = data.databoxObjectManager ? true : false;
					
						var _rect = new Rect(rect.x + rect.width - 122, rect.y, 80, EditorGUIUtility.singleLineHeight);
						if (GUI.Button(_rect, "Databox", "miniButton"))
						{
							PopupWindow.Show(_rect, new PopUps.PopupDataboxVariables (data.databoxObjectManager, data.variables[_id]));
						}
						GUI.enabled = true;
					}
					#else
					if (data.useDatabox)
					{
					GUILayout.Label("Databox not installed");
					}
					#endif
				
				}
				else
				{
					GUI.Label(new Rect(rect.x + 125, rect.y, rect.width - 230, EditorGUIUtility.singleLineHeight), data.variables[_id].databoxID + " | " + data.variables[_id].tableID + " | " + data.variables[_id].entryID + " | " + data.variables[_id].valueID);
					if (GUI.Button(new Rect(rect.x + rect.width - 122, rect.y, 80, EditorGUIUtility.singleLineHeight), "Release", "miniButton"))
					{
						data.variables[_id].useDatabox = false;
						data.variables[_id].databoxID = "";
						data.variables[_id].tableID = "";
						data.variables[_id].entryID = "";
						data.variables[_id].valueID = "";
					}
				}
				
				if (GUI.Button(new Rect(rect.x + rect.width - 41, rect.y, 22, EditorGUIUtility.singleLineHeight), linkIcon))
				{
					data.variables[index].showConnectedNodes = !data.variables[index].showConnectedNodes;
				}
				
				if (GUI.Button(new Rect(rect.x + rect.width - 18, rect.y, 22, EditorGUIUtility.singleLineHeight), "x"))
				{
					if (data.variables[index].connectedNodes != null && data.variables[index].connectedNodes.Count > 0)
					{
						if(EditorUtility.DisplayDialog("Remove blackboard variable?", "Do you really want to remove this variable? Warning: It is still being used by one or more nodes", "yes", "no"))
						{
							DoRemove(index);
						}
					}
					else
					{
						DoRemove(index);
					}
				}
				
				if (index >= data.variables.Count)
					return;
		
				if (data.variables[index].showConnectedNodes)
				{

					if (data.variables[_id].connectedNodes != null && data.variables[_id].connectedNodes.Count > 0)
					{
						GUI.Box(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight + (data.variables[_id].connectedNodes.Count * EditorGUIUtility.singleLineHeight)),"", editorSkin.GetStyle("BoxLine"));
						
						for (int c = 0; c < data.variables[_id].connectedNodes.Count; c ++)
						{
							if (data.variables[_id].connectedNodes[c] == null)
							{
								data.variables[_id].connectedNodes.RemoveAt(c);
							}
							else
							{
								if (data.variables[_id].connectedNodes[c].graphOwner != null)
								{
								
									if (graphEditor != null)
									{
										if (GUI.Button(new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight + (c * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "", FlowReactorEditorStyles.graphExplorerButton))
										{
											graphEditor.GotoNode(data.variables[_id].connectedNodes[c]);
										}
									}
									
									GUI.contentColor = data.variables[_id].connectedNodes[c].color;
									GUI.Label(new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight + (c * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), data.variables[_id].connectedNodes[c].name + " : " + data.variables[_id].connectedNodes[c].rootGraph.name + "/" + data.variables[_id].connectedNodes[c].graphOwner.name);
								
									GUI.contentColor = Color.white;
								}
								else
								{
									data.variables[_id].connectedNodes.RemoveAt(c);
								}
							}
							
						}
					}
					else
					{
						//GUI.Label(new Rect(rect.x, rect.y + 20, rect.width, EditorGUIUtility.singleLineHeight), "not connected to any nodes in any graph");
					}
				}
				
				
				HighlightConnectedVariables(index, rect);
				
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(data);
			}
		}
		
		void DragItem(int _itemIndex, Rect _rect, FRVariable _variable, Guid _bbVariableGuid, Event _event)
		{
			if (globalDragLocked && _event.type != EventType.MouseUp)
				return;
				
			Rect _dragRect = new Rect(_rect.x, _rect.y, 20, 20);
			if (_dragRect.Contains(Event.current.mousePosition))
			{
				if (_event.type == EventType.MouseDrag)
				{
					isDragging = true;
					dragItemIndex = _itemIndex;
					BlackBoardVariableDragProperties.eventType = EventType.MouseDrag;
					BlackBoardVariableDragProperties.variable = _variable;
					BlackBoardVariableDragProperties.isDragging = true;
					BlackBoardVariableDragProperties.blackboardGuid = blackboardGuid;
					BlackBoardVariableDragProperties.startDragPosition = new Vector2(_rect.x, _rect.y);
					BlackBoardVariableDragProperties.blackboardVariableGuid = _bbVariableGuid;
					BlackBoardVariableDragProperties.editor = this;
					reorderableList.draggable = false;
					
					Repaint();
					_event.Use();
					
				}
			}
			
			if (isDragging && dragItemIndex == _itemIndex)
			{
				BlackBoardVariableDragProperties.mousePosition = _event.mousePosition;
			}
			
			if (_event.type == EventType.MouseUp && isDragging)
			{
				dragItemIndex = -1;
				isDragging = false;
				
				BlackBoardVariableDragProperties.isDragging = false;
				BlackBoardVariableDragProperties.eventType = EventType.MouseUp;
				
			}
			
			if (!BlackBoardVariableDragProperties.isDragging)
			{
				reorderableList.draggable = true;
			}
			
			// 
		
			GUI.Box(_dragRect, new GUIContent(dragIcon, "Drag variable to node variable field"));
		}
		
		void AddDropdownCallback(Rect rect, ReorderableList list)
		{
			var menu = new GenericMenu();
					
			foreach(var key in variableSceneTypes.Keys)
			{
				menu.AddItem(new GUIContent(key), false, AddVariableListCallback, variableSceneTypes[key]);
			}
					
			menu.ShowAsContext();
		}
		
		void AddVariableListCallback(object obj)
		{
			var _t = obj as Type;

			var _assembly = _t.Assembly;
			var _selected = obj.ToString().Replace("+", ".");
			
			if (data.variables == null)
			{
				data.variables = new OrderedDictionary<Guid, FRVariable>();	
			}

			//if (Type.GetType(_selected + ", FlowReactor") == null)
			//{
			//	Debug.Log("NULL SELECTED");
			//}
			
			var _type = _assembly.GetType(obj.ToString());
			
			var _v = (FRVariable)Activator.CreateInstance(_type);
			//var _v = (FRVariable)Activator.CreateInstance(Type.GetType(_selected));
			_v.name = "My" + _type.Name.ToString(); // Type.GetType(_selected).Name.ToString();
			data.variables.Add(Guid.NewGuid(), _v);
			
			data.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
			foreach(var key in data.variables.Keys)
			{
				data.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
			}
			
			EditorUtility.SetDirty(data);
		}
		
		void OnReorder(ReorderableList list, int oldIndex, int newIndex)
		{
			
			var _tmpDictionary = new List<FRVariable>();
			var _tmpKeys = new List<Guid>();
			
			foreach (var t in data.variables.Keys)
			{
				_tmpDictionary.Add(data.variables[t]);
				_tmpKeys.Add(t);
				
			}
			
			// modify lists
			var _oldValue = _tmpDictionary[oldIndex];
			var _oldIndex = _tmpKeys[oldIndex];
			
			_tmpDictionary.RemoveAt(oldIndex);
			_tmpKeys.RemoveAt(oldIndex);
			
			_tmpDictionary.Insert(newIndex, _oldValue);
			_tmpKeys.Insert(newIndex, _oldIndex);
			
			
			data.variables = new OrderedDictionary<Guid, FRVariable>();
			
			for(int i = 0; i < _tmpDictionary.Count; i++)
			{
				data.variables.Add(_tmpKeys[i], _tmpDictionary[i]);
			}
			
			globalDragLocked = false;
			
			EditorUtility.SetDirty(data);
		
		}
		
		void OnRemove(ReorderableList _list)
		{
			if (data.variables[_list.index].connectedNodes != null && data.variables[_list.index].connectedNodes.Count > 0)
			{
				if(EditorUtility.DisplayDialog("Remove blackboard variable?", "Do you really want to remove this variable? Warning: It is still being used by one or more nodes", "yes", "no"))
				{
					DoRemove(_list);
				}
			}
			else
			{
				DoRemove(_list);
			}
		}
		
		void DoRemove(ReorderableList _list)
		{
			var element = _list.serializedProperty.GetArrayElementAtIndex(_list.index);
			
			data.variables.RemoveAt(_list.index);
						
			data.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
			foreach(var key in data.variables.Keys)
			{
				data.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
			}
			
			EditorUtility.SetDirty(data);
		}
		
		void DoRemove(int _index)
		{
			SerializedProperty sp = reorderableList.serializedProperty; //.GetArrayElementAtIndex(_index);
		
			if(sp.GetArrayElementAtIndex(_index) != null)
			{
				sp.DeleteArrayElementAtIndex(_index);
			}
		
			data.variables.RemoveAt(_index);
						
			data.tempVariablesList = new List<BlackboardSystem.BlackBoard.VariablesData>();
			
			foreach(var key in data.variables.Keys)
			{
				data.tempVariablesList.Add(new BlackboardSystem.BlackBoard.VariablesData(key.ToString()));
			}
			 
			serializedObject.Update();
			reorderableList.DoLayoutList(); 
 
		
			serializedObject.ApplyModifiedProperties();
			
			EditorUtility.SetDirty(data);
		}

		
		public override void OnInspectorGUI()
		{		
			DrawList(null, Guid.Empty);
		}
		
		
		public void DrawList(GraphEditor _graphEditor, Guid _guid)
		{
			
			Event current = Event.current;
			switch(current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				break;
			case EventType.DragExited:
				FRVariableGUIUtility.sceneRefObjectDragging = false;
				break;
			}
			
			
			if (data == null)
				return;
				
			if (editorSkin == null)
			{
				editorSkin = EditorHelpers.LoadSkin();
			}
			
			//BlackBoardVariableDragProperties.blackboardGuid 
			blackboardGuid	= _guid;
			
			graphEditor = _graphEditor;
			
			#if FLOWREACTOR_DATABOX
			if (editorSkin == null)
			{
				editorSkin = EditorHelpers.LoadSkin();
			}
			
			using ( new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
			{
				data.useDatabox = GUILayout.Toggle(data.useDatabox, "use Databox");	
			
			
				if (data.useDatabox)
				{
					data.databoxObjectManager = (DataboxObjectManager)EditorGUILayout.ObjectField(data.databoxObjectManager, typeof(DataboxObjectManager), false);
				}
				
				if (GUILayout.Button("?", GUILayout.Width(20)))
				{
					data.showDataboxInfo = !data.showDataboxInfo;
				}
			}
			#endif
			
			
			//#if !FLOWREACTOR_DATABOX
			DrawDataboxInfo();
			//#endif
			
			 
				
			serializedObject.Update();
			reorderableList.DoLayoutList();

		
			serializedObject.ApplyModifiedProperties();
		}
		
		public static FlowReactor.BlackboardSystem.BlackBoard CreateNewBlackboard()
		{
			
			var _path = EditorUtility.SaveFilePanel("Create new blackboard", Application.dataPath, "blackboard", "asset");
			
			if (string.IsNullOrEmpty(_path))
				return null;
				
			var _name = System.IO.Path.GetFileName(_path);
			
			if (_path.StartsWith(Application.dataPath)) {
				_path =  "Assets" + _path.Substring(Application.dataPath.Length);
			}
			
			FlowReactor.BlackboardSystem.BlackBoard bbasset = ScriptableObject.CreateInstance<FlowReactor.BlackboardSystem.BlackBoard>();
			
			AssetDatabase.CreateAsset(bbasset, _path);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = bbasset;
			
			return bbasset;
		}
		
		public void HighlightConnectedVariables(int _itemIndex, Rect _rect)
		{
			if (data.variables[_itemIndex].connectedNodes == null)
				return;
			for (int n = 0; n < data.variables[_itemIndex].connectedNodes.Count; n ++)
			{
				if (data.variables[_itemIndex].connectedNodes[n] != null)
				{
					if (data.variables[_itemIndex].connectedNodes[n].rootGraph == null)
						continue;
						
					if (data.variables[_itemIndex].connectedNodes[n] == data.variables[_itemIndex].connectedNodes[n].rootGraph.selectedNode)
					{
						GUI.Label(new Rect(_rect.x - 2, _rect.y - 1, _rect.width + 4, _rect.height + 2), "", editorSkin.GetStyle("FRVariable"));
					}
				}
			}
		}
		
		public void DrawDataboxInfo()
		{
			if (data.showDataboxInfo)
			{
				using ( new GUILayout.VerticalScope("Box"))
				{
					EditorGUILayout.HelpBox("Databox is a data editor which allows you to manage all of your data in tables and asset files. Databox is an additional asset " +
						"which is available at the Asset-Store. FlowReactor has a native support for it. If you already own Databox you can enable it here.", MessageType.Info);
				
					if (GUILayout.Button("Enable Databox for FlowReactor"))
					{
						FlowReactorInstallAddons.InstallDatabox();
					}
					
					if (GUILayout.Button("Databox @ Asset-Store"))
					{
						Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/databox-data-editor-save-solution-155189");
					}
					
				}
			}
		}
	}
}
#endif