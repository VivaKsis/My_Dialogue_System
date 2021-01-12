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
	[FRVariableAttribute(Name = "List<float>")]
	public class FRFloatList : FRVariable
	{
		[SerializeField]
		private List<float> _floatList;
		[SerializeField]
		public  List<float> Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRFloatList>(variableGuid);
								}
							}
						}
					
			
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRFloatList>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								return _g._floatList;
							}
							else
							{
								return _floatList;
							}
						}
						else
						{
							var _rov = _ov as FRFloatList;
							return _rov._floatList;
						}

					case VariableType.local:
					
						return _floatList;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRFloatList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRFloatList;
							return _rExpVar._floatList;
						}
						else
						{
							return _floatList;
						}
						
					default:
						return _floatList;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRFloatList>(variableGuid);
								}
							}
						}
	
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRFloatList>(Guid.Parse(variableGuid));
						
							if (_g == null){return;}
							
							if (_g._floatList == null)
							{
								_g._floatList = new List<float>();
							}
							
							_g._floatList = value;
							
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRFloatList;
							_rov._floatList = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
		
						break;
					case VariableType.local:
					
						_floatList = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRFloatList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRFloatList;
							_rExpVar._floatList = value;
						}
						else
						{
							_floatList = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRFloatList()
		{
			_floatList = new List<float>();
		}
		
		public FRFloatList(List<float> _g)
		{
			Value = _g;
		}
		
		public void New()
		{
			var _v = Value;
			_v = new List<float>();
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			if (_floatList == null)
				return;
			#if UNITY_EDITOR		
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
					if (GUILayout.Button("add", "toolbarButton"))
					{
						_floatList.Add(0f);
					}
					
					if (GUILayout.Button("clear", "toolbarButton"))
					{
						_floatList = new List<float>();
					}
				}
				
				for (int i = 0; i < _floatList.Count; i ++)
				{
					using (new GUILayout.HorizontalScope())
					{
						_floatList[i] = EditorGUILayout.FloatField(_floatList[i]);

						if (GUILayout.Button("-", "miniButton", GUILayout.Width(20)))
						{
							_floatList.RemoveAt(i);
						}
					}
				}
			}
			#endif
		}
		
		public override float GetGUIHeight()
		{
			return (_floatList.Count + 1) * 20;
		}
		
		public override void Draw(Rect rect)
		{
			if (_floatList == null)
				return;
				
			#if UNITY_EDITOR
			if (GUI.Button(new Rect(rect.x ,rect.y, rect.width / 2, 20), "add", "toolbarButton"))
			{
				_floatList.Add(0f);
			}
			
			if (GUI.Button(new Rect(rect.x + (rect.width / 2) ,rect.y, rect.width / 2, 20), "clear", "toolbarButton"))
			{
				_floatList = new List<float>();
			}

			for (int i = 0; i < _floatList.Count; i ++)
			{
				_floatList[i] = EditorGUI.FloatField(new Rect(rect.x, rect.y + (20 * i) + 20, rect.width - 20, 20), _floatList[i]);
		
				if (GUI.Button(new Rect(rect.x + (rect.width - 20), rect.y + (20 * i) + 20, 20, 20), "-", "miniButton"))
				{
					_floatList.RemoveAt(i);
				}
			}
			#endif
		}
	}
}