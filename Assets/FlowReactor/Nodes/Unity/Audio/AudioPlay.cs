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
	[NodeAttributes( "Unity/Audio" , "Plays the audio clip played by an audio source on a game object." , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class AudioPlay : Node
	{
		public FRGameObject audioSourceObject;
		
		[Title("Set random pitch on play")]
		public FRBoolean randomPitch;
		public FRFloat minPitch;
		public FRFloat maxPitch;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node) //, int _nodeIndex, Node _node)
		{
			base.Init(_graph, _node); // _nodeIndex, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("audioPlayIcon.png");
	
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
			if (randomPitch.Value)
			{
				audioSourceObject.Value.GetComponent<AudioSource>().pitch = Random.Range(minPitch.Value, maxPitch.Value);	
			}
			
			audioSourceObject.Value.GetComponent<AudioSource>().Play();
			
			ExecuteNext(0, _flowReactor);
		}
	}
}