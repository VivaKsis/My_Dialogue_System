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
	[NodeAttributes( "Unity/Physics" , "Sets properties of a hinge joint" , "actionNodeColor" , 1 )]
	public class SetHingeJointProperties : Node
	{
		public FRGameObject hingeJointObject;
		
		[Title("Spring")]
		public FRBoolean useSpring;
		public FRFloat spring;
		public FRFloat damper;
		public FRFloat targetPosition;
		[Title("Motor")]
		public FRBoolean useMotor;
		public FRFloat targetVelocity;
		public FRFloat force;
		public FRButton freeSpin;
		[Title("Limits")]
		public FRBoolean useLimits;
		public FRFloat min;
		public FRFloat max;
		public FRFloat bounciness;
		public FRFloat bouncinessMinVelocity;
		public FRFloat constantDistance;
		[Title("-")]
		public FRFloat breakForce;
		public FRFloat breakTorque;
		public FRBoolean enableCollision;
		
		
		HingeJoint hingeJoint;


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
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			hingeJoint = hingeJointObject.Value.GetComponent<HingeJoint>();
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			
			////////////////////////
			
		
			hingeJoint.useSpring = useSpring.Value;
			
			if (useSpring.Value)
			{
				JointSpring _spring = new JointSpring();
				_spring.spring = spring.Value;
				_spring.damper = damper.Value;
				_spring.targetPosition = targetPosition.Value;
				
				hingeJoint.spring =	_spring;
			}
			
			hingeJoint.useMotor = useMotor.Value;
			
			if (useMotor.Value)
			{
				JointMotor _motor = new JointMotor();
				_motor.targetVelocity = targetVelocity.Value;
				_motor.force = force.Value;
				_motor.freeSpin = freeSpin.Value;
				
				hingeJoint.motor = _motor;
			}
			
			hingeJoint.useLimits = useLimits.Value;
			
			if (useLimits.Value)
			{
				JointLimits _limits = new JointLimits();
				_limits.min = min.Value;
				_limits.max = max.Value;
				_limits.bounciness = bounciness.Value;
				_limits.bounceMinVelocity = bouncinessMinVelocity.Value;
				_limits.contactDistance = constantDistance.Value;
				
				hingeJoint.limits = _limits;
			}
		
			
			hingeJoint.enableCollision = enableCollision.Value;
			
			
			ExecuteNext(0, _flowReactor);
		}
	}
}