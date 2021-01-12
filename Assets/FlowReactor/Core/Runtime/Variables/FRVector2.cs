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
	[FRVariableAttribute(Name = "Vector2")]
	public class FRVector2 : FRVariable
	{
		[SerializeField]
		private Vector2 _vector2;
		[SerializeField]
		public Vector2 Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector2>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRVector2>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<Vector2Type>(Guid.Parse(variableGuid)).Value;
									#else
									return _f._vector2;
									#endif
								}
								else
								{
									return _f._vector2;
								}
							}
							else
							{
								return _vector2;
							}
						}
						else
						{
							var _rov = _ov as FRVector2;
							return _rov._vector2;
						}
					case VariableType.local:
					
						return _vector2;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector2>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector2;
							return _rExpVar._vector2;
						}
						else
						{
							return _vector2;
						}
						
						
					default:
						return _vector2;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRVector2>(variableGuid);
								}
							}
						}
						
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRVector2>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._vector2 == value){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								assignedBlackboard.GetDataboxData<Vector2Type>(Guid.Parse(variableGuid)).Value = value;
								#endif
							}
							else
							{
								_f._vector2 = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRVector2;
							
							if (_fov._vector2 == value){return;}
							
							_fov._vector2 = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						break;
					case VariableType.local:
					
						if (value == _vector2){return;}
						
						_vector2 = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRVector2>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRVector2;
							_rExpVar._vector2 = value;
						}
						else
						{
							_vector2 = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRVector2()
		{
			_vector2 = Vector2.zero;
		}
		
		public FRVector2(Vector2 _v)
		{
			_vector2 = _v;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.Vector2Field("", Value);
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.Vector2Field(rect, "", Value);
			#endif
		}
	}
}