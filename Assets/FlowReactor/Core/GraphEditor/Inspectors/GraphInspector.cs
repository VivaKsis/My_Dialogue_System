//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using FlowReactor;

namespace FlowReactor.Editor
{
	[CustomEditor(typeof(Graph))]
	public class GraphInspector : UnityEditor.Editor
	{
		Graph graph;
		Texture2D logo;
		
		static FlowReactorComponent fr;
		
		
		public void OnEnable()
		{
			graph = (Graph)target;		
			logo = EditorHelpers.LoadGraphic("logo_wide.png");
		}
		
		
		public override void OnInspectorGUI()
		{
			using (new GUILayout.HorizontalScope("Box"))
			{
				GUILayout.Label(logo);
			}
			
			if (GUILayout.Button("Edit", GUILayout.Height(40)))
			{

				//GraphEditor window = null;
				
				//var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
				//if (_settings.graphWindows.TryGetValue(graph.GetInstanceID(), out window))
				//{
				//	window.Init(window, graph);
				//	window.Focus();
				//}
				//else
				//{
				//	window = CreateInstance<GraphEditor>();
				//	_settings.graphWindows.Add(graph.GetInstanceID(), window);
				//	window.Init(window, graph);
				//}
				var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
				_settings.OpenGraphWindow(graph);
			}
			
			// DEBUG
			#if FLOWREACTOR_DEBUG
			DrawDefaultInspector();
			#endif
		}
		
		
		[OnOpenAssetAttribute(1)]
		public static bool OpenGraph(int instanceID, int line)
		{
			object _obj = EditorUtility.InstanceIDToObject(instanceID);
			var _graph = _obj as Graph;
			if (_graph != null)
			{
				//var window = CreateInstance<GraphEditor>();
				//window.Init(window, _graph);
				var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
				_settings.OpenGraphWindow(_graph);
	
				
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
#endif