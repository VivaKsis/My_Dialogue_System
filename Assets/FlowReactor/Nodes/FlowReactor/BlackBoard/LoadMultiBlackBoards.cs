using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.BlackboardSystem;
using FlowReactor.OdinSerializer;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/BlackBoard" , "Load multiple BlackBoards from a single file" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class LoadMultiBlackBoards : Node
	{
		public string fileName;
		public FlowReactor.BlackboardSystem.BlackBoard.SavePath loadPath;
		public FlowReactor.BlackboardSystem.BlackBoard.SaveFormat loadFormat;
		
		[SerializeField]
		public List<BlackBoard> blackboards = new List<BlackBoard>();
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("loadIcon.png");
	
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
	
	
		// Draw custom node inspector
		public override void DrawCustomInspector()
		{
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label ("File Name:");
				fileName = GUILayout.TextField (fileName);
			}
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label ("Save Path:");
				loadPath = (FlowReactor.BlackboardSystem.BlackBoard.SavePath)EditorGUILayout.EnumPopup(loadPath);
			}
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label ("Format:");
				loadFormat = (FlowReactor.BlackboardSystem.BlackBoard.SaveFormat)EditorGUILayout.EnumPopup(loadFormat);
			}	
			
			EditorHelpers.DrawUILine();
			GUILayout.Label("Blackboards");
			
			if (GUILayout.Button("Add"))
			{
				blackboards.Add(null);
			}
			
			for (int b = 0; b < blackboards.Count; b ++)
			{
				using (new GUILayout.HorizontalScope("Box"))
				{
					if (blackboards[b] != null)
					{
						GUILayout.Label(blackboards[b].name);
					}
					
					blackboards[b] = (BlackBoard) EditorGUILayout.ObjectField(blackboards[b], typeof(BlackBoard), false);
					if (GUILayout.Button("x"))
					{
						blackboards.RemoveAt(b);
					}
				}
			}
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			Dictionary<int, byte[]> serializedBlackboards = new Dictionary<int, byte[]>();
			
			var _path = "";
			DataFormat _f = DataFormat.JSON;
			byte[] bytes = null;
				
			switch(loadFormat)
			{
			case BlackBoard.SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case BlackBoard.SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
				
			switch (loadPath)
			{
				case BlackBoard.SavePath.PlayerPrefs:
						
					string _loadString = UnityEngine.PlayerPrefs.GetString(fileName);
					bytes = System.Convert.FromBase64String(_loadString);
					serializedBlackboards = SerializationUtility.DeserializeValue<Dictionary<int, byte[]>>(bytes, _f);
						
					break;
				case BlackBoard.SavePath.PersistentPath:
					
					_path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
					bytes = System.IO.File.ReadAllBytes(_path);
					serializedBlackboards = SerializationUtility.DeserializeValue<Dictionary<int, byte[]>>(bytes, _f);
						
					break;
				case BlackBoard.SavePath.StreamingAssets:
					
					_path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);		
					bytes = System.IO.File.ReadAllBytes(_path);
					serializedBlackboards = SerializationUtility.DeserializeValue<Dictionary<int, byte[]>>(bytes, _f);
						
					break;
			}
			
			foreach(var key in serializedBlackboards.Keys)
			{
				for (int b = 0; b < blackboards.Count; b ++)
				{
					if (blackboards[b].GetInstanceID() == key)
					{
						// assign data
						blackboards[b].DeserializeBlackboard(serializedBlackboards[key], loadFormat);
					}
				}
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
