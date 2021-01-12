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
	[NodeAttributes( "Unity/Physics" , "Casts a ray against all colliders in the scene." , "actionNodeColor" , 2 )]
	public class Raycast : Node
	{

		public FRGameObject fromGameObject;
		public FRVector3 fromVector3;

		
		public FRGameObject toGameObject;
		public FRVector3 toVector3;	
		public FRVector3 direction;
		
		public FRFloat distance;
		public FRLayerMask layerMask;

		public FRGameObject setHitGameObject;
		public FRVector3 setHitPoint;
		public FRVector3 setHitNormal;
		public FRFloat setHitDistance;
		
		public FRColor debugRayColor;
		
		
		public bool useFromGameObject;
		public bool useFromVector3;
		public bool useToGameObject;
		public bool useToVector;
		public bool useToDirection;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			icon = EditorHelpers.LoadIcon("rayIcon.png");
			
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			
			outputNodes[0].id = "Hit";
			outputNodes[1].id = "No hit";
			
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 85);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		
		public override void DrawCustomInspector()
		{
			using (new GUILayout.VerticalScope("Box"))
			{
			
				GUILayout.Label("From:", "boldLabel");
			
				useFromGameObject = GUILayout.Toggle(useFromGameObject, "GameObject");
				if (useFromGameObject)
				{
					useFromVector3 = false;
					FRVariableGUIUtility.DrawVariable(fromGameObject, this, false, editorSkin);
				}
				
				
				useFromVector3 = GUILayout.Toggle(useFromVector3, "Vector3");
				if (useFromVector3)
				{
					useFromGameObject = false;
					FRVariableGUIUtility.DrawVariable(fromVector3, this, false, editorSkin);
				}
				
				GUILayout.Label("To:", "boldLabel");
				useToGameObject = GUILayout.Toggle(useToGameObject, "GameObject");
				if(useToGameObject)
				{
					useToDirection = false;
					useToVector = false;
					FRVariableGUIUtility.DrawVariable(toGameObject, this, false, editorSkin);
				}
				
				useToVector = GUILayout.Toggle(useToVector, "Vector3");
				if (useToVector)
				{
					useToGameObject = false;
					useToDirection = false;
					FRVariableGUIUtility.DrawVariable(toVector3, this, false, editorSkin);
				}
				
				useToDirection = GUILayout.Toggle(useToDirection, "Direction");
				if (useToDirection)
				{
					useToGameObject = false;
					useToVector = false;
					FRVariableGUIUtility.DrawVariable(direction, this, false, editorSkin);
				}
				
			}

			using (new GUILayout.VerticalScope("Box"))
			{
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Distance");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(distance, this, false, editorSkin);
				}
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("LayerMask");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(layerMask, this, false, editorSkin);
				}
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Set hit results", "boldLabel");
			
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Hit GameObject");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(setHitGameObject, this, false, editorSkin);
				}
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Hit Vector3");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(setHitPoint, this, false, editorSkin);
				}
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Hit Normal");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(setHitNormal, this, false, editorSkin);
				}
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Hit Distance");
					GUILayout.FlexibleSpace();
					FRVariableGUIUtility.DrawVariable(setHitDistance, this, false, editorSkin);
				}
			}
				
			using (new GUILayout.HorizontalScope("Box"))
			{
				GUILayout.Label("Debug Color");
				GUILayout.FlexibleSpace();
				FRVariableGUIUtility.DrawVariable(debugRayColor, this, false, editorSkin);
			}
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			RaycastHit _hit;
			Vector3 _from = Vector3.zero;
			Vector3 _direction = Vector3.zero;
			
			if (useFromGameObject)
			{
				if (useToGameObject)
				{
					_from = fromGameObject.Value.transform.position;
					_direction = (toGameObject.Value.transform.position - fromGameObject.Value.transform.position);
				}
				else if (useToVector)
				{
					_from = fromGameObject.Value.transform.position;
					_direction = (toVector3.Value - fromGameObject.Value.transform.position);
				}
				else if (useToDirection)
				{
					_from = fromGameObject.Value.transform.position;
					_direction = direction.Value;
				}
			
			}
			else if (useFromVector3)
			{
				
				if (useToGameObject)
				{			
					_from = fromVector3.Value;
					_direction = (toGameObject.Value.transform.position - fromVector3.Value);
				}
				else if (useToVector)
				{
					_from = fromVector3.Value;
					_direction = (toVector3.Value - fromVector3.Value);
				}
				else if (useToDirection)
				{
					_from = fromVector3.Value;
					_direction = direction.Value;
				}
			}
			
			if (Physics.Raycast(_from, _direction, out _hit, distance.Value, layerMask.Value))
			{
				Debug.DrawRay(_from, _direction, debugRayColor.Value, .5f);
				setHitGameObject.Value = _hit.collider.gameObject;
				setHitPoint.Value = _hit.point;
				setHitNormal.Value = _hit.normal;
				setHitDistance.Value = _hit.distance;
					
				ExecuteNext(0, _flowReactor);
			}
			else
			{
				ExecuteNext(1, _flowReactor);
			}
			
		}
	}
}