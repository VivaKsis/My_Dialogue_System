//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	FlowReactor Settings Object.
	Stores colors, custom  colors, shortcuts and is being used to temporarily store copied nodes
*/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.OdinSerializer;
using FlowReactor.Editor;

namespace FlowReactor.Editor
{
	// Create a new type of Settings Asset.
	public class FREditorSettings : ScriptableObject
	{
		public string version = "";
		
		// Cache copied nodes
		public List<UnityEngine.Object> objectReferences;
		// Cache node selection
		public Dictionary<int, Node> tmpSelectedNodes;
		
		public string errorReport;
		
		// store all open graph windows
		[System.Serializable]
		public class GraphWindows
		{
			public int id;
			public GraphEditor graphEditor;
			
			public GraphWindows (int _id, GraphEditor _graphEditor)
			{
				id = _id;
				graphEditor = _graphEditor;
			}
		}
		
		[SerializeField]
		public List<GraphWindows> graphWindows;
		
		[SerializeField]
		public List<NodeCategoryTree.NodeData> favoredNodes;
		
		
		public Graph currentlySelectedGraph;
		
		
		[System.Serializable]
		public class EditorColors
		{
			public string id;
			public Color color;
			
			public EditorColors ( string _id, Color _color)
			{
				id = _id;
				color = _color;
			}
		}
		
		// Store editor colors
		//default colors
		public List<EditorColors> defaultColorsDark;
		public List<EditorColors> defaultColorsLight;
		
		//inspector colors
		public List<EditorColors> inspectorColors;
		
		//custom category colors
		public List<EditorColors> categoryColors;
		
		[SerializeField]
		public bool createExpandedNodes = false;
		
		// Shortcuts
		public KeyCode keyFocus;
		public KeyCode keyCreateComment;
		public KeyCode keyCreateGroup;
		public KeyCode keyCreateSubGraph;
		public KeyCode keyAlignNodesLeft;
		public KeyCode keyAlignNodesRight;
		public KeyCode keyAlignNodesBottom;
		public KeyCode keyAlignNodesTop;
		//public KeyCode keyAlignNodesHorizontally;
		//public KeyCode keyAlignNodesVertically;
		public KeyCode keyAlignNodesAutomatically;
		public KeyCode keyGotoParentGraph;
		public KeyCode keyExpandNodes;
		public KeyCode keyCollapseNodes;
		
		//[SerializeField]
		//public int verticalAlignmentSpace;
		//[SerializeField]
		//public int horizontalAlignmentSpace;
		
		public FREditorSettings(){}
		
		
		public Color GetColor(string _colorID)
		{
			Color _color = Color.black;
			
			if (EditorGUIUtility.isProSkin)
			{
				for (int d = 0; d < defaultColorsDark.Count; d ++)
				{
					if (defaultColorsDark[d].id == _colorID)
					{
						return defaultColorsDark[d].color;
					}
				}
			}
			else
			{
				for (int d = 0; d < defaultColorsLight.Count; d ++)
				{
					if (defaultColorsLight[d].id == _colorID)
					{
						return defaultColorsLight[d].color;
					}
				}
			}
			
			for (int c = 0; c < categoryColors.Count; c ++)
			{
				if (categoryColors[c].id == _colorID)
				{
					return categoryColors[c].color;
				}
			}
			
			
			// Try to parse color id to color
			if (ColorUtility.TryParseHtmlString(_colorID, out _color))
			{
				return _color;
			}
			else
			{	
				if (EditorGUIUtility.isProSkin)
				{
					_color = defaultColorsDark[0].color;
				}
				else
				{
					_color = defaultColorsLight[0].color;
				}
			}
			
			return _color;
		}
		
		public Color GetInspectorColor(string _colorID)
		{
			Color _color = Color.white;
			
			//if (EditorGUIUtility.isProSkin)
			//{
			for (int d = 0; d < inspectorColors.Count; d ++)
			{
				if (inspectorColors[d].id == _colorID)
				{
					return inspectorColors[d].color;
				}
			}
			//}
			
			return _color;
		}
		
		public void ResetColors()
		{
			
			defaultColorsDark = new List<EditorColors>()
			{
		
				new EditorColors("actionNodeColor", new Color(125f/255f, 190f/255f, 160f/255, 255f/255f)),
				new EditorColors("coroutineNodeColor", new Color(200f/255f, 255f/255f, 160f/255, 255f/255f)),
				new EditorColors("eventNodeColor", new Color(120f/255f, 195f/255f, 255f/255, 255f/255f)),
				new EditorColors("flowNodeColor",  Color.white),
				new EditorColors("subGraphNodeColor", new Color(165f/255f, 140f/255f, 205f/255, 255f/255f))
			};
			
			defaultColorsLight = new List<EditorColors>()
			{
		
				new EditorColors("actionNodeColor", new Color(0f/255f, 170f/255f, 90f/255, 255f/255f)),
				new EditorColors("coroutineNodeColor", new Color(100/255f, 255f/255f, 0f/255, 255f/255f)),
				new EditorColors("eventNodeColor", new Color(20f/255f, 120f/255f, 200f/255, 255f/255f)),
				new EditorColors("flowNodeColor",  Color.white),
				new EditorColors("subGraphNodeColor", new Color(110f/255f, 70f/255f, 175f/255, 255f/255f))
			};
			
			inspectorColors = new List<EditorColors>()
			{
				new EditorColors("inspectorColor", new Color(165f/255f, 140f/255f, 205f/255, 255f/255f)),
				new EditorColors("blackboardColor", Color.black),
				new EditorColors("eventboardColor", new Color(120f/255f, 195f/255f, 255f/255, 255f/255f)),
			};
			
				
			categoryColors = new List<EditorColors>();
		}
		
		
		public void ResetShortcuts()
		{
			keyFocus = KeyCode.F;
			keyCreateComment = KeyCode.Q;
			keyCreateGroup = KeyCode.G;
			keyCreateSubGraph = KeyCode.S;
			keyAlignNodesLeft = KeyCode.LeftArrow;
			keyAlignNodesRight = KeyCode.RightArrow;
			keyAlignNodesTop = KeyCode.UpArrow;
			keyAlignNodesBottom = KeyCode.DownArrow;
			//keyAlignNodesHorizontally = KeyCode.H;
			//keyAlignNodesVertically = KeyCode.V;
			keyAlignNodesAutomatically = KeyCode.A;
			keyGotoParentGraph = KeyCode.Backspace;
			keyExpandNodes = KeyCode.KeypadPlus;
			keyCollapseNodes = KeyCode.KeypadMinus;
		}
		
		public void OpenGraphWindow(Graph graph)
		{
			GraphEditor window = null;
			
			int _id = graph.GetInstanceID();
			
			// check if other graph windows are already open
			// happens if user has closed unity and left some graph windows open
			// then we have to add them to the graphwindows list again.
			if (graphWindows.Count == 0 || graphWindows == null)
			{
				GraphEditor[] _w = Resources.FindObjectsOfTypeAll<GraphEditor>();
				
				if (_w.Length > 0)
				{
					//Debug.Log(_w.Length);
					
					for (int w = 0; w < _w.Length; w ++)
					{
						graphWindows.Add(new GraphWindows(_w[w].rootGraph.GetInstanceID(), _w[w]));
					}
				}
			}
			
			for (int i = 0; i < graphWindows.Count; i ++)
			{
				if (graphWindows[i].id == _id)
				{
					window = graphWindows[i].graphEditor;
				}
			}
			
			if (window != null)
			{
				window.Init(window, graph);
				window.Focus();
			}
			else
			{
				//window = CreateInstance<GraphEditor>();
				window = (GraphEditor)EditorWindow.CreateWindow<GraphEditor>(typeof(GraphEditor));
				graphWindows.Add(new GraphWindows(graph.GetInstanceID(), window));
				window.Init(window, graph);
			}
			
			// Check if graph needs to be updated
			GraphUpdater.UpdateGraph(graph);
			
			EditorUtility.SetDirty(this);
		}
		
		public void CleanupGraphWindows(Graph _graph)
		{
			
			int _id = _graph.GetInstanceID();
			
			for (int i = 0; i < graphWindows.Count; i ++)
			{
				if (graphWindows[i].id == _id)
				{
					graphWindows.RemoveAt(i);
				}
			}
		}
	
	
	    internal static FREditorSettings GetOrCreateSettings()
		{
			var _found = AssetDatabase.FindAssets( string.Format( "t:" + typeof(FREditorSettings).Name ));
			FREditorSettings settings = null;
			
			if (_found.Length > 0)
			{
	
				var assetPath = AssetDatabase.GUIDToAssetPath( _found[0] );
				settings = AssetDatabase.LoadAssetAtPath<FREditorSettings>(assetPath);
				
				if (settings.graphWindows == null)
				{
					settings.graphWindows = new List<GraphWindows>();
				}
			}
			else
			{
				var _path = "Assets/FlowReactorSettings.asset"; //Path.Combine(EditorHelpers.GetRelativeSettingsPath(), "FlowReactorSettings.asset");
				settings = ScriptableObject.CreateInstance<FREditorSettings>();
				
				settings.ResetColors();
				settings.ResetShortcuts();
				
				settings.graphWindows = new List<GraphWindows>();
				
				AssetDatabase.CreateAsset(settings, _path);
				AssetDatabase.SaveAssets();
				
				
				//Debug.Log("reset settings");
			}
			
	        return settings;
		}
	
	    
		internal static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		} 
	    
		[UnityEditor.Callbacks.DidReloadScripts]
		static void UpdateSettings()
		{
			// Get current settings
			var _tmpSettings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			
			if (_tmpSettings.version != EditorHelpers.GetEditorVersion())
			{
				// get new settings
				var _newSettings = ScriptableObject.CreateInstance<FREditorSettings>();
			
				_newSettings.ResetColors();
				_newSettings.ResetShortcuts();
				
				// Compare default colors
				if (_tmpSettings.defaultColorsDark.Count != _newSettings.defaultColorsDark.Count)
				{
					for (int d = 0; d < _newSettings.defaultColorsDark.Count; d ++)
					{
						var _exists = false;
						for (int e = 0; e < _tmpSettings.defaultColorsDark.Count; e ++)
						{
							if (_tmpSettings.defaultColorsDark[e].id == _newSettings.defaultColorsDark[d].id)
							{
								_exists = true;
							}
						}
						
						if (!_exists)
						{
							_tmpSettings.defaultColorsDark.Add(_newSettings.defaultColorsDark[d]);
							_tmpSettings.defaultColorsLight.Add(_newSettings.defaultColorsLight[d]);
						}
					}
				}
				
				
				if (_tmpSettings.inspectorColors == null || _tmpSettings.inspectorColors.Count == 0)
				{
					_tmpSettings.inspectorColors = new List<EditorColors>()
					{
						new EditorColors("inspectorColor", new Color(165f/255f, 140f/255f, 205f/255, 255f/255f)),
						new EditorColors("blackboardColor", Color.black),
						new EditorColors("eventboardColor", new Color(120f/255f, 195f/255f, 255f/255, 255f/255f)),
					};
				}				
				
				_tmpSettings.version = EditorHelpers.GetEditorVersion();
			}
			
		}
	    
	}
	
	// Register a SettingsProvider using IMGUI for the drawing framework:
	static class FlowReactorEditorSettingsIMGUIRegister
	{
	    [SettingsProvider]
	    public static SettingsProvider CreateMyCustomSettingsProvider()
		{
	    	
	        // First parameter is the path in the Settings window.
	        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
		    var provider = new SettingsProvider("FREditorSettings", SettingsScope.User)
	        {
	            // By default the last token of the path is used as display name if no label is provided.
		        label = "FlowReactor",
	            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
	            guiHandler = (searchContext) =>
	            {
		            var settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
		           
		          
		            
		            FREditorSettingsGUI.Draw(settings);
	            },
	
	            // Populate the search keywords to enable smart search filtering and label highlighting:
		        keywords = new HashSet<string>(new[] { "FlowReactor" })
	        };
	
	        return provider;
	    }
	}
	
	
}
#endif