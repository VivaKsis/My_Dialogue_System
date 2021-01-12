using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Transform", "Smooth look at mouse based on vector axis", 1, NodeAttributes.NodeType.Normal)]
	public class SmoothLookAtMouse : Node
	{

		[Title("Camera owner:")]
		public FRGameObject camera;
		[Title("")]
		public FRFloat sensitivity = new FRFloat(0.05f);
	
		
		public float minimumX = -360F;
		public float maximumX = 360F;
	 
		public float minimumY = -60F;
		public float maximumY = 60F;
		
		UnityEngine.Camera _camera;
		float xVelocity;
		float yVelocity;
		float rotationX;
		float rotationY;
		Quaternion originalRotation;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("lookIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			_camera = camera.Value.GetComponent<UnityEngine.Camera>();
			originalRotation = camera.Value.transform.localRotation;
		}
	
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			Look();
			
			ExecuteNext(0, _flowReactor);
		}
		
		void Look()
		{
			
			rotationX += Input.GetAxis("Mouse X") * sensitivity.Value;
			rotationY += Input.GetAxis("Mouse Y") * sensitivity.Value;
			
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			rotationY = ClampAngle(rotationY, minimumY, maximumY);

			Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
			
			camera.Value.transform.localRotation = originalRotation * xQuaternion * yQuaternion;			
		}
		
		
		float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}
	}
}