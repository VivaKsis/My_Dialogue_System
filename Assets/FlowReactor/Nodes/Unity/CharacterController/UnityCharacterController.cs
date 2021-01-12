using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/CharacterController", "The default Unity character controller", "actionNodeColor", 1, NodeAttributes.NodeType.Normal)]
	public class UnityCharacterController : Node
	{
		[Title("Character controller owner:")]
		public FRGameObject controller;
		
		[Title("Settings:")]
		public FRFloat horizontal;
		public FRFloat vertical;
		public FRFloat speed;	
		public FRFloat jumpHeight;	
		public FRBoolean doJump;
		public FRFloat gravity = new FRFloat(9.81f);
		public FRBoolean moveInLocalSpace;
	 
		UnityEngine.CharacterController cc;
		Vector3 velocity = Vector3.zero;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("characterControllerIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 65);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			if (controller.Value != null)
			{
				cc = controller.Value.GetComponent<UnityEngine.CharacterController>();
			}
			else
			{
				Debug.Log("Character Controller : No controller GameObject set");
			}
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			MoveCharacter();
						
			ExecuteNext(0, _flowReactor);
		}
		
		
		void MoveCharacter()
		{
			if (cc.isGrounded)
			{
				velocity = new Vector3(horizontal.Value, 0, vertical.Value).normalized;
				
				velocity *= speed.Value;
				
			
				if (doJump.Value)
				{
					velocity.y = jumpHeight.Value;
				}
			}
			
			// Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
			// when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
			// as an acceleration (ms^-2)
			velocity.y -= gravity.Value * Time.deltaTime;

			if (moveInLocalSpace.Value)
			{
				velocity = cc.transform.TransformDirection(velocity);
			}
    
			// Move the controller
			cc.Move(velocity * Time.deltaTime);

		}
	
	}
}