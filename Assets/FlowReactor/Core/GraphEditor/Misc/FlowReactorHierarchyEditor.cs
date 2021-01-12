//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	Draws an icon in the Unity hierarchy window
	and lets user put graph object onto the hierarchy.
*/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditorInternal;

using FlowReactor;

namespace FlowReactor.Editor
{
	[InitializeOnLoad]
	public class FlowReactorHierarchyEditor : UnityEditor.Editor
	{
	
		static Texture2D icon;
		static List<int> markedObjects;
		static GameObject targetObject;
		
		static FlowReactorHierarchyEditor ()
		{
			// Init
			icon = EditorHelpers.LoadGraphic("gizmo.png");
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemOnGUI;  
			EditorApplication.projectWindowItemOnGUI += ProjectItemOnGUI;
		
		}

		
		static void ProjectItemOnGUI(string instanceID, Rect selectionRect)
		{
			Event current = Event.current;
			switch(current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
				
					FRVariableGUIUtility.sceneRefObjectDragging = false;
					FRVariableGUIUtility.sceneRefObject = Selection.activeGameObject;
					break;
				case EventType.DragExited:
					
					break;
			}
		}
		
		static void HierarchyItemOnGUI (int instanceID, Rect selectionRect)
		{
		
			// place the icon to the right of the list:
			Rect r = new Rect (selectionRect); 
			r.x = r.width + 50;
			r.width = 30;
			r.height = 20;
			
			
		
			GameObject go = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
			
			if (go != null)
			{
				if (go.GetComponent<FlowReactorComponent>()) 
					GUI.Label (r, icon);
			}
			
			// Drag graph onto gamobject
			targetObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			
			if (selectionRect.Contains(Event.current.mousePosition)) 
			{
				CreateFlowReactorComponent();
			}
			

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
							
							
							FRVariableGUIUtility.sceneRefObject = 	DragAndDrop.objectReferences[0] as GameObject;
						}
					}
					break;
				case EventType.DragExited:
					
					break;
			}
		}

		static void CreateFlowReactorComponent()
		{
				
			Event current = Event.current;
			switch(current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (current.button == 0)
					{
						if (DragAndDrop.objectReferences.Length > 0)
						{
							if (DragAndDrop.objectReferences[0] is Graph)
							{
								if (current.type == EventType.DragPerform)
								{
									DragAndDrop.AcceptDrag();
									FlowReactorComponent _flowReactor = targetObject.AddComponent<FlowReactorComponent>();
									_flowReactor.graph = (Graph)DragAndDrop.objectReferences[0];
									EditorGUIUtility.PingObject(_flowReactor);
									Selection.activeGameObject = targetObject;
									current.Use();
								}
								DragAndDrop.PrepareStartDrag();
							}
						}
					}
					break;
			}
		}
	}
}
#endif
