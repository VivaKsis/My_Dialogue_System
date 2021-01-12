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
	[NodeAttributes( "Unity/Transform" , "Checks the distance between two game objects" , "actionNodeColor" , 2 )]
	public class CheckDistance : Node
	{
		
		[Title("From:")]
		public FRGameObject agent;
		public FRVector3 vector3;
		
		[Title("To:")]
		public FRGameObject targetObject;
		public FRVector3 targetVector;
		
		[Title("")]
		public FRFloat distance;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			icon = EditorHelpers.LoadIcon("distanceIcon.png");
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "<";
			outputNodes[1].id = ">";
			
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
			
			var _debugPos = Vector3.zero;
			if (targetObject.Value == null)
			{
				Debug.DrawRay(targetVector.Value, Vector3.up, Color.cyan, 1f);
				_debugPos = targetVector.Value;
			}
			else
			{
				Debug.DrawRay(targetObject.Value.transform.position, Vector3.up, Color.cyan, 1f);
				_debugPos = targetObject.Value.transform.position;
			}

			if (Check())
			{
				ExecuteNext(0, _flowReactor);
			}
			else
			{
				ExecuteNext(1, _flowReactor);
			}
		}
		
		bool Check()
		{
			Vector3 _offset = Vector3.zero;
			
			if (targetObject.Value != null)
			{
				if (agent.Value != null)
				{
					_offset = targetObject.Value.transform.position - agent.Value.transform.position;
				}
				else
				{
					_offset = targetObject.Value.transform.position - vector3.Value;
				}
			}
			else
			{
				if (agent.Value != null)
				{
					_offset = targetVector.Value - agent.Value.transform.position;
				}
				else
				{
					_offset = targetVector.Value - vector3.Value;
				}
			}
			
			float _sqrLen = _offset.sqrMagnitude;

			// square the distance we compare with
			if (_sqrLen < distance.Value * distance.Value)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}