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
	[NodeAttributes( "Unity/GameObject" , "Instantiates a gameobject at specified position" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class Instantiate : Node
	{
		[Title("Instantiate:")]
		public FRGameObject gameObject;
		[Title("Spawn settings:")]
		[HelpBox("If spawnPoint game object is null, then the node will use the spawnPointVector instead.", HelpBox.MessageType.info)]
		public FRGameObject spawnPoint;
		public FRVector3 spawnPointVector;
		public FRVector3 spawnRotationVector;
		[HelpBox("If true, rotation of instantiated object will be set to 0,0,0,0", HelpBox.MessageType.info)]
		public FRBoolean quaternionIdentity;
		
		[Title("Store to:")]
		public FRGameObject result;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("duplicateIcon.png");
	
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
			if (spawnPoint.Value != null)
			{
				result.Value = Instantiate(gameObject.Value, spawnPoint.Value.transform.position, quaternionIdentity.Value ? Quaternion.identity : spawnPoint.Value.transform.rotation);
			}
			else
			{
				result.Value = Instantiate(gameObject.Value, spawnPointVector.Value,  quaternionIdentity.Value ? Quaternion.identity : Quaternion.Euler(new Vector3(spawnRotationVector.Value.x, spawnRotationVector.Value.y, spawnRotationVector.Value.z)));
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
