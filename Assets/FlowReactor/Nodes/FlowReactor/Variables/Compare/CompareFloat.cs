using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Compare", "CompareFloat", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class CompareFloat : Node
	{
		
		public FRFloat compare;
		public FRFloat with;
		
		string[] compareWith = new string[]{"=", "!=", "<", ">"};
		
		[SerializeField]
		int selectedCompare = 0;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("debugIcon.png");
			
			disableDefaultInspector = true;

			outputNodes[0].id = "True";
			outputNodes[1].id = "False";
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Compare method:");
			
			
			using (new GUILayout.HorizontalScope("Box"))
			{
				GUILayout.Label(compare.Value.ToString());
				selectedCompare = EditorGUILayout.Popup(selectedCompare, compareWith, GUILayout.Width(100));
				GUILayout.Label(with.Value.ToString());
			}
		
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			switch (selectedCompare)
			{
				case 0:
				
					if (IsFloatEqual(compare.Value, with.Value))
					{
						ExecuteNext(0, _flowReactor);
					}
					else
					{
						ExecuteNext(1, _flowReactor);
					}
					break;
				case 1:
					if (!IsFloatEqual(compare.Value, with.Value))
					{
						ExecuteNext(0, _flowReactor);
					}
					else
					{
						ExecuteNext(1, _flowReactor);
					}
					break;
				case 2:
					if (compare.Value < with.Value)
					{
						ExecuteNext(0, _flowReactor);
					}
					else
					{
						ExecuteNext(1, _flowReactor);
					}
					break;
				case 3:
					if (compare.Value > with.Value)
					{
						ExecuteNext(0, _flowReactor);
					}
					else
					{
						ExecuteNext(1, _flowReactor);
					}
					break;
			}
		}
		
		bool IsFloatEqual(float _a, float _b)
		{
			return Mathf.Approximately(_a, _b);
		}
	}
}