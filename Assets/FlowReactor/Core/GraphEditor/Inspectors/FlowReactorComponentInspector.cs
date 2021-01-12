//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if FLOWREACTOR_DATABOX
using Databox;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Editor
{	
	[CustomEditor(typeof(FlowReactorComponent))]
	public class FlowReactorComponentInspector : UnityEditor.Editor
	{
	
		public static FlowReactorComponent fr;
	    
		Texture2D editIcon;
		Texture2D refreshIcon;
		Texture2D okIcon;
		Texture2D cancelIcon;
		
		GUISkin editorSkin;
		Color[] colorsDarkSkin = new Color[] { new Color(63f/255f, 63f/255f, 63f/255f), new Color(56f/255f, 56f/255f, 56f/255f) };
		Color[] colorsLighSkin = new Color[] { new Color(165f/255f, 165f/255f, 165f/255f), new Color(203f/255f, 203f/255f, 203f/255f) };
		
		
	    void OnEnable()
		{
			try
			{
				fr = (FlowReactorComponent)target as FlowReactorComponent;
			
				if (fr.overrideSceneVariables == null)
				{
					fr.overrideSceneVariables = new Dictionary<string, FRVariable>();
				}
				
				#if UNITY_EDITOR
				//
				if (!EditorApplication.isPlaying)
				{
					#if FLOWREACTOR_DEBUG
					Debug.Log("Collect exposed variables");
					#endif
					fr.CollectAndUpdateAllExposedVariables();
				}
				#endif
			}
			catch{}
		} 
		
		
	    public override void OnInspectorGUI()
		{		
			 
			if (editorSkin == null)
			{
				editorSkin = EditorHelpers.LoadSkin();
				editIcon = EditorHelpers.LoadIcon("editIcon.png");
				refreshIcon = EditorHelpers.LoadIcon("refreshIcon.png");
				okIcon = EditorHelpers.LoadIcon("checkmarkIcon.png");
				cancelIcon = EditorHelpers.LoadIcon("cancelIcon.png");
			}
			
			
			if (fr != null && fr.graph != null )
			{
				GUI.enabled = true;
			}
			else
			{
				GUI.enabled = false;
			}
			
			
			using (new GUILayout.HorizontalScope("Box"))
			{
			
				if (GUILayout.Button((Application.isPlaying && fr.runUniqueInstance) ? "Open instance" : "Open", GUILayout.Height(50)))
				{
					//var window = CreateInstance<GraphEditor>();
					//window.Init(window, fr.graph);
					var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
					_settings.OpenGraphWindow( fr.graph );
					
					// Check if graph needs to be updated
					GraphUpdater.UpdateGraph(fr.graph);
				}	
			
				if (Application.isPlaying && fr.runUniqueInstance)
				{
					if (GUILayout.Button("Open original", GUILayout.Height(50)))
					{
						//var window = CreateInstance<GraphEditor>();
						//window.Init(window, fr.originalGraph);
						var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
						_settings.OpenGraphWindow( fr.originalGraph );
					}
				}
				
			} 	

			GUI.enabled = true;
			
			EditorGUI.BeginChangeCheck();
			
			using (new GUILayout.HorizontalScope("Box"))
			{

				fr.graph = (Graph) EditorGUILayout.ObjectField(fr.graph, typeof(Graph), false);

				
				if (GUILayout.Button("New Graph"))
				{
					NodeCreator.CreateNewGraphWithDefaultNodes(fr);
				}
			
			}
			
			using (new GUILayout.VerticalScope(("Box")))
			{
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Graph settings");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("?", GUILayout.Width(18))){Application.OpenURL("https://flowreactor.io/documentation/flowreactor-component/");}
				}
				
				fr.runUniqueInstance = GUILayout.Toggle(fr.runUniqueInstance, "unique instance");
				fr.useGlobalUpdateLoop = GUILayout.Toggle(fr.useGlobalUpdateLoop, "global update loop");
			}
			
			if (fr.graph == null)
				return;
				

			// cache keys which needs to be removed
			List<string> cleanupKeys = new List<string>();
			

		
			EditorHelpers.DrawUILine();
		
#region nodecontrollables
		
			// NODE CONTROLLABLES
			////////////////////////////
			using (new GUILayout.HorizontalScope("Toolbar"))
			{
				GUILayout.Label("Node controlled objects", "boldLabel");
				
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button(new GUIContent(refreshIcon, "Collect all controllable nodes"), "toolbarButton"))
				{
					fr.graph.RegisterINodeControllables(fr);
				
					UpdateControllablesDictionary();
				}
				
				if (GUILayout.Button("?", "toolbarButton"))
				{
					Application.OpenURL("https://flowreactor.io/documentation/frnodemodules/");
				}
			}
			
			if (fr.nodeControllables != null && fr.nodeControllables.Count > 0)
			{
				EditorGUI.BeginChangeCheck();
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					if (fr.nodeControllables != null)
					{
					
					
						foreach(Node node in fr.nodeControllables.Keys)
						{
							using (new GUILayout.VerticalScope("Box"))
							{
								using (new GUILayout.HorizontalScope("Toolbar"))
								{
									GUILayout.Label(node.nodeData.title);
									
									GUILayout.FlexibleSpace();
								}
							
								Dictionary<string, INodeControllable> tempController = new Dictionary<string, INodeControllable>();
								
								foreach (var faces in fr.nodeControllables[node].interfaces.Keys)
								{
									using (new GUILayout.HorizontalScope())
									{
										GUILayout.Label(faces);
										var _lastRect = GUILayoutUtility.GetLastRect();
										
										var _obj = EditorGUILayout.ObjectField((UnityEngine.Object)fr.nodeControllables[node].interfaces[faces], typeof(INodeControllable), true);
										
										if (GUILayout.Button("select"))
										{
											PopupWindow.Show(_lastRect, new PopupShowINodeControllableObjects(node, faces));
										}
										//fr.gameControllerInterfaces[node].interfaces[faces]  = _obj as INodeController;
										tempController.Add(faces, _obj as INodeControllable);
									}
								}
								
								foreach(var t in tempController.Keys)
								{
									fr.nodeControllables[node].interfaces[t] = tempController[t];
								}
								
								
								
							}
						}
						
						
					}
					else
					{
						GUILayout.Label("null");

					}
				
				}
				
				
				if (EditorGUI.EndChangeCheck())
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
				}
			}
			
			
			EditorHelpers.DrawUILine();
			////////////////////////////
#endregion

#region exposedvariables
			
			// EXPOSED VARIABLES
			////////////////////////////
			using (new GUILayout.HorizontalScope("Toolbar"))
			{
				GUILayout.Label("Exposed variables", "boldLabel");
				
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button(new GUIContent(refreshIcon, "Manually collect and update exposed variables from nodes"), "ToolbarButton"))
				{
					fr.CollectAndUpdateAllExposedVariables();
				}
				
				#if FLOWREACTOR_DEBUG
				GUI.color = Color.yellow;
				if (GUILayout.Button("Clear exposed variables",  "ToolbarButton"))
				{
					fr.exposedNodeVariables = new Dictionary<string, Dictionary<string, FRVariable>>();
					fr.graph.exposedNodeVariables = new Dictionary<string, Graph.ExposedVariables>();
				}
				GUI.color = Color.white;
				#endif
			}
			
			
			if (fr.exposedNodeVariables != null)
			{
				if (fr.exposedNodeVariables.Keys.Count > 0)
				{
					var i = 0;
					
					using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
					{
							
						foreach (string exposedNodeKey in fr.exposedNodeVariables.Keys)
						{
							if (EditorGUIUtility.isProSkin)
							{
								GUI.color = colorsDarkSkin[i % 2];
							}
							else
							{
								GUI.color = colorsLighSkin[i % 2];
							}
							using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxWhite")))
							{
								GUI.color = Color.white;
						
								GUILayout.Label(exposedNodeKey, "boldLabel");
							
							
								List<string> _variableKeys = fr.exposedNodeVariables[exposedNodeKey].Keys.ToList();								
								for(int s = 0; s < _variableKeys.Count; s ++)
								{
									using (new GUILayout.HorizontalScope())
									{ 
										var _variableKey = _variableKeys[s];
										var _exposedVariable = fr.exposedNodeVariables[exposedNodeKey][_variableKey];
										var _varType = _exposedVariable.GetType().Name.ToString();
										GUILayout.Label(new GUIContent(_varType, "GetExposedVariable<" + _varType + ">("+"\"" + exposedNodeKey + "\"," + "\"" + _variableKey + "\");"), GUILayout.Width(80));
										
										GUI.enabled = _exposedVariable.editExposedName;
										_exposedVariable.exposedName = GUILayout.TextField(_exposedVariable.exposedName, GUILayout.Width(100));
										GUI.enabled = true;
										
										
										if (_exposedVariable.editExposedName)
										{
											if (string.IsNullOrEmpty(_exposedVariable.exposedName))
											{
												GUI.enabled = false;
											}
										
										 
											if (GUILayout.Button(okIcon, GUILayout.Width(20), GUILayout.Height(20)))
											{
											
												_exposedVariable.editExposedName = false;
											
										
												var _fromName = _variableKey;
												var _newName = _exposedVariable.exposedName;
												 
												#if FLOWREACTOR_DEBUG
												Debug.Log("update key from: " + _fromName + " to: " + _newName);
												#endif
											
												// check if new name already exists
												bool _nameAlreadyExists = false;
												foreach (var variableKeyB in fr.exposedNodeVariables[exposedNodeKey].Keys)												
												{
													if (variableKeyB.Equals(_newName))												
													{
														_nameAlreadyExists = true;
													}
												}
												
												if (!_nameAlreadyExists)
												{
									
													var _nodeVariableField = fr.graph.exposedNodeVariables[exposedNodeKey].variables[_fromName].nodeOwner.GetType().GetField(fr.graph.exposedNodeVariables[exposedNodeKey].variables[_fromName].name);
													var _nodeVariable = _nodeVariableField.GetValue(fr.graph.exposedNodeVariables[exposedNodeKey].variables[_fromName].nodeOwner) as FRVariable;
		
													_nodeVariable.exposedName = _newName;
												
												
													fr.exposedNodeVariables[exposedNodeKey].UpdateKey(_fromName, _newName);
													fr.graph.exposedNodeVariables[exposedNodeKey].variables.UpdateKey(_fromName, _newName);
												}
												else
												{
												
													_exposedVariable.exposedName = _fromName;
												}
											}
											 
											GUI.enabled = true;
										
											if (GUILayout.Button(cancelIcon, GUILayout.Width(20), GUILayout.Height(20)))
											{
												fr.exposedNodeVariables[exposedNodeKey][_variableKey].exposedName = _variableKey;
												fr.exposedNodeVariables[exposedNodeKey][_variableKey].editExposedName = false;
											}
											
											
										}
										else
										{
											if (GUILayout.Button(editIcon, GUILayout.Width(20), GUILayout.Height(20)))
											{
												fr.exposedNodeVariables[exposedNodeKey][_variableKey].editExposedName = true;
											}
										}
										
										if (fr.exposedNodeVariables[exposedNodeKey].ContainsKey(_variableKey))
										{
											GUILayout.Label("Value: ", GUILayout.Width(50));
											fr.exposedNodeVariables[exposedNodeKey][_variableKey].Draw(true, null);	
										}
											
											
										//GUILayout.Label("Runtime access: GetData<" + fr.exposedNodeVariables[exposed].variables[variableKeys[s]].GetType().ToString() + ">(");
									}
								}
							}
							
							i++;
						}
					}
				}
			}
			
			////////////////////////////
#endregion
				

#region sceneoverrides
			// BLACKBOARD SCENE OVERRIDES
			////////////////////////////
			if (fr.graph.blackboards != null && fr.graph.blackboards.Keys.Count > 0)
			{
				EditorHelpers.DrawUILine();
				
				using (new GUILayout.HorizontalScope("Toolbar"))
				{
					GUILayout.Label("Blackboard scene overrides", "boldLabel");
				}
				
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("BoxLine")))
				{
					foreach (var bb in fr.graph.blackboards.Keys)
					{
						if (fr.graph.blackboards[bb].blackboard == null)
						{
							EditorGUILayout.HelpBox("Blackboard is empty, please assign a blackboard asset to the graph", MessageType.Warning);
							continue;
						}
						
						using (new GUILayout.HorizontalScope(FlowReactorEditorStyles.overflowButton))
						{
							fr.graph.blackboards[bb].foldout = EditorGUILayout.Foldout(fr.graph.blackboards[bb].foldout, fr.graph.blackboards[bb].blackboard.name);
						}
						
						if (fr.graph.blackboards[bb].foldout)
						{

							foreach(var key in fr.graph.blackboards[bb].blackboard.variables.Keys)
							{
								FRVariable value = null;
								
								// object is in the scene but variable is not set as overridable						
								if (fr.overrideSceneVariables.TryGetValue(key.ToString(), out value) && !IsObjectPrefabInProject())
								{
								
									using (new GUILayout.HorizontalScope())
									{
										EditorGUI.BeginChangeCheck();
									
										if (fr.graph.blackboards[bb].blackboard.variables[key].sceneReferenceOnly)
										{
											GUI.enabled = false;
											value.overrideVariable = true;
										}
										value.overrideVariable = GUILayout.Toggle(value.overrideVariable, "Override", GUILayout.Width(100));
										GUI.enabled = true;
										
										if (value.overrideVariable == false)
										{
											//fr.overrideSceneVariables.Remove(key.ToString());
											
											GUI.enabled = false;
											GUILayout.Label(fr.graph.blackboards[bb].blackboard.variables[key].name);
											
											if (fr.graph.blackboards[bb].blackboard.variables[key].useDatabox)
											{
												GUILayout.Label(fr.graph.blackboards[bb].blackboard.variables[key].databoxID + " | " +
													fr.graph.blackboards[bb].blackboard.variables[key].tableID + " | " +
													fr.graph.blackboards[bb].blackboard.variables[key].entryID + " | " +
													fr.graph.blackboards[bb].blackboard.variables[key].tableID);
											}
											else
											{
											
												fr.graph.blackboards[bb].blackboard.variables[key].Draw(true, null);
											}
											GUI.enabled = true;
											
										}
										else
										{
											GUI.enabled = true;										
											//GUILayout.Label(value.name);
											GUILayout.Label(fr.graph.blackboards[bb].blackboard.variables[key].name);
											value.Draw(true, null);
										}


										GUI.enabled = true;

										if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
										{
											UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
										}
										
									}
								
								}
								// object lives in the scene
								else
								{
									
									using (new GUILayout.HorizontalScope())
									{
										var v = fr.graph.blackboards[bb].blackboard.variables[key] as FRVariable;
										
										if (!IsObjectPrefabInProject())
										{
											if (v.sceneReferenceOnly)
											{
												GUI.enabled = false;
												v.overrideVariable = true;
											}
										}
										else
										{
											GUI.enabled = false;
										}
										v.overrideVariable = GUILayout.Toggle(v.overrideVariable, "Override", GUILayout.Width(100));
										GUI.enabled = true;
										
										if (v.overrideVariable)
										{
									
											// create new item and copy it to the override variables
											var _newOverrideVariable = (FRVariable)Activator.CreateInstance(fr.graph.blackboards[bb].blackboard.variables[key].GetType());
											_newOverrideVariable.overrideVariable = true;
											_newOverrideVariable.name = v.name;
											
											fr.overrideSceneVariables.Add(key.ToString(), _newOverrideVariable);
											fr.overrideSceneVariables[key.ToString()].overrideVariable = true; 
											fr.overrideSceneVariables[key.ToString()].name = v.name;
											
											v.overrideVariable = false;// blackboard is always false

										}
										
									
										GUI.enabled = false;
										GUILayout.Label(v.name);
										if (fr.graph.blackboards[bb].blackboard.variables[key].useDatabox)
										{
											GUILayout.Label(fr.graph.blackboards[bb].blackboard.variables[key].databoxID + " | " +
												fr.graph.blackboards[bb].blackboard.variables[key].tableID + " | " +
												fr.graph.blackboards[bb].blackboard.variables[key].entryID + " | " +
												fr.graph.blackboards[bb].blackboard.variables[key].tableID);
										}
										else
										{
											fr.graph.blackboards[bb].blackboard.variables[key].Draw(true, null);
										}
										GUI.enabled = true;
									}
								}
							}
							
							GUILayout.Space(10);
							EditorHelpers.DrawUILine();
						}
						
						
						// cleanup override variables if they don't exist in the blackboard anymore					
						foreach(var kc in fr.overrideSceneVariables.Keys)
						{
							bool _overrideVarExists = false;
							foreach (var bc in fr.graph.blackboards.Keys)
							{
								if (fr.graph.blackboards[bc].blackboard.variables.ContainsKey(Guid.Parse(kc)))
								{
									_overrideVarExists = true;
									
								}
							}
							if (!_overrideVarExists)
							{
								Debug.Log("cleanup");
								cleanupKeys.Add(kc.ToString());
							}
						}
					}
					
				}
			}
			
			for (int c = 0; c < cleanupKeys.Count; c ++)
			{
				#if FLOWREACTOR_DEBUG
				Debug.Log("remove override variable " + cleanupKeys[c]);
				#endif
				Debug.Log("remove");
				fr.overrideSceneVariables.Remove(cleanupKeys[c]);
			}
			
			// only for debug
			#if FLOWREACTOR_DEBUG
			DrawDefaultInspector();
			#endif
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}
			
			
			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();
		}
		
		////////////////////////////
		
#endregion
		
		bool IsObjectPrefabInProject()
		{
			if (PrefabUtility.GetPrefabAssetType(fr.gameObject) == PrefabAssetType.NotAPrefab)
			{
				return false;
			}
			else
			{
				if (fr.gameObject.scene.name == null)
				{
					return true;				
				}
				else
				{
					return false;
				}
			}
		}
		
		// Check if user has removed nodes with registered INodeControllable fields from the graph
		void UpdateControllablesDictionary()
		{
			bool _exist = false;
			List<Node> _remNodes = new List<Node>();
			foreach (Node _node in fr.nodeControllables.Keys)
			{
				_exist = fr.graph.FindINodeControllable(_node);

				if (!_exist)
				{
					// remove controllable
					_remNodes.Add(_node);
				}
			}
					
			for (int n = 0; n < _remNodes.Count; n ++ )
			{
				fr.nodeControllables.Remove(_remNodes[n]);
			}
		}
		
		
		public class PopupShowINodeControllableObjects : PopupWindowContent
		{
			Node node;
			string interf;
			
			List<INodeControllable> _allInterfaces = new List<INodeControllable>();

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, 300);
			}
			
			public override void OnGUI(Rect rect)
			{	
				if (_allInterfaces != null && _allInterfaces.Count > 0)
				{
					for (int i = 0; i < _allInterfaces.Count; i ++)
					{
						if (GUILayout.Button(_allInterfaces[i].GetType().Name))
						{
							fr.nodeControllables[node].interfaces[interf] = _allInterfaces[i];
							editorWindow.Close();
						}
					}
				}
			}
			
			public override void OnOpen()
			{
				
				List<INodeControllable> interfaces = new List<INodeControllable>();
				GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
				foreach( var rootGameObject in rootGameObjects )
				{
					INodeControllable[] childrenInterfaces = rootGameObject.GetComponentsInChildren<INodeControllable>();
					foreach( var childInterface in childrenInterfaces )
					{
						_allInterfaces.Add(childInterface);
					}
				}
			}
		
			public override void OnClose(){}
			
			public PopupShowINodeControllableObjects(Node _node, string _interface)
			{
				node = _node;
				interf = _interface;
			}
		}
	}
	
	
}
#endif