//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FlowReactor.Editor
{
	[InitializeOnLoad]
	public class FlowReactorWelcomeWindow : EditorWindow
	{
	
		static FREditorSettings settings;
		static Vector2 windowSize;
		static string[] options = new string[]{"Documentation", "Support", "Addons", "More"};
		static int selectedOption = 0;
		static bool showOnStartUp = true;
		
		static GUISkin editorSkin;
		
		static Texture2D splashIcon;
		static Texture2D documentationIcon;
		static Texture2D gettingStartedIcon;
		static Texture2D conceptIcon;
		static Texture2D addonIcon;
		static Texture2D marZBanner;
		static Texture2D databoxBanner;
		static Texture2D twcBanner;
		
		static bool showChangelog;
		static string changelog;
		static string editorVersion;
		
		static Vector2 changelogScrollPos;
		
		public static FlowReactorWelcomeWindow Instance { get; private set; }
		public static bool IsOpen {
			get { return Instance != null; }
		}
		
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() 
		{
			LoadResources();
		}
		
		[MenuItem("Tools/FlowReactor/Welcome")]
		public static void Init()
		{
			LoadResources();
			
			settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			FlowReactorWelcomeWindow window = EditorWindow.CreateInstance<FlowReactorWelcomeWindow>();
			
			window.maxSize = new Vector2(500f, 600f);
			window.minSize = window.maxSize;
			
			Instance = window;
			
			selectedOption = 0;
			
			showOnStartUp = EditorPrefs.GetBool("FLOWREACTOR_SHOW_WELCOME");
			
			#if UNITY_2019_OR_NEWER
			window.ShowModalUtility();
			#else
			window.ShowUtility();	
			#endif
		}
		
		static void LoadResources()
		{
			splashIcon = EditorHelpers.LoadGraphic("logo.png");
			documentationIcon = EditorHelpers.LoadIcon("documentationIcon.png");
			gettingStartedIcon = EditorHelpers.LoadIcon("gettingStartedIcon.png");
			conceptIcon = EditorHelpers.LoadIcon("conceptWorkflowIcon.png");
			addonIcon = EditorHelpers.LoadIcon("addonIcon.png");
			marZBanner = EditorHelpers.LoadGraphic("marZBanner.png");
			databoxBanner = EditorHelpers.LoadGraphic("databoxBanner.png");
			twcBanner = EditorHelpers.LoadGraphic("twcBanner.png");
			editorSkin = EditorHelpers.LoadSkin();
		
		
			changelog = EditorHelpers.LoadChangelog();
			editorVersion = EditorHelpers.GetEditorVersion();
		}
	
	
		void OnGUI()
		{
			DrawGUI(splashIcon, settings);
		}
		
		public static void DrawGUI(Texture2D _logo, FREditorSettings _settings)
		{
			if (editorSkin == null)
			{
				LoadResources();
				showOnStartUp = EditorPrefs.GetBool("FLOWREACTOR_SHOW_WELCOME");
			}
			
			var centeredStyle = new GUIStyle("Label");
			centeredStyle.alignment = TextAnchor.UpperCenter;
			
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{		
				GUILayout.Label(_logo, centeredStyle);
			
				if (showChangelog)
				{
					using (new GUILayout.AreaScope(new Rect(10, 10, 480, 290)))
					{
						using (var scrollView = new EditorGUILayout.ScrollViewScope(changelogScrollPos))
						{
							changelogScrollPos = scrollView.scrollPosition;
							//GUI.TextArea(new Rect(10, 10, 480, 290), changelog);
							GUILayout.TextArea(changelog);
						}
					}
				}
			}
			

			//using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			//{
			//	GUILayout.TextArea("Thank you for using FlowReactor. \nPlease note this version is still in Beta so there might\nbe some issues. Please report any issues you encounter\nto the official support channels.", "Label");
			//}

			selectedOption = GUILayout.Toolbar(selectedOption, options);
			
			
			switch(selectedOption)
			{
				//Documentation
				case 0:
					Documentation();
					break;
				//Support
				case 1:
					Support();
					break;
				//Addons
				case 2:
					Addons();
					break;
				case 3:
					More();
					break;
			}

			
			GUILayout.FlexibleSpace();
			
			EditorHelpers.DrawUILine();
			
			using (new GUILayout.HorizontalScope())//editorSkin.GetStyle("Box")))
			{
				GUILayout.Label(editorVersion);
				
				if (GUILayout.Button("Changelog"))
				{
					showChangelog = !showChangelog;
				}
				
				EditorGUI.BeginChangeCheck();
				showOnStartUp = EditorGUILayout.Toggle("show on start-up", showOnStartUp);
				
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool("FLOWREACTOR_SHOW_WELCOME", showOnStartUp);
				}
				
				if (GUILayout.Button("DEBUG MODE"))
				{
					FlowReactorInstallAddons.EnableDebugMode();
				}
			}
		}
		
		static void Documentation()
		{

			using (new GUILayout.VerticalScope())
			{
				if (GUILayout.Button(new GUIContent("Documentation:\nHead over to the official online documentation", documentationIcon), FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("https://flowreactor.io/documentation");
				}
			}
		
			using (new GUILayout.HorizontalScope())
			{
			
				if (GUILayout.Button(new GUIContent("Concept and workflow:\nRead about the basic concept and worflow of FlowReactor.", conceptIcon), FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("https://flowreactor.io/documentation/2-concept-and-workflow/");
				}
			}
			
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button(new GUIContent("Getting started:\nStart creating your first graph with nodes", gettingStartedIcon), FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("https://flowreactor.io/documentation/3-getting-started/");
				}
			}
		}
		
		static void Tutorials()
		{
			
		}
		
		static void Support()
		{
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("E-Mail", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40)))
				{
					Application.OpenURL("mailto:mail@doorfortyfour.com");
				}
			}	
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Discord", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40)))
				{
					Application.OpenURL("https://discord.gg/a5uf3nM");
				}
			}	
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Forum", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40)))
				{
					Application.OpenURL("https://hub.flowreactor.io/forum");
				}
			}	
			
			
		}
		
		static void Addons()
		{
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button(new GUIContent(" Enable Databox", addonIcon), FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					FlowReactorInstallAddons.InstallDatabox();
				}
			}	
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button(new GUIContent (" Enable DOTween", addonIcon), FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					FlowReactorInstallAddons.InstallDOTween();
				}
			}
		}
		
		static void More()
		{
			GUILayout.Label("More projects by doorfortyfour");

			using (new GUILayout.HorizontalScope())
			{
				GUI.color = Color.black;
				GUI.contentColor = Color.white;
				if (GUILayout.Button("", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://www.marz-game.com");
				}
				GUI.color = Color.white;
				
				var _lastRect = GUILayoutUtility.GetLastRect();
				GUI.Label(_lastRect, marZBanner);
			}
			
			using (new GUILayout.HorizontalScope())
			{
				GUI.color = Color.black;
				GUI.contentColor = Color.white;
				if (GUILayout.Button("", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://databox.doorfortyfour.com/");
				}
				GUI.color = Color.white;
				
				var _lastRect = GUILayoutUtility.GetLastRect();
				GUI.Label(_lastRect, databoxBanner);
			}
			
			using (new GUILayout.HorizontalScope())
			{
				GUI.color = Color.black;
				GUI.contentColor = Color.white;
				if (GUILayout.Button("", FlowReactorEditorStyles.dictionaryElement, GUILayout.Height(40), GUILayout.ExpandWidth(true)))
				{
					Application.OpenURL("http://tileworldcreator.doorfortyfour.com/");
				}
				GUI.color = Color.white;
				
				var _lastRect = GUILayoutUtility.GetLastRect();
				GUI.Label(_lastRect, twcBanner);
			}
			
			
		}
		
	}
}
#endif