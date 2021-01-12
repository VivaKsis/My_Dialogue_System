using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("Internal", "Comment", 0, NodeAttributes.NodeType.Comment)]
	public class Comment : Node
	{
		
		#if UNITY_EDITOR
		bool resize = false;
		Rect originalSize = new Rect(0,0,0,0);
		
	
		public override void Init (Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			hideInput = true;
			
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			disableVariableInspector = true;
			
			groupColor = new Color(Random.Range(70f, 200f)/255f, Random.Range(70f, 200f)/255f, Random.Range(70f, 200f)/255f, 255f / 255f);

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 200, 100);
		}

		public override void DrawGUI (string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{

			base.DrawGUI("Comment", _id, _graph, _editorSkin);
			
			if (guiDisabled)
				return;
				
				
			
			GUI.contentColor = Color.black;
			nodeData.description = GUI.TextArea(new Rect(nodeRect.x + 10, nodeRect.y + 10, nodeRect.width - 35, nodeRect.height - 20), nodeData.description, editorSkin.GetStyle("CommentText"));
			GUI.contentColor = Color.white;

			

			if (GUI.RepeatButton(new Rect(nodeRect.x + (nodeRect.width - 22), nodeRect.y + (nodeRect.height - 20), 20, 20), "", editorSkin.GetStyle("Resize"))){}
	
	        GUILayout.FlexibleSpace();
	        
			var _lastRect = new Rect(nodeRect.x + (nodeRect.width - 20), nodeRect.y + (nodeRect.height - 20), 20, 20);
		
			EditorGUIUtility.AddCursorRect(_lastRect,MouseCursor.ResizeUpLeft);
			
			if (_lastRect.Contains(Event.current.mousePosition)) // && !flowReactor.isDragging)
			{		
				resize = true;     
			}
			
			// store all node rects only if group is not locked
			if (Event.current.type == EventType.MouseDown && nodeRect.Contains(Event.current.mousePosition))
			{
				rootGraph.mouseClickInGroupNode = true;
			}
		
				
			// Resize group
			if (resize && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDrag && Event.current.button == 0 && !rootGraph.dragMinimap && !rootGraph.resizeMinimap)
			{
				
				nodeRect.width += Event.current.delta.x * 0.5f;
				nodeRect.height += Event.current.delta.y * 0.5f;
	        	
				rootGraph.selectionBoxDragging = false;
        	}
			else
			{
				if ( rootGraph.globalMouseEvents != Graph.GlobalMouseEvents.mouseDrag && !_lastRect.Contains(Event.current.mousePosition) )
				{		
					resize = false;     
				}
			}
			
			if ((rootGraph.globalMouseEvents  == Graph.GlobalMouseEvents.mouseUp))
			{
				rootGraph.mouseClickInGroupNode = false;
				resize = false;
			}
		
			originalSize = nodeRect;
			
			if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 25, nodeRect.y + 5, 20, 20), "", editorSkin.GetStyle("DeleteComment")))
			{
				DeleteNodeDialog(graphOwner);
			}
		
			
			GUILayout.FlexibleSpace();
			
		}
		#endif

		
		public override void OnDelete()
		{
			rootGraph.mouseClickInGroupNode = false;
		}

    }
}
