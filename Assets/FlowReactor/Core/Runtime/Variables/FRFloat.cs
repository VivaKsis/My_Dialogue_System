//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "Float")]
	public class FRFloat : FRVariable
	{ 
	
		[SerializeField]
		public float _float;
		[SerializeField]
		public float Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRFloat>(variableGuid);
								}
							}
						}
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRFloat>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<FloatType>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._float;
									#endif
								}
								else
								{
									return _f._float;
								}
							}
							else
							{
								return _float;
							}
						}
						else
						{
							var _rov = _ov as FRFloat;
							return _rov._float;
						}
	
					case VariableType.local:
					
						return _float;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRFloat>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRFloat;
							return _rExpVar._float;
						}
						else
						{
							return _float;
						}
						
					default:
						return _float;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRFloat>(variableGuid);
								}
							}
						}
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRFloat>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._float == value){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<FloatType>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_f._float = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRFloat;
							
							if (_fov._float == value){return;}
							
							_fov._float = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
	
						break;
					case VariableType.local:
						
						if (value == _float){return;}
						
						_float = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRFloat>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRFloat;
							_rExpVar._float = value;
						}
						else
						{
							_float = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRFloat()
		{
			_float = 0f;
		}
		 
		public FRFloat (float f)
		{
			_float = f;
		}
		
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR

			if (_attributes != null)
			{
				var _att = _attributes.FirstOrDefault(x => x.GetType() == typeof(FRFloatRange));
				
				if (_att != null)
				{
					var _a = _att as FRFloatRange;
					Value = EditorGUILayout.Slider(Value, _a.min, _a.max);
				}
				else
				{
					Value = EditorGUILayout.FloatField(Value);	
				}
			}
			else
			{
				Value = EditorGUILayout.FloatField(Value);	
			}
			#endif
		}
		
		public override void Draw(Rect _rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.FloatField(_rect, Value);
			#endif
		}
	}
}