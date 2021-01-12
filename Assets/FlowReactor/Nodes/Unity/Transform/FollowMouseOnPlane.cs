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
	[NodeAttributes( "Unity/Transform" , "Object follows the mouse based on plane" , "actionNodeColor" , 1 )]
	public class FollowMouseOnPlane : Node
	{
	
		public FRGameObject cameraObject;
		public FRGameObject target;
		[HelpBox("Default is XZ. Only set to true if you want YZ direction", HelpBox.MessageType.info)]
		public FRBoolean planeYZDirection;
		public FRVector3 planeOffset;
		
		UnityEngine.Camera camera;

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
		
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			camera = cameraObject.Value.GetComponent<UnityEngine.Camera>();
		}
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			Ray _cameraRay = camera.ScreenPointToRay(Input.mousePosition);
			
			// default is XZ
			Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);;
			if (planeYZDirection.Value)
			{
				_groundPlane = new Plane(Vector3.right, Vector3.zero);
			}
			
			_groundPlane.Translate(-planeOffset.Value);
			
			Vector3 _targetPoint = Vector3.zero;
			float _rayLength;
			
			if (_groundPlane.Raycast(_cameraRay, out _rayLength))
			{
				_targetPoint = _cameraRay.GetPoint(_rayLength);
			}
			
			target.Value.transform.position = _targetPoint;
			
			
			ExecuteNext(0, _flowReactor);
		}
	}
}