//---------------------------------------------------------------------------------
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//	FLOWREACTOR - Eventboard
//
//	Eventboard scriptable object class.
//	Stores all events
//
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FlowReactor.OdinSerializer;
using FlowReactor.OrderedDictionary;
using FlowReactor.Nodes;

namespace FlowReactor.EventSystem
{
	[CreateAssetMenu(fileName = "EventBoard", menuName = "FlowReactor/New Eventboard", order = 1)]
	public class EventBoard : SerializedScriptableObject
	{
	
		[System.Serializable]
		public class Event
		{
			public string name;
			public bool foldout;
			public bool connectedNodeFoldout;
			
			[System.NonSerialized]
			public List<Node> eventListeners = new List<Node>();
			
			public List<FlowReactorEventListener> eventComponentListeners = new List<FlowReactorEventListener>();
			public OrderedDictionary<Guid, FRVariable> parameters = new OrderedDictionary<Guid, FRVariable>();
			
			
			// Keep track of all call event and event listener nodes
			// we keep that separate from the actual event listener nodes which will register during runtime	
			public List<Node> callEventNodes = new List<Node>();
			public List<Node> listenerEventNodes = new List<Node>();
			
			
			public void RaiseByScript()
			{
				if (eventListeners != null)
				{
					for (int i = 0; i < eventListeners.Count; i ++)
					{
						var eventNode = eventListeners[i] as EventListener;
						//Debug.Log("event " + eventNode." raised ");
						eventNode.OnEventRaised(new OrderedDictionary<Guid, FRVariable>());
					}
				}
				
				if (eventComponentListeners != null)
				{
					for (int i = 0; i < eventComponentListeners.Count; i ++)
					{
						var eventNode = eventComponentListeners[i] as FlowReactorEventListener;
						if (eventNode != null)
						{
							eventNode.OnEventRaised();
						}
					}
				}
			}

			
			public void Raise(OrderedDictionary<Guid, FRVariable> _params)
			{
				
			
				if (eventListeners != null)
				{
					for (int i = 0; i < eventListeners.Count; i ++)
					{
						var eventNode = eventListeners[i] as EventListener;
					
						if (_params == null)
						{
							_params = new OrderedDictionary<Guid, FRVariable>();
						}
						//Debug.Log("event " + eventNode + " raised ");
						eventNode.OnEventRaised(_params);
					}
				}
				
				if (eventComponentListeners != null)
				{
					for (int i = 0; i < eventComponentListeners.Count; i ++)
					{
						var eventNode = eventComponentListeners[i] as FlowReactorEventListener;
						if (eventNode != null)
						{
							eventNode.OnEventRaised();
						}
					}
				}
			}
			
		
			
			public void Register (Node listener)
			{
				if (eventListeners == null)
				{
					eventListeners = new List<Node>();	
				}
				
				if (!eventListeners.Contains(listener))
				{
					eventListeners.Add(listener);
				}
			}
			
			public void DeRegister (Node listener)
			{
				if (eventListeners == null)
					return;
				if (eventListeners.Contains(listener))
				{
					eventListeners.Remove(listener);
				}
			}
			
			public void RegisterComponent(FlowReactorEventListener _listener)
			{
				if (eventComponentListeners == null)
				{
					eventComponentListeners = new List<FlowReactorEventListener>();	
				}
				
				if (!eventComponentListeners.Contains(_listener))
				{
					eventComponentListeners.Add(_listener);
				}
			}
			
			public void DeRegisterComponent(FlowReactorEventListener _listener)
			{
				if (eventComponentListeners == null)
					return;
				if (eventComponentListeners.Contains(_listener))
				{
					eventComponentListeners.Remove(_listener);
				}
			}
			
			// Editor only
			public void RegisterCallEventNode(Node _node)
			{
				if (callEventNodes == null)
				{
					callEventNodes = new List<Node>();
				}
				
				if (!callEventNodes.Contains(_node))
				{
					callEventNodes.Add(_node);
				}
			}
			
			public void DeRegisterCallEventNode(Node _node)
			{
				if (callEventNodes == null)
				{
					callEventNodes = new List<Node>();
				}
				
				if (callEventNodes.Contains(_node))
				{
					callEventNodes.Remove(_node);
				}
			}
			
			public void RegisterEventListenerNode(Node _node)
			{
				if (listenerEventNodes == null)
				{
					listenerEventNodes = new List<Node>();	
				}
				
				if (!listenerEventNodes.Contains(_node))
				{
					listenerEventNodes.Add(_node);
				}
			}
			
			public void DeRegisterEventListenerNode(Node _node)
			{
				if (listenerEventNodes == null)
				{
					listenerEventNodes = new List<Node>();
				}
				
				if (listenerEventNodes.Contains(_node))
				{
					listenerEventNodes.Remove(_node);
				}
			}
			
			
			public Event(string _name)
			{
				name = _name;	
			}
			
			public Event(){}
		}
		
		
		public OrderedDictionary<Guid, Event> events = new OrderedDictionary<Guid, Event>();
		
		public void DisconnectNodes(Graph _graph)
		{
			foreach(var e in events.Keys)
			{
				for (int c = 0; c < events[e].callEventNodes.Count; c ++)
				{
					if (events[e].callEventNodes[c].graphOwner = _graph)
					{
						events[e].callEventNodes.RemoveAt(c);
					}
				}
				
				for (int c = 0; c < events[e].listenerEventNodes.Count; c ++)
				{
					if (events[e].listenerEventNodes[c].graphOwner = _graph)
					{
						events[e].listenerEventNodes.RemoveAt(c);
					}
				}
			}
		}
		
		public void AddNewEvent(string _name)
		{
			events.Add(System.Guid.NewGuid(), new Event(_name));
		}
		
		
		/// <summary>
		/// Call an event on this Eventboard.
		/// Make sure event names are unique.
		/// </summary>
		/// <param name="eventName"></param>
		public void CallEventByName(string eventName)
		{
			foreach (var e in events)
			{
				if (e.Value.name == eventName)
				{
					e.Value.RaiseByScript();
				}
			}
		}
	}
}
