using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Editor;

namespace FlowReactor.Nodes
{
	/// <summary>
	/// Can be expanded to different resource types
	/// </summary>
	/// 
	[NodeAttributes( "FlowReactor/BlackBoard" , "Load a prefab asset from the resource folder" , "actionNodeColor" , 1 )]
	public class LoadPrefab : Node
	{

		public FRGameObject result;

		[SerializeField]
		public string resourcePath;
		[SerializeField]
		public string fullPath;
		[SerializeField]
		public string objType;
		[SerializeField]
		public string objectIcon;
		
		bool isDragging;
		
		Object obj;
		Object lastObj;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
	
		public override void DrawCustomInspector()
		{
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope())
				{
					if (!string.IsNullOrEmpty(objectIcon))
					{
						GUILayout.Label(EditorGUIUtility.IconContent(objectIcon), GUILayout.Width(40), GUILayout.Height(40));
					}			
				
					DropAreaGUI();
					
					
					if (GUILayout.Button(EditorGUIUtility.IconContent("ViewToolZoom"), GUILayout.Width(32), GUILayout.Height(32)))
					{
						Object _obj = AssetDatabase.LoadAssetAtPath(fullPath, typeof(Object));
						Selection.activeObject = _obj;
					}
				
				}
			}
			
			using (new GUILayout.VerticalScope())
			{
				
				GUILayout.Label("Asset: " + resourcePath, "boldLabel");		
			}
			
			GUILayout.Label("Store to:");
			FRVariableGUIUtility.DrawVariable(result, this, false, editorSkin);
			
			if (obj != lastObj)
			{
				lastObj = obj;
				
				string _path = AssetDatabase.GetAssetPath(obj);
				
				
				fullPath = _path;
				
				_path = Path.Combine(Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path));
			
				var _fullPathWithoutExtension = _path;
				
				var _ind = _path.IndexOf("Resources");
				resourcePath = _path.Substring(_ind+10, _fullPathWithoutExtension.Length-(_ind + 10));
				
				objType = obj.GetType().Name.ToString();
				
				switch(objType)
				{
				case "GameObject":
					objectIcon = "d_Prefab Icon";
					break;
				case "Texture2D":
					objectIcon = "Texture2D Icon";
					break;
				case "AudioClip":
					objectIcon = "AudioClip Icon";
					break;
				case "MonoScript":
					objectIcon = "cs Script Icon";
					break;
				case "TextAsset":
					objectIcon = "TextAsset Icon";
					break;
				case "Material":
					objectIcon = "Material Icon";
					break;
				case "AudioMixerController":
					objectIcon = "AudioMixerGroup Icon";
					break;
				case "Shader":
					objectIcon = "Shader Icon";
					break;
				case "AnimationClip":
					objectIcon = "AnimationClip Icon";
					break;
				default:
					objectIcon = "GameObject Icon";
					break;
				}
			}
			
			
		
		}
		#endif

		#if UNITY_EDITOR
		public void DropAreaGUI()
		{
	
			Event _evt = Event.current;
			Rect _dropArea = GUILayoutUtility.GetRect(120f, 40f);
		
			if (isDragging)
			{
				GUI.color = Color.green;
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Box (_dropArea, "Drag new object here");
			GUI.color = Color.white;
			switch (_evt.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!_dropArea.Contains(_evt.mousePosition))
				{
					isDragging = false;
					return;
				}
				else
				{
					isDragging = true;
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if (_evt.type == EventType.DragPerform)
					{
						isDragging = false;
						DragAndDrop.AcceptDrag();
						foreach (Object _dobj in DragAndDrop.objectReferences)
						{
							if (_dobj.GetType() == typeof(GameObject))
							{
								obj = _dobj;
							}
						}
					}
					
				}
				break;
			}
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			result.Value = Resources.Load(resourcePath, typeof(GameObject)) as GameObject;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}