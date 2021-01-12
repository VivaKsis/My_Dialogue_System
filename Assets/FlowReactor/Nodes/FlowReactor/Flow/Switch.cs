using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Flow", "Switch output to selected index", "flowNodeColor", 1)]
	public class Switch : Node
	{
	
		public FRInt selectedOutput;
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("randomIcon.png");
			
			disableDefaultInspector = true;
			disableVariableInspector = false;
			
			
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}

	
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Weights:");
			
			if (GUILayout.Button("Add Output"))
			{
				AddOutput("");
			}
			
			for (int o = 0; o < outputNodes.Count; o ++)
			{
				using (new GUILayout.HorizontalScope("Box"))
				{	
					outputNodes[o].id = EditorGUILayout.TextField(outputNodes[o].id);
					
					if (GUILayout.Button("x"))
					{
						RemoveOutput(o);
					}
				}
			}
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			ExecuteNext(selectedOutput.Value, _flowReactor);
		}
		
	
	}
}