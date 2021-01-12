//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.AI;

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "NavMeshAgent")]
	public class FRNavMeshAgent : FRVariable
	{
		[SerializeField]
		private NavMeshAgent _agent;
		[SerializeField]
		public  NavMeshAgent Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRNavMeshAgent>(variableGuid);
								}
							}
						}

						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRNavMeshAgent>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as NavMeshAgent;
									#else
									return _f._agent;
									#endif
								}
								else
								{
									return _f._agent;
								}
							}
							else
							{
								return _agent;
							}
						}
						else
						{
							var _rov = _ov as FRNavMeshAgent;
							return _rov._agent;
						}
					case VariableType.local:
					
						return _agent;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRNavMeshAgent>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRNavMeshAgent;
							return _rExpVar._agent;
						}
						else
						{
							return _agent;
						}
						
					default:
						return _agent;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRNavMeshAgent>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRNavMeshAgent>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._agent == value){return;}
							
							_f._agent = value; 
			
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRNavMeshAgent;
							
							if (_fov._agent == value){return;}
							
							_fov._agent = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						
						break;
					case VariableType.local:
					
						if (value == _agent){return;}
						
						_agent = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRNavMeshAgent>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRNavMeshAgent;
							_rExpVar._agent = value;
						}
						else
						{
							_agent = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRNavMeshAgent(){}
		
		public FRNavMeshAgent (NavMeshAgent _a)
		{
			_agent = _a;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(NavMeshAgent), _allowSceneObjects) as NavMeshAgent;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(NavMeshAgent), false) as NavMeshAgent;
			#endif
		}
	}
}
