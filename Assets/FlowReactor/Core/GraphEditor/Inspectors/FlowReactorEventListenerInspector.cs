//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

using FlowReactor.EventSystem;

namespace FlowReactor.Editor
{
	[CustomEditor(typeof(FlowReactorEventListener))]
	public class FlowReactorEventListenerInspector : UnityEditor.Editor
	{
		FlowReactorEventListener listener;
		EventBoardEditor editor;
		
		
		
		public void OnEnable()
		{
			listener = (FlowReactorEventListener)target;	
		}
		
		public override void OnInspectorGUI()
		{
			
			GUILayout.Label("Eventboard:", "boldLabel");
			listener.eventBoard = EditorGUILayout.ObjectField(listener.eventBoard, typeof(EventBoard), false) as EventBoard;
			
		
			if (listener.eventBoard == null)
			{
				EditorGUILayout.HelpBox("Create or assign a new eventboard", MessageType.Info);
				return;
			}
				
			
					
			if (editor == null)
			{
				editor = UnityEditor.Editor.CreateEditor(listener.eventBoard) as EventBoardEditor;
			}
			
			if (editor == null)
				return;
					
			
			editor.DrawNodeInspector(listener.eventBoard, listener.selectedEventID, out listener.selectedEventID, out listener.selectedEventIDInt);
	
			SerializedProperty sprop = serializedObject.FindProperty("unityEvent");
			EditorGUILayout.PropertyField(sprop);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif