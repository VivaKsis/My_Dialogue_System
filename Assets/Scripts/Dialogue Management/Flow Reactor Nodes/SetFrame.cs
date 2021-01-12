using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using System;
using FlowReactor.BlackboardSystem;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "Dialogue Management" , "Base node class for setting various frames" , "actionNodeColor" , new string[]{""} , NodeAttributes.NodeType.Normal )]
	public class SetFrame : Node
	{
		public BlackBoard SceneFramesBlackBoard;
		protected List<GameObject> sceneFrameList;

		// Editor node methods
#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = true;

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		protected void RenewObjectFromSceneToBlackboard()
		{
			Debug.LogWarning("Scene Frames Blackboard Renew");
			Transform canvasTransform = GameObject.FindGameObjectWithTag("Main Canvas").transform;
			sceneFrameList = new List<GameObject>();

			for (int b = 0; b < canvasTransform.childCount; b++)
			{
				GameObject gameObject = canvasTransform.GetChild(b).gameObject;

				if (gameObject.GetComponent<Frame>() != null)
				{
					sceneFrameList.Add(gameObject);
				}
			}

			SceneFramesBlackBoard.GetVariableByName<FRGameObjectList>("SceneFrames").Value = sceneFrameList;
		}
		protected void CheckIfBlackBoardVariablesComplete()
		{
			sceneFrameList = SceneFramesBlackBoard.GetVariableByName<FRGameObjectList>("SceneFrames").Value;
			for (int a = 0; a < sceneFrameList.Count; a++)
			{
				if (sceneFrameList[a] == null)
				{
					Debug.LogWarning("Scene Frames in the blackboard is not completed. Renew");
					RenewObjectFromSceneToBlackboard();
				}
			}
		}
		

#endif
		public virtual void SetFrameField()
        {
        }

		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			
		}
	}
}