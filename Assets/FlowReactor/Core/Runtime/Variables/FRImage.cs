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
	[FRVariableAttribute(Name = "UI Image")]
	public class FRImage : FRVariable
	{
		[SerializeField]
		private Image _image;
		[SerializeField]
		public  Image Value
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
							foreach(var fr in graph.rootGraph .flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRImage>(variableGuid);
								}
							}
						}
					
				
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRImage>(Guid.Parse(variableGuid));
							if (_f != null)
							{
								if (_f.useDatabox)
								{
									#if FLOWREACTOR_DATABOX
									return assignedBlackboard.GetDataboxData<ResourceType>(Guid.Parse(variableGuid)).Load() as Image;
									#else
									return _f._image;
									#endif
								}
								else
								{
									return _f._image;
								}
							}
							else
							{
								return _image;
							}
						}
						else
						{
							var _rov = _ov as FRImage;
							return _rov._image;
						}
						
					case VariableType.local:
					
						return _image;
						
					case VariableType.exposed:
					
						FRVariable _expVar = null;
						if (graph != null && graph.rootGraph != null)
						{
							foreach(var fr in graph.rootGraph.flowReactorComponents.Keys)
							{
								if (graph.rootGraph.flowReactorComponents[fr] != null)
								{
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRImage>(exposedNodeName, exposedName);
								}
							}
						}
							
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRImage;
							return _rExpVar._image;
						}
						else
						{
							return _image;
						}
						
					default:
					
						return _image;
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
									_ov = graph.rootGraph.flowReactorComponents[fr].GetOverrideVariableByGuid<FRImage>(variableGuid);
								}
							}
						}
					
					
						if (_ov == null || !_ov.overrideVariable)
						{
							var _f = assignedBlackboard.GetData<FRImage>(Guid.Parse(variableGuid));
							
							if (_f == null){return;}
							
							if (_f._image == value){return;}
							
							_f._image = value; 
						
							if (_f.OnValueChanged != null){ _f.OnValueChanged(this);}
						}
						else
						{
							var _fov = _ov as FRImage;
							
							if (_fov._image == value){return;}
							
							_fov._image = value;
							
							if (_fov.OnValueChanged != null){ _fov.OnValueChanged(this);}
						}
						break;
					case VariableType.local:
					
						if (value == _image){return;}
						
						_image = value;
						
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
									_expVar = graph.rootGraph.flowReactorComponents[fr].GetExposedVariable<FRImage>(exposedNodeName, exposedName);
								}
							}
						}
						
						if (_expVar != null)
						{
							var _rExpVar = _expVar as FRImage;
							_rExpVar._image = value;
						}
						else
						{
							_image = value;
						}
						
						if (OnValueChanged != null){ OnValueChanged(this);}
						
						break;
				}
			}
		}
		
		public FRImage (){}
		
		public FRImage (Image _i)
		{
			_image = _i;
		}
		
		public override void Draw(bool _allowSceneObjects, object[] _attributes)
		{
			#if UNITY_EDITOR
			Value = EditorGUILayout.ObjectField(Value, typeof(Image), _allowSceneObjects) as Image;
			#endif
		}
		
		public override void Draw(Rect rect)
		{
			#if UNITY_EDITOR
			Value = EditorGUI.ObjectField(rect, Value, typeof(Image), false) as Image;
			#endif
		}
	
	}
}