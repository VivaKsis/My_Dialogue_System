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
	[NodeAttributes( "FlowReactor/Character" , "Aim at target. Handles simple humanoid IK and player rotation. Must be used with a LateRepeat node" , "actionNodeColor" , 1 )]
	public class CharacterAim : Node
	{
		public FRAnimator animator;
		public FRGameObject player;
		
		[Title("Target settings")]
		[HelpBox("The aim object the player should look at", HelpBox.MessageType.info)]
		public FRGameObject aimTarget;
		public FRVector3 rotationOffset;
		public FRFloat smooth;
		public FRFloat clampAngle = new FRFloat(80);
		
		Transform chest;
		Quaternion newRotation;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// special case if node shouldn't have an input
			hideInput = false;
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
			chest = animator.Value.GetBoneTransform(HumanBodyBones.Chest);	
		}

		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			chest.LookAt(aimTarget.Value.transform.position);	
			chest.rotation = chest.rotation * Quaternion.Euler(rotationOffset.Value);
			var _rotationY = chest.localEulerAngles.y;
			// Clamp chest rotation
			chest.localEulerAngles = new Vector3(chest.localEulerAngles.x, ClampAngle (_rotationY, -clampAngle.Value, clampAngle.Value), chest.localEulerAngles.z);
			
			// Check if aim target is behind player
			// if so turn the player
			var heading = aimTarget.Value.transform.position - player.Value.transform.position;
			float dot = Vector3.Dot(heading, player.Value.transform.forward);
			if (dot < 0.5f) // could be 0
			{
				var _aimPos = new Vector3(aimTarget.Value.transform.position.x, player.Value.transform.position.y, aimTarget.Value.transform.position.z);
				var lookPos = _aimPos - player.Value.transform.position;
				//lookPos.y = 0;
				//var rotation = Quaternion.LookRotation(lookPos);
				//player.Value.transform.rotation = Quaternion.Slerp(player.Value.transform.rotation, rotation, Time.deltaTime * smooth.Value);
				
				
				//newRotation = Quaternion.LookRotation(lookPos.normalized);
				//player.Value.transform.rotation = Quaternion.Slerp(player.Value.transform.rotation, newRotation, Time.deltaTime * smooth.Value);
				//player.Value.transform.forward = lookPos.normalized;
				
				newRotation = Quaternion.LookRotation(lookPos.normalized);
			}
		
	 
			TurnAround(newRotation);
			
			ExecuteNext(0, _flowReactor);
		}
		
		void TurnAround(Quaternion _targetRotation)
		{
			if (Quaternion.Angle(player.Value.transform.rotation, _targetRotation) >= 0.01f)
			{
				player.Value.transform.rotation = Quaternion.Slerp(player.Value.transform.rotation, _targetRotation, Time.deltaTime * smooth.Value);
			}
		}
		
		float ClampAngle(float angle, float from, float to)
		{
			if (angle < 0f) angle = 360 + angle;
			if (angle > 180f) return Mathf.Max(angle, 360+from);
			return Mathf.Min(angle, to);
		}
	}
}