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
	[FRVariableAttribute(Name = "CanvasGroup")]
	public class FRCanvasGroup : FRVariable
	{
		[SerializeField]
		private CanvasGroup _canvasGroup;
		[SerializeField]
		public  CanvasGroup Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRCanvasGroup>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRCanvasGroup>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as CanvasGroup;
									#else
									return _f._canvasGroup;
									#endif
								}
								else
								{
									return _f._canvasGroup;
								}
							}
							else
							{
								return _canvasGroup;
							}
						}
						else
						{
							var _rov = _ov as FRCanvasGroup;
							return _rov._canvasGroup;
						}
				
					case VariableType.local:
					
						return _canvasGroup;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRCanvasGroup>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRCanvasGroup;
							return _rExpVar._canvasGroup;
						}
						else
						{
							return _canvasGroup;
						}
						
					default:
						return _canvasGroup;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRCanvasGroup>(variableGuid);
								}
							}
						}
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRCanvasGroup>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._canvasGroup == value){return;}
							
							if (_f.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as CanvasGroup;
								v = value;
								#endif
							}
							else
							{
								_f._canvasGroup = value; 
							}
							
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRCanvasGroup;
							
							if (_fov._canvasGroup == value){return;}
							
							_fov._canvasGroup = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
				
						break;
					case VariableType.local:
					
						if (value == _canvasGroup){return;}
						
						_canvasGroup = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRCanvasGroup>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRCanvasGroup;
							_rExpVar._canvasGroup = value;
						}
						else
						{
							_canvasGroup = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRCanvasGroup (){}
		
		public FRCanvasGroup (CanvasGroup _c)
		{
			_canvasGroup = _c;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(CanvasGroup), _allowSceneObjects) as CanvasGroup;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(CanvasGroup), false) as CanvasGroup;
			#endif
		}
	}
}