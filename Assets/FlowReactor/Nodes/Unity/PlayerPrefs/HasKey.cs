using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/PlayerPrefs" , "Returns true if key exists in the preferences." , "actionNodeColor" , 2 )]
	public class HasKey : Node
	{
		public FRString key;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "True";
			outputNodes[1].id = "False";
	
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
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
			var _exists = UnityEngine.PlayerPrefs.HasKey(key.Value);
			
			if (_exists)
			{
				ExecuteNext(0, _flowReactor);
			}
			else
			{
				ExecuteNext(1, _flowReactor);
			}
		}
	}
}