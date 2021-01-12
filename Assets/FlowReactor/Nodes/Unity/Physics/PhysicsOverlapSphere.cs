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
	[NodeAttributes( "Unity/Physics" , "Returns an array with all colliders touching or inside the sphere." , "actionNodeColor" , 1 )]
	public class PhysicsOverlapSphere : Node
	{
		
		public FRLayerMask layerMask;
		public FRGameObject position;
		public FRFloat radius;
		
		[Title("Store objects to:")]
		[HideInNode]
		public FRGameObjectList returnObjects;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("overlapSphereIcon.png");

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
			var _colliders = Physics.OverlapSphere(position.Value.transform.position, radius.Value, layerMask.Value);
			
			returnObjects.New();
			
			List<GameObject> _tmpObjects = new List<GameObject>();
			for (int c = 0; c < _colliders.Length; c ++)
			{
				_tmpObjects.Add(_colliders[c].gameObject);
			}
			
			returnObjects.Value = _tmpObjects;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}