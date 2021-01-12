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
	[NodeAttributes( "Unity/AI/Agent" , "Returns a random position on the navmesh in a certain radius from the agent." , "actionNodeColor" , 1 )]
	public class GetRandomNavMeshPosition : Node
	{
		public FRGameObject agent;
		public FRFloat radius;
		public FRLayerMask layerMask;
		
		public FRVector3 result;
		
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
			result.Value = RandomPosition();
			
			ExecuteNext(0, _flowReactor);
		}
		
		
		Vector3 RandomPosition()
		{
			var _pos = Vector3.zero;
			
			for (int i = 0; i < 30; i++)
	        {
				Vector3 randomPoint = agent.Value.transform.position + Random.insideUnitSphere * radius.Value;
				NavMeshHit hit;
				if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
	            {
					_pos = hit.position;
					break;
	            }
	        }

	        return _pos;
        
        }
	}
}