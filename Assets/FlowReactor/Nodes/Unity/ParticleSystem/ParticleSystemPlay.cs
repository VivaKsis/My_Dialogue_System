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
	[NodeAttributes( "Unity/ParticleSystem" , "Play particle system" , "actionNodeColor" , 1 )]
	public class ParticleSystemPlay : Node
	{
		[Title("Particle System owner:")]
		public FRGameObject particleSystem;

		UnityEngine.ParticleSystem ps;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("particleSystemIcon.png");
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
		
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			ps = particleSystem.Value.GetComponent<UnityEngine.ParticleSystem>();
		}
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			if (!ps.isPlaying)
			{
				ps.Play();
			}
			
			ExecuteNext(0, _flowReactor);
		}
	}
}