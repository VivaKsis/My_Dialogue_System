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
	[FRVariableAttribute(Name = "Animator")]
	public class FRAnimator : FRVariable
	{
		[SerializeField]
		private Animator _animator;
		[SerializeField]
		public  Animator Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRAnimator>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRAnimator>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _go = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Animator;
									return _go;
									#else
									return _g._animator;
									#endif
								}
								else
								{
									return _g._animator;
								}
							}
							else
							{
								return _animator;
							}
						}
						else
						{
							var _rov = _ov as FRAnimator;
							return _rov._animator;
						}
				
					case VariableType.local:
					
						return _animator;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRAnimator>(exposedNodeName, exposedName);
								}
							}
						}
							
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRAnimator;
							return _rExpVar._animator;
						}
						else
						{
							return _animator;
						}
						
					default:
						return _animator;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRAnimator>(variableGuid);
								}
							}
						}
				
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRAnimator>(Guid.Parse(blackboardGuid));
							
							if (_g == null){return;}
							
							if (_g._animator == value){return;}
							
							if (_g.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var _v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Animator;
								_v = value;
								#endif
							}
							else
							{
								_g._animator = value;
							}
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRAnimator;
							_rov._animator = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						
					break;
					
				case VariableType.local:
				
					if (value == _animator){return;}
						
					_animator = value;
					
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
								_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRAnimator>(exposedNodeName, exposedName);
							}
						}
					}
						
					if (_expVar != null)
					{
						var _rExpVar = _expVar as FRAnimator;
						_rExpVar._animator = value;
					}
					else
					{
						_animator = value;
					}
					
					if (OnValueChanged != null){ OnValueChanged(this);}
					
					
					break;
				}
			}
		}
		
		public FRAnimator(){}
		
		public FRAnimator(Animator _g)
		{
			Value = _g;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(Animator), _allowSceneObject) as Animator;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(Animator), false) as Animator;
			#endif
		}
	
	}
}