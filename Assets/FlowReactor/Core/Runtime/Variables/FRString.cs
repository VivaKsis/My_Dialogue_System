//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowReactor
{
	[System.Serializable]
	[FRVariableAttribute(Name = "String")]
	public class FRString : FRVariable
	{
		
		[SerializeField]
		private string _string;
		[SerializeField]
		public string Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRString>(variableGuid);
								}
							}
						}
						
						if (_ov == null || !_ov.overrideVariable)
						{
							var _s = assignedBlackboard.GetData<FRString>(Guid.Parse(variableGuid));
							if (_s != null)
							{
								if (_s.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<StringType>(Guid.Parse(variableGuid)).Value;
									#else
									return _s._string;
									#endif
								}
								else
								{
									return _s._string;
								}
							}
							else
							{
								return _string;
							}
						}
						else
						{
							var _rov = _ov as FRString;
							return _rov._string;
						}
						
					case VariableType.local:
					
						return _string;
						
					case VariableType.exposed:
			
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
								
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRString>(exposedNodeName, exposedName);
								}
							
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRString;
							return _rExpVar._string;
						}
						else
						{
							return _string;
						}
						
						
					default:
						return _string;
						
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRString>(variableGuid);
								}
							}
						}
						
						if (_ov == null || !_ov.overrideVariable)
						{
							var _s = assignedBlackboard.GetData<FRString>(Guid.Parse(variableGuid));
							
							if (_s == null){return;}
							
							if (_s._string == value){return;}
							
							if (_s.useDatabox)
							{
								#if FLOWREACTOR_DATABOX
								var _v = assignedBlackboard.GetDataboxData<StringType>(Guid.Parse(variableGuid));
								_v.Value = value;
								#endif
							}
							else
							{
								_s._string = value;
							}
							
							if (_s.OnValueChanged != null){ _s.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRString;
							_rov._string = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						
						break;
						
					case VariableType.local:
					
						if (value == _string){return;}
							
						_string = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRString>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRString;
							_rExpVar._string = value;
						}
						else
						{
							_string = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}

			}
		}
	
		float rectHeight = 0f;
		
		public FRString()
		{
			_string = "";
		}
		
		public FRString(string _s)
		{
			_string = _s;
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			Value = GUILayout.TextArea(Value);
			var _r = GUILayoutUtility.GetLastRect();
			rectHeight = _r.height;
		}
		
		public override void Draw(Rect rect)
		{
			Value = GUI.TextArea(rect, Value);
		}

		public override float GetGUIHeight()
		{
			if (rectHeight > 1)
			{
				return rectHeight;
			}
			else
			{
				return base.GetGUIHeight();
			}
		}
	}
}