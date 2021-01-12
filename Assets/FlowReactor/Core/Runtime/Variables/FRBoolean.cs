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

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "Boolean")]
	public class FRBoolean : FRVariable
	{
	
		[SerializeField]
		private bool _bool;
		[SerializeField]
		public  bool Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRBoolean>(variableGuid);
								}
							}
						}

						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRBoolean>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<BoolType>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._bool;
									#endif
								}
								else
								{
									return _f._bool;
								}
							}
							else
							{
								return _bool;
							}
						}
						else
						{
							var _rov = _ov as FRBoolean;
							return _rov._bool;
						}
		
					case VariableType.local:
					
						return _bool;
							
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRBoolean>(exposedNodeName, exposedName);
								}
							}
						}
							
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRBoolean;
							return _rExpVar._bool;
						}
						else
						{
							return _bool;
						}
						
					default:
					
						return _bool;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRBoolean>(variableGuid);
								}
							}
						}
					
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _b = assignedBlackboard.GetData<FRBoolean>(Guid.Parse(variableGuid));
							
							if (_b == null){return;}
							
							if (_b._bool == value){return;}
							
							if (_b.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<BoolType>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_b._bool = value; 
							}
							
							if (_b.OnValueChanged != null) { _b.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRBoolean;
							
							if (_fov._bool == value){return;}
							
							_fov._bool = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}

						break;
					case VariableType.local:
						if (value == _bool){return;}
						
						_bool = value;
						
						if (OnValueChanged != null) { OnValueChanged(this);}
					
						break;
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRBoolean>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRBoolean;
							_rExpVar._bool = value;
						}
						else
						{
							_bool = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRBoolean(){}
		
		public FRBoolean(bool _b)
		{
			_bool = _b;
		}
		
		#if UNITY_EDITOR
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			Value = EditorGUILayout.Toggle(Value);
		}
		
		public override void Draw(Rect rect)
		{
			Value = EditorGUI.Toggle(rect, Value);
		}
		#endif
	}
}