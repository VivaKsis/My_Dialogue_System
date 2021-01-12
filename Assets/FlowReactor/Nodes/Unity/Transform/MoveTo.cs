using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Transform" , "Moves an object to a target position without using a repeat node. Executes next node when target position reached." , "coroutineNodeColor" , new string[]{"Target reached"} , NodeAttributes.NodeType.Coroutine )]
	public class MoveTo : CoroutineNode
	{
	
		[SceneObjectOnly]
		public FRGameObject gameObject;
		
		[Title("Targets")]
		[SceneObjectOnly]
		public FRGameObject targetObject;
		public FRVector3 targetVector3;

		[Title("")]
		public FRFloat speed;

		[HelpBox("The distance to the target until target is reached", HelpBox.MessageType.info)]
		public FRFloat distanceToTarget = new FRFloat(0.1f);
	
		bool targetReached = false;

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


		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}

		// Execute coroutine
		public override IEnumerator OnExecuteCoroutine(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			targetReached = false;
			
			Vector3 target = Vector3.zero;
		
			if (targetObject.Value == null)
			{
				target = targetVector3.Value;
				 
			}
			else
			{
				target = targetObject.Value.transform.position;
			}

			while (!targetReached)
			{
				// Move gameobject position a step closer to the target.
				float step =  speed.Value * Time.deltaTime; // calculate distance to move

				gameObject.Value.transform.position = Vector3.MoveTowards(gameObject.Value.transform.position, target, step);
				
				float _dist = Vector3.Distance(gameObject.Value.transform.position, target);
				if (_dist <= distanceToTarget.Value)
				{
					targetReached = true;
				}
				
				yield return null;
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}