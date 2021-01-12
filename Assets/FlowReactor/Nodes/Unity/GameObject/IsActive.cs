using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/GameObject" , "Returns the active state of a game object" , "actionNodeColor" , 2 )]
	public class IsActive : Node
	{
		public FRGameObject gameObject;
		
		public enum SelectedActiveCheck
		{
			activeSelf,
			activeInHierarchy
		}
		
		public SelectedActiveCheck selectedActiveCheck;

		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			outputNodes[0].id = "True";
			outputNodes[1].id = "False";
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 80);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
	
		public override void DrawCustomInspector()
		{
			selectedActiveCheck = (SelectedActiveCheck)EditorGUILayout.EnumPopup(selectedActiveCheck);
			
			FRVariableGUIUtility.DrawVariable(gameObject, this, false, editorSkin);
		}
		#endif

		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{

			////////////////////////
			switch (selectedActiveCheck)
			{
			case SelectedActiveCheck.activeSelf:
				if (gameObject.Value.activeSelf)
				{
					ExecuteNext(0, _flowReactor);
				}
				else
				{					
					ExecuteNext(1, _flowReactor);
				}
				break;
			case SelectedActiveCheck.activeInHierarchy:
				if (gameObject.Value.activeInHierarchy)
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
	}
}