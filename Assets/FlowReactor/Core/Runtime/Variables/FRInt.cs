//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "Int")]
	public class FRInt : FRVariable
	{
		[SerializeField]
		private int _int;
		[SerializeField]
		public  int Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRInt>(variableGuid);
								}
							}
						}
					
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRInt>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<IntType>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._int;
									#endif
								}
								else
								{
									return _f._int;
								}
							}
							else
							{
								return _int;
							}
						}
						else
						{
							var _rov = _ov as FRInt;
							return _rov._int;
						}

					case VariableType.local:
						
						return _int;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRInt>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRInt;
							return _rExpVar._int;
						}
						else
						{
							return _int;
						}
						
					default:
						return _int;
				
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRInt>(variableGuid);
								}
							}
						}
						
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRInt>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._int == value){return;}
						
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<IntType>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_f._int = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							
							var _fov = _ov as FRInt;
							
							if (_fov._int == value){return;}
							
							_fov._int = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						break;
					case VariableType.local:
					
						if (value == _int){return;}
						
						_int = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRInt>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRInt;
							_rExpVar._int = value;
						}
						else
						{
							_int = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRInt()
		{
			_int = 0;
		}
		
		public FRInt (int _i)
		{
			_int = _i;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR

			if (_attributes != null)
			{
				var _att = _attributes.FirstOrDefault(x => x.GetType() == typeof(FRIntRange));
				
				if (_att != null)
				{
					var _a = _att as FRIntRange;
					Value = EditorGUILayout.IntSlider(Value, _a.min, _a.max);
				}
				else
				{
					Value = EditorGUILayout.IntField(Value);
				}
			}
			else
			{
				Value = EditorGUILayout.IntField(Value);
			}
			#endif 
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.IntField(rect, Value);
			#endif
		}

	}
	
}
