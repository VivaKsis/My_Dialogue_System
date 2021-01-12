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
	[NodeAttributes( "Unity/Lights" , "Set intensity of a light" , "actionNodeColor" , 1 )]
	public class SetLightIntensity : Node
	{
		[Title("Light owner:")]
		public FRGameObject gameObject;
		public FRFloat intensity;

		Light light;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			icon = EditorHelpers.LoadIcon("lightIcon.png");

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
			if (gameObject.Value != null)
			{
				light = gameObject.Value.GetComponent<Light>();
			}
		}

		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			light.intensity = intensity.Value;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}