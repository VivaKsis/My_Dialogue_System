//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Editor
{
	public class NodeWizard : EditorWindow 
	{
		[SerializeField]
		string templatePath;
		static string[] options = new string[]{"1. Basic", "2. Color", "3. Advanced"};
		static int selectedOption = 0;
	
		[SerializeField]
		string nodePath = "";
		static string assetPath = "";
		static string nodeNamespace = "FlowReactor.Nodes";
		static string nodeName = "Name";
		static string nodeCategory = "Category";
		static string nodeDescription = "Description";
		//static int nodeOutputs = 1;
		static string nodeColor = "actionNodeColor";
		
		// Advanced settings
		static bool disableDefaultInspector = true;
		static bool disableVariableInspector = false;
		static bool drawCustomInspector = false;
		
		bool errorName = false;
		bool errorPath = false;
		
		Vector2 scrollView = Vector2.zero;
		
		static FREditorSettings settings;
		static Dictionary<string, Color> availableColors;
		static Texture2D whiteBox;
		static Vector2 colorScrollPos = Vector2.zero;
		static Vector2 scrollPos = Vector2.zero;
		
		static Texture2D[] nodeTypesIcons = new Texture2D[3];
		static int selectedNodeType = 0;
		static int lastSelectedNodeType = -1;
		static Texture2D actionNodeIcon;
		static Texture2D coroutineNodeIcon;
		static Texture2D eventNodeIcon;
		static Texture2D wizardIcon;
		
		
		static List<string> nodeOutputs = new List<string>();
		
		public class AvailableNodeModules
		{
			public string module;
			public string description;
			public string customDrawInspectorCode;
			public string nodeVariablesCode;
			
			public AvailableNodeModules (string _module, string _description, string _nodeVariablesCode, string _customDrawInspectorCode)
			{
				module = _module;
				description = _description;
				nodeVariablesCode = _nodeVariablesCode;
				customDrawInspectorCode = _customDrawInspectorCode;
			}
		}
		
		static List<AvailableNodeModules> nodeModules = new List<AvailableNodeModules>();
		static Dictionary<string, bool> selectedNodeModules = new Dictionary<string, bool>();

		[MenuItem("Assets/Create/FlowReactor/New Node")]
		public static void Init()
		{
			
		
			assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

			if (File.Exists(assetPath))
				assetPath = Path.GetDirectoryName(assetPath);
			if (string.IsNullOrEmpty(assetPath)) assetPath = "Assets/";
				
			GetAvailableColors();
			LoadIcons();
			
			nodeOutputs = new List<string>();
			nodeOutputs.Add("");
			
			FindNodeModules();
			
			NodeWizard window = (NodeWizard)EditorWindow.GetWindow(typeof (NodeWizard));

			window.Show();
		}
		
		[UnityEditor.Callbacks.DidReloadScripts]
		static void LoadIcons()
		{
			actionNodeIcon = EditorHelpers.LoadGraphic("createActionNodeIcon.png");
			coroutineNodeIcon = EditorHelpers.LoadGraphic("createCoroutineNodeIcon.png");
			eventNodeIcon = EditorHelpers.LoadGraphic("createEventNodeIcon.png");
			wizardIcon = EditorHelpers.LoadGraphic("nodeWizardIcon.png");
			
			nodeTypesIcons = new Texture2D[3];
			nodeTypesIcons[0] = actionNodeIcon;
			nodeTypesIcons[1] = coroutineNodeIcon;
			nodeTypesIcons[2] = eventNodeIcon;
		
			nodeOutputs = new List<string>();
			nodeOutputs.Add("");
			
			FindNodeModules();
			
		}
		
		static void GetAvailableColors()
		{ 
			settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			availableColors = new Dictionary<string, Color>();
			
			if (EditorGUIUtility.isProSkin)
			{
				for(int d = 0; d < settings.defaultColorsDark.Count; d ++)
				{
					availableColors.Add(settings.defaultColorsDark[d].id, settings.defaultColorsDark[d].color);
				}
			}
			else
			{
				for(int d = 0; d < settings.defaultColorsLight.Count; d ++)
				{
					availableColors.Add(settings.defaultColorsLight[d].id, settings.defaultColorsLight[d].color);
				}
			}
			
			for(int c = 0; c < settings.categoryColors.Count; c ++)
			{
				availableColors.Add(settings.categoryColors[c].id, settings.categoryColors[c].color);
			}
			
			whiteBox = EditorHelpers.LoadGraphic("whiteBox.png");
		}
		
		void OnGUI()
		{
			if (string.IsNullOrEmpty(nodePath))
			{
				nodePath = assetPath;
			}
			
			using (new GUILayout.HorizontalScope())
			{
				//GUILayout.Label(wizardIcon, GUILayout.Width(40));
				//EditorGUILayout.HelpBox("This wizard creates a new node class based on this form. Simply fill it out and click on create node.", MessageType.None);
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.richText = true;
				GUILayout.Label(new GUIContent("<size=20>Node Wizard</size>", wizardIcon), style);
				
			}
			
			EditorHelpers.DrawUILine();
			
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope("TextArea", GUILayout.Width(100)))
				{
				
					
					//selectedOption = GUILayout.Toolbar(selectedOption, options);
					selectedOption = GUILayout.SelectionGrid(selectedOption, options, 1);
					
					GUILayout.FlexibleSpace();
				}
				
				using (new GUILayout.VerticalScope())
				{
					switch(selectedOption)
					{
					case 0:
						BasicSettings();
						break;
					case 1:
						ColorSettings();
						break;
					case 2:
						AdvancedSettings();
						break;
					}
				}
			}
			
			EditorHelpers.DrawUILine();
			
			SaveSettings();
			
		}
		
		void BasicSettings()
		{
			scrollView = EditorGUILayout.BeginScrollView(scrollView);
			
		
			using (new GUILayout.VerticalScope("Box"))
			{
				
				GUILayout.Label("Node type:", "BoldLabel");
				
				selectedNodeType = GUILayout.Toolbar(selectedNodeType, nodeTypesIcons, GUILayout.Height(100));
	
				switch (selectedNodeType)
				{
					case 0:
						EditorGUILayout.HelpBox("Use action nodes for running custom code", MessageType.Info);
						break;
					case 1:
						EditorGUILayout.HelpBox("Use coroutine nodes to create more complex behaviours with coroutines", MessageType.Info);
						break;
					case 2:
						EditorGUILayout.HelpBox("Use event nodes to create special event nodes like the OnStart node", MessageType.Info);
						break;
				}
				
				
				if (lastSelectedNodeType != selectedNodeType)
				{
					if (availableColors == null || !availableColors.ContainsKey(nodeColor)) 
					{
						GetAvailableColors();
					}
					
					// change color
					switch (selectedNodeType)
					{
						case 0:
							nodeColor = "actionNodeColor";
							break;
						case 1:
							nodeColor = "coroutineNodeColor";
							break;
						case 2:
							nodeColor = "eventNodeColor";
							break;
					}
					
					lastSelectedNodeType = selectedNodeType;
				
				}
			}		
	
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Global properties", "BoldLabel");
			
				
				EditorGUILayout.LabelField("Node name and description");
				if (errorName)
				{
					EditorGUILayout.HelpBox("Please set a name for your node", MessageType.Error);
				}
				
				nodeName = EditorGUILayout.TextField(nodeName);
				nodeDescription = EditorGUILayout.TextArea(nodeDescription, GUILayout.Height(50));
				
				
				EditorGUILayout.LabelField("Namespace:");
				nodeNamespace = EditorGUILayout.TextField(nodeNamespace);
				
				EditorGUILayout.LabelField("Node category:");
				nodeCategory = EditorGUILayout.TextField (nodeCategory);
				
				
				//EditorGUILayout.LabelField("Number of outputs:");
				//nodeOutputs = EditorGUILayout.IntField(new GUIContent("", "set the number of outputs for this node"), nodeOutputs);
				
				EditorGUILayout.LabelField("Node outputs:");
				if (GUILayout.Button("Add Output"))
				{
					nodeOutputs.Add("");
				}
				using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
				{
					scrollPos = scrollView.scrollPosition;
					
					for (int i = 0; i < nodeOutputs.Count; i ++)
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label((i+1).ToString() + ":", GUILayout.Width(20));
							nodeOutputs[i] = EditorGUILayout.TextField(nodeOutputs[i]);
							if (GUILayout.Button("x", GUILayout.Width(20)))
							{
								nodeOutputs.RemoveAt(i);
							}
						}
					}
				}
				
				GUILayout.FlexibleSpace();
			}
			
			
			//GUILayout.FlexibleSpace();
			
	
			EditorGUILayout.EndScrollView();
		}
		
		void ColorSettings()
		{
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Node color:", "BoldLabel");
				EditorGUILayout.HelpBox("custom colors can be defined in the settings window of FlowReactor", MessageType.Info);
				
				if (GUILayout.Button("Settings"))
				{
					EditorApplication.ExecuteMenuItem("Edit/Preferences...");
				}
				
				if (availableColors == null || !availableColors.ContainsKey(nodeColor)) 
				{
					GetAvailableColors();
				}
				
				var _lastRect = GUILayoutUtility.GetLastRect();
				_lastRect = new Rect(_lastRect.x, _lastRect.y + 25, _lastRect.width, _lastRect.height);
				EditorGUIUtility.DrawColorSwatch(_lastRect, availableColors[nodeColor]);
				
				GUILayout.Space(25);
				
				using (var scrollView = new GUILayout.ScrollViewScope(colorScrollPos, GUILayout.Height(60)))
				{
					colorScrollPos = scrollView.scrollPosition;
					
					
					using (new GUILayout.HorizontalScope("Box"))
					{
						foreach(var c in availableColors.Keys)
						{

							GUI.contentColor = availableColors[c];
							if (GUILayout.Button(new GUIContent("", whiteBox), GUILayout.Width(30), GUILayout.Height(30)))
							{
								nodeColor = c;
							}
							GUI.contentColor = Color.white;			
							
						}
						
						GUILayout.FlexibleSpace();
					
					}
				}
				
				
				GUILayout.FlexibleSpace();
			}
		}
		
		void AdvancedSettings()
		{
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Advanced settings:", "boldLabel");
				
				disableDefaultInspector = GUILayout.Toggle(disableDefaultInspector, "disable default inspector");
				disableVariableInspector = GUILayout.Toggle(disableVariableInspector, "disable variable inspector");
				drawCustomInspector = GUILayout.Toggle(drawCustomInspector, "draw custom inspector");
		
				
		
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				using (new GUILayout.HorizontalScope("Toolbar"))
				{
					GUILayout.Label("Node Modules:", "boldLabel");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("?", "ToolbarButton"))
					{
						Application.OpenURL("https://flowreactor.io/documentation/frnodemodules/");
					}
				}
				for (int i = 0; i < nodeModules.Count; i ++)
				{
					EditorHelpers.DrawUILine();
					
					using (new GUILayout.VerticalScope())
					{
						
						selectedNodeModules[nodeModules[i].module] = EditorGUILayout.Toggle(nodeModules[i].module, selectedNodeModules[nodeModules[i].module]);
						EditorGUILayout.HelpBox(nodeModules[i].description, MessageType.Info);
					}
				}
				
				GUILayout.FlexibleSpace();
			}
			
		}
		
		static void FindNodeModules()
		{
			System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			//System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(T)) select type).ToArray();
			System.Type[] _found = System.Reflection.Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.IsSubclassOf (typeof(FRNodeUtilityModule))).ToArray();
				
			nodeModules = new List<AvailableNodeModules>();
			selectedNodeModules = new Dictionary<string, bool>();
			
			foreach(var currentType in _found)
			{
				var attributes = currentType.GetCustomAttributes(typeof(NodeModule), false);
				if (attributes.Length > 0)
				{
					var _attr = attributes[0] as NodeModule;
					nodeModules.Add(new AvailableNodeModules(currentType.Name, _attr.description, _attr.moduleVariablesCode, _attr.customDrawInspectorCode));
					selectedNodeModules.Add(currentType.Name, false);
				}
				else
				{
					nodeModules.Add(new AvailableNodeModules(currentType.Name, "", "", ""));
					selectedNodeModules.Add(currentType.Name, false);
				}
				
			
			}
		}
		
		void SaveSettings()
		{
			EditorGUILayout.LabelField("Save path:");
			if (errorPath)
			{
				EditorGUILayout.HelpBox("Please select a path", MessageType.Error);
			}
			
			using (new EditorGUILayout.HorizontalScope("Box"))
			{
		
				nodePath = EditorGUILayout.TextField (nodePath);
				
				if (GUILayout.Button("...", GUILayout.Width(30)))
				{
					nodePath = EditorUtility.SaveFolderPanel("select path", "Assets/", nodeName);
				}
			
			}
			
			
			if (GUILayout.Button("Create Node", GUILayout.Height(50)))
			{
				if (nodeName == "")
				{
					errorName = true;
					return;	
				}
				
				if (nodePath == "")
				{
					errorPath = true;
					return;
				}
				
				
				errorName = false;
				errorPath = false;
				
				Create();
				
			}
		}
		
		void Create ()
		{		
	
			if (selectedNodeType == 0 || selectedNodeType == 2)
			{
				templatePath = System.IO.Path.Combine(EditorHelpers.GetRelativeWizardPath(), "templateDefaultNode.txt");		
			}
			else if (selectedNodeType == 1)
			{
				templatePath = System.IO.Path.Combine(EditorHelpers.GetRelativeWizardPath(), "templateCoroutineNode.txt");
			}
			
			
			StreamReader sr = new StreamReader(templatePath);
			var _content = sr.ReadToEnd();
			
			sr.Close();
			
			nodeName = nodeName.Trim();
			
			// Replace all node properties
			if (_content.Contains("NODE_SPACE"))
			{
				_content = _content.Replace("NODE_SPACE", nodeNamespace);
			}
			
			if (_content.Contains("NODE_NAME"))
			{
				_content = _content.Replace("NODE_NAME", nodeName);
			}
			
			if (_content.Contains("NODE_TITLE"))
			{
				_content = _content.Replace("NODE_TITLE", '"' + nodeName + '"');
			}
			
			if (_content.Contains("NODE_CATEGORY"))
			{
				_content = _content.Replace("NODE_CATEGORY", '"' + nodeCategory + '"');
			}
			
			if (_content.Contains("NODE_DESCRIPTION"))
			{
				string _desc = nodeDescription.Replace("\r", "").Replace("\n", "");
				_content = _content.Replace("NODE_DESCRIPTION", '"' + _desc + '"');
			}
			
			if (_content.Contains("NODE_COLOR"))
			{
				_content = _content.Replace("NODE_COLOR", '"' + nodeColor + '"');
			}
			
			if (_content.Contains("NODE_OUTPUTS"))
			{
				var _addOutputs = "new string[]{";
				for (int j = 0; j < nodeOutputs.Count; j ++)
				{
					if (j == 0)
					{
						_addOutputs += "\"" + nodeOutputs[j] + "\"";
					}
					else
					{
						_addOutputs += ", \"" + nodeOutputs[j] + "\"";
					}
				}
				_addOutputs += "}";
				
				_content = _content.Replace("NODE_OUTPUTS", _addOutputs);
			}
			
			
			if (_content.Contains("NODE_TYPE"))
			{
				switch (selectedNodeType)
				{
				case 0:
					_content = _content.Replace("NODE_TYPE", "NodeAttributes.NodeType.Normal");
					break;
				case 1:
					_content = _content.Replace("NODE_TYPE", "NodeAttributes.NodeType.Coroutine");
					break;
				case 2:
					_content = _content.Replace("NODE_TYPE", "NodeAttributes.NodeType.Event");
					break;
				}
			}
			
			// Modules		
			
			var _m = 0;
			var _moduleDrawInspectorCode = "";
			var _nodeModuleVariablesCode = "";
			var _modulesActive = false;
			foreach(var module in selectedNodeModules.Keys)
			{
				if (selectedNodeModules[module])
				{
					_moduleDrawInspectorCode += "\n\t\t\t" + nodeModules[_m].customDrawInspectorCode;
					_nodeModuleVariablesCode += "\n\t\t" + nodeModules[_m].nodeVariablesCode;
					_modulesActive = true;
				}
				
				_m ++;
			}
			
			if (_content.Contains("NODE_MODULESVARIABLES"))
			{
				_content = _content.Replace("NODE_MODULESVARIABLES", _nodeModuleVariablesCode);
			}
			
			
			if (_content.Contains("NODE_MODULES"))
			{
				var _moduleText = "";
				if (_modulesActive)
				{
					_moduleText = "\t\t// Modules";
				}
				
				foreach (var module in selectedNodeModules.Keys)
				{
					if (selectedNodeModules[module])
					{
						var _moduleName = module.Replace("FR", "");
						_moduleText += "\n\t\t" + module + " " + "module" + _moduleName + " = new " + module + "();";
					}
				}
				
				if (!string.IsNullOrEmpty(_moduleText))
				{
					_content = _content.Replace("NODE_MODULES", _moduleText);
				}
				else
				{
					_content = _content.Replace("NODE_MODULES", "");	
				}
			
			}
			
			
			if (_content.Contains("NODE_MODULECUSTOMINSPECTORCODE"))
			{
				if (!string.IsNullOrEmpty(_moduleDrawInspectorCode))
				{
					drawCustomInspector = true;
				}
				
				_content = _content.Replace("NODE_MODULECUSTOMINSPECTORCODE", _moduleDrawInspectorCode);
			}
			
			//if (selectedNodeModules["FRNodeEventDispatcher"])
			//{
			//	drawCustomInspector = true;
			//	_content = _content.Replace("NODE_EVENTDISPATCHERMODULE", "moduleNodeEventDispatcher.DrawEditor(moduleNodeEventDispatcher, rootGraph);");
			//}
			
			
			if (_content.Contains("NODE_DISABLEDEFAULTINSPECTOR"))
			{
				_content = _content.Replace("NODE_DISABLEDEFAULTINSPECTOR", "disableDefaultInspector = " + disableDefaultInspector.ToString().ToLower() + ";");
			}
			
			if (_content.Contains("NODE_DISABLEVARIABLEINSPECTOR"))
			{
				_content = _content.Replace("NODE_DISABLEVARIABLEINSPECTOR", "disableVariableInspector = " + disableVariableInspector.ToString().ToLower() + ";");
			}
			
			if (_content.Contains("NODE_DISABLECUSTOMINSPECTOR"))
			{
				_content = _content.Replace("NODE_DISABLECUSTOMINSPECTOR", "disableDrawCustomInspector = " + (!drawCustomInspector).ToString().ToLower() + ";");
			}
			

			if (!drawCustomInspector)
			{
				string _startString = "// +";
					
				int startPos = _content.LastIndexOf(_startString) + _startString.Length + 1;
				int length = _content.IndexOf("// -") - startPos;
				string _sub = _content.Substring(startPos, length);

				_content = _content.Replace(_sub, "");
				
				_content = _content.Replace("// +", "");
				_content = _content.Replace("// -", "");
			}
			else
			{
				_content = _content.Replace("// +", "");
				_content = _content.Replace("// -", "");
			
					
			}
			
			
			
			StreamWriter sw = new StreamWriter(System.IO.Path.Combine(nodePath, (nodeName + ".cs")));
			//sw.Write(_content);
			sw.Write(_content.Replace("\r\n", "\n")); 
			sw.Close();
			
			AssetDatabase.Refresh();
			
			EditorUtility.DisplayDialog("Success","Node " + nodeName + " created successfully", "ok");
		
		}
	}
}
#endif