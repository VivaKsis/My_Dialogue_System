//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using FlowReactor;

namespace FlowReactor.Editor
{
	public class FREditorSettingsGUI
	{
		static string newColorCategory = "";
		static FREditorSettings settings;
		
		static bool colorIDError = false;
		static GUISkin editorSkin;
		
		static Vector2 scrollPosition;
		
		public static void Draw(FREditorSettings _settings)
		{
			using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
			{
				scrollPosition = scrollView.scrollPosition;
				
				if (editorSkin  == null)
				{
					editorSkin = EditorHelpers.LoadSkin();
				}
			
			
				GUI.skin.GetStyle("Label").richText = true;
			
				/////////////////
				// COLORS
				////////////////
				
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					GUILayout.Label("<b><size=12>Colors</size></b>");
					GUILayout.Space(10);
					
					GUILayout.Label("Default colors ID:");
					
					// default colors
					for (int d = 0; d < _settings.defaultColorsDark.Count; d ++)
					{
						using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
						{
							GUILayout.Label( _settings.defaultColorsDark[d].id, GUILayout.Width(100));
					
							GUILayout.Label("Dark skin:");
							_settings.defaultColorsDark[d].color = EditorGUILayout.ColorField(_settings.defaultColorsDark[d].color);
							GUILayout.FlexibleSpace();
							GUILayout.Label("Light skin:");
							_settings.defaultColorsLight[d].color = EditorGUILayout.ColorField(_settings.defaultColorsLight[d].color);
							
						}
					}
					
					GUILayout.Label("Inspector colors");
					for (int i = 0; i < _settings.inspectorColors.Count; i ++)
					{
						using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
						{
							GUILayout.Label(_settings.inspectorColors[i].id, GUILayout.Width(100));
							_settings.inspectorColors[i].color = EditorGUILayout.ColorField(_settings.inspectorColors[i].color);
						}
					}
					
				
					EditorHelpers.DrawUILine();
					
				
					GUILayout.Label("Custom colors ID:");
					using (new GUILayout.VerticalScope())
					{
						// custom colors
						GUILayout.Label("New color ID");
						newColorCategory = GUILayout.TextField(newColorCategory);
				           
						if (colorIDError)
						{
							EditorGUILayout.HelpBox("color id already exists", MessageType.Warning);
						}
				           
						if (GUILayout.Button("Add new color"))
						{
							var _entryExists = _settings.categoryColors.Any(x => x.id == newColorCategory);
							if (!_entryExists)
							{
								colorIDError = false;
								_settings.categoryColors.Add(new FREditorSettings.EditorColors(newColorCategory, Color.black));
							}
							else
							{
								colorIDError = true;
							}
						}
					}
					
					for(int c = 0; c < _settings.categoryColors.Count; c ++)
					{
						using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
						{
							GUILayout.Label(_settings.categoryColors[c].id);
							GUILayout.FlexibleSpace();
							_settings.categoryColors[c].color = EditorGUILayout.ColorField(	_settings.categoryColors[c].color);
							
							if (GUILayout.Button("x", GUILayout.Width(20)))
							{
								_settings.categoryColors.RemoveAt(c);
							}
						}
					}
					
					GUILayout.Space(5);
					EditorHelpers.DrawUILine();
					
					if (_settings.currentlySelectedGraph != null)
					{
						if (GUILayout.Button("Reload nodes with new colors", GUILayout.Height(30)))
						{
							for (int n = 0; n < _settings.currentlySelectedGraph.nodes.Count; n ++)
							{
								_settings.currentlySelectedGraph.nodes[n].ReloadColor(_settings);	
							}
							
							for (int s = 0; s < _settings.currentlySelectedGraph.subGraphs.Count; s ++)
							{
								_settings.currentlySelectedGraph.subGraphs[s].ReloadColors(_settings);
							}
						}
					}
			            	
					if (GUILayout.Button("Reset to default colors", GUILayout.Height(30)))
					{
						if (EditorUtility.DisplayDialog("Reset colors?", "Are you sure you wish to reset all colors?", "yes", "no"))
						{
							_settings.ResetColors();
						}
					}
				}
				
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					EditorHelpers.DrawUILine();
					GUILayout.Space(10);
					GUILayout.Label("<b><size=12>General</size></b>");
					GUILayout.Space(10);
					
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Expand nodes by default:");
						_settings.createExpandedNodes = EditorGUILayout.Toggle("", _settings.createExpandedNodes, GUILayout.MaxWidth(200));
					}
					
					//using (new GUILayout.HorizontalScope())
					//{
					//	GUILayout.Label("Horizontal alignment space:");
					//	_settings.horizontalAlignmentSpace = EditorGUILayout.IntField(_settings.horizontalAlignmentSpace);
					//}
					
					//using (new GUILayout.HorizontalScope())
					//{
					//	GUILayout.Label("Vertical alignment space:");
					//	_settings.verticalAlignmentSpace = EditorGUILayout.IntField(_settings.verticalAlignmentSpace);
					//}
				}
					
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					/////////////////
					// SHORTCUTS
					////////////////
					
					EditorHelpers.DrawUILine();
					GUILayout.Space(10);
					GUILayout.Label("<b><size=12>Editor Shortcuts</size></b>");
					EditorGUILayout.HelpBox("It's important to make sure that shortcuts don't conflict with Unity editor shortcuts!", MessageType.Info);
					GUILayout.Space(10);
					
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Focus: CTRL + ");
						_settings.keyFocus = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyFocus, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Create comment: CTRL + ");
						_settings.keyCreateComment = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyCreateComment, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Create Group: CTRL + ");
						_settings.keyCreateGroup = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyCreateGroup, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Create Sub-Graph: CTRL + ALT + ");
						_settings.keyCreateSubGraph = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyCreateSubGraph, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Goto parent graph: CTRL + ");
						_settings.keyGotoParentGraph = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyGotoParentGraph, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Expand nodes: CTRL + ");
						_settings.keyExpandNodes = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyExpandNodes, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Collapse nodes: CTRL + ");
						_settings.keyCollapseNodes = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyCollapseNodes, GUILayout.MaxWidth(200));
					}
					
					GUILayout.Label("Nodes alignment:", "boldLabel");
					
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Align to left: CTRL + ");
						_settings.keyAlignNodesLeft = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyAlignNodesLeft, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Align to right: CTRL + ");
						_settings.keyAlignNodesRight = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyAlignNodesRight, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Align to top: CTRL + ");
						_settings.keyAlignNodesTop = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyAlignNodesTop, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Align to bottom: CTRL + ");
						_settings.keyAlignNodesBottom = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyAlignNodesBottom, GUILayout.MaxWidth(200));
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Align automatically: CTRL + ");
						_settings.keyAlignNodesAutomatically = (KeyCode)EditorGUILayout.EnumPopup("", _settings.keyAlignNodesAutomatically, GUILayout.MaxWidth(200));
					}
					
					if (GUILayout.Button("Reset to default shortcuts", GUILayout.Height(30)))
					{
						if (EditorUtility.DisplayDialog("Reset shortcuts?", "Are you sure you wish to reset all shortcuts?", "yes", "no"))
						{
							_settings.ResetShortcuts();
						}
					}
					
					if (_settings != null)
					{
						EditorUtility.SetDirty(_settings);
					}
				}
				
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					/////////////////
					// RE-Initialize Nodes : Should be used with caution!
					// Reloads icon, resets node size and sets nodes output back to initial count
					////////////////
					if (_settings.currentlySelectedGraph != null)
					{
						EditorHelpers.DrawUILine();
						GUILayout.Space(10);
						
						GUILayout.Label("<b><size=12>Re-Initialize nodes</size></b>");
						EditorGUILayout.HelpBox("Reloads node icons, resets node size and sets the output counts back to their initial count. Warning: This could break some nodes", MessageType.Warning);
						
						if (GUILayout.Button("Re-Initialize all nodes", GUILayout.Height(30)))
						{
							if (EditorUtility.DisplayDialog("Re-Initialize all nodes", "Are you sure you want to re-initialize all nodes?", "yes", "no"))
							{
								for (int n = 0; n < _settings.currentlySelectedGraph.nodes.Count; n ++)
								{
									_settings.currentlySelectedGraph.nodes[n].ReInitialize();	
								}
										
								for (int s = 0; s < _settings.currentlySelectedGraph.subGraphs.Count; s ++)
								{
									_settings.currentlySelectedGraph.subGraphs[s].ReInitializeNodes();
								}
							}
						}
					}
				}
				
				GUILayout.Space(50);
			}
		}
	}
}
#endif