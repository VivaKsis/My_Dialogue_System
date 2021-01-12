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
	[NodeAttributes( "Unity/Transform" , "Gets the rotation of a game object" , "actionNodeColor" , 1 )]
	public class GetRotation : Node
	{
		public FRGameObject gameObject;
		[Title("Store rotation to:")]
		public FRVector3 rotation;
		public FRFloat xEulerAngle;
		public FRFloat yEulerAngle;
		public FRFloat zEulerAngle;
		
		public enum WorldSpace
		{
			local,
			world
		}
		
		WorldSpace worldSpace = WorldSpace.local;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
	
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		public override void DrawCustomInspector()
		{
			worldSpace = (WorldSpace)EditorGUILayout.EnumPopup("Space", worldSpace);
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			switch (worldSpace)
			{
				case WorldSpace.local:

					rotation.Value = new Vector3(gameObject.Value.transform.localRotation.eulerAngles.x, gameObject.Value.transform.localRotation.eulerAngles.y, gameObject.Value.transform.localRotation.eulerAngles.z);
					
					xEulerAngle.Value = gameObject.Value.transform.localRotation.eulerAngles.x;
					yEulerAngle.Value = gameObject.Value.transform.localRotation.eulerAngles.y;
					zEulerAngle.Value = gameObject.Value.transform.localRotation.eulerAngles.z;
					
					break;
				case WorldSpace.world:
				
					rotation.Value = new Vector3(gameObject.Value.transform.rotation.eulerAngles.x, gameObject.Value.transform.rotation.eulerAngles.y, gameObject.Value.transform.rotation.eulerAngles.z);
					
					xEulerAngle.Value = gameObject.Value.transform.rotation.eulerAngles.x;
					yEulerAngle.Value = gameObject.Value.transform.rotation.eulerAngles.y;
					zEulerAngle.Value = gameObject.Value.transform.rotation.eulerAngles.z;
					
					
					break;
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}