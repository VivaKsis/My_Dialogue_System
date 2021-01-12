using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/AI/Agent" , "Sets a destination from a waypoint game objects list" , "actionNodeColor" , 2 )]
	public class SetAgentWaypointDestination : Node
	{
		public FRNavMeshAgent agent;
		[HideInNode]
		public FRGameObjectList waypoints;
		
		[Title("Store current waypoint")]
		[HideInNode]
		public FRGameObject currentWapoint;
		
		int waypointIndex = -1;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			icon = EditorHelpers.LoadIcon("waypointsIcon.png");
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "next";
			outputNodes[1].id = "end reached";
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		
		// Reset waypoint index
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			waypointIndex = -1;
		}
		
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			if (waypointIndex + 1 < waypoints.Value.Count)
			{
				waypointIndex ++;
				
				currentWapoint.Value = waypoints.Value[waypointIndex];
				agent.Value.SetDestination(waypoints.Value[waypointIndex].transform.position);
				
				ExecuteNext(0, _flowReactor);
			}
			else
			{
			
				waypointIndex = 0;
				
				currentWapoint.Value = waypoints.Value[waypointIndex];
				agent.Value.SetDestination(waypoints.Value[waypointIndex].transform.position);

				
				ExecuteNext(1, _flowReactor);
			}
			
			
		}
	}
}