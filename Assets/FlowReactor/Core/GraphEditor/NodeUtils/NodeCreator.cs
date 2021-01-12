//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	Node creation class. Handles all node creation methods. New, Copy, Copy at runtime etc.
*/
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public class NodeCreator
	{
	
		static NodeCategoryTree allNodes;
	
		
		public static Node CreateNode(NodeCategoryTree.NodeData _nodeData, Graph _rootGraph, Graph graphOwner, string _type)
		{
			Vector2 _position = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y) * _rootGraph.zoomFactor;
			return CreateNode(_nodeData, _rootGraph, graphOwner, _type, _position, false);
		}
		
		public static Node CreateNode(NodeCategoryTree.NodeData _nodeData, Graph _rootGraph, Graph _graphOwner, string _type, Vector2 _position)
		{
			return CreateNode(_nodeData, _rootGraph, _graphOwner, _type, _position, false);
		}
		
		public static Node CreateNode(NodeCategoryTree.NodeData _nodeData, Graph _rootGraph, Graph _graphOwner, string _type, Vector2 _position, bool _isCopy)
		{
		

			if (_nodeData == null)
			{
				allNodes = CollectAvailableNodes.CollectNodes();
				_nodeData = allNodes.GetData(_type);
			}
		
			Node node = Node.CreateNode(_type, _nodeData.typeName);

			// Assign attributes to node
			node.outputNodes = new List<Node.NodeOutput>(); //(_nodeAttributes.outputSlotCount);
			if (_nodeData.outputSlotCount > -1)
			{
				for (int i = 0; i < _nodeData.outputSlotCount; i ++)
				{
					node.outputNodes.Add(new Node.NodeOutput());
				}
			}
			if (_nodeData.nodeOutputs != null)
			{
				for (int i = 0; i < _nodeData.nodeOutputs.Length; i ++)
				{
					node.outputNodes.Add(new Node.NodeOutput(_nodeData.nodeOutputs[i]));
				}
			}
			

			//_position = new Vector2(_position.x / _rootGraph.zoomFactor, _position.y / _rootGraph.zoomFactor);

			node.inputNodes = new List<Node.InputNode>();
			node.nodeRect = new Rect(_position.x, _position.y, node.nodeRect.width, node.nodeRect.height);
		
			// Get settings 
			var _editorSettings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;   
		
			node.color = _editorSettings.GetColor(_nodeData.color);
			node.nodeData = _nodeData;
			node.isCopy = _isCopy;
			node.guid = Guid.NewGuid().ToString();
			
			_graphOwner.AddNode(node);
		
			node.rootGraph = _rootGraph;
			node.graphOwner = _graphOwner;
			//node.nodeStringType = _type;
			if (!_isCopy)
			{
				node.Init(_rootGraph, node); // _graphOwner.nodes.Count, node);
			}
			
			if (node.outputNodes.Count > 1)
			{
				node.nodeRect.height += (node.outputNodes.Count - 1) * 20;
			}
			
			node.SetupVariables();
			
			// check if newly created node has been created inside a group
			// if true, add new node to this group
			for( int n = 0; n < _rootGraph.currentGraph.nodes.Count; n ++)
			{
				if (_rootGraph.currentGraph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Group &&
					_rootGraph.currentGraph.nodes[n] != node && _rootGraph.currentGraph.nodes[n].nodeRect.Contains(new Vector2(node.nodeRect.x, node.nodeRect.y)))
				{
					var _g = _rootGraph.currentGraph.nodes[n] as Group;
					_g.AddNodeToGroup(node);
				}
			}
		
			EditorUtility.SetDirty(node);
			EditorUtility.SetDirty(_rootGraph);
			
			node.lastZoomCoordsOrigin = _rootGraph.currentGraph.zoomCoordsOrigin;
			
			// Better than AssetDatabase SaveAsset or Refresh for refreshing node
			Undo.SetCurrentGroupName( "Create Node" );
			int undoGroup = Undo.GetCurrentGroup();
			Undo.RecordObject(	node, "Create Node");
			Undo.CollapseUndoOperations( undoGroup );
			
	
			return node;
			
		}
		
		
		public static Node CreateNodeFromCopyRuntime(NodeCategoryTree.NodeData _nodeData, Node _originalNode, Graph _rootGraph, Graph _graphOwner, string _type)
		{
			Node _n = CreateNode(_originalNode.nodeData, _rootGraph, _graphOwner, _type, new Vector2(_originalNode.nodeRect.x, _originalNode.nodeRect.y), true);
		
			CopyAll(_originalNode, _n);		
			
			_n.node = _n;
			_n.rootGraph = _rootGraph;
			_n.graphOwner = _graphOwner;
			_n.inputNodes = new List<Node.InputNode>();
			_n.nodeData = new NodeCategoryTree.NodeData(_originalNode.nodeData.title, _originalNode.nodeData.typeName, _originalNode.nodeData.nameSpace, _originalNode.nodeData.category, _originalNode.nodeData.description, _originalNode.nodeData.color, _originalNode.nodeData.outputSlotCount, _originalNode.nodeData.nodeOutputs, _originalNode.nodeData.nodeType);
			
			_n.nodeRect = new Rect(_n.nodeRect.position.x + 20, _n.nodeRect.position.y + 20, _n.nodeRect.width, _n.nodeRect.height);
			
			return _n;
		}
		
		public static void CopyAll<T>(T source, T target)
		{
			var type = typeof(T);
			foreach (var sourceProperty in type.GetProperties())
			{
				var targetProperty = type.GetProperty(sourceProperty.Name);
				targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
			}
			foreach (var sourceField in type.GetFields())
			{
				var targetField = type.GetField(sourceField.Name);
				targetField.SetValue(target, sourceField.GetValue(source));
			}       
		}
		
	
		public static Node CreateNodeFromCopy(NodeCategoryTree.NodeData _nodeData, Node _originalNode, Graph _rootGraph, Graph _graphOwner, string _type, int _copyPastNodeOffsetPositionMultiplier)
		{
			Node _n = CreateNode(_originalNode.nodeData, _rootGraph, _graphOwner, _type, new Vector2(_originalNode.nodeRect.x, _originalNode.nodeRect.y));
			EditorUtility.CopySerialized(_originalNode, _n);
			
		
			_n.node = _n;
			_n.rootGraph = _rootGraph;
			_n.graphOwner = _graphOwner;
			_n.inputNodes = new List<Node.InputNode>();
			_n.nodeData = new NodeCategoryTree.NodeData(_originalNode.nodeData.title, _originalNode.nodeData.typeName, _originalNode.nodeData.nameSpace, _originalNode.nodeData.category, _originalNode.nodeData.description, _originalNode.nodeData.color, _originalNode.nodeData.outputSlotCount, _originalNode.nodeData.nodeOutputs, _originalNode.nodeData.nodeType);
			
			_n.nodeRect = new Rect(_n.nodeRect.position.x + (20 * _copyPastNodeOffsetPositionMultiplier + 1), _n.nodeRect.position.y + (20 * _copyPastNodeOffsetPositionMultiplier + 1), _n.nodeRect.width, _n.nodeRect.height);
			
			return _n;
		}
		
		
		public static void CreateDefaultNodes(NodeCategoryTree _tree, Graph _graph)
		{
		
			var _nodeData = _tree.GetData("OnStart");
			
			NodeCreator.CreateNode(_nodeData, _graph, _graph, "OnStart", new Vector2(100, 100));
		
			_graph.parentLevels = new List<Graph.ParentLevels>();
			_graph.parentLevels.Add(new Graph.ParentLevels(_graph.name, _graph));
			
		
		}
		
		
		public static void CreateNewGraphWithDefaultNodes()
		{
			CreateNewGraphWithDefaultNodes(null);	
		}
		
		public static void CreateNewGraphWithDefaultNodes(FlowReactorComponent _fr)
		{
			
			var _path = EditorUtility.SaveFilePanelInProject("Create new graph", "graph", "asset", "Please enter a graph name");
			
			if (string.IsNullOrEmpty(_path))
				return;
				
			var _name = System.IO.Path.GetFileName(_path);
			
			// create new graph, state and start node
			Graph graph = Graph.CreateGraph(_name, _path);
			graph.nodes = new List<Node>();
		
			if (_fr != null)
			{
				_fr.graph = graph;
			}
			
			NodeCreator.CreateNode(null, graph, graph, "OnStart", new Vector2(100, 100));
		
			graph.parentLevels = new List<Graph.ParentLevels>();
			graph.parentLevels.Add(new Graph.ParentLevels("Root", graph));
		}
		
	
	}
}
#endif