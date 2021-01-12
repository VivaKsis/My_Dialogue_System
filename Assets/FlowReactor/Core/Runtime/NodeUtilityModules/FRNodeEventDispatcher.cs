using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.EventSystem;
using FlowReactor.OrderedDictionary;

namespace FlowReactor.NodeUtilityModules
{
	// Fire an event from a node with event parameters.
	
	// Node module attributes
	// Description:
	[NodeModule("Allows to raise FlowReactor events from this node",
	// Variable code:
	"// Eventboard reference for the event dispatcher module " +
	"\n\t\tpublic FlowReactor.EventSystem.EventBoard eventBoard;",
	// Custom inspector code:
	"GUILayout.Label(\"Eventboard:\");" + "\n\t\t\t" + "eventBoard = EditorGUILayout.ObjectField(eventBoard, typeof(FlowReactor.EventSystem.EventBoard), false) as FlowReactor.EventSystem.EventBoard;")]
	
	[System.Serializable]
	public class FRNodeEventDispatcher : FRNodeUtilityModule
	{
		
		private EventBoard eventBoard;
		
		public int selectedEB;
		public bool selectExternalEventBoard;
		
	    public FRNodeEventDispatcher() { }
	
	    public void Initialize(EventBoard eventBoard)
	    {
	        this.eventBoard = eventBoard;
		}
	
		public void RaiseEvent(EventBoard _eventboard, string eventName, params object[] parameterValues)
	    {
	        if (_eventboard == null)
	           return;

		    this.eventBoard = _eventboard;
			
	        if (string.IsNullOrWhiteSpace(eventName))
	            return;
	
	        EventBoard.Event foundEvent = FindEventByName(eventName);
	
	        if (foundEvent != null)
	        {
	            OrderedDictionary<Guid, FRVariable> eventParameters = BuildParameterList(foundEvent.parameters, parameterValues);
		        
		        if (foundEvent.parameters.Keys.Count != eventParameters.Keys.Count)
		        {
		        	
		        }
		        
		        foundEvent.Raise(eventParameters);
		        
	        }
	    }
	
	    private OrderedDictionary<Guid, FRVariable> BuildParameterList(OrderedDictionary<Guid, FRVariable> parameterDefinitions, object[] parameterValues)
	    {
	        OrderedDictionary<Guid, FRVariable> result = new OrderedDictionary<Guid, FRVariable>();
	
	        int parameterValueIndex = 0;
	        foreach (Guid key in parameterDefinitions.Keys)
	        {
	            Type parameterType  = parameterDefinitions[key].GetType();
	            FRVariable variable = (FRVariable)Activator.CreateInstance(parameterType);
	            PropertyInfo valuePropertyInfo = parameterType.GetProperty("Value");
	
	            if (valuePropertyInfo != null)
	            {
	                if (parameterValueIndex < parameterValues.Length)
	                {
	                    valuePropertyInfo.SetValue(variable, parameterValues[parameterValueIndex]);
	                }
	                else
	                {
	                    valuePropertyInfo.SetValue(variable, null);
	                }
	            }
	
	            result.Add(key, variable);
	
	            ++parameterValueIndex;
	        }
	
	        return result;
	    }
	
	    private EventBoard.Event FindEventByName(string eventName)
	    {
	        string normalizedEventName = eventName.ToUpperInvariant();
	        ICollection<EventBoard.Event> events = this.eventBoard.events?.Values;
	
	        foreach (EventBoard.Event evt in events)
	        {
	            var normalizedEventBoardEventName = evt.name?.ToUpperInvariant();
	
	            if ((normalizedEventBoardEventName != null) && normalizedEventName.Equals(normalizedEventBoardEventName))
	            {
	                return evt;
	            }
	        }
	
	        return null;
	    }
	  
	}
}