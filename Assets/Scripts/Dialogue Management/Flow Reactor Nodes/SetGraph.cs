using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Nodes
{
	[NodeAttributes("Dialogue Management", "Sets scene objects into nodes' fields by id. Is executed automatically after start or by button in editor" , "actionNodeColor" , new string[]{""} , NodeAttributes.NodeType.Normal )]
	public class SetGraph : Node
	{
		[SerializeField] private Graph _graph;
		public Graph _Graph => _graph;

		private bool graphIsSet;


		private void SetGraphFields()
		{
			List<Node> nodes = _graph.nodes;
			for (int a = 0; a < nodes.Count; a++)
			{
				if (nodes[a] is SetFrame setFrame)
				{
					setFrame.SetFrameField();
				}
			}
			graphIsSet = true;
		}

		// Editor node methods
#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = false;
			disableVariableInspector = false;
			disableDrawCustomInspector = false;

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		public override void DrawCustomInspector()
        {
			if (GUILayout.Button("Set Graph"))
            {
				SetGraphFields();
			}
		}

#endif

        private void OnEnable()
        {
			graphIsSet = false;
		}
		private void OnDisable()
		{
			graphIsSet = false;
		}

		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
            if (!graphIsSet)
            {
				SetGraphFields();
			}
			ExecuteNext(0, _flowReactor);
		}
	}
}