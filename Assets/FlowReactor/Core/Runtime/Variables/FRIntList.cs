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
	[FRVariableAttribute(Name = "List<int>")]
	public class FRIntList : FRVariable
	{
		[SerializeField]
		private List<int> _intList;
		[SerializeField]
		public  List<int> Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRIntList>(variableGuid);
								}
							}
						}
					
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRIntList>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _il = assignedBlackboard.GetDataboxData<IntListType>(Guid.Parse(variableGuid));
									return _il.Value;
									#else
									return _g._intList;
									#endif
								}
								else
								{
									return _g._intList;
								}
							}
							else
							{
								return _intList;
							}
						}
						else
						{
							var _rov = _ov as FRIntList;
							return _rov._intList;
						}
					case VariableType.local:
					
						return _intList;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRIntList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRIntList;
							return _rExpVar._intList;
						}
						else
						{
							return _intList;
						}
						
						
					default:
						return _intList;
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
							_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRIntList>(variableGuid);
						}
					}
				}

					if (_ov == null || !_ov.overrideVariable)
					{
						var _g = assignedBlackboard.GetData<FRIntList>(Guid.Parse(variableGuid));
					
						if (_g == null){return;}
						
						if (_g._intList == null)
						{
							_g._intList = new List<int>();
						}
						
						_g._intList = value;
						
						
						if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
					}
					else
					{
						var _rov = _ov as FRIntList;
						_rov._intList = value;
						
						if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
					}
					break;
				case VariableType.local:
				
					_intList = value;
					
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
								_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRIntList>(exposedNodeName, exposedName);
							}
						}
					}
						
					if (_expVar != null)
					{
						var _rExpVar = _expVar as FRIntList;
						_rExpVar._intList = value;
					}
					else
					{
						_intList = value;
					}
						
					if (OnValueChanged != null){ OnValueChanged(this);}
					
					break;
				}
			}
		}
		
		public FRIntList()
		{
			_intList = new List<int>();
		}
		
		public FRIntList(List<int> _g)
		{
			Value = _g;
		}
		
		public void New()
		{
			var _v = Value;
			_v = new List<int>();
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			if (_intList == null)
				return;
			#if UNITY_EDITOR		
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
					if (GUILayout.Button("add", "toolbarButton"))
					{
						_intList.Add(0);
					}
					
					if (GUILayout.Button("clear", "toolbarButton"))
					{
						_intList = new List<int>();
					}
				}
				
				for (int i = 0; i < _intList.Count; i ++)
				{
					using (new GUILayout.HorizontalScope())
					{
						_intList[i] = EditorGUILayout.IntField(	_intList[i]);

						if (GUILayout.Button("-", "miniButton", GUILayout.Width(20)))
						{
							_intList.RemoveAt(i);
						}
					}
				}
			}
			#endif
		}
		
		public override float GetGUIHeight()
		{
			return (_intList.Count + 1) * 20;
		}
		
		public override void Draw(Rect rect)
		{
			if (_intList == null)
				return;
				
			#if UNITY_EDITOR
			if (GUI.Button(new Rect(rect.x ,rect.y, rect.width / 2, 20), "add", "toolbarButton"))
			{
				_intList.Add(0);
			}
			
			if (GUI.Button(new Rect(rect.x + (rect.width / 2) ,rect.y, rect.width / 2, 20), "clear", "toolbarButton"))
			{
				_intList = new List<int>();
			}

			for (int i = 0; i < _intList.Count; i ++)
			{
				_intList[i] = EditorGUI.IntField(new Rect(rect.x, rect.y + (20 * i) + 20, rect.width - 20, 20), _intList[i]);
		
				if (GUI.Button(new Rect(rect.x + (rect.width - 20), rect.y + (20 * i) + 20, 20, 20), "-", "miniButton"))
				{
					_intList.RemoveAt(i);
				}
			}
			#endif
		}
	}
}