//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
 
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "GameObject")]
	public class FRGameObject : FRVariable
	{
		[SerializeField]
		private GameObject _gameObject;
		[SerializeField]
		public  GameObject Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRGameObject>(variableGuid);
								}
							}
						}
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRGameObject>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _go = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as GameObject;
									return _go;
									#else
									return _g._gameObject;
									#endif
								}
								else
								{
									return _g._gameObject;
								}
							}
							else
							{ 
								return _gameObject;
							}
						}
						else
						{
							var _rov = _ov as FRGameObject;
							return _rov._gameObject;
						}
		
					case VariableType.local:
					
						return _gameObject;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRGameObject>(exposedNodeName, exposedName);
								}
							}
						} 
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRGameObject;
							return _rExpVar._gameObject;
						}
						else
						{
							return _gameObject;
						}
						
					default:
						return _gameObject;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRGameObject>(variableGuid);
								}
							}
						}
					
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRGameObject>(Guid.Parse(variableGuid));
							
							if (_g == null){return;}
							
							if (_g._gameObject == value){return;}
							
							if (_g.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var _v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as GameObject;
								_v = value;
								#endif
							}
							else
							{
								_g._gameObject = value;
							}
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRGameObject;
							_rov._gameObject = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
	
						break;
					case VariableType.local: 
					 
						if (value == _gameObject){return;}
							
						_gameObject = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRGameObject>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRGameObject;
							_rExpVar._gameObject = value;
						}
						else
						{
							_gameObject = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRGameObject(){}
		
		public FRGameObject(GameObject _g)
		{
			Value = _g;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(GameObject), _allowSceneObjects) as GameObject;			
			#endif
		}
	
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(GameObject), true) as GameObject;
			#endif
		}
		
	}
}