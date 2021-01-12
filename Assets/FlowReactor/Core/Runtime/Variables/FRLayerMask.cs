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
using UnityEditorInternal;
#endif

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "LayerMask")]
	public class FRLayerMask : FRVariable
	{ 
	
		[SerializeField]
		public LayerMask _layerMask;
		[SerializeField]
		public LayerMask Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRLayerMask>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRLayerMask>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								return _f._layerMask;
							}
							else
							{
								return _layerMask;
							}
						}
						else
						{
							var _rov = _ov as FRLayerMask;
							return _rov._layerMask;
						}
					case VariableType.local:
					
						return _layerMask;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRLayerMask>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRLayerMask;
							return _rExpVar._layerMask;
						}
						else
						{
							return _layerMask;
						}
						
					default:
						return _layerMask;
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
								_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRLayerMask>(variableGuid);
							}
						}
					}
				
				
					if (_ov == null || !_ov.overrideVariable)
					{
						var _f = assignedBlackboard.GetData<FRLayerMask>(Guid.Parse(variableGuid));
						
						if (_f == null){return;}
						
						if (_f._layerMask == value){return;}
						
						//if (_f.useDatabox)
						//{
						//	#if FLOWREACTOR_DATABOX
						//	assignedBlackboard.GetDataboxData<FloatType>(Guid.Parse(variableGuid)).Value = value;
						//	#endif
						//}
						//else
						//{
							_f._layerMask = value; 
						//}
						
						if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
					}
					else
					{
						var _fov = _ov as FRLayerMask;
						
						if (_fov._layerMask == value){return;}
						
						_fov._layerMask = value;
						
						if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
					}
					break;
				case VariableType.local:
				
					if (value == _layerMask){return;}
					
					_layerMask = value;
					
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
								_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRLayerMask>(exposedNodeName, exposedName);
							}
						}
					}
						
					if (_expVar != null)
					{
						var _rExpVar = _expVar as FRLayerMask;
						_rExpVar._layerMask = value;
					}
					else
					{
						_layerMask = value;
					}
						
					if (OnValueChanged != null){ OnValueChanged(this);}
					
					break;
				}
			}
		}
		
		public FRLayerMask(){	}
		 
		public FRLayerMask (LayerMask l)
		{
			_layerMask = l;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
			LayerMask tempMask = EditorGUILayout.MaskField( InternalEditorUtility.LayerMaskToConcatenatedLayersMask(Value), InternalEditorUtility.layers);
			Value = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			LayerMask tempMask = EditorGUI.MaskField(rect, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(Value), InternalEditorUtility.layers);
			Value = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
			#endif
		}
	}
}