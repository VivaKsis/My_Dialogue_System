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
using UnityEngine.UI;

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "UI Button")]
	public class FRButton : FRVariable
	{
		[SerializeField]
		private Button _button;
		[SerializeField]
		public Button Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRButton>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRButton>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Button;
									#else
									return _f._button;
									#endif
								}
								else
								{
									return _f._button;
								}
							}
							else
							{
								return _button;
							}
						}
						else
						{
							var _rov = _ov as FRButton;
							return _rov._button;
						}
				
					case VariableType.local:
					
						return _button;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRButton>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRButton;
							return _rExpVar._button;
						}
						else
						{
							return _button;
						}
					
					default:
						return _button;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRButton>(variableGuid);
								}
							}
						}
						
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRButton>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._button == value){return;}
							
							_f._button = value; 
			
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRButton;
							
							if (_fov._button == value){return;}
							
							_fov._button = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						
						break;
					case VariableType.local:
					
						if (value == _button){return;}
						
						_button = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRButton>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRButton;
							_rExpVar._button = value;
						}
						else
						{
							_button = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRButton(){}
		
		public FRButton (Button _b)
		{
			_button = _b;
		}
	
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(Button), _allowSceneObjects) as Button;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(Button), false) as Button;
			#endif
		}
		
	}
}
