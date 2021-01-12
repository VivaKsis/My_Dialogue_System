using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Utils" , "Add a todo list to your graph" , "#00FBFF" , 0 , NodeAttributes.NodeType.Event )]
	public class ToDo : Node
	{
		
		public class Tasks
		{
			public string taskName;
			public bool completed;
			
			public Tasks ()
			{
				taskName = "";
			}
		}
	
		public List<Tasks> tasks = new List<Tasks>();
		
		public float progress;
		
		// Editor node methods
		#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);

			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 200, 90);
			
			// add initial task
			tasks.Add(new Tasks());
		}
		
	
	
		
		// Draw custom node inspector
		public override void DrawCustomInspector()
		{
			node.color = EditorGUILayout.ColorField(node.color);
		}
		
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
	
			// Progress
			var _completed = 0f;
			var _startCountIndex = 0;
			if (tasks.Count > 1)
			{
				_startCountIndex = 1;
			}
		
			for (int p = _startCountIndex; p < tasks.Count; p ++)
			{
				if (tasks[p].completed)
				{
					_completed ++;
				}
			}
		
			
			if (tasks.Count > 0)
			{		
				progress = _completed / (tasks.Count > 1 ? tasks.Count - 1 : tasks.Count);
			}
			else
			{
				progress = 0f;	
			}
			
			EditorGUI.ProgressBar(new Rect(nodeRect.x + 5, nodeRect.y - 15, nodeRect.width - 10, 5), progress, "");
		
			
			// Set an offset to draw custom gui
			var _drawRectOffset = new Rect(nodeRect.x + 5, nodeRect.y + 25, nodeRect.width - 10, nodeRect.height);
			
			using (new GUILayout.AreaScope(_drawRectOffset))
			{
				using (new GUILayout.HorizontalScope("Box"))
				{
					if (tasks.Count > 1)
					{
						GUI.enabled = false;
						if (_completed == tasks.Count - 1)
						{
							
							EditorGUILayout.Toggle("", true, GUILayout.Width(20));
							
						}
						else
						{
							EditorGUILayout.Toggle("", false, GUILayout.Width(20));
						}
						GUI.enabled = true;
					}
					else
					{
						tasks[0].completed = EditorGUILayout.Toggle("", tasks[0].completed, GUILayout.Width(20));
					}
					
					tasks[0].taskName = EditorGUILayout.TextField(tasks[0].taskName);
				}
			
				if (GUILayout.Button("Add subtask"))
				{
					// resize node rect
					nodeRect.height += 30;

					tasks.Add(new Tasks());
				}
				
				// show tasks
				for (int t = 1; t < tasks.Count; t ++)
				{
					using (new GUILayout.HorizontalScope("Box"))
					{
						tasks[t].completed = EditorGUILayout.Toggle("", tasks[t].completed, GUILayout.Width(20));
						tasks[t].taskName = EditorGUILayout.TextField(tasks[t].taskName);
						
						
						if (GUILayout.Button("x", "toolbarButton"))
						{
							tasks.RemoveAt(t);
							
							// resize node rect
							nodeRect.height -= 30;
						}
					}	
				}
			}
		}
		
		#endif

		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
		}
	}
}