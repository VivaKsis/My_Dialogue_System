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
	[NodeAttributes( "Unity/Camera" , "FPS mouse look" , "actionNodeColor" , 1 )]
	public class FPSMouseLook : Node
	{
		public FRFloat sensitivityX = new FRFloat(1.5f);
		public FRFloat sensitivityY = new FRFloat(1.5f);
		public FRInt averageSteps = new FRInt(3);
	
		[Title("Apply to:")]
		public FRGameObject playerRoot;
		public FRGameObject camera;
		
		private List<float> _rotArrayX = new List<float>();
		private List<float> _rotArrayY = new List<float>();
		private float _rotAverageX;
		private float _rotAverageY;
		private float _mouseDeltaX;
		private float _mouseDeltaY;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
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
			Look();
			
			ExecuteNext(0, _flowReactor);
		}
		
		void Look()
		{
			_rotAverageX = 0f;
			_rotAverageY = 0f;
			_mouseDeltaX = 0f;
			_mouseDeltaY = 0f;
 
			_mouseDeltaX += Input.GetAxis("Mouse X") * sensitivityX.Value;
			_mouseDeltaY += Input.GetAxis("Mouse Y") * sensitivityY.Value;
 
			// Add current rot to list, at end
			_rotArrayX.Add(_mouseDeltaX);
			_rotArrayY.Add(_mouseDeltaY);
 
			// Reached max number of steps? Remove oldest from list
			if (_rotArrayX.Count >= averageSteps.Value)
				_rotArrayX.RemoveAt(0);
 
			if (_rotArrayY.Count >= averageSteps.Value)
				_rotArrayY.RemoveAt(0);
 
			// Add all of these rotations together
			for (int i_counterX = 0; i_counterX < _rotArrayX.Count; i_counterX++)
				_rotAverageX += _rotArrayX[i_counterX];
 
			for (int i_counterY = 0; i_counterY < _rotArrayY.Count; i_counterY++)
				_rotAverageY += _rotArrayY[i_counterY];
 
		
			// Get average
			_rotAverageX /= _rotArrayX.Count;
			_rotAverageY /= _rotArrayY.Count;
 
 
			// Apply
			if (playerRoot.Value != null)
			{
				playerRoot.Value.transform.Rotate(0f, _rotAverageX, 0f, Space.World);
			}
			if (camera.Value != null)
			{
				camera.Value.transform.Rotate(-_rotAverageY, 0f, 0f, Space.Self);
			}
		}
	}
	

}