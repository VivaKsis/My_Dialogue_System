﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Scene" , "Loads the Scene asynchronously in the background." , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class LoadSceneAsync : Node
	{
		public FRString scene;
		public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
		
		AsyncOperation asyncLoad;
		bool loading = false;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("sceneIcon.png");

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			loading = true;
			asyncLoad = SceneManager.LoadSceneAsync(scene.Value, loadSceneMode);
			
		}
		
		public override void OnUpdate(FlowReactorComponent _flowReactor)
		{
			if (loading && asyncLoad.isDone)
			{
				loading = false;
				ExecuteNext(0, _flowReactor);
			}
		}
	}
}