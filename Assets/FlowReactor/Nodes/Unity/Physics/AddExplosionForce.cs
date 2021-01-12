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
	[NodeAttributes( "Unity/Physics" , "Applies a force to a game object that simulates explosion effects." , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class AddExplosionForce : Node
	{
		
		[Title("Add force to:")]
		public FRGameObject gameObject;
		public FRGameObjectList gameObjects;
		[Title("Force position")]
		public FRGameObject center;
		[Title("Force settings:")]
		public FRFloat radius = new FRFloat(5.0f);
		public FRFloat power = new FRFloat(10.0f);
		/*
		Applies the force as if it was applied from beneath the object. 
		This is useful since explosions that throw things up instead of pushing things to the side look cooler.
		A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.
		*/
		public FRFloat upwardsModifier = new FRFloat(2f);
		public ForceMode forceMode;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("bombIcon.png");
	
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
			Vector3 explosionPos = center.Value.transform.position;
			
			if (gameObject.Value != null)
			{
				Rigidbody rb = gameObject.Value.GetComponent<Rigidbody>();
		
				if (rb != null)
				{
					rb.AddExplosionForce(power.Value, explosionPos, radius.Value, upwardsModifier.Value, forceMode);
				}
			}
			
			if (gameObjects.Value != null)
			{
				for (int i = 0; i < gameObjects.Value.Count; i ++)
				{
					var _tmpRB = gameObjects.Value[i].gameObject.GetComponent<Rigidbody>();
					if (_tmpRB != null)
					{
						_tmpRB.AddExplosionForce(power.Value, explosionPos, radius.Value, upwardsModifier.Value, forceMode);
					}
				}
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}