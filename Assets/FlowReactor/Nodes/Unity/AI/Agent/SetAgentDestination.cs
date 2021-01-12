using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.AI;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/AI/Agent" , "Sets or updates the destination thus triggering the calculation for a new path." , "actionNodeColor" , 1 )]
	public class SetAgentDestination : Node
	{
		public FRNavMeshAgent agent;
		[HelpBox("If destination game object is null, then the node will use the destination vector instead.", HelpBox.MessageType.info)]
		[Title("Set destination to:")]
		public FRGameObject destinationGameObject;
		public FRVector3 destinationVector;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			icon = EditorHelpers.LoadIcon("destinationIcon.png");
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
			if (destinationGameObject.Value != null)
			{
				agent.Value.SetDestination(destinationGameObject.Value.transform.position);
			}
			else
			{
				agent.Value.SetDestination(destinationVector.Value);				
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}