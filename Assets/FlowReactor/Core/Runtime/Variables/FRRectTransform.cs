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
	[FRVariableAttribute(Name = "RectTransform")]
	public class FRRectTransform : FRVariable
	{
		[SerializeField]
		private RectTransform _rectTransform;
		[SerializeField]
		public  RectTransform Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRRectTransform>(variableGuid);
								}
							}
						}
						
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRRectTransform>(Guid.Parse(variableGuid));
							
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as RectTransform;
									#else
									return _f._rectTransform;
									#endif
								}
								else
								{
									return _f._rectTransform;
								}
							}
							else
							{
								return _rectTransform;
							}
						}
						else
						{
							var _rov = _ov as FRRectTransform;
							return _rov._rectTransform;
						}
					case VariableType.local:
					
						return _rectTransform;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRRectTransform>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRRectTransform;
							return _rExpVar._rectTransform;
						}
						else
						{
							return _rectTransform;
						}		
						
					default:
						return _rectTransform;
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
							_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRRectTransform>(variableGuid);
						}
					}
				}
				
				
					if (_ov == null || !_ov.overrideVariable)
					{
						var _f = assignedBlackboard.GetData<FRRectTransform>(Guid.Parse(variableGuid));
						
						if (_f == null){return;}
						
						if (_f._rectTransform == value){return;}
						
						if (_f.useDatabox)
						{
							#if FLOWREACTOR_DATABOX
							var v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as RectTransform;
							v = value;
							#endif
						}
						else
						{
							_f._rectTransform = value; 
						}
						
						if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
					}
					else
					{
						var _fov = _ov as FRRectTransform;
						
						if (_fov._rectTransform == value){return;}
						
						_fov._rectTransform = value;
						
						if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
					}
					
					break;
				case VariableType.local:
				
					if (value == _rectTransform){return;}
					
					_rectTransform = value;
					
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
								_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRRectTransform>(exposedNodeName, exposedName);
							}
						}
					}
						
					if (_expVar != null)
					{
						var _rExpVar = _expVar as FRRectTransform;
						_rExpVar._rectTransform = value;
					}
					else
					{
						_rectTransform = value;
					}
						
					if (OnValueChanged != null){ OnValueChanged(this);}
					
					break;
				}
			}
		}
		
		public FRRectTransform (){}
		
		public FRRectTransform (RectTransform _r)
		{
			_rectTransform = _r;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(RectTransform), _allowSceneObjects) as RectTransform;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(RectTransform), false) as RectTransform;
			#endif
		}

	}
}