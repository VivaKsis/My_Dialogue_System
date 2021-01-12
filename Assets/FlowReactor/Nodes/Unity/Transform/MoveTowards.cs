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
	[NodeAttributes( "Unity/Transform" , "Use the MoveTowards node to move an object at the current position toward the target position. Use a repeat node to move the target smoothly" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class MoveTowards : Node
	{
		public FRGameObject gameObject;
		public FRGameObject targetObject;
		public FRVector3 targetVector3;


		[Title("")]
		public FRFloat speed;

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
			// Move gameobject position a step closer to the target.
			float step =  speed.Value * Time.deltaTime; // calculate distance to move
			
			Vector3 target = Vector3.zero;
			if (targetObject.Value == null)
			{
				target = targetVector3.Value;
			}
			else
			{
				target = targetObject.Value.transform.position;
			}
			
			gameObject.Value.transform.position = Vector3.MoveTowards(gameObject.Value.transform.position, target, step);

		
			ExecuteNext(0, _flowReactor);
		}
	}
}