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
	[NodeAttributes( "Unity/Transform" , "Gets the position of a game object" , "actionNodeColor" , 1 )]
	public class GetPosition : Node
	{
		
		public FRGameObject gameObject;
		[Title("Store position to:")]
		public FRVector3 position;
		
		public FRFloat xPos;
		public FRFloat yPos;
		public FRFloat zPos;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
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
			position.Value = gameObject.Value.transform.position;
			xPos.Value = gameObject.Value.transform.position.x;
			yPos.Value = gameObject.Value.transform.position.y;
			zPos.Value = gameObject.Value.transform.position.z;
			
			
			ExecuteNext(0, _flowReactor);
		}
	}
}