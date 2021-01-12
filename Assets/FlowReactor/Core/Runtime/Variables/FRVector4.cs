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
	[FRVariableAttribute(Name = "Vector4")]
	public class FRVector4 : FRVariable
	{
		[SerializeField]
		private Vector4 _vector4;
		[SerializeField]
		public Vector4 Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector4>(variableGuid);
								}
							}
						}
				
						if (_ov == null)
						{
							var _f = assignedBlackboard.GetData<FRVector4>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<Vector4Type>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._vector4;;
									#endif
								}
								else
								{
									return _f._vector4;
								}
							}
							else
							{
								return _vector4;
							}
						}
						else
						{
							var _rov = _ov as FRVector4;
							return _rov._vector4;
						}
						
					case VariableType.local:
					
						return _vector4;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector4>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector4;
							return _rExpVar._vector4;
						}
						else
						{
							return _vector4;
						}

					default:
						return _vector4;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector4>(variableGuid);
								}
							}
						}
					
						if (_ov == null)
						{
							var _f = assignedBlackboard.GetData<FRVector4>(Guid.Parse(variableGuid));
							
							if (_f._vector4 == value){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<Vector4Type>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_f._vector4 = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRVector4;
							
							if (_fov._vector4 == value){return;}
							
							_fov._vector4 = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						
						break;
					case VariableType.local:
					
						if (value == _vector4){return;}
						
						_vector4 = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector4>(exposedNodeName, exposedName);
								}
							}
						}
							
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector4;
							_rExpVar._vector4 = value;
						}
						else
						{
							_vector4 = value;
						}
							
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRVector4()
		{
			_vector4 = Vector4.zero;
		}
		
		public FRVector4(Vector4 _v)
		{
			_vector4 = _v;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.Vector4Field("", Value);
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.Vector4Field(rect, "", Value);
			#endif
		}
	}
}