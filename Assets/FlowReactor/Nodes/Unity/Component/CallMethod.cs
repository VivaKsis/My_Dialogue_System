using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;
using FlowReactor.OrderedDictionary;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes( "Unity/Component" , "Call a method on a script" , "actionNodeColor" , 1 )]
	public class CallMethod : Node
	{

		public FRGameObject owner;

		bool isDragging;
		
		[SerializeField]
		string componentType = "";
		[SerializeField]
		int selectedMethod = 0;
		[SerializeField]
		public class MethodData
		{
			public string methodName;
			public List<string> parameterNames = new List<string>();
			public OrderedDictionary<Guid, FRVariable> methodParameters = new OrderedDictionary<Guid, FRVariable>();
		}
		
		public List<MethodData> methodData = new List<MethodData>();
	
	
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		public override void DrawCustomInspector()
		{
			GUILayout.Label("");
			var _lastRect = GUILayoutUtility.GetLastRect();
			DropAreaGUI(_lastRect);
			
			GUILayout.Space(30);
			
			
			using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
			{
				FRVariableGUIUtility.DrawVariable("script owner:", owner, this, false, editorSkin);
			}
			
			using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
			{
				if (methodData.Count > 0)
				{
					var _mlist = methodData.Select(x => x.methodName != null ? x.methodName.ToString() : "").ToArray();
					selectedMethod = EditorGUILayout.Popup(selectedMethod, _mlist);
				}
			}
			
			using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
			{
				var _i = 0;
				if (selectedMethod < methodData.Count)
				{
					foreach (var v in methodData[selectedMethod].methodParameters.Keys)
					{
						using (new GUILayout.HorizontalScope())
						{
							FRVariableGUIUtility.DrawVariable(methodData[selectedMethod].parameterNames[_i], methodData[selectedMethod].methodParameters[v], this, false, editorSkin);
						}
						
						_i ++;
					}
				}
			}
		}
		#endif
		

		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
		
			System.Type _type = System.Type.GetType(componentType);	
			

			MethodInfo _method = _type.GetMethod(methodData[selectedMethod].methodName);
			
			object[] _args = new object[methodData[selectedMethod].methodParameters.Keys.Count];
			var _methodList = methodData[selectedMethod].methodParameters.Keys.ToList();
	
			for (int p = 0; p < _methodList.Count; p ++)
			{
				_args[p] = (object)methodData[selectedMethod].methodParameters[_methodList[p]].GetType().GetProperty("Value").GetValue(methodData[selectedMethod].methodParameters[_methodList[p]]);
			}
		
			if (owner.Value == null )
			{
				Debug.Log("owner is null");
			}
			_method.Invoke(owner.Value.GetComponent(componentType), _args);
			
		
			
			ExecuteNext(0, _flowReactor);
		}
		
		#if UNITY_EDITOR
		void DropAreaGUI(Rect _lastRect)
		{
		
			Event _evt = Event.current;
			Rect _dropArea = new Rect(_lastRect.x, _lastRect.y, _lastRect.width, 40); //GUILayoutUtility.GetRect(position.width, position.height);
		
			if (isDragging)
			{
				GUI.color = Color.green;
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Box (_dropArea, "Drag and drop component");
			GUI.color = Color.white;
			switch (_evt.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!_dropArea.Contains(_evt.mousePosition))
				{
					isDragging = false;
					return;
				}
				else
				{
					isDragging = true;
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if (_evt.type == EventType.DragPerform)
					{
						isDragging = false;
						DragAndDrop.AcceptDrag();
						
						//methodsName = new List<string>();
						
						foreach (System.Object _dobj in DragAndDrop.objectReferences)
						{
							componentType = _dobj.GetType().Name;
							
							BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
							MethodInfo[] monoMethods= _dobj.GetType().GetMethods(flags);
							
							methodData = new List<MethodData>();
							
							//methodParameters = new OrderedDictionary<Guid, FRVariable>();
							//parametersName = new List<string>();
							
							foreach (var method in monoMethods)
							{
								
								methodData.Add(new MethodData());
								
								var _parameters = method.GetParameters();
								
								foreach (var parameter in _parameters)
								{
							
									var _ok = false;
									switch (parameter.ParameterType.ToString())
									{
										case "UnityEngine.Vector3":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRVector3());
											break;
										case "UnityEngine.Vector2":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRVector2());
											break;
										case "System.Single":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRFloat());
											break;
										case "System.Int32":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRInt());
											break;
										case "System.Boolean":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRBoolean());
											break;
										case "System.String":
											_ok = true;
											methodData[methodData.Count-1].methodParameters.Add(Guid.NewGuid(), new FRString());
											break;
								
									}		
									
									if (_ok)
									{
										methodData[methodData.Count-1].parameterNames.Add(parameter.Name);
									}
								}
								
								methodData[methodData.Count-1].methodName = method.Name;
							}
							   
						}
					}
					
				}
				break;
			}	
		}
		#endif
	}
}