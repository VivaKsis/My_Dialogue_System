using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Character", "The FlowReactor Character Controller has some advancved movement settings", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class AdvancedCharacterController : Node
	{
		public FRGameObject player;
	
		public FRFloat speed = new FRFloat(2f);
		public FRFloat horizontal;
		public FRFloat vertical;
		
		public FRBoolean doJump;
		public FRInt maxJumpsAllowed = new FRInt(2);
		public FRFloat jumpSpeed = new FRFloat(1f);
		
		public FRFloat gravity = new FRFloat(9.81f);
		
		public FRBoolean doDash;
		public FRFloat dashDistance;  
		public FRVector3 dashDrag;
		

		public FRBoolean isGrounded;
		public FRBoolean isMovingBackwards;
		public FRBoolean isMovingSideways;

	    Vector3 velocity;
		int jumpCount = 0;
		Vector3 moveDirection;
		Vector3 oldPosition;
		Collider collider;
		
		UnityEngine.CharacterController controller;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("debugIcon.png");
	
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
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			controller = player.Value.GetComponent<UnityEngine.CharacterController>();
			collider = player.Value.GetComponent<Collider>();
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			MoveCharacter();
						
			ExecuteNext(0, _flowReactor);
		}

	    void MoveCharacter()
		{

			isGrounded.Value =  Physics.CheckSphere(collider.bounds.min, 0.1f);
		
		    if (isGrounded.Value)
		    {
		    	if (velocity.y < 0)
		    	{
	            	velocity.y = 0f;
		    	}
		    	
		    	jumpCount = 0;
		    }
	
		    Vector3 move = new Vector3(horizontal.Value, 0, vertical.Value).normalized;
		    controller.Move(move * Time.deltaTime * speed.Value);

		    //Quaternion _rotate = Quaternion.identity;
		    
		    //if (move != Vector3.zero && !playerAnimator.Value.GetBool("Aim"))
		    //{
		    //	_rotate = Quaternion.LookRotation(move);
		    //	player.Value.transform.rotation = Quaternion.Slerp(player.Value.transform.rotation, _rotate, Time.deltaTime * 100);
		    //	//player.Value.transform.forward = move;
		    //}
		    
		    if (doJump.Value)
		    {
		    	if (isGrounded.Value)
		    	{
				    velocity.y += Mathf.Sqrt(jumpSpeed.Value * 2f * gravity.Value);	
		    		jumpCount ++;
		    	}
		    	else
		    	{
		    		if (jumpCount < maxJumpsAllowed.Value)
		    		{
		    			velocity.y += Mathf.Sqrt(jumpSpeed.Value * 2f * gravity.Value);	
		    			jumpCount ++;
		    		}
		    	}
		    }
	        
		    if (doDash.Value)
	        {
			    velocity += Vector3.Scale(player.Value.transform.forward, dashDistance.Value * new Vector3((Mathf.Log(1f / (Time.deltaTime * dashDrag.Value.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * dashDrag.Value.z + 1)) / -Time.deltaTime)));
	        }
	
	
		    velocity.y -= gravity.Value * Time.deltaTime;
	
		    velocity.x /= 1 + dashDrag.Value.x * Time.deltaTime;
	        velocity.y /= 1 + dashDrag.Value.y * Time.deltaTime;
	        velocity.z /= 1 + dashDrag.Value.z * Time.deltaTime;
		
		 
		    controller.Move(velocity * Time.deltaTime);
	        
			
			// Get movement direction
			var _movement = (player.Value.transform.position - oldPosition);	
			var _dot = Vector3.Dot(player.Value.transform.forward.normalized, _movement.normalized);

			if (_dot > 0.8f)
			{
				// Move forward
				isMovingBackwards.Value = false;
				isMovingSideways.Value = false;
			}
			else if (_dot < -0.8f)
			{
				// Move backward
				isMovingBackwards.Value = true;
				isMovingSideways.Value = false;
			}
			else if (_dot < 0.2f && _dot > -0.2f)
			{
				// Move Sideways
				isMovingBackwards.Value = false;
				isMovingSideways.Value = true;
			}
		}
	    
		public override void OnLateUpdate(FlowReactorComponent _flowReactor)
		{
			oldPosition = player.Value.transform.position;
		}
	}
}