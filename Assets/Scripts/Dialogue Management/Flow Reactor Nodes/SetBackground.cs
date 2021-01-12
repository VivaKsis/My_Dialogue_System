using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Nodes
{
	[NodeAttributes("Dialogue Management", "Set collocutor's image in the selected frame", "#7300ff", new string[] { "" }, NodeAttributes.NodeType.Normal)]
	public class SetBackground : Node
	{
		[SerializeField] private Sprite _sprite;
		public Sprite Sprite => _sprite;

		#region Unity Editor

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

#endif

		#endregion

		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			GameObject canvas = ReferenceContainer.Instance.MainCanvasRectTransform.gameObject;
			Image image = canvas.AddComponent<Image>();
			image.sprite = _sprite;

			ExecuteNext(0, _flowReactor);
		}
	}
}