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
	[FRVariableAttribute(Name = "Vector3")]
	public class FRVector3 : FRVariable
	{
		[SerializeField]
		private Vector3 _vector3;
		[SerializeField]
		public Vector3 Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector3>(variableGuid);
								}
							}
						}
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRVector3>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<Vector3Type>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._vector3;;
									#endif
								}
								else
								{
									return _f._vector3;
								}
							}
							else
							{
								return _vector3;
							}
						}
						else
						{
							var _rov = _ov as FRVector3;
							return _rov._vector3;
						}
					case VariableType.local:
					
						return _vector3;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector3>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector3;
							return _rExpVar._vector3;
						}
						else
						{
							return _vector3;
						}		
					
					default:
						return _vector3;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector3>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRVector3>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._vector3 == value){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<Vector3Type>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_f._vector3 = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRVector3;
							
							if (_fov._vector3 == value){return;}
							
							_fov._vector3 = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						
						break;
					case VariableType.local:
					
						if (value == _vector3){return;}
						
						_vector3 = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector3>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector3;
							_rExpVar._vector3 = value;
						}
						else
						{
							_vector3 = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRVector3()
		{
			_vector3 = Vector3.zero;
		}
		
		public FRVector3(Vector3 _v)
		{
			_vector3 = _v;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.Vector3Field("", Value);
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.Vector3Field(rect, "", Value);
			#endif
		}
	}
}