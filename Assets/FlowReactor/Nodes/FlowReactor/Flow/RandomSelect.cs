using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Flow", "Selects output randomly based on the output weight value.", "flowNodeColor", 1)]
	public class RandomSelect : Node
	{
	
		public List<float> weights = new List<float>();
	
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("diceIcon.png");
			
			disableDefaultInspector = true;
			disableVariableInspector = true;
			
			weights.Add(0f);
			
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
				weights.Add(0f);
				AddOutput("O");
			}
			
			for (int w = 0; w < weights.Count; w ++)
			{
				using (new GUILayout.HorizontalScope("Box"))
				{	
					weights[w] = EditorGUILayout.FloatField(weights[w]);
					outputNodes[w].id = weights[w].ToString();
					
					if (GUILayout.Button("x"))
					{
						weights.RemoveAt(w);
						
						RemoveOutput(w);
					}
				}
			}
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			var _out = GetRandomWeightedIndex();
			
			if (_out == -1)
			{
				_out = 0;
			}
			
			ExecuteNext(_out, _flowReactor);
		}
		
		public int GetRandomWeightedIndex()
		{
			if (weights == null || weights.Count == 0) return -1;
	 
			float w;
			float t = 0;
			int i;
			for (i = 0; i < weights.Count; i++)
			{
				w = weights[i];
	 
				if (float.IsPositiveInfinity(w))
				{
					return i;
				}
				else if (w >= 0f && !float.IsNaN(w))
				{
					t += weights[i];
				}
			}
	 
			float r = Random.value;
			float s = 0f;
	 
			for (i = 0; i < weights.Count; i++)
			{
				w = weights[i];
				if (float.IsNaN(w) || w <= 0f) continue;
	 
				s += w / t;
				if (s >= r) return i;
			}
	 
			return -1;
		}
	}
}