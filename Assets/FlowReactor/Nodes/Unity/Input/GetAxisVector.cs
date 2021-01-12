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
	[NodeAttributes( "Unity/Input" , "Gets a world position vector from two input axis." , "actionNodeColor" , 1 )]
	public class GetAxisVector : Node
	{
		[Title("Get Axis:")]
		public FRString horizontalAxis;
		public FRString verticalAxis;
		
		[Title("Magnitude multiplier")]
		public FRFloat multiplier = new FRFloat(1f);
		
		[Title("Store result:")]
		public FRVector3 setVector;
		public FRFloat setMagnitude;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
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
			var _x = Input.GetAxis(horizontalAxis.Value);
			var _z = Input.GetAxis(verticalAxis.Value);
		
			setVector.Value = new Vector3(_x, 0, _z);
			
			float mag = Mathf.Clamp01(new Vector2(Input.GetAxis(horizontalAxis.Value), Input.GetAxis(verticalAxis.Value)).magnitude);
			setMagnitude.Value = mag * multiplier.Value;
			
			ExecuteNext(0, _flowReactor);
		}
	}
}