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
	[FRVariableAttribute(Name = "List<string>")]
	public class FRStringList : FRVariable
	{
		[SerializeField]
		private List<string> _stringList;
		[SerializeField]
		public  List<string> Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRStringList>(variableGuid);
								}
							}
						}
						
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRStringList>(Guid.Parse(variableGuid));
							if (_g != null)
							{
								if (_g.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									var _sl = assignedBlackboard.GetDataboxData<StringListType>(Guid.Parse(variableGuid));
									return _sl.Value;
									#else
									return _g._stringList;
									#endif
								}
								else
								{
									return _g._stringList;
								}
							}
							else
							{
								return _stringList;
							}
						}
						else
						{
							var _rov = _ov as FRStringList;
							return _rov._stringList;
						}
						
					case VariableType.local:
					
						return _stringList;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRStringList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRStringList;
							return _rExpVar._stringList;
						}
						else
						{
							return _stringList;
						}
						
						
					default:
						return _stringList;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRStringList>(variableGuid);
								}
							}
						}
	
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRStringList>(Guid.Parse(variableGuid));
						
							if (_g == null){return;}
							
							if (_g._stringList == null)
							{
								_g._stringList = new List<string>();
							}
							
							_g._stringList = value;
							
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRStringList;
							_rov._stringList = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						
						break;
					case VariableType.local:
					
						_stringList = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRStringList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRStringList;
							_rExpVar._stringList = value;
						}
						else
						{
							_stringList = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRStringList()
		{
			_stringList = new List<string>();
		}
		
		public FRStringList(List<string> _g)
		{
			Value = _g;
		}
		
		public void New()
		{
			var _v = Value;
			_v = new List<string>();
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			if (_stringList == null)
				return;
			#if UNITY_EDITOR		
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
					if (GUILayout.Button("add", "toolbarButton"))
					{
						_stringList.Add(null);
					}
					
					if (GUILayout.Button("clear", "toolbarButton"))
					{
						_stringList = new List<string>();
					}
				}
				
				for (int i = 0; i < _stringList.Count; i ++)
				{
					using (new GUILayout.HorizontalScope())
					{
						_stringList[i] = EditorGUILayout.TextField(	_stringList[i]);

						if (GUILayout.Button("-", "miniButton", GUILayout.Width(20)))
						{
							_stringList.RemoveAt(i);
						}
					}
				}
			}
			#endif
		}
		
		public override float GetGUIHeight()
		{
			return (_stringList.Count + 1) * 20;
		}
		
		public override void Draw(Rect rect)
		{
			if (_stringList == null)
				return;
				
			#if UNITY_EDITOR
			if (GUI.Button(new Rect(rect.x ,rect.y, rect.width / 2, 20), "add", "toolbarButton"))
			{
				_stringList.Add(null);
			}
			
			if (GUI.Button(new Rect(rect.x + (rect.width / 2) ,rect.y, rect.width / 2, 20), "clear", "toolbarButton"))
			{
				_stringList = new List<string>();
			}

			for (int i = 0; i < _stringList.Count; i ++)
			{
				_stringList[i] = EditorGUI.TextField(new Rect(rect.x, rect.y + (20 * i) + 20, rect.width - 20, 20), _stringList[i]);
		
				if (GUI.Button(new Rect(rect.x + (rect.width - 20), rect.y + (20 * i) + 20, 20, 20), "-", "miniButton"))
				{
					_stringList.RemoveAt(i);
				}
			}
			#endif
		}
	}
}