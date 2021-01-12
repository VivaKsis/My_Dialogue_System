using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;

namespace FlowReactor.Nodes.Unity
{
	#pragma warning disable CS0649
	[NodeAttributes( "Unity/Component" , "Sets the value of any public property or field on the targeted Unity Object." , "actionNodeColor" , 1 )]
	public class SetProperty : Node
	{
		public FRGameObject gameObject;
		
		bool isDragging;
		
		[SerializeField]
		string typeName;
		[SerializeField]
		List<string> variableNames;
		[SerializeField]
		List<string> propertyNames;
		[SerializeField]
		int selectedVariable = 0;
		
		// values
		[SerializeField]
		FRVector3 v3;
		[SerializeField]
		FRVector2 v2;
		[SerializeField]
		FRFloat floatValue;
		[SerializeField]
		FRInt intValue;
		[SerializeField]
		FRString stringValue;
		[SerializeField]
		FRBoolean boolValue;

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
				FRVariableGUIUtility.DrawVariable("owner:", gameObject, this, false, editorSkin);
			}
			
			if (variableNames != null && variableNames.Count > 0 && selectedVariable < propertyNames.Count)
			{
				
				using (new GUILayout.VerticalScope(editorSkin.GetStyle("Box")))
				{
					GUILayout.Label(typeName, "boldLabel");
					GUILayout.Label("selected property");
					selectedVariable = EditorGUILayout.Popup(selectedVariable, variableNames.ToArray());
					
				}
				
				
				using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
				{
					GUILayout.Label("set to:");
					switch (propertyNames[selectedVariable])
					{
						case "UnityEngine.Vector3":
							
							FRVariableGUIUtility.DrawVariable(v3, this, false, editorSkin);
							break;
						case "UnityEngine.Vector2":
						
							FRVariableGUIUtility.DrawVariable(v2, this, false, editorSkin);
							break;
						case "System.Single":
							
							FRVariableGUIUtility.DrawVariable(floatValue, this, false, editorSkin);
							break;
						case "System.Int32":
							
							FRVariableGUIUtility.DrawVariable(intValue, this, false, editorSkin);
							break;
						case "System.Boolean":
							
							FRVariableGUIUtility.DrawVariable(boolValue, this, false, editorSkin);
							break;
						case "System.String":
						
							FRVariableGUIUtility.DrawVariable(stringValue, this, false, editorSkin);
							break;
							
					}
				}
			}
			
		}
	
	
	
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
						
						variableNames = new List<string>();
						propertyNames = new List<string>();
						
						foreach (System.Object _dobj in DragAndDrop.objectReferences)
						{
							typeName = _dobj.GetType().Name;
							
							foreach (var thisVar in _dobj.GetType().GetProperties())
							{
								try
								{
									#if FLOWREACTOR_DEBUG
									Debug.Log("Component:  " + _dobj.GetType().Name + "        Var Name:  " + thisVar.Name + "         Type:  " + thisVar.PropertyType + "       Value:  " + thisVar.GetValue(_dobj,null) + "\n" );
									#endif
									
									variableNames.Add(thisVar.Name);
									propertyNames.Add(thisVar.PropertyType.ToString());			
								}
								catch 
								{
								}
							}   
						}
					}
					
				}
				break;
			}	
		}
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			var c = gameObject.Value.GetComponent(typeName);
			
			if (c != null)
			{
				switch (propertyNames[selectedVariable])
				{
				case "UnityEngine.Vector3":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)v3.Value);
					break;
				case "UnityEngine.Vector2":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)v2.Value );
					break;
				case "System.Single":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)floatValue.Value);
					break;
				case "System.Int32":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)intValue.Value);
					break;
				case "System.Boolean":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)boolValue.Value);
					break;
				case "System.String":
					c.GetType().GetProperty(variableNames[selectedVariable]).SetValue(c, (System.Object)stringValue.Value);
					break;
				}
			}
			else
			{
				Debug.LogError("FlowReactor - Set Property: component does not exist");
			}

			
			ExecuteNext(0, _flowReactor);
		}
	}
	#pragma warning restore CS0649
}