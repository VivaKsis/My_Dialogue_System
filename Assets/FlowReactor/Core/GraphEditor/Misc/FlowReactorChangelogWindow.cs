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

namespace FlowReactor.Editor
{
	public class FlowReactorChangelogWindow : EditorWindow
	{
		static string changelog;
		Vector2 scrollPos;
		
		static FREditorSettings settings;
		
		
		//[UnityEditor.Callbacks.DidReloadScripts]
		static void ShowChangelogAfterCompilation()
		{
			if (settings == null)
			{
				settings = FREditorSettings.GetOrCreateSettings();
			}
			
			var _newVersion = EditorHelpers.GetEditorVersion();
			if (settings.version != _newVersion)
			{
				settings.version = _newVersion;
				FlowReactorChangelogWindow.Init();
			}
		}
	  
		 
		public static void Init()
		{
			changelog = EditorHelpers.LoadChangelog();
			
			FlowReactorChangelogWindow window = (FlowReactorChangelogWindow)EditorWindow.GetWindow(typeof(FlowReactorChangelogWindow));
			window.maxSize = new Vector2(500f, 600f);
			window.minSize = new Vector2(500f, 600f);
			window.Show();
		}
		
		void OnGUI()
		{
			EditorGUILayout.HelpBox("FlowReactor - Changelog", MessageType.Info);

			using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
			{
				scrollPos = scrollView.scrollPosition;
				GUILayout.TextArea(changelog);
			}

			if (GUILayout.Button("Close", GUILayout.Height(50)))
			{
				Close();
			}
		}
	}
}
#endif