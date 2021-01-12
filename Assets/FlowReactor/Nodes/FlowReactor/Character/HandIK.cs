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
	[NodeAttributes( "FlowReactor/Character" , "Sets an animator hand bone to an IK target. Use it with LateRepeat node" , "actionNodeColor" , 1 )]
	public class HandIK : Node
	{
		public FRAnimator animator;
		
		[Title("Left hand")]
		public FRFloat leftHandWeight;
		public FRGameObject leftHandTarget;
		public FRVector3 leftHandRotationOffset;
		
		[Title("Right hand")]
		public FRFloat rightHandWeight;
		public FRGameObject rightHandTarget;
		public FRVector3 rightHandRotationOffset;
		
		float currentStartTime;
		bool isActive = false;
		
		Transform leftHand;
		Transform rightHand;
	
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
			if (animator.Value == null)
				return;
				
			leftHand = animator.Value.GetBoneTransform(HumanBodyBones.LeftHand);
			rightHand = animator.Value.GetBoneTransform(HumanBodyBones.RightHand);
			
			var _ik = animator.Value.transform.gameObject.GetComponent<AnimatorIKUpdate>();
			if (_ik == null)
			{
				animator.Value.transform.gameObject.AddComponent<AnimatorIKUpdate>().Register(this);
			}	
			else
			{
				_ik.Register(this);
			}
		}
		
		// Gets called by the Animator IK Update script 
		public void OnIKUpdate()
		{
			if (!isActive)
				return;
				
			if (leftHandWeight.Value > 0)
			{
				animator.Value.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight.Value);
				animator.Value.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.Value.transform.position); 
			}
			if (rightHandWeight.Value > 0)
			{
				animator.Value.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight.Value);
				animator.Value.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.Value.transform.position);   
			}
		}

		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			currentStartTime = Time.time;
			isActive = true;
		
		
			if (leftHandWeight.Value > 0f)
			{
				leftHand.rotation = leftHand.rotation * Quaternion.Euler(leftHandRotationOffset.Value);
			}
			
			if (rightHandWeight.Value > 0f)
			{
				rightHand.rotation = rightHand.rotation * Quaternion.Euler(rightHandRotationOffset.Value);
			}

			
			ExecuteNext(0, _flowReactor);
		}
		
		
		public override void OnUpdate(FlowReactorComponent _flowReactor)
		{
			if (!graphOwner.isActive)
			{
				isActive = false;
				return;
			}
			
			if (isActive)
			{
				if (Time.time > currentStartTime + 0.1f)
				{
					isActive = false;
				}
			}
		}
	}
}