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
	[FRVariableAttribute(Name = "AudioSource")]
	public class FRAudioSource : FRVariable
	{
		[SerializeField]
		private AudioSource _audioSource;
		[SerializeField]
		public  AudioSource Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRAudioSource>(variableGuid);
								}
							}
						}
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRAudioSource>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _go = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as AudioSource;
									return _go;
									#else
									return _g._audioSource;
									#endif
								}
								else
								{
									return _g._audioSource;
								}
							}
							else
							{
								return _audioSource;
							}
						}
						else
						{
							var _rov = _ov as FRAudioSource;
							return _rov._audioSource;
						}
	
					case VariableType.local:
					
						return _audioSource;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRAudioSource>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRAudioSource;
							return _rExpVar._audioSource;
						}
						else
						{
							return _audioSource;
						}
					default:
						return _audioSource;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRAudioSource>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRAudioSource>(Guid.Parse(variableGuid));
						
							if (_g == null){return;}
							
							if (_g._audioSource == value){return;}
							
							if (_g.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var _v = assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as AudioSource;
								_v = value;
								#endif
							}
							else
							{
								_g._audioSource = value;
							}
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRAudioSource;
							_rov._audioSource = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						break;
						
					case VariableType.local:
					
						if (value == _audioSource){return;}
							
						_audioSource = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRAudioSource>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRAudioSource;
							_rExpVar._audioSource = value;
						}
						else
						{
							_audioSource = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRAudioSource(){}
		
		public FRAudioSource(AudioSource _a)
		{
			Value = _a;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(AudioSource), _allowSceneObjects) as AudioSource;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(AudioSource), false) as AudioSource;
			#endif
		}

	}
}