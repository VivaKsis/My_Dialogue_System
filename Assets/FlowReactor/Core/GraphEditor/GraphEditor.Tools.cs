//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public partial class GraphEditor : EditorWindow
	{
			
		public enum NodeAlignment
		{
			Horizontally,
			Vertically,
			HorizontallyAndVertically
		}
		
		/// <summary>
		/// Topbar shows available graphs
		/// </summary>
		void TopBar()
		{
			var _rect = new Rect(rootGraph.tmpInspectorWidth, 0, position.width - rootGraph.tmpInspectorWidth, 40);
			using (new GUILayout.AreaScope(_rect))
			{
				using (new GUILayout.HorizontalScope("toolbar"))
				{
						
					if(GUILayout.Button(new GUIContent(" Properties", inspectorIcon), inspectorButtonStyle))
					{

						rootGraph.showInspector = !rootGraph.showInspector;
						if (!rootGraph.showInspector)
						{
							rootGraph.tmpInspectorWidth = 0;	
						}
						else
						{
							rootGraph.tmpInspectorWidth = rootGraph.inspectorWidth;
					   
						}
					    
					}
					
					#if !FLOWREACTOR_DEBUG
					GUI.enabled = !rootGraph.currentGraph.isCopy;
					#endif
					
					if (GUILayout.Button(EditorUtility.IsDirty(rootGraph) ? "Save * " : "Save", inspectorButtonStyle))
					{
						EditorUtility.SetDirty(rootGraph.currentGraph);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					
					GUI.enabled = true;
			   
					GUILayout.FlexibleSpace();
				}
			    
				if (rootGraph.currentGraph == null)
					return;
				    
				// Breadcrumb navigation obsolete?!
				#region breadcrumb
				using (new GUILayout.HorizontalScope("toolbar"))
				{  
				    
			    	
					for (int p = 0; p < rootGraph.parentLevels.Count; p ++)
					{
						if (rootGraph.parentLevels[p].graph == null)
							continue;
				    	
						GUIStyle _style = new GUIStyle();
						if (rootGraph.currentGraph == rootGraph.parentLevels[p].graph && !rootGraph.currentGraph.isRoot)
						{
							_style = breadCrumbMidOn;
						}
						else if (rootGraph.currentGraph == rootGraph.parentLevels[p].graph && rootGraph.currentGraph.isRoot)
						{
							_style = breadCrumbRightOn;
						}
						else if (p > 0 && rootGraph.currentGraph != rootGraph.parentLevels[p].graph && !rootGraph.currentGraph.isRoot)
						{
							_style = breadCrumbMid;
						}
						else if (p == 0 && rootGraph.currentGraph != rootGraph.parentLevels[p].graph && !rootGraph.currentGraph.isRoot)
						{
							_style = breadCrumbRight;
						}
						else if (rootGraph.currentGraph != rootGraph.parentLevels[p].graph && rootGraph.currentGraph.isRoot)
						{
							_style = breadCrumbRight;
						}
				    	
				    	
						if (GUILayout.Button(new GUIContent(" " + rootGraph.parentLevels[p].name, subGraphIcon), _style)) //, GUILayout.Width(100)))
						{
							MoveToGraph(p);
						}
					}
			    	
			    	
					GUILayout.FlexibleSpace();
				}
				#endregion
			 
			}
		}
		
		void MoveToGraph(int _level)
		{
			// update root parentlevel name
			rootGraph.parentLevels[0].name = rootGraph.name;
						    
			isDragging = false;
			rootGraph.isMouseOverNode = false;
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
				    				
			for (int s = rootGraph.parentLevels.Count - 1; s > _level; s --)
			{
				rootGraph.parentLevels.RemoveAt(s);
			}	
		    #if FLOWREACTOR_DEBUG
			Debug.Log("goto: " + rootGraph.parentLevels[_level].graph.name);
			#endif
			rootGraph.currentGraph = rootGraph.parentLevels[_level].graph;
		}
		
		void Toolbar()
		{
			using (new GUILayout.AreaScope(new Rect(rootGraph.tmpInspectorWidth, 35, rootGraph.showInspector ? position.width - rootGraph.tmpInspectorWidth  : position.width, 40), "", editorSkin.GetStyle("Box")))
			{
				using (new GUILayout.HorizontalScope())
				{
					#if !FLOWREACTOR_DEBUG
					GUI.enabled = !rootGraph.currentGraph.isCopy;
					#endif
					
					if (GUILayout.Button(minimapIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						rootGraph.showMinimap = !rootGraph.showMinimap;
					}
					
					if (GUILayout.Button(focusIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						FocusCanvas(true);
					}
					
					if (GUILayout.Button(commentIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						CreateComment();
					}
					
					if (rootGraph.selectedNodes.Count == 0)
					{
						GUI.enabled = false;	
					}
					else
					{
						GUI.enabled = true;	
					}

					if (GUILayout.Button(groupIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						CreateGroupFromSelected();
					}
					
					if (GUILayout.Button(createSubGraphIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						CreateSubGraphFromSelected();
					}
					
					GUI.enabled = true;	
					GUILayout.Space(20);
				
					if (GUILayout.Button(expandNodesIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						ExpandAllNodes();
					}
					
					if (GUILayout.Button(collapseNodesIcon, GUILayout.Width(toolbarButtonSize), GUILayout.Height(toolbarButtonSize)))
					{
						CollapseAllNodes();
					}
					
					
					GUILayout.Space(20);
					if (rootGraph.selectedNodes.Count == 0)
					{
						GUI.enabled = false;	
					}
					else
					{
						GUI.enabled = true;	
					}
					if (GUILayout.Button(alignLeftIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						AlignLeft();
					}
					
					if (GUILayout.Button(alignRightIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						AlignRight();
					}
					
					if (GUILayout.Button(alignTopIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						AlignTop();
					}
					
					if (GUILayout.Button(alignBottomIcon, GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						AlignBottom();
					}
					
					//if (GUILayout.Button(new GUIContent(distributeHorizontalIcon, "Distribute selected nodes horizontally"), GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					//{
					//	DistributeNodes(GraphEditor.NodeAlignment.Horizontally);
					//}
					
					//if (GUILayout.Button(new GUIContent("V", "Distribute selected nodes vertically"), GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					//{
					//	DistributeNodes(GraphEditor.NodeAlignment.Vertically);
					//}
					
					if (GUILayout.Button(new GUIContent(distributeHorizontalIcon, "Distribute selected nodes automatically"), GUILayout.Height(toolbarButtonSize), GUILayout.Width(toolbarButtonSize)))
					{
						DistributeNodes(GraphEditor.NodeAlignment.HorizontallyAndVertically);
					}
					
					GUI.enabled = true;	

					#if FLOWREACTOR_DEBUG
					GUILayout.Space(100);
					
					GUI.color = Color.yellow;
					if (GUILayout.Button("Unhide nodes", GUILayout.Height(toolbarButtonSize), GUILayout.Width(100)))
					{
						DEBUG_UnhideNodesAndGraphsFromAsset();
					}
					
					if (GUILayout.Button("Hide nodes", GUILayout.Height(toolbarButtonSize), GUILayout.Width(100)))
					{
						DEBUG_HideNodesAndGraphsFromAsset();
					}
					
					if (GUILayout.Button("Cleanup root subgraphs", GUILayout.Height(toolbarButtonSize), GUILayout.Width(100)))
					{
						DEBUG_CleanupRootSubGraphs();
					}
					
					if (GUILayout.Button("Cleanup nodes withour graphowner", GUILayout.Height(toolbarButtonSize), GUILayout.Width(100)))
					{
						DEBUG_CleanupNodesWithoutGraphOwner();
					}
					GUI.color = Color.white;
					#endif
					
				
				}
			}
			
		
		}
		
		void MiniMap()
		{
			
			var _miniMapRect = new Rect(position.width - rootGraph.minimapSize.x, position.height - (rootGraph.minimapSize.y + 2), rootGraph.minimapSize.x, rootGraph.minimapSize.y);
			var _nodeBounds = ResolveMinimapBoundRect();
			var _focusRect = TransformRectSpace(new Rect(rootGraph.showInspector ? nodeCanvas.x - rootGraph.tmpInspectorWidth : nodeCanvas.x, nodeCanvas.y, rootGraph.showInspector ? position.width - rootGraph.tmpInspectorWidth : nodeCanvas.width, nodeCanvas.height), _nodeBounds, _miniMapRect);
			
			
			// MINIMAP EVENTS			
			var resizeRect = new Rect(_miniMapRect.x - 6, _miniMapRect.y - 6 , 6, 6);
			EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeUpLeft);
				
		
			EditorGUIUtility.AddCursorRect(_miniMapRect, MouseCursor.MoveArrow);
			if (_miniMapRect.Contains(Event.current.mousePosition) && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown)
			{		
				rootGraph.selectedNode = null;
				rootGraph.selectedNodes = new Dictionary<int, Node>();
				
				var norm = Rect.PointToNormalized(_miniMapRect,Event.current.mousePosition);
				var pos = Rect.NormalizedToPoint(_nodeBounds, norm);
				FocusPosition(pos);
				rootGraph.dragMinimap = true;
				Repaint();
			}
			if ( currentEvent.rawType == EventType.MouseUp )
			{
				rootGraph.dragMinimap = false;
			}
			if (rootGraph.dragMinimap && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDrag) 
			{		
				var norm = Rect.PointToNormalized(_miniMapRect,  Event.current.mousePosition);
				var pos = Rect.NormalizedToPoint(_nodeBounds, norm);
				FocusPosition(pos);
				Repaint();
			}
			
			if ( currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && resizeRect.Contains(Event.current.mousePosition) ) 
			{
				rootGraph.resizeMinimap = true;
				currentEvent.Use();
			}
			if ( currentEvent.rawType == EventType.MouseUp ) 
			{
				rootGraph.resizeMinimap = false;
			}
			if ( rootGraph.resizeMinimap && currentEvent.type == EventType.MouseDrag ) 
			{

				rootGraph.minimapSize -= currentEvent.delta;			
				rootGraph.minimapSize = new Vector2(Mathf.Clamp(rootGraph.minimapSize.x, rootGraph.minimapMinSize.x, rootGraph.minimapSize.x), Mathf.Clamp(rootGraph.minimapSize.y, rootGraph.minimapMinSize.y, rootGraph.minimapSize.y));
			
				currentEvent.Use();

			}
		
		
		
			GUI.Box(_miniMapRect, "", "TextArea");
			GUI.Box(new Rect(_miniMapRect.x + 2, _miniMapRect.y + 1, _miniMapRect.width, _miniMapRect.height), "", editorSkin.GetStyle("Box"));
			GUI.Box(_focusRect, "");
			
			for (int n = 0; n < rootGraph.currentGraph.nodes.Count; n ++)
			{
				//if (rootGraph.currentGraph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Comment)
				//	continue;
					
				var _tmpNode = rootGraph.currentGraph.nodes[n];
				var _miniNodeRect = TransformRectSpace(_tmpNode.nodeRect, _nodeBounds, _miniMapRect);
							
				if (_tmpNode.hasError)
				{
					GUI.color = Color.red;
					GUI.Box(new Rect(_miniNodeRect.x - 5, _miniNodeRect.y - 5, _miniNodeRect.width + 10, _miniNodeRect.height + 10), "", editorSkin.GetStyle("BoxWhite"));
					GUI.color = Color.white;
				}
				
				switch (_tmpNode.nodeData.nodeType)
				{
					case NodeAttributes.NodeType.Group:
						GUI.color = new Color(_tmpNode.groupColor.r, _tmpNode.groupColor.g, _tmpNode.groupColor.b, 60f/255f);
						GUI.Box(_miniNodeRect, _tmpNode.title, "Box");
						break;
					case NodeAttributes.NodeType.Comment:
						GUI.color = Color.white;
						GUI.Box(_miniNodeRect, "", editorSkin.GetStyle("CommentNode"));
						break;
					default:
						GUI.color = _tmpNode.color;
						GUI.Box(_miniNodeRect, "", editorSkin.GetStyle("BoxWhite"));
						break;
				}
				
				// Draw node connections
				for (int e = 0; e < _tmpNode.outputNodes.Count; e++)
				{	            	
					if (_tmpNode.outputNodes[e] != null && _tmpNode.outputNodes[e].outputNode != null)
					{
					
						var _startRect = _miniNodeRect;
						_startRect = new Rect(_startRect.x, _startRect.y, _startRect.width, _startRect.height);
				            	
						var _miniNodeOutputRect = TransformRectSpace(_tmpNode.outputNodes[e].outputNode.nodeRect, _nodeBounds, _miniMapRect);
						var _endRect = _miniNodeOutputRect;

						var _startPos = new Vector3(_startRect.x + _startRect.width, _startRect.y + (_startRect.height / 2), 0);
						var _endPos = new Vector3(_endRect.x, _endRect.y + (_endRect.height / 2), 0 );
						Handles.DrawBezier(_startPos, _endPos, _startPos + Vector3.right * 10, _endPos+ Vector3.left * 10, _tmpNode.color, null, 2);

					}
				}	
				
				GUI.color = Color.white;
			
			}
			
			

			var resizeRect2 = new Rect(_miniMapRect.x - 10, _miniMapRect.y - 8, 20, 20);
			GUI.Label(resizeRect2, minimapResizeIcon);
			
		}
		
		Rect TransformRectSpace(Rect rect, Rect oldContainer, Rect newContainer) 
		{
			var result = new Rect();
			result.xMin = Mathf.Lerp(newContainer.xMin, newContainer.xMax, Mathf.InverseLerp(oldContainer.xMin, oldContainer.xMax, rect.xMin));
			result.xMax = Mathf.Lerp(newContainer.xMin, newContainer.xMax, Mathf.InverseLerp(oldContainer.xMin, oldContainer.xMax, rect.xMax));
			result.yMin = Mathf.Lerp(newContainer.yMin, newContainer.yMax, Mathf.InverseLerp(oldContainer.yMin, oldContainer.yMax, rect.yMin));
			result.yMax = Mathf.Lerp(newContainer.yMin, newContainer.yMax, Mathf.InverseLerp(oldContainer.yMin, oldContainer.yMax, rect.yMax));
			return result;
		}

		Rect ResolveMinimapBoundRect() 
		{

			var arr1 = new Rect[rootGraph.currentGraph.nodes.Count];
			for ( var i = 0; i < rootGraph.currentGraph.nodes.Count; i++ )
			{
				arr1[i] = rootGraph.currentGraph.nodes[i].nodeRect;
			}

			var _nBounds = GetRectBoundRect(arr1);
			_nBounds = new Rect(_nBounds.x - 200, _nBounds.y - 200, _nBounds.width + 400, _nBounds.height + 400);
			var _finalBound = _nBounds;
			
			return _finalBound;
		}

		Rect GetRectBoundRect(Rect[] positions)
		{
			var _xMin = float.PositiveInfinity;
			var _xMax = float.NegativeInfinity;
			var _yMin = float.PositiveInfinity;
			var _yMax = float.NegativeInfinity;

			for ( var i = 0; i < positions.Length; i++ ) {
				_xMin = Mathf.Min(_xMin, positions[i].x);
				_xMax = Mathf.Max(_xMax, positions[i].x);
				_yMin = Mathf.Min(_yMin, positions[i].y);
				_yMax = Mathf.Max(_yMax, positions[i].y);
			}

			return Rect.MinMaxRect(_xMin, _yMin, _xMax, _yMax);
		}
		
		// Used with an offset for group node
		Rect GetVector2BoundRect(Vector2[] _points)
		{
			var _minX = _points.Min(p => p.x);
			var _minY = _points.Min(p => p.y);
			var _maxX = _points.Max(p => p.x);
			var _maxY = _points.Max(p => p.y);
			
			return new Rect((_minX - 150), (_minY - 100), (_maxX-_minX) + 300, (_maxY-_minY) + 200);
		}
		
		//void NodeSidePanel()
		//{
		//	//NodePanelPopup.NodePanel.DrawPanelRect(position);
		//	if (nodeSidePanel == null)
		//	{
		//		nodeSidePanel= new NodePanelPopup.NodePanel(position, rootGraph, this);
				
		//	}
			
		//	nodeSidePanel.DrawPanelRect(position);
		//}
		
		void CreateComment()
		{
			NodeCreator.CreateNode(null, rootGraph, rootGraph.currentGraph, "Comment");
		}
		
		void CreateGroupFromSelected()
		{
			var _group = NodeCreator.CreateNode(null, rootGraph, rootGraph.currentGraph, "Group");
			
			Vector2[] _points = new Vector2[rootGraph.selectedNodes.Count];
			
			int _index = 0;
			foreach(var key in rootGraph.selectedNodes.Keys)
			{
				_points[_index] = new Vector2(rootGraph.selectedNodes[key].nodeRect.center.x, rootGraph.selectedNodes[key].nodeRect.center.y);
				_index ++;
			}
			
			if (zoomFactor != 1)
			{
				FocusCanvas(false);
			}
			
			_group.nodeRect = GetVector2BoundRect(_points);
		}
		
	
		
		void CreateSubGraphFromSelected()
		{
			//Debug.Log("root graph " + rootGraph.name);
			Graph _subGraph = Graph.CreateSubGraph("SubGraph", rootGraph.currentGraph, rootGraph);
			
			_subGraph.isRoot = false;
			_subGraph.rootGraph = rootGraph;
			
			rootGraph.currentGraph.subGraphs.Add(_subGraph);
			
			Vector2 _position = Vector2.zero;
			
			// Add selected nodes to subgraph
			foreach (var key in rootGraph.selectedNodes.Keys)
			{
				_subGraph.nodes.Add(rootGraph.selectedNodes[key]);
				rootGraph.selectedNodes[key].graphOwner = _subGraph;
				
				rootGraph.currentGraph.nodes.Remove(rootGraph.selectedNodes[key]);
				
				// disconnect input and output nodes which do not belong to subgraph
				for (int i = 0; i < rootGraph.selectedNodes[key].inputNodes.Count; i ++)
				{
					var _isInSelection = false;
					Node _nodeNotInSelection = rootGraph.selectedNodes[key].inputNodes[i].inputNode;
					foreach(var inputKey in rootGraph.selectedNodes.Keys)
					{
						if (rootGraph.selectedNodes[key].inputNodes[i].inputNode == rootGraph.selectedNodes[inputKey])
						{
							_isInSelection = true;
						}
					}
					
					if (!_isInSelection)
					{
						rootGraph.selectedNodes[key].RemoveInputNode(_nodeNotInSelection);
						
						for (int m = 0; m < _nodeNotInSelection.outputNodes.Count; m ++)
						{
							if (_nodeNotInSelection.outputNodes[m].outputNode == rootGraph.selectedNodes[key])
							{
								_nodeNotInSelection.outputNodes[m].outputNode = null;
							}
						}
					}
				}
				
				for (int o = 0; o < rootGraph.selectedNodes[key].outputNodes.Count; o ++)
				{
					var _isInSelection = false;
					Node _nodeNotInSelection = rootGraph.selectedNodes[key].outputNodes[o].outputNode;
					foreach (var outputKey in rootGraph.selectedNodes.Keys)
					{
						if (rootGraph.selectedNodes[key].outputNodes[o].outputNode == rootGraph.selectedNodes[outputKey])
						{
							_isInSelection = true;
						}
					}
					
					if (!_isInSelection)
					{
						if (_nodeNotInSelection != null)
						{
							_nodeNotInSelection.RemoveInputNode(rootGraph.selectedNodes[key]);
						}
						
						rootGraph.selectedNodes[key].outputNodes[o].outputNode = null;
						
					}
				}
				
	
				
				
				_position += rootGraph.selectedNodes[key].nodeRect.center;
				
				
				// clean up subgraph list
				// if we're nesting a selected subgraph we need to remove it from the current subgraph list and add it to the newly created subgraph
				if (rootGraph.selectedNodes[key].nodeData.nodeType == NodeAttributes.NodeType.SubGraph)
				{
					var _subgraphNode = rootGraph.selectedNodes[key] as SubGraph;
					rootGraph.currentGraph.subGraphs.Remove(_subgraphNode.subGraph);
					
					_subGraph.subGraphs.Add(_subgraphNode.subGraph);
				}
			}
			
		
	
			
			_position /= rootGraph.selectedNodes.Keys.Count;
		
			var _subGraphNode = NodeCreator.CreateNode(null, rootGraph, rootGraph.currentGraph, "SubGraph", _position);
		
			var _sub = _subGraphNode as SubGraph;
			_sub.subGraph = _subGraph;
			_subGraph.subGraphNode = _sub;
			
			Vector2 _enterGraphNodePosition = new Vector2(_position.x - 400, _position.y);
			Vector2 _exitGraphNodePosition = new Vector2(_position.x - 400, _position.y + 80);
		
			// Create OnEnterGraph and OnExitGraph node
			NodeCreator.CreateNode(null, rootGraph, _subGraph, "OnEnterGraph", _enterGraphNodePosition);
			NodeCreator.CreateNode(null, rootGraph, _subGraph, "ExitGraph", _exitGraphNodePosition);
		
		
		
			// assign new rootgraph to all sub nodes
			for (int s = 0; s <rootGraph.subGraphs.Count; s++)
			{
				rootGraph.subGraphs[s].AssignNewRootGraph(rootGraph);
			}
		}
		
		void ExpandAllNodes()
		{
			if (rootGraph.selectedNodes != null && rootGraph.selectedNodes.Keys.Count > 0)
			{
				foreach (var node in rootGraph.selectedNodes.Keys)
				{
					
					rootGraph.selectedNodes[node].OpenNodeVariableUI();
				}
			}
			else
			{
				rootGraph.ExpandAllNodes();
			}
		}
		
		void CollapseAllNodes()
		{
			if (rootGraph.selectedNodes != null && rootGraph.selectedNodes.Keys.Count > 0)
			{
				foreach (var node in rootGraph.selectedNodes.Keys)
				{
					rootGraph.selectedNodes[node].CloseNodeVariableUI();
				}
			}
			else
			{
				rootGraph.CollapseAllNodes();
			}
		}
		
		void AlignLeft()
		{
			Undo.SetCurrentGroupName( "Align Nodes Left" );
			var undoGroup = Undo.GetCurrentGroup();
			
			var _xPos = Mathf.Infinity;
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				if (rootGraph.selectedNodes[node].nodeRect.x < _xPos)
				{
					_xPos = rootGraph.selectedNodes[node].nodeRect.x;
				}
			}
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				var _r = rootGraph.selectedNodes[node].nodeRect;
				Undo.RecordObject(rootGraph.selectedNodes[node], "Align Nodes Left" );
				rootGraph.selectedNodes[node].nodeRect = new Rect(_xPos, _r.y, _r.width, _r.height);
			}
			
			Undo.CollapseUndoOperations(undoGroup);
		}
		
		void AlignRight()
		{
			Undo.SetCurrentGroupName( "Align Nodes Right" );
			var undoGroup = Undo.GetCurrentGroup();
			
			var _xPos = -Mathf.Infinity;
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				if (rootGraph.selectedNodes[node].nodeRect.x > _xPos)
				{
					_xPos = rootGraph.selectedNodes[node].nodeRect.x;
				}
			}
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				var _r = rootGraph.selectedNodes[node].nodeRect;
				Undo.RecordObject(rootGraph.selectedNodes[node], "Align Nodes Right" );
				rootGraph.selectedNodes[node].nodeRect = new Rect(_xPos, _r.y, _r.width, _r.height);
			}
			
			Undo.CollapseUndoOperations(undoGroup);
		}
		
		void AlignTop()
		{
			Undo.SetCurrentGroupName( "Align Nodes Top" );
			var undoGroup = Undo.GetCurrentGroup();
			
			var _yPos = Mathf.Infinity;
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				if (rootGraph.selectedNodes[node].nodeRect.y < _yPos)
				{
					_yPos = rootGraph.selectedNodes[node].nodeRect.y;
				}
			}
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				var _r = rootGraph.selectedNodes[node].nodeRect;
				Undo.RecordObject(rootGraph.selectedNodes[node], "Align Nodes Top" );
				rootGraph.selectedNodes[node].nodeRect = new Rect(_r.x, _yPos, _r.width, _r.height);
			}
			
			Undo.CollapseUndoOperations(undoGroup);
		}
		
		void AlignBottom()
		{
			Undo.SetCurrentGroupName( "Align Nodes Bottom" );
			var undoGroup = Undo.GetCurrentGroup();
			
			var _yPos = -Mathf.Infinity;
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				if (rootGraph.selectedNodes[node].nodeRect.y > _yPos)
				{
					_yPos = rootGraph.selectedNodes[node].nodeRect.y;
				}
			}
			
			foreach (var node in rootGraph.selectedNodes.Keys)
			{
				var _r = rootGraph.selectedNodes[node].nodeRect;
				Undo.RecordObject(rootGraph.selectedNodes[node], "Align Nodes Bottom" );
				rootGraph.selectedNodes[node].nodeRect = new Rect(_r.x, _yPos, _r.width, _r.height);
			}
			
			Undo.CollapseUndoOperations(undoGroup);
		}
		
		
		void DistributeNodes(GraphEditor.NodeAlignment _alignment)
		{
			if (rootGraph.selectedNodes == null || rootGraph.selectedNodes.Count == 0)
				return;
				
			float _space = 50;
			var _xPos = Mathf.Infinity;
			
			List<Node> sortedList = rootGraph.selectedNodes.Values.ToList();
		
			_xPos = sortedList[0].nodeRect.x;
		
			List<Node> _firstNodes = new List<Node>();
		
			for (int s = 0; s < sortedList.Count; s ++)
			{
				sortedList[s].hasBeenAligned = false;
			}
			
			// Search the first node
			for (int s = 0; s < sortedList.Count; s ++)
			{
				bool _hasConnection = false;
				for (int i = 0; i < sortedList[s].inputNodes.Count; i ++)
				{
					for (int n = 0; n < sortedList.Count; n ++)
					{
						if (sortedList[s].inputNodes[i].inputNode.guid == sortedList[n].guid)
						{
							_hasConnection = true;
						}
					}
				}
				
				if (!_hasConnection)
				{
					_firstNodes.Add(sortedList[s]);
				}
			}
		
			if (_firstNodes.Count > 0)
			{
				// Traverse all next nodes from first node
				for (int f = 0; f < _firstNodes.Count; f ++)
				{
					if (f > 0)
					{
						_firstNodes[f].AlignNode(_firstNodes[f].nodeRect.x, _firstNodes[f].nodeRect.y, _firstNodes[f], _firstNodes[f-1], sortedList, _alignment, settings);
					}
					else
					{
						_firstNodes[f].AlignNode(_firstNodes[f].nodeRect.x, _firstNodes[f].nodeRect.y, _firstNodes[f], _firstNodes[0], sortedList, _alignment, settings);
					}
				}
			}
			else
			{
				
				sortedList = sortedList.OrderBy(o=>o.nodeRect.x).ToList();
				var _lastWidth = 0f;
				for (int n = 0; n < sortedList.Count; n ++)
				{
					var _r = sortedList[n].nodeRect;
					
					sortedList[n].nodeRect = new Rect(_xPos + _lastWidth, _r.y, _r.width, _r.height);
					_lastWidth += _r.width + _space;
				}
			}
		}

		void GotoParentGraph()
		{
			var _p = rootGraph.parentLevels.Count - 2;
			if (_p >= 0)
			{
				rootGraph.parentLevels[0].name = rootGraph.name;
							    
				isDragging = false;
				rootGraph.isMouseOverNode = false;
				rootGraph.selectedNode = null;
				rootGraph.selectedNodes = new Dictionary<int, Node>();
					    				
				for (int s = rootGraph.parentLevels.Count - 1; s > _p; s --)
				{
					rootGraph.parentLevels.RemoveAt(s);
				}	

				rootGraph.currentGraph = rootGraph.parentLevels[_p].graph;
			}
		}
		
		public void GotoNode(Node _node)
		{
			// Rebuild graph levels and search for graph
			var _path = "";
			var _graphList = new List<Graph>();
			var _rootGraph = _node.rootGraph;
			var _graphOwner = _node.graphOwner;
			var _p = _rootGraph.TraverseGraph(_graphOwner, "", out _path, _graphList, out _graphList);
						
			var _graphs = _path.Split("/"[0]);
							
			_rootGraph.parentLevels = new List<Graph.ParentLevels>();
					
			var _index = 0;
							
			foreach (var p in _graphs)
			{
				_rootGraph.parentLevels.Add(new Graph.ParentLevels(p, _graphList[_index]));
				_index ++;
			}

						
			_rootGraph.isMouseOverNode = false;
			_rootGraph.selectedNode = null;
			_rootGraph.selectedNodes = new Dictionary<int, FlowReactor.Nodes.Node>();

			_rootGraph.currentGraph = _graphOwner;
							
			FocusPosition(_node.nodeRect.center);
							
			_rootGraph.selectedNodes = new Dictionary<int, FlowReactor.Nodes.Node>();
			_rootGraph.selectedNodes.Add(_node.GetInstanceID(), _node);
			
		}
		
		// Only for debugging
		void DEBUG_UnhideNodesAndGraphsFromAsset()
		{
			for (int i = 0; i < rootGraph.nodes.Count; i ++)
			{
				rootGraph.nodes[i].hideFlags = HideFlags.None;
			}
			
			for (int i = 0; i < rootGraph.subGraphs.Count; i++)
			{
				rootGraph.subGraphs[i].DEBUG_UnhideAllNodes();
			}
			
			AssetDatabase.Refresh();
		}
		
		void DEBUG_HideNodesAndGraphsFromAsset()
		{
			for (int i = 0; i < rootGraph.nodes.Count; i ++)
			{
				rootGraph.nodes[i].hideFlags = HideFlags.HideInHierarchy;
			}
			
			for (int i = 0; i < rootGraph.subGraphs.Count; i++)
			{
				rootGraph.subGraphs[i].DEBUG_HideAllNodes();
			}
			
			
			AssetDatabase.Refresh();
		}
		
		void DEBUG_CleanupRootSubGraphs()
		{
			Debug.Log("cleanup: " + rootGraph.name);
			for (int s = 0; s < rootGraph.subGraphs.Count; s ++)
			{
				if (rootGraph.subGraphs[s] == null)
					continue;
					
				if (rootGraph.subGraphs[s].subGraphNode == null)
				{
					AssetDatabase.RemoveObjectFromAsset(rootGraph.subGraphs[s]);
				}
			}
			
			for (int s = 0; s < rootGraph.subGraphs.Count; s ++)
			{
				if (rootGraph.subGraphs[s] == null)
				{
					rootGraph.subGraphs.RemoveAt(s);
				}
			}
		}
		
		void DEBUG_CleanupNodesWithoutGraphOwner()
		{
			Debug.Log("cleanup: " + rootGraph.name);
			
			for (int n = 0; n < rootGraph.nodes.Count; n ++)
			{
				if (rootGraph.nodes[n].graphOwner == null)
				{
					AssetDatabase.RemoveObjectFromAsset(rootGraph.nodes[n]);
				}
			}
			
			for (int s = 0; s < rootGraph.subGraphs.Count; s ++)
			{
				for (int n = 0; n < rootGraph.subGraphs[s].nodes.Count; n ++)
				{
					if (rootGraph.subGraphs[s].nodes[n].graphOwner == null)
					{
						AssetDatabase.RemoveObjectFromAsset(rootGraph.subGraphs[s].nodes[n]);
					}
				}
			}
			
		}
	}
}
#endif