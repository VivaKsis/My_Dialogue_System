using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("Internal", "Group", 0, NodeAttributes.NodeType.Group)]
	public class Group : Node
	{
		[HideInInspector]
		public bool ok = false;
		public string groupTitle = "GROUP";
		
		[System.Serializable]
		public class OutputNodes
		{
			public int id;
			public Node node;
			
			public OutputNodes (int _id, Node _node)
			{
				id = _id;
				node = _node;
			}
		}

		public List<OutputNodes> outputStateNodes = new List<OutputNodes>();
		public List<OutputNodes> inputStateNodes = new List<OutputNodes>();
		
		[System.Serializable]
		public class ChildNodes
		{
			public Node node;
			public Vector2 relativePos;
			
			public ChildNodes ( Node _node, Vector2 _pos)
			{
				node = _node;
				relativePos = _pos;
			}
		}
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public List<ChildNodes> nodeChilds = new List<ChildNodes>();
		
		List<Node> newNodes = new List<Node>();
		List<Node> removeNodes = new List<Node>();

		Rect originalSize = new Rect(0,0,0,0);
		Rect wrapedRectSize = new Rect(0, 0, 200f, 45f);
		
		FlowReactorComponent flowReactor;
		
		#if UNITY_EDITOR
		public override void Init (Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			hideInput = true;
			
			groupColor = new Color(Random.Range(70f, 200f)/255f, Random.Range(70f, 200f)/255f, Random.Range(70f, 200f)/255f, 255f / 255f);
			
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 200, 100);
			
			AddNodesToGroup();	
		}

		public override void DrawGUI (string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{

			base.DrawGUI(groupTitle, _id, _graph, _editorSkin);
			
			if (guiDisabled)
				return;
				
				
			if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDrag)
			{
				DetectDraggingNode();
			}
			
			groupColor = EditorGUI.ColorField(new Rect(nodeRect.x + 10, nodeRect.y + 10, 35, 16), groupColor);
			
			GUI.contentColor = groupColor;
			groupTitle = GUI.TextField(new Rect(nodeRect.x + 50, nodeRect.y+3, groupTitle.Length * 10, 30), groupTitle, "boldLabel");
			GUI.contentColor = Color.white;
			
			
			GUI.color = groupColor;
			if (GUI.RepeatButton(new Rect(nodeRect.x + (nodeRect.width - 22), nodeRect.y + (nodeRect.height - 20), 20, 20), "", editorSkin.GetStyle("Resize")))
			{
				rootGraph.mouseClickInGroupNode = true;
				rootGraph.currentGroupNodeDragging = this;
			}
			
			GUI.color = Color.white;
	        GUILayout.FlexibleSpace();
	        
			var _lastRect = new Rect(nodeRect.x + (nodeRect.width - 20), nodeRect.y + (nodeRect.height - 20), 20, 20);
		
			EditorGUIUtility.AddCursorRect(_lastRect,MouseCursor.ResizeUpLeft);
		
			if (_lastRect.Contains(Event.current.mousePosition)) // && !flowReactor.isDragging)
			{		
				ok = true;     
			}
			
			// store all node rects only if group is not locked
			if (Event.current.type == EventType.MouseDown && nodeRect.Contains(Event.current.mousePosition) && !rootGraph.isMouseOverNode && rootGraph.selectedNode == null)
			{
				rootGraph.mouseClickInGroupNode = true;
				rootGraph.currentGroupNodeDragging = this;
				// Removed! Adding nodes just because a node intersects the group may be unintended.
				//AddNodesToGroup();
			}
					
				
			// Resize group
			if (ok && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDrag && Event.current.button == 0 && !rootGraph.isMouseOverNode && !rootGraph.dragMinimap && !rootGraph.resizeMinimap) // Event.current.type == EventType.MouseDrag)
			{
				
				nodeRect.width += Event.current.delta.x * 0.5f;
				nodeRect.height += Event.current.delta.y * 0.5f;
	        	
				rootGraph.selectionBoxDragging = false;
        	}
	        else
	        {
		        if ( rootGraph.globalMouseEvents != Graph.GlobalMouseEvents.mouseDrag && !_lastRect.Contains(Event.current.mousePosition) )
		        {		
			        ok = false;     
		        }
	        }
			
			if (Event.current.type == EventType.MouseUp) //rootGraph.globalMouseEvents  == Graph.GlobalMouseEvents.mouseUp)
			{

				rootGraph.mouseClickInGroupNode = false;
				rootGraph.currentGroupNodeDragging = null;
				ok = false;
				
				if (removeNodes.Count > 0)
				{
					RemoveNodesFromGroupMenu();
				}
				
				if (newNodes.Count > 0)
				{
					AddNodesToGroupMenu();
				}
				
			
			}


			if (!ok  && !rootGraph.dragNodeFromNodeMenu && !rootGraph.isMouseOverNode && !rootGraph.selectionBoxDragging && !rootGraph.dragMinimap && !rootGraph.resizeMinimap)
			{ 
	        	// Move all nodes within group
				if ( Event.current.type == EventType.MouseDrag && Event.current.button == 0 && nodeRect.Contains(Event.current.mousePosition) && rootGraph.currentGroupNodeDragging == this)
				{
				
			        for (int c = 0; c < nodeChilds.Count; c ++)
			        {
				        nodeChilds[c].node.nodeRect.position += new Vector2(Event.current.delta.x, Event.current.delta.y);		
			        } 
			        
		        	MoveDelta(Event.current.delta);  
	        	}
			}
				
				
			originalSize = nodeRect;
			
			GUILayout.FlexibleSpace();
		}
		
		void AddNodesToGroupMenu()
		{
		
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Add to group"), false, AddNewNodes);
			menu.AddItem(new GUIContent("Cancel"), false, CancelAddingRemovingNodes);
			
			menu.ShowAsContext();
			
			rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;
			rootGraph.globalMouseButton = -1;
			
			Event.current.Use();
		}
		
		void RemoveNodesFromGroupMenu()
		{
			
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Remove from group"), false, RemoveNodes);
			menu.AddItem(new GUIContent("Cancel"), false, CancelAddingRemovingNodes);
			
			menu.ShowAsContext();
			
			rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;
			rootGraph.globalMouseButton = -1;
			
			Event.current.Use();
		}
		
		
		void CancelAddingRemovingNodes()
		{
			newNodes = new List<Node>();
			removeNodes = new List<Node>();
			rootGraph.selectedNodes = new Dictionary<int, Node>();
		}
		
		
		
		void AddNodesToGroup()
		{
			nodeChilds = new List<ChildNodes>();
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				if (rootGraph.selectedNodes[node].nodeData.nodeType != NodeAttributes.NodeType.Group)
				{
					var _rect = rootGraph.selectedNodes[node].nodeRect;
					nodeChilds.Add(new ChildNodes(rootGraph.selectedNodes[node], new Vector2(_rect.x - nodeRect.x, _rect.y - nodeRect.y)));
				}
			}
			
		}
		
		//void DetectNewNodes()
		//{
		//	nodeRect = originalSize;
		//	newNodes = new List<Node>();
		//	foreach (var node in rootGraph.selectedNodes.Keys)
		//	{
		//		if (nodeRect.Contains(new Vector2(rootGraph.selectedNodes[node].nodeRect.x, rootGraph.selectedNodes[node].nodeRect.y)) &&
		//			rootGraph.selectedNodes[node] != this && rootGraph.selectedNodes[node].nodeData.nodeType != NodeAttributes.NodeType.Group) // && !flowReactor.graphs[flowReactor.currentGraphIndex].states[flowReactor.currentLevel].nodes[g].isHidden)
		//		{
		//			// store current relative position to group node so we can restore it after unhiding nodes
		//			//var _rect = rootGraph.currentGraph.nodes[g].nodeRect;
		//			//nodeChilds.Add(new ChildNodes(rootGraph.currentGraph.nodes[g], new Vector2(_rect.x - nodeRect.x, _rect.y - nodeRect.y)));
				
		//			// check if there are new nodes added to the group
		//			var _add = true;
		//			for(int i = 0; i < nodeChilds.Count; i ++)
		//			{
		//				if (nodeChilds[i].node == rootGraph.selectedNodes[node])
		//				{
		//					_add = false;
		//				}
		//			}
					
		//			if (_add)
		//			{
		//				newNodes.Add(rootGraph.selectedNodes[node]);
		//			}
					
		//		}
		//	}
			
		//	Debug.Log(newNodes.Count);
		//}
		
		void DetectDraggingNode()
		{
			
			if (rootGraph.selectedNode == null)
				return;
				
			newNodes = new List<Node>();
			removeNodes = new List<Node>();
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				
				if (nodeRect.Contains(new Vector2(rootGraph.selectedNodes[node].nodeRect.x, rootGraph.selectedNodes[node].nodeRect.y)) &&
					rootGraph.selectedNodes[node] != this && rootGraph.selectedNodes[node].nodeData.nodeType != NodeAttributes.NodeType.Group) // && !flowReactor.graphs[flowReactor.currentGraphIndex].states[flowReactor.currentLevel].nodes[g].isHidden)
				{
					var _add = true;
					for(int i = 0; i < nodeChilds.Count; i ++)
					{
						if (nodeChilds[i].node == rootGraph.selectedNodes[node])
						{
							_add = false;
						}
					}
					
					if (_add)
					{
						newNodes.Add(rootGraph.selectedNodes[node]);
					}
					
				}
			}
			
			// check if nodes in groups has been moved out of the group
			for(int i = 0; i < nodeChilds.Count; i ++)
			{
				if (!nodeRect.Contains(new Vector2(nodeChilds[i].node.nodeRect.x, nodeChilds[i].node.nodeRect.y)) &&
					nodeChilds[i].node != this && nodeChilds[i].node.nodeData.nodeType != NodeAttributes.NodeType.Group) // && !flowReactor.graphs[flowReactor.currentGraphIndex].states[flowReactor.currentLevel].nodes[g].isHidden)
				{	
					removeNodes.Add(nodeChilds[i].node);
				}
			}
		}
		
		public void AddNodeToGroup(Node _node)
		{
			var _rect = _node.nodeRect;
			nodeChilds.Add(new ChildNodes(_node, new Vector2(_rect.x - nodeRect.x, _rect.y - nodeRect.y)));
		}
		
		void AddNewNodes()
		{
			for(int i = 0; i < newNodes.Count; i ++)
			{
				var _rect = newNodes[i].nodeRect;
				nodeChilds.Add(new ChildNodes(newNodes[i], new Vector2(_rect.x - nodeRect.x, _rect.y - nodeRect.y)));
			}
			
			
			// Iterate through all groups except this one and check
			// if node was in another group before. If so remove it.
			for( int n = 0; n < rootGraph.currentGraph.nodes.Count; n ++)
			{
				if (rootGraph.currentGraph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Group &&
					rootGraph.currentGraph.nodes[n] != this)
				{
					var _groupNode = rootGraph.currentGraph.nodes[n] as Group;
					for (int c = 0; c < _groupNode.nodeChilds.Count; c ++)
					{
						for(int s = 0; s < newNodes.Count; s ++)
						{
							if (_groupNode.nodeChilds[c].node == newNodes[s].node)
							{
								_groupNode.nodeChilds.RemoveAt(c);
								_groupNode.removeNodes = new List<Node>();
							}
						}
					}
				}
			}
			
			newNodes = new List<Node>();
			removeNodes = new List<Node>();
			
		}
		
		void RemoveNodes()
		{
			for(int i = 0; i < removeNodes.Count; i ++)
			{
				for (int n = 0; n < nodeChilds.Count; n ++)
				{
					if (nodeChilds[n].node == removeNodes[i])
					{
						nodeChilds.RemoveAt(n);
					}
				}
			}
			
			newNodes = new List<Node>();
			removeNodes = new List<Node>();
		}
		
		public override void OnDelete()
		{
			rootGraph.mouseClickInGroupNode = false;
		}
		#endif
    }
}
