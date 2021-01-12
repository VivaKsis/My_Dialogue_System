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
	
	[NodeAttributes( "FlowReactor/BlackBoard" , "Save multiple BlackBoards to a single file" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class SaveMultiBlackBoards : Node
	{
		public string fileName;
		public BlackBoard.SavePath savePath;
		public BlackBoard.SaveFormat saveFormat;
		
		[SerializeField]
		public List<BlackBoard> blackboards = new List<BlackBoard>();
	
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("saveIcon.png");
	
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
				savePath = (FlowReactor.BlackboardSystem.BlackBoard.SavePath)EditorGUILayout.EnumPopup(savePath);
			}
			
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label ("Format:");
				saveFormat = (FlowReactor.BlackboardSystem.BlackBoard.SaveFormat)EditorGUILayout.EnumPopup(saveFormat);
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
			
			for(int b = 0; b < blackboards.Count; b ++)
			{
				//get serialized data
				var _b = blackboards[b].SerializeBlackboard(saveFormat);
				
				serializedBlackboards.Add(blackboards[b].GetInstanceID(), _b);
			}
			
			byte[] bytes = null;
			var _path = "";
			DataFormat _f = DataFormat.JSON;
			switch(saveFormat)
			{
			case BlackBoard.SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case BlackBoard.SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
			
			switch (savePath)
			{
				case  BlackBoard.SavePath.PlayerPrefs:
						
					bytes = SerializationUtility.SerializeValue(serializedBlackboards, _f);
					string _saveString = System.Convert.ToBase64String(bytes);
					UnityEngine.PlayerPrefs.SetString(fileName, _saveString);
						
					break;
				case BlackBoard.SavePath.PersistentPath:
					_path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
						
					bytes = SerializationUtility.SerializeValue(serializedBlackboards, _f);
					System.IO.File.WriteAllBytes(_path, bytes);
					break;
				case BlackBoard.SavePath.StreamingAssets:
					_path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
						
					bytes = SerializationUtility.SerializeValue(serializedBlackboards, _f);
					System.IO.File.WriteAllBytes(_path, bytes);
					break;
			}
			
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
