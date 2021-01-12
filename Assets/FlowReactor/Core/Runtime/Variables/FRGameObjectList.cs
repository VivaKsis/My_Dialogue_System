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
	[FRVariableAttribute(Name = "List<GameObject>")]
	public class FRGameObjectList : FRVariable
	{
		[SerializeField]
		private List<GameObject> _gameObjectList;
		[SerializeField]
		public  List<GameObject> Value
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRGameObjectList>(variableGuid);
								}
							}
						}
						
					
							if (_ov == null || !_ov.overrideVariable)
							{
								var _g = assignedBlackboard.GetData<FRGameObjectList>(Guid.Parse(variableGuid));
								if (_g != null)
								{
									return _g._gameObjectList;
								}
								else
								{
									return _gameObjectList;
								}
							}
							else
							{
								var _rov = _ov as FRGameObjectList;
								return _rov._gameObjectList;
							}
							
					case VariableType.local:
					
						return _gameObjectList;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRGameObjectList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRGameObjectList;
							return _rExpVar._gameObjectList;
						}
						else
						{
							return _gameObjectList;
						}
						
					default:
						return _gameObjectList;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRGameObjectList>(variableGuid);
								}
							}
						}
	
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _g = assignedBlackboard.GetData<FRGameObjectList>(Guid.Parse(variableGuid));
						
							if (_g == null){return;}
							
							if (_g._gameObjectList == null)
							{
								_g._gameObjectList = new List<GameObject>();
							}
							
							_g._gameObjectList = value;
							
							
							if (_g.OnValueChanged != null){ _g.OnValueChanged(this);}
						}
						else
						{
							var _rov = _ov as FRGameObjectList;
							_rov._gameObjectList = value;
							
							if (_rov.OnValueChanged != null){ _rov.OnValueChanged(this);}
						}
						break;
						
					case VariableType.local:
					
						_gameObjectList = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRGameObjectList>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRGameObjectList;
							_rExpVar._gameObjectList = value;
						}
						else
						{
							_gameObjectList = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRGameObjectList()
		{
			_gameObjectList = new List<GameObject>();
		}
		
		public FRGameObjectList(List<GameObject> _g)
		{
			Value = _g;
		}
		
		public void New()
		{
			var _v = Value;
			_v = new List<GameObject>();
		}
		
		public override void Draw(bool _allowSceneObject, object[] _attributes)
		{
			if (_gameObjectList == null)
				return;
			#if UNITY_EDITOR		
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
					if (GUILayout.Button("add", "toolbarButton"))
					{
						_gameObjectList.Add(null);
					}
					
					if (GUILayout.Button("clear", "toolbarButton"))
					{
						_gameObjectList = new List<GameObject>();
					}
				}
				
				for (int i = 0; i < _gameObjectList.Count; i ++)
				{
					using (new GUILayout.HorizontalScope())
					{
						_gameObjectList[i] = EditorGUILayout.ObjectField(	_gameObjectList[i], typeof(GameObject), true) as GameObject;

						if (GUILayout.Button("-", "miniButton", GUILayout.Width(20)))
						{
							_gameObjectList.RemoveAt(i);
						}
					}
				}
			}
			#endif
		}
		
		public override float GetGUIHeight()
		{
			return (_gameObjectList.Count + 1) * 20;
		}
		
		public override void Draw(Rect rect)
		{
			if (_gameObjectList == null)
				return;
				
			#if UNITY_EDITOR
			if (GUI.Button(new Rect(rect.x ,rect.y, rect.width / 2, 20), "add", "toolbarButton"))
			{
				_gameObjectList.Add(null);
			}
			
			if (GUI.Button(new Rect(rect.x + (rect.width / 2) ,rect.y, rect.width / 2, 20), "clear", "toolbarButton"))
			{
				_gameObjectList = new List<GameObject>();
			}

			for (int i = 0; i < _gameObjectList.Count; i ++)
			{
				_gameObjectList[i] = EditorGUI.ObjectField(new Rect(rect.x, rect.y + (20 * i) + 20, rect.width - 20, 20), _gameObjectList[i], typeof(GameObject), true) as GameObject;
		
				if (GUI.Button(new Rect(rect.x + (rect.width - 20), rect.y + (20 * i) + 20, 20, 20), "-", "miniButton"))
				{
					_gameObjectList.RemoveAt(i);
				}
			}
			#endif
		}
	}
}