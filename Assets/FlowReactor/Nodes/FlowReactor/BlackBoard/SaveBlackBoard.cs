using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/BlackBoard" , "Save BlackBoard to a file" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class SaveBlackBoard : Node
	{
		public string fileName;
		public FlowReactor.BlackboardSystem.BlackBoard.SavePath savePath;
		public FlowReactor.BlackboardSystem.BlackBoard.SaveFormat saveFormat;
		
		public int selectedBlackBoard;
		
		string[] availableBlackBoards = new string[0];
		string[] availableBlackBoardsGuid = new string[0];
		
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
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				availableBlackBoardsGuid = graphOwner.blackboards.Keys.Select(x => x.ToString()).ToArray();
				availableBlackBoards = graphOwner.blackboards.Values.Select(x => x.blackboard != null ? x.blackboard.name.ToString() : "empty").ToArray();
				GUILayout.Label ("BlackBoard:");
				selectedBlackBoard = EditorGUILayout.Popup(selectedBlackBoard, availableBlackBoards);
			}
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			graphOwner.blackboards[Guid.Parse(availableBlackBoardsGuid[selectedBlackBoard])].blackboard.SaveToFile(fileName, savePath, saveFormat);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}