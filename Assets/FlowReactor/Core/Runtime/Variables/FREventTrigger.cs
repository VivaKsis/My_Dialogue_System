//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "Event Trigger")]
	public class FREventTrigger : FRVariable
	{
	
		[SerializeField]
		private EventTrigger _et;
		[SerializeField]
		public  EventTrigger Value
		{
			get
			{
				switch (type)
				{
					case VariableType.blackboard:
					
						FRVariable _ov = null;
							
						// check if this variable has a scene override
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FREventTrigger>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FREventTrigger>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as EventTrigger;
									#else
									return _f._et;
									#endif
								}
								else
								{
									return _f._et;
								}
							}
							else
							{
								return _et;
							}
						}
						else
						{
							var _rov = _ov as FREventTrigger;
							return _rov._et;
						}
		
					case VariableType.local:
					
						return _et;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FREventTrigger>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FREventTrigger;
							return _rExpVar._et;
						}
						else
						{
							return _et;
						}
						
					default:
						return _et;
				}
			}
			set
			{
				switch (type)
				{
					case VariableType.blackboard:
					
						FRVariable _ov = null;
						
						// check if this variable has a scene override
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FREventTrigger>(variableGuid);
								}
							}
						}
					
	
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FREventTrigger>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._et == value){return;}
							
							_f._et = value; 
			
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FREventTrigger;
							
							if (_fov._et == value){return;}
							
							_fov._et = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
	
						break;
					case VariableType.local:
					
						if (value == _et){return;}
						
						_et = value;
						
						if (OnValueChanged != null){ OnValueChanged(this);}
					
						break;
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FREventTrigger>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FREventTrigger;
							_rExpVar._et = value;
						}
						else
						{
							_et = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FREventTrigger (){}
		
		public FREventTrigger (EventTrigger _e)
		{
			_et = _e;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = (EventTrigger)EditorGUILayout.ObjectField(Value, typeof(EventTrigger), _allowSceneObjects);
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = (EventTrigger)EditorGUI.ObjectField(rect, Value, typeof(EventTrigger), false);
			#endif
		}
	
		
	}
}