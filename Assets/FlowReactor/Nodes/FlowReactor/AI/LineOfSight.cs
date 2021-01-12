using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/AI" , "Check if target object is in line of sight" , "actionNodeColor" , 2 )]
	public class LineOfSight : Node
	{
		
		public FRGameObject agent;
		public FRGameObject target;
		public FRLayerMask layerMask;
		public FRString targetLayer;
		
		public FRFloat angle;
		public FRFloat distance;
		
		enum LineOfSightOutput
		{
			InSight = 0,
			NotInSight = 1
		}

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "In sight";
			outputNodes[1].id = "Not in sight";
			
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
			Vector3 forward =  agent.Value.transform.TransformDirection(Vector3.forward);
			var _direction = (target.Value.transform.position - agent.Value.transform.position);
			//var _dot =  Vector3.Dot(forward, _direction);
			var _targetForward = agent.Value.transform.forward;
			
			
			Debug.DrawRay(agent.Value.transform.position, Quaternion.Euler(0, angle.Value, 0) * agent.Value.transform.forward * distance.Value, Color.yellow);
			Debug.DrawRay(agent.Value.transform.position, Quaternion.Euler(0, -angle.Value, 0) * agent.Value.transform.forward * distance.Value, Color.yellow);
			
			var _angle = Vector3.Angle(_direction, _targetForward);
			if (_angle < angle.Value) // && _dot > -0.5f)
			{

					RaycastHit _hit;
					if (Physics.Raycast(agent.Value.transform.position, _direction, out _hit, distance.Value, layerMask.Value))
					{
						if (_hit.collider.gameObject.layer == LayerMask.NameToLayer(targetLayer.Value))
						{
							Debug.DrawRay(agent.Value.transform.position, (target.Value.transform.position - agent.Value.transform.position), Color.green);
							ExecuteNext(LineOfSightOutput.InSight, _flowReactor);
						}
						else
						{
							ExecuteNext(LineOfSightOutput.NotInSight, _flowReactor);
						}
					}
					else
					{
						ExecuteNext(LineOfSightOutput.NotInSight, _flowReactor);
					}

			}
			else
			{
				ExecuteNext(LineOfSightOutput.NotInSight, _flowReactor);
			}
		}
	}
}