//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	FlowReactor editor helper class.
	Mainly used to return editor paths.
*/

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace FlowReactor
{
	public static class EditorHelpers
	{
		static string assetBasePath = "Assets";
		private static Dictionary<string, string> pathCache = new Dictionary<string, string>(10);

		private static string GetFilePath(string file)
		{
			if (pathCache.TryGetValue(file, out string path))
			{
				return path;
			}

			string filename = Directory.EnumerateFiles(assetBasePath, file, SearchOption.AllDirectories).FirstOrDefault();

			if (string.IsNullOrEmpty(filename))
			{
				return string.Empty;
			}

			path = $"{Path.GetDirectoryName(filename).Replace("\\", "/")}/";

			pathCache.Add(file, path);

			return path;
		}
		
		public static string GetRelativeEditorPath()
		{
			return GetFilePath("GraphEditor.cs");
		}
		
		public static string GetRelativeFlowReactorPath2()
		{
			return GetFilePath("FlowReactorComponent.cs");
		}
		
		public static string GetRelativeResPath()
		{
			var _theme = "Light";

			#if UNITY_EDITOR
			if (EditorGUIUtility.isProSkin)
			{
				_theme = "Dark";
			}
			#endif
			
	
			var _res = System.IO.Directory.EnumerateFiles("Assets/", "ResPath.cs", System.IO.SearchOption.AllDirectories);
			
			var _path = "";
			
			var _found = _res.FirstOrDefault();
			if (!string.IsNullOrEmpty(_found))
			{
				_path = _found.Replace("ResPath.cs", "").Replace("\\", "/");
				_path = Path.Combine(_path, _theme);
			}
			
			return _path;
			
			
		}
		
		public static string GetRelativeSettingsPath()
		{	
			return GetFilePath("FREditorSettings.cs");
		}
		
		public static string GetRelativeWizardPath()
		{
			return GetFilePath("NodeWizard.cs");
		}
		
		public static string GetRelativeRuntimeDataPath()
		{
			return GetFilePath("OdinBuildAutomation.cs");
		}
		
		public static string GetRelativeGraphicPath()
		{
			return System.IO.Path.Combine(EditorHelpers.GetRelativeResPath(), "Graphics");
		}
		
		public static string GetRelativeIconPath()
		{
			return System.IO.Path.Combine(EditorHelpers.GetRelativeResPath(), "Icons");
		}
		
		// Load general editor graphics
		public static Texture2D LoadGraphic(string _name)
		{
			#if UNITY_EDITOR
			return (Texture2D)(AssetDatabase.LoadAssetAtPath(GetRelativeGraphicPath() + "/" + _name, typeof(Texture2D)));
			#else
			return null;
			#endif
		}
		
		// Load node icon
		public static Texture2D LoadIcon(string _name)
		{
			#if UNITY_EDITOR
			return (Texture2D)(AssetDatabase.LoadAssetAtPath(GetRelativeIconPath() + "/" + _name, typeof(Texture2D)));
			#else
			return null;
			#endif
		}

		
		// EDITOR
		//////////////////////
	
		#if UNITY_EDITOR
		public static GUISkin LoadSkin()
		{
			var _skinName = "frskin_light.guiskin";
			if (EditorGUIUtility.isProSkin)
			{
				_skinName = "frskin_dark.guiskin";
			}
			
			var _editorSkinPath = System.IO.Path.Combine(EditorHelpers.GetRelativeResPath(), "Skin");
			return (GUISkin)(AssetDatabase.LoadAssetAtPath(_editorSkinPath + "/" + _skinName, typeof(GUISkin)));
		}
		
		public static string GetEditorVersion()
		{
			var _changelog = LoadChangelog();
			var _lines = _changelog.Split(new string[3] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
			
			return _lines[0];
		}
		
		public static string LoadChangelog()
		{
			var _log = (TextAsset)AssetDatabase.LoadAssetAtPath(Path.Combine(GetRelativeSettingsPath(), "Changelog.txt"), typeof(TextAsset));
			if (_log != null)
			{
				return _log.text;
			}
			else
			{
				return "";
			}
		}
		
		public static void DrawUILine(int thickness = 2, int padding = 0)
		{
			try{
				Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
				r.height = thickness;
				r.y+=padding/2;
				r.x-=12;
				r.width +=20;
				EditorGUI.DrawRect(r, lineColor);
			}
			catch{}
		}
		
	
		public static Color lineColor
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					return new Color(40f/255f, 40f/255f, 40f/255f);
				}
				else
				{
					return new Color(165f/255f, 165f/255f, 165f/255f);
				}
			}
		}
		#endif
	
	}
}
#endif
