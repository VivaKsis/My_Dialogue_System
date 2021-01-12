using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/AI" , "Returns a cover point based on agent position and player position" , "actionNodeColor" , 1 )]
	public class SearchForCover : Node
	{
		public FRGameObject agent;
		public FRFloat range;
		[HelpBox("If true agent only moves to cover point if it's valid instead of moving to a random point instead.")]
		public FRBoolean alwaysCover;
		
		[HelpBox("The layers this node should check for visibility between cover point and target object.")]
		public FRLayerMask visibilityCheckLayerMask;
		
		[HelpBox("Search cover from target object")]
		public FRGameObject targetObject;
		public FRString targetLayerName;
		
		[Title("Store cover point to:")]
		public FRVector3 coverPoint;

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
			coverPoint.Value = GetCoverPoint();
			Debug.DrawRay(coverPoint.Value, Vector3.up, Color.white, 1.0f);
			
			ExecuteNext(0, _flowReactor);
		}
		
		
		
		Vector3 GetCoverPoint()
		{
			NavMeshHit _hit;

			var _rnd = Random.insideUnitCircle * range.Value;
			var rndPoint = new Vector3(agent.Value.transform.position.x + _rnd.x, agent.Value.transform.position.y, agent.Value.transform.position.z + _rnd.y);
			
		
			for (int i = 0; i < 30; i ++)
			{
				
				if( NavMesh.FindClosestEdge(rndPoint, out _hit, NavMesh.AllAreas))
				{
					Debug.DrawRay(_hit.position, Vector3.up * 3, Color.blue, 3f);
				
					if (Check( _hit.position))
					{
						return _hit.position;
					}
				}
			}

			// If we couldn't find any valid cover, get random point
			if (!alwaysCover.Value)
			{
				return RandomPoint();
			}
			else
			{
				return agent.Value.transform.position;
			}
		}
		
		// Check if cover is valid
		bool Check(Vector3 _p)
		{
			RaycastHit _hit;
			if (Physics.Raycast(_p, (targetObject.Value.transform.position - _p), out _hit, Vector3.Distance(_p, targetObject.Value.transform.position), visibilityCheckLayerMask.Value))
			{
				//Debug.Log("Raycast " + _hit.transform.gameObject.name);
				float _minDistance = Vector3.Distance(_p, targetObject.Value.transform.position);
			
				if (_hit.transform.gameObject.layer ==  LayerMask.NameToLayer(targetLayerName.Value) && _minDistance > 3f)
				{
					// targetobject can see agent
					Debug.DrawRay(_p, _hit.point - _p, Color.red, 3f);
					//Debug.Log("cover is invalid");
					return false;
				
				}
				else
				{
					// target object cant see agent. Cover is valid
					Debug.DrawRay(_p, _hit.point - _p, Color.green, 3f);
					//Debug.Log("Cover is valid");
					
					// Has path
					NavMeshPath path = new NavMeshPath();
					if (NavMesh.CalculatePath(targetObject.Value.transform.position, _p, NavMesh.AllAreas, path))
					{
						return true;
					}
					else
					{
						return false;
					}
					
				}
			}
			
			return false;
		}
    
		
		Vector3 RandomPoint()
		{
			for (int i = 0; i < 30; i++) {
				
				// get a random point which is 0.5x closer then range
				var _rnd = Random.insideUnitCircle * (range.Value * 0.5f);
				Vector3 randomPoint = new Vector3(agent.Value.transform.position.x + _rnd.x, agent.Value.transform.position.y, agent.Value.transform.position.z + _rnd.y);
				
				NavMeshHit hit;
				if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
					
					return hit.position;
				}
			}

			return Vector3.zero;
		}
	}
}