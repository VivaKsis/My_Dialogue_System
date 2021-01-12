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
	[FRVariableAttribute(Name = "Material")]
	public class FRMaterial : FRVariable
	{
		[SerializeField]
		private Material _material;
		[SerializeField]
		public  Material Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRMaterial>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRMaterial>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _go = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Material;
									return _go;
									#else
									return _g._material;
									#endif
								}
								else
								{
									return _g._material;
								}
							}
							else
							{
								return _material;
							}
						}
						else
						{
							var _rov = _ov as FRMaterial;
							return _rov._material;
						}
					case VariableType.local:
					
						return _material;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRMaterial>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRMaterial;
							return _rExpVar._material;
						}
						else
						{
							return _material;
						}
						
					default:
						return _material;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRMaterial>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRMaterial>(Guid.Parse(blackboardGuid));
							
							if (_g == null){return;}
							
							if (_g._material == value){return;}
							
							if (_g.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var _v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Material;
								_v = value;
								#endif
							}
							else
							{
								_g._material = value;
							}
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRMaterial;
							_rov._material = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						break;
					case VariableType.local:
					
						if (value == _material){return;}
							
						_material = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRMaterial>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRMaterial;
							_rExpVar._material = value;
						}
						else
						{
							_material = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRMaterial(){}
		
		public FRMaterial(Material _g)
		{
			Value = _g;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(Material), _allowSceneObjects) as Material;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(Material), false) as Material;
			#endif
		}

	}
}