//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	Event listener component which can be used on a game object in the scene 
	to listen to a selected eventboard event from FlowReactor
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using FlowReactor;

namespace FlowReactor.EventSystem
{
	[AddComponentMenu("FlowReactor/FlowReactorEventListener")]
	public class FlowReactorEventListener : MonoBehaviour
	{
		public EventBoard eventBoard;
		public UnityEvent unityEvent;
		
	
		public string selectedEventID;
		public int selectedEventIDInt;
		
		void OnEnable()
		{
			if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
			{
				eventBoard.events[Guid.Parse(selectedEventID)].RegisterComponent(this);
			}
		}
		
		void OnDisable()
		{
			if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
			{
				eventBoard.events[Guid.Parse(selectedEventID)].DeRegisterComponent(this);
			}
		}
		
		public void OnEventRaised()
		{
			unityEvent.Invoke();
		}
	}
}