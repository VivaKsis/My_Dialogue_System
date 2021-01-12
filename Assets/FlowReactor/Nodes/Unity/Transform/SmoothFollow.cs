using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Transform", "SmoothFollow", 1, NodeAttributes.NodeType.Normal)]
	public class SmoothFollow : Node
	{
		[Title("Object")]
		public FRGameObject gameObject;
		[Title("Follow object")]
		public FRGameObject targetGameObject;
		
		[Title("Settings")]
		public FRVector3 positionOffset;
		public FRFloat distance;
		public FRFloat height;
		public FRFloat heightDamping;
		public FRFloat positionDamping;
		public FRFloat rotationDamping;
		public FRBoolean rotateX;
		public FRBoolean rotateY;
		public FRBoolean rotateZ;
		
		
		public FRFloat rotationFixedValueX;
		public FRFloat rotationFixedValueY;
		public FRFloat rotationFixedValueZ;
		
		float startTime;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("followIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif

		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			startTime = Time.time;
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			Follow();
			
			ExecuteNext(0, _flowReactor);
		}
		
		void Follow()
		{
			// LERP
			float distRotation = (Time.time - startTime) * rotationDamping.Value;
			float rotationFraction = distRotation / 1000;
			
			float distPosition = (Time.time - startTime) * positionDamping.Value;
			float positionFraction = distPosition / 1000;
			
			float distHeight = (Time.time - startTime) * heightDamping.Value;
			float heightFraction = distHeight / 1000;
			
			
			
			// Calculate the current rotation angles
			float wantedRotationAngleX = targetGameObject.Value.transform.eulerAngles.x;
			float wantedRotationAngleY = targetGameObject.Value.transform.eulerAngles.y;
			float wantedRotationAngleZ = targetGameObject.Value.transform.eulerAngles.z;
			
			float wantedHeight = targetGameObject.Value.transform.position.y + height.Value + positionOffset.Value.y;
			
			float currentRotationAngleX = gameObject.Value.transform.eulerAngles.x;
			float currentRotationAngleY = gameObject.Value.transform.eulerAngles.y;
			float currentRotationAngleZ = gameObject.Value.transform.eulerAngles.z;
			
			float currentHeight = gameObject.Value.transform.position.y;
				
			// Damp the rotation
			if (rotateX.Value)
			{
				currentRotationAngleX = Mathf.Lerp (currentRotationAngleX, wantedRotationAngleX, rotationFraction); //rotationDamping.Value * Time.deltaTime);
			}
			else
			{
				currentRotationAngleX = rotationFixedValueX.Value;
			}
			
			if (rotateY.Value)
			{
				currentRotationAngleY = Mathf.Lerp (currentRotationAngleY, wantedRotationAngleY, rotationFraction); //, rotationDamping.Value * Time.deltaTime);
			}
			else
			{
				currentRotationAngleY = rotationFixedValueY.Value;
			}
			
			if (rotateZ.Value)
			{
				currentRotationAngleZ = Mathf.Lerp (currentRotationAngleZ, wantedRotationAngleZ, rotationFraction); //, rotationDamping.Value * Time.deltaTime);
			}
			else
			{
				currentRotationAngleZ = rotationFixedValueZ.Value;
			}
			
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightFraction); //heightDamping.Value * Time.deltaTime);
				
			// Convert the angle into a rotation
			//Quaternion currentRotation = Quaternion.Euler (rotateX.Value ? currentRotationAngleX : 0, rotateY.Value ? currentRotationAngleY : 0, rotateZ.Value ? currentRotationAngleZ : 0);
			Quaternion currentRotation = Quaternion.Euler (currentRotationAngleX, currentRotationAngleY, currentRotationAngleZ);
			//Quaternion currentRotation = Quaternion.RotateTowards (rotateX.Value ? currentRotationAngleX : 0, rotateY.Value ? currentRotationAngleY : 0, rotateZ.Value ? currentRotationAngleZ : 0, rotationDamping.Value * Time.deltaTime);
			// Set the position of the gameobject on the x-z plane to:
			// distance meters behind the target
			Vector3 wantedPosition = targetGameObject.Value.transform.position + positionOffset.Value;
			wantedPosition -= currentRotation * Vector3.forward * distance.Value;
			gameObject.Value.transform.position = Vector3.Lerp(gameObject.Value.transform.position, wantedPosition, positionFraction); // positionDamping.Value * Time.deltaTime);
			
			// Set the height of the gameobject
			gameObject.Value.transform.position = new Vector3(gameObject.Value.transform.position.x, currentHeight, gameObject.Value.transform.position.z);
	     
			// Always look at the target
			//gameObject.Value.transform.LookAt (targetGameObject.Value.transform);
			
			// Better option when switching target objects during runtime
			Quaternion rotation = Quaternion.LookRotation((targetGameObject.Value.transform.position + positionOffset.Value) - gameObject.Value.transform.position);
			gameObject.Value.transform.rotation = Quaternion.Slerp(gameObject.Value.transform.rotation, rotation, rotationFraction); //Time.deltaTime * rotationDamping.Value);
			
			//gameObject.Value.transform.rotation = rotation; //Quaternion.RotateTowards (gameObject.Value.transform.rotation, rotation, rotationDamping.Value * Time.deltaTime);
	 
		}
	}
}