using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using FlowReactor;
using FlowReactor.EventSystem;
using FlowReactor.BlackboardSystem;

//
// Simple example of how to call an event by script and how to get a variable by script
//
namespace FlowReactor.Demo
{
	public class APIDemo : MonoBehaviour
	{
		public FlowReactorComponent flowReactor;
		
		public EventBoard events;
		public BlackBoard variables;
		
		string newLogMessage;
	   
		void OnGUI()
		{
			
			GUILayout.Label("Eventboard:");
			
			// Call the event named OnEvent1 on the assigned Evenboard
			if (GUILayout.Button("call event"))
			{
				events.CallEventByName("OnEvent1");
			}
			
			GUILayout.Label("Blackboard:");
			
			// Get the int variable named "health" on the assigned Blackboard
			if (GUILayout.Button("Get variable"))
			{
				var _healthVariable = variables.GetVariableByName<FRInt>("health");
				
				Debug.Log(_healthVariable.Value);
			}
			
			// Modify the health variable. No need to assign variable back to blackboard
			// as we're using a reference.
			if (GUILayout.Button("Modify variable"))
			{
				var _healthVariable = variables.GetVariableByName<FRInt>("health");
				
				_healthVariable.Value --;
				
				Debug.Log(_healthVariable.Value);
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
			
				GUILayout.Label("FlowReactor component:");
				GUILayout.Label("Get:");
				if (GUILayout.Button("Get exposed variable"))
				{
					var _stringVariable = flowReactor.GetExposedVariable<FRString>("DebugLog", "log");
					Debug.Log("Exposed variable: " + _stringVariable.Value);
				}
				
				GUILayout.Label("Set:");
				newLogMessage = GUILayout.TextField(newLogMessage);
			
				if (GUILayout.Button("Set exposed variable"))
				{
					var _stringVariable = flowReactor.GetExposedVariable<FRString>("DebugLog", "log");
					_stringVariable.Value = newLogMessage;
				}
				
				if (GUILayout.Button("Execute Debug.Log node"))
				{
					events.CallEventByName("OnEvent2");
				}
			}
			
		}
		
		// This is being called by the FlowReactorEventListener component in the scene
		public void CallMeAfterEvent()
		{
			Debug.Log("Script: event has been called");
		}
	}
}