using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Variables
{
	[NodeAttributes("FlowReactor/Variables/Compare", "Check if gameobject is in list", "flowNodeColor", 2, NodeAttributes.NodeType.Normal)]
	public class CompareGameObjectInList : Node
	{
		
		public FRGameObject compare;
		public FRGameObjectList with;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("objectVarIcon.png");

			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			outputNodes[0].id = "True";
			outputNodes[1].id = "False";
			
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 85);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			bool isInList = false;
			for(int i = 0; i < with.Value.Count; i ++)
			{
				if (compare.Value.gameObject == with.Value[i].gameObject)
				{
					isInList = true;
				}
			}
			
			if (isInList)
			{
				ExecuteNext(0, _flowReactor);
			}
			else
			{
				ExecuteNext(1, _flowReactor);
			}
		}
	}
}