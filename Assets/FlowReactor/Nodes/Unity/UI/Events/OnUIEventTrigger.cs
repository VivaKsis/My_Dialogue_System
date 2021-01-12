using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.EventSystems;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/UI/Events", "On UI Trigger Event", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnUIEventTrigger : Node
	{
		public FREventTrigger uiObject;
		public EventTriggerType triggerType;

		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("eventIcon.png");
			disableDefaultInspector = true;
			disableDrawCustomInspector = false;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		public override void DrawCustomInspector()
		{
			triggerType = (EventTriggerType)EditorGUILayout.EnumPopup(triggerType);
		}
		#endif
		
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			 
			if (uiObject.Value != null)
			{
				var _t = uiObject.Value.GetComponent<EventTrigger>();
						 
				entry.eventID = triggerType;
				entry.callback.AddListener( (eventData) => { DoEvent(_flowReactor); } );
				_t.triggers.Add(entry);
			}
		}
		
		
		public void DoEvent(FlowReactorComponent _flowReactor)
		{    
			if (!graphOwner.isActive)
				return;
		
			ExecuteNext(0, _flowReactor);

		}
		
	}
}
