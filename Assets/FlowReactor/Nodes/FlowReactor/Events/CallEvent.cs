using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Editor;
using FlowReactor.EventSystem;
using FlowReactor.OrderedDictionary;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Events" , "Call a FlowReactor event" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class CallEvent : Node
	{
		[HideInInspector]
		public EventBoard eventBoard;
		[HideInInspector]
		public OrderedDictionary<Guid, FRVariable> parameters = new OrderedDictionary<Guid, FRVariable>();
		
		[HideInInspector]
		[SerializeField]
		int selectedEB;
		[HideInInspector]
		[SerializeField]
		int lastSelectedEB;
		[HideInInspector]
		[SerializeField]
		string selectedEventID = "";
		[HideInInspector]
		[SerializeField]
		string lastSelectedEventID;
		[HideInInspector]
		[SerializeField]
		int selectedEventIDInt;
	
		
		GameObject testObject;
		#if UNITY_EDITOR
		EventBoardEditor editor;
		PopUps.GenericStringPopup eventBoardPopup;
		#endif
		Dictionary<string, System.Type> variableSceneTypes = new Dictionary<string, System.Type>();
		
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("eventIcon.png");
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			
			// Automatically subscribe to first event in first eventboard
			if (rootGraph.eventboards.Count > 0)
			{
			
				var eb = rootGraph.eventboards.Keys.Select(x => x.ToString()).ToArray();
				eventBoard = rootGraph.eventboards[Guid.Parse(eb[0])].eventboard;
				if (eventBoard.events != null && eventBoard.events.Count > 0)
				{
					var _ebKeys = eventBoard.events.Keys.Select(x => x.ToString()).ToArray();
					lastSelectedEventID = _ebKeys[0];
					selectedEventID = _ebKeys[0];
					lastSelectedEB = 0;
					eventBoard.events[Guid.Parse(_ebKeys[0])].RegisterCallEventNode(this);
				}
			}
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		public override void OnDelete()
		{	
			if (eventBoard == null)
				return;
				
			try{
				if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
				{
					eventBoard.events[Guid.Parse(selectedEventID)].DeRegisterCallEventNode(this);
				}
			}catch{}
		}
	
	
	
		public override void DrawCustomInspector()
		{
			if (rootGraph.eventboards.Count == 0)
			{
				EditorGUILayout.HelpBox("No eventboard available in this graph", MessageType.Info);
				return;
			}
			
			
			var eb = rootGraph.eventboards.Keys.Select(x => x.ToString()).ToArray();

    
			if (lastSelectedEB != selectedEB)
			{
				RegisterNode();
			}
			
			
			var options = new string[rootGraph.eventboards.Keys.Count];
			var index = 0;
			foreach (var board in rootGraph.eventboards.Keys)
			{
				if (rootGraph.eventboards[board].eventboard == null)
				{
					options[index] = "empty";
				}
				else
				{
					options[index] = rootGraph.eventboards[board].eventboard.name;
				}
				
				index ++;
			}
			
			if (eventBoardPopup == null)
			{
				eventBoardPopup = new PopUps.GenericStringPopup(options, out selectedEB);
			}
			
			EditorGUI.BeginChangeCheck();
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				GUILayout.Label("Eventboard:");
				
				//selectedEB = EditorGUILayout.Popup(selectedEB, ebNames, EditorStyles.popup);
				
				var _rect = GUILayoutUtility.GetLastRect();
				if (GUILayout.Button(options[selectedEB]))
				{
				
					PopupWindow.Show(_rect, eventBoardPopup);	
				}	
			}
			
			selectedEB = eventBoardPopup._selected;
		
			if (eb == null || eb.Length == 0)
			{
				EditorGUILayout.HelpBox("Create or assign a new eventboard", MessageType.Info);
				return;
			}
			
			
			if (selectedEB < eb.Length)
			{
				if (rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard != null)
				{
					
					if (editor == null || rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard != eventBoard)
					{
						eventBoard = rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard;
					
						editor = UnityEditor.Editor.CreateEditor(eventBoard) as EventBoardEditor;
					}
				
					if (editor == null)
						return;
						
				
					editor.DrawNodeInspector(eventBoard, selectedEventID, out selectedEventID, out selectedEventIDInt);
				}
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				RegisterNode();
			}
			
			
			// EVENT PARAMETERS
			///////////////////
			
			if (eventBoard != null && eventBoard.events.Keys.Count > 0)
			{
				GUILayout.Label("Send event parameters:");
				
				Guid _id = Guid.Empty;
	
				if (Guid.TryParse(selectedEventID, out _id))
				{
		
					if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
					{
						foreach(var ep in eventBoard.events[Guid.Parse(selectedEventID)].parameters.Keys)
						{
							var _exists = false;
							foreach(var p in parameters.Keys)
							{
								if (p == ep)
								{
									_exists = true;
								}
							}
							if (!_exists)
							{
								Refresh();
							}
						}
						
						var _parameterKeyList = parameters.Keys.ToList();
						foreach(var p in _parameterKeyList)
						{
							var _exists = false;
							foreach(var ep in eventBoard.events[Guid.Parse(selectedEventID)].parameters.Keys)
							{
								if (p == ep)
								{
									_exists = true;
								}
							}
							if (!_exists)
							{
								parameters.Remove(p);
							}
						}
						
						foreach(var p in parameters.Keys)
						{
							using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
							{
								//GUILayout.Label(parameters[p].name);
								FRVariableGUIUtility.DrawVariable(parameters[p].name, parameters[p], this, false, editorSkin);
							}
						}
				
					}
				}
			
			}
		}
		
		
		void RegisterNode()
		{
			var eb = rootGraph.eventboards.Keys.Select(x => x.ToString()).ToArray();
			if (lastSelectedEB < eb.Length)
			{
				if (!string.IsNullOrEmpty(lastSelectedEventID))
				{
					if (rootGraph.eventboards.ContainsKey(Guid.Parse(eb[lastSelectedEB])) && rootGraph.eventboards[Guid.Parse(eb[lastSelectedEB])].eventboard.events.ContainsKey(Guid.Parse(lastSelectedEventID)))
					{
						rootGraph.eventboards[Guid.Parse(eb[lastSelectedEB])].eventboard.events[Guid.Parse(lastSelectedEventID)].DeRegisterCallEventNode(this);
					}
				}
			}
				
			if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
			{
				eventBoard.events[Guid.Parse(selectedEventID)].RegisterCallEventNode(this);
			}
			lastSelectedEventID = selectedEventID;
			lastSelectedEB = selectedEB;
		}
	
		void Refresh()
		{

			lastSelectedEventID = selectedEventID;

			
			// rebuild parameters
			foreach(var ep in eventBoard.events[Guid.Parse(selectedEventID)].parameters.Keys)
			{
				
				var _exists = false;
				foreach(var p in parameters.Keys)
				{
					if (p == ep)
					{
						_exists = true;
					}
				
				}
				
				if (!_exists)
				{
					var _v = (FRVariable)Activator.CreateInstance(eventBoard.events[Guid.Parse(selectedEventID)].parameters[ep].GetType());
					_v.name = eventBoard.events[Guid.Parse(selectedEventID)].parameters[ep].name;
					//Debug.Log(p.ToString());
					parameters.Add(ep, _v);
					
				}
			}
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
			{

				eventBoard.events[Guid.Parse(selectedEventID)].Raise(parameters);

			}
				
	
			ExecuteNext(0, _flowReactor);
		}
		
		public override void AssignNewGraphInstance(Graph _rootGraph, Graph _graphOwner)
		{
			foreach (var key in parameters.Keys)
			{
				parameters[key].graph = _graphOwner;
			}
		}
	}
}