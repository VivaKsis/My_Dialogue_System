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
	//[FRVariableAttribute(Name = "Generic Enum")]
	public class FREnum<T> : FRVariable where T : System.Enum
	{ 
	
		[SerializeField]
		public T _enum;
		[SerializeField]
		public T Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FREnum<T>>(variableGuid);
								}
							}
						}
						
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FREnum<T>>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									//#if FLOWREACTOR_DATABOX
									//return assignedBlackboard.GetDataboxData<FloatType>(Guid.Parse(variableGuid)).Value;
									//#else
									Debug.LogWarning("Not supported type");
									return _f._enum;
									//#endif
								}
								else
								{
									return _f._enum;
								}
							}
							else
							{
								return _enum;
							}
						}
						else
						{
							var _rov = _ov as FREnum<T>;
							return _rov._enum;
						}
	
					case VariableType.local:
					
						return _enum;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FREnum<T>>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FREnum<T>;
							return _rExpVar._enum;
						}
						else
						{
							return _enum;
						}
						
					default:
						return _enum;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FREnum<T>>(variableGuid);
								}
							}
						}
					
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FREnum<T>>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._enum.Equals(value)){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								//assignedBlackboard.GetDataboxData<FloatType>(Guid.Parse(variableGuid)).Value = value;
								Debug.LogWarning("Not supported type");
								#endif
							}
							else
							{
								_f._enum = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FREnum<T>;
							
							if (_fov._enum.Equals(value)){return;}
							
							_fov._enum = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
	
						break;
					case VariableType.local:
					
						if (value.Equals(_enum)){return;}
						
						_enum = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FREnum<T>>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FREnum<T>;
							_rExpVar._enum = value;
						}
						else
						{
							_enum = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FREnum()
		{
			Value = System.Activator.CreateInstance<T>();
			_enum = Value;
		}
		
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
	
			Value = (T)EditorGUILayout.EnumPopup(Value);

			#endif
		}
		
		public override void Draw(Rect _rect)
		{
			#if UNITY_EDITOR
			Value = (T)EditorGUI.EnumPopup(_rect, Value);
			#endif
		}
		
		// Return generic enum values as a string dictionary
		//public Dictionary<int, string> NamedValues<T>() where T : System.Enum
		//{
		//	var result = new Dictionary<int, string>();
		//	var values = Enum.GetValues(typeof(T));

		//	foreach (int item in values)
		//		result.Add(item, Enum.GetName(typeof(T), item));
		//	return result;
		//}
	}
}