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
	[NodeAttributes( "Unity/Transform" , "Set the parent of the transform." , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class SetParent : Node
	{
		
		[HelpBox("Setting the parent to ‘null’ unparents the GameObject and turns child into a top-level object in the hierarchy", HelpBox.MessageType.info)]
		public FRGameObject parentObject;
		public FRGameObject childObject;
		
		[HelpBox("If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before.", HelpBox.MessageType.info)]
		public FRBoolean worldPositionStays;
		
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
		#endif


		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			
			////////////////////////
			if (parentObject.Value != null)
			{
				childObject.Value.transform.SetParent(parentObject.Value.transform, worldPositionStays.Value);
			}
			else
			{
				childObject.Value.transform.SetParent(null);
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}