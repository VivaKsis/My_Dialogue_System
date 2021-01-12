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
	[NodeAttributes( "Unity/AI/Agent" , "Store the current NavMesh Agent velocity" , "actionNodeColor" , 1 )]
	public class GetAgentVelocity : Node
	{
	
		public FRNavMeshAgent agent;

		[Title("Store to:")]
		public FRVector3 velocity;
		public FRFloat velocityMagnitude;

		#if UNITY_EDITOR
		// Editor node initialization
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
			velocity.Value = agent.Value.velocity;
			velocityMagnitude.Value = agent.Value.velocity.magnitude;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}