//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEditor;

using FlowReactor.Nodes;
using FlowReactor.EventSystem;
using FlowReactor.OdinSerializer;
using FlowReactor.Editor;
using FlowReactor;


namespace FlowReactor
{
	[CreateAssetMenu(fileName = "Graph", menuName = "FlowReactor/New Graph", order = 0)]
	public class Graph : SerializedScriptableObject
	{
		// During runtime we need to keep track of all flowreactor components which are running this graph
		public Dictionary<int, FlowReactorComponent> flowReactorComponents;
		
		public string version;
		
		public bool isActive;
		public bool isRoot;
		public bool isCopy;
			
		// if this graph is not the root, store the root graph
		public Graph rootGraph; 
		public SubGraph subGraphNode;
		public SubGraphInstance subGraphNodeInstance;
		
		public List<Node> nodes;
		public List<Node> tmpNodes;
		public List<Graph> subGraphs;
		
		[System.Serializable]
		public class GraphInspectors
		{
			public int selectedTab;
			public float height;
			public Rect splitRect;
			public bool isDragging;
			
			public GraphInspectors(int _selected, float _height)
			{
				selectedTab = _selected;
				height = _height;
			}
		}
		public List<GraphInspectors> inspectors = new List<GraphInspectors>();

		// Store blackboards
		public class Blackboards
		{
			public FlowReactor.BlackboardSystem.BlackBoard blackboard;
			public FlowReactor.BlackboardSystem.BlackBoard lastBlackboard;
			
			public bool foldout;
			#if UNITY_EDITOR
			public FlowReactor.Editor.BlackBoardEditor blackboardEditor;
			#endif
			public Blackboards()
			{
				blackboard = null;
				lastBlackboard = null;
			}
		}
		
		public Dictionary<Guid, Blackboards> blackboards;
		
		// Store eventboards
		public class Eventboards
		{
			public EventBoard eventboard;
			public bool foldout;
			#if UNITY_EDITOR
			public FlowReactor.Editor.EventBoardEditor eventboardEditor;
			#endif
			
			public Eventboards()
			{
				eventboard = null;
			}
		}
		
		public Dictionary<Guid, Eventboards> eventboards;


		[System.Serializable]
		public class ExposedVariables
		{
			public Node node;
			public Dictionary<string, FRVariable> variables;
			
			public ExposedVariables()
			{
				variables = new Dictionary<string, FRVariable>();
			}
			public ExposedVariables(Node _node, string _variableName, FRVariable _variable)
			{
				node = _node;
				
				if (variables == null)
				{
					variables = new Dictionary<string, FRVariable>();
				}
				
				_variable.exposedName = _variableName;
				variables.Add(_variableName, _variable);
			}
		}
		
		public Dictionary<string, ExposedVariables> exposedNodeVariables = new Dictionary<string, ExposedVariables>();
		
		
		//Editor Values
		public Vector2 zoomCoordsOrigin;
		public float zoomFactor = 1f;
		public float inspectorWidth;
		public float tmpInspectorWidth;
		public bool showInspector;
		public bool isMouseOverNode;
		public bool mouseDownOnInspector;
		public Node selectedNode;
		public Node lastSelectedNode;
		public Node currentNode;
		public Dictionary<int, Node> selectedNodes;
		public int selectedNodeIndex;
		public int lastSelectedNodeIndex;
		public bool drawModeOn;
		public bool selectionBoxDragging;
		public bool isNodeDragging;
		public int highlightHandleIndex;
		public Vector2 minimapSize;
		public bool showMinimap;
		public bool dragMinimap;
		//public bool showNodeSidePanel;
		public bool resizeMinimap;
		public Vector2 minimapMinSize = new Vector2(100, 100);
		public bool foldout;
		
		public enum GlobalMouseEvents
		{
			mouseDown,
			mouseUp,
			mouseDrag,
			scrollWheel,
			ignore
		}
		
		public GlobalMouseEvents globalMouseEvents;
		public int globalMouseButton;
		
		public bool nodeSelectionPanelOpen;
		public bool dragNodeFromNodeMenu;
		public int lastSelectedOutput;
		public Vector2 nodeConnectPoint;
		public bool mouseClickInGroupNode;
		public Group currentGroupNodeDragging;
		
		
		// Keep track of the current open graph and sub-graph
		[System.Serializable]
		public class ParentLevels
		{
			public string name;
			public Graph graph;
		    	
			public ParentLevels (string _name, Graph _graph)
			{
				name = _name;
				graph = _graph;
			}
		}
		public List<ParentLevels> parentLevels;
		
		// The current graph which is being edited
		public Graph currentGraph;

		
		public void OnEnable()
		{
			flowReactorComponents = new Dictionary<int, FlowReactorComponent>();
		}
		
		public void OnDisable()
		{
			// Cleanup empty subgraphs
			for (int s = 0; s < subGraphs.Count; s++)
			{
				if (subGraphs[s] == null)
				{
					subGraphs.RemoveAt(s);
				}
			}
			
		}
		
		// Copy this graph when running as Unique Instance
		public Graph Copy(Graph _rootGraph, SubGraphInstance _subGraphInstanceNode, SubGraph _subGraphNode)
		{
			
			Graph _copy = null;
			
			_copy = Instantiate(this);
			
			_copy.name = this.name + " (Instance)";
			_copy.flowReactorComponents = new Dictionary<int, FlowReactorComponent>();
			_copy.isActive = this.isActive;
			_copy.isRoot = this.isRoot;
			_copy.isCopy = true;
			_copy.inspectorWidth = 400;
			
			if (_subGraphNode != null)
			{
				#if FLOWREACTOR_DEBUG
				Debug.Log("assign subgraph node " + _copy.name + " " + _subGraphNode.name);
				#endif
				_copy.subGraphNode = _subGraphNode;
			}
			
			if (_rootGraph == null && _copy.isRoot)
			{
				_copy.rootGraph = _copy;
				_rootGraph = _copy;
			}
			else
			{
				_copy.rootGraph = _rootGraph;
			}
			
			_copy.currentGraph = this.currentGraph;
			
			if (_subGraphInstanceNode != null)
			{
				_copy.subGraphNodeInstance = _subGraphInstanceNode;
			}
		
			_copy.nodes = new List<Node>();
			
			var _copiedSubGraphs = new List<Graph>();
			var _copiedNodes = new List<Node>();

			for (int n = 0; n < this.nodes.Count; n ++)
			{
				var _copyNode = Instantiate(this.nodes[n]) as Node;
				_copyNode.name = _copyNode.name + " (Instance) "; // + Guid.NewGuid().ToString();
				_copy.nodes.Add(_copyNode);
				
				_copiedNodes.Add(_copyNode);
				_copy.nodes[n].isCopy = true;
			
				
				var _f = _copy.nodes[n].GetType().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
			
				for (int f = 0; f < _f.Length; f ++)
				{
					//Debug.Log(_f[f].FieldType.BaseType);
					if (_f[f].FieldType.BaseType == typeof(FRVariable))
					{
					
						var _v = (FRVariable)_f[f].GetValue(_copy.nodes[n]);
						
						//var _vCopy = (FRVariable)SerializationUtility.CreateCopy(_v);
						
						//_cf[f].SetValue(_copy.nodes[n], _vCopy);
						
						//var _cv = (FRVariable)_cf[f].GetValue(_copy.nodes[n]);
						//_cv.isCopy = true;
						
						//Debug.Log(_v.GetType());
						//_v.graph = (Graph)_copy;
						//_v.copyGraph = _copy;
					
						_v.isCopy = true;
						//var _cg = _f[f].FieldType.BaseType.GetField("copyGraph", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
						
						//_cg.SetValue(_v, (System.Object)_copy);
						
						var _cg2 = _f[f].FieldType.BaseType.GetField("_graph", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
						_cg2.SetValue(_v, (System.Object)_copy);
						

					}
		
				}
				
				

				
				if (_copyNode.nodeData.nodeType == NodeAttributes.NodeType.SubGraph)
				{
					#if FLOWREACTOR_DEBUG
					//Debug.Log("copy subgraph");
					#endif
					var _sub = _copyNode as SubGraph;
					var _copySub = _sub.subGraph.Copy(_rootGraph, null, _sub); //(_copy, null, _sub);
					
					_sub.subGraph = _copySub;
					_copiedSubGraphs.Add(_copySub);
				}
				
			
			
			}
			
			for (int n = 0; n < _copy.nodes.Count; n ++)
			{
				_copy.nodes[n].AssignNewOutAndInputs(_copy);
			}
			
			for (int n = 0; n < _copy.nodes.Count; n ++)
			{
				_copy.nodes[n].rootGraph = _rootGraph;
				_copy.nodes[n].AssignNewGraphInstance(_rootGraph, _copy);
			}
			
		
		
			_copy.subGraphs = _copiedSubGraphs; // new List<Graph>(this.subGraphs);
			if (this.blackboards != null)
			{
				_copy.blackboards = new Dictionary<Guid, Blackboards>(this.blackboards);
			}
			else
			{
				_copy.blackboards = new Dictionary<Guid, Blackboards>();
			}
			if (this.eventboards != null)
			{
				_copy.eventboards = new Dictionary<Guid, Eventboards>(this.eventboards);
			}
			else
			{
				_copy.eventboards = new Dictionary<Guid, Eventboards>();
			}
			
			_copy.parentLevels = new List<ParentLevels>(this.parentLevels);


			return _copy;
		}
		
		
		//public void CopyAll<T>(T source, T target)
		//{
		//	var type = typeof(T);
		//	foreach (var sourceProperty in type.GetProperties())
		//	{
		//		var targetProperty = type.GetProperty(sourceProperty.Name);
		//		targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
		//	}
		//	foreach (var sourceField in type.GetFields())
		//	{
		//		var targetField = type.GetField(sourceField.Name);
		//		targetField.SetValue(target, sourceField.GetValue(source));
		//	}       
		//}
		
		
		// EDITOR Subgraph copy method
		#if UNITY_EDITOR
		public Graph CopyEditorSubGraph(Graph _rootGraph, Graph _parentGraph, FlowReactor.Editor.FREditorSettings settings)
		{
			Graph _subGraph = Graph.CreateSubGraph("SubGraph", _parentGraph, _rootGraph);
			
			_subGraph.isRoot = false;
			_subGraph.isActive = false;
	
			_parentGraph.subGraphs.Add(_subGraph);


			var _copiedNodes = new List<Node>();
			var _selectedNodesList = new List<Node>(nodes);
			
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			
			for (int n = 0; n < _selectedNodesList.Count; n ++)
			{
				SerializationUtility.SerializeValue(_selectedNodesList[n], DataFormat.Binary, out settings.objectReferences);
				var _original1 = settings.objectReferences[0] as Node;
				var _newCopy = NodeCreator.CreateNodeFromCopy(_original1.nodeData, _original1, _rootGraph, _subGraph, _original1.nodeData.typeName, 0);
				
				
				_newCopy.outputNodes = new List<Node.NodeOutput>();
				_newCopy.inputNodes = new List<Node.InputNode>();
				
				for (int o = 0; o < _original1.outputNodes.Count; o ++)
				{
					_newCopy.outputNodes.Add(new Node.NodeOutput(_original1.outputNodes[o].outputNode));
					_newCopy.outputNodes[_newCopy.outputNodes.Count-1].guid = _original1.outputNodes[o].guid;
				}
				
				
				for (int i = 0; i < _original1.inputNodes.Count; i ++)
				{
					_newCopy.inputNodes.Add(new Node.InputNode(_original1.inputNodes[i].inputNode));
					_newCopy.inputNodes[_newCopy.inputNodes.Count-1].guid = _original1.inputNodes[i].guid;
				}
				
			
				
				rootGraph.selectedNodes.Add(_newCopy.GetInstanceID(), _newCopy);
		
				
				_copiedNodes.Add(_newCopy);
			}
			
			// connect in and outputs
			for (int c = 0; c < _copiedNodes.Count; c ++)
			{
				_copiedNodes[c].AssignNewOutAndInputsFromList(_copiedNodes);
				
			}
			
			
			// Assign new guids
			for (int n = 0; n < _copiedNodes.Count; n ++)
			{
				_copiedNodes[n].guid = Guid.NewGuid().ToString();
				
				for (int o = 0; o < _copiedNodes[n].outputNodes.Count; o ++)
				{
					if ( _copiedNodes[n].outputNodes[o].outputNode != null)
					{
						for (int i = 0; i < _copiedNodes[n].outputNodes[o].outputNode.inputNodes.Count; i ++)
						{
							if (_copiedNodes[n].outputNodes[o].outputNode.inputNodes[i].inputNode == _copiedNodes[n])
							{
								_copiedNodes[n].outputNodes[o].outputNode.inputNodes[i].guid = _copiedNodes[n].guid;
							}
						}
					}
				}
				
				for (int o = 0; o < _copiedNodes[n].inputNodes.Count; o ++)
				{
					for (int i = 0; i < _copiedNodes[n].inputNodes[o].inputNode.outputNodes.Count; i ++)
					{
						if (_copiedNodes[n].inputNodes[o].inputNode.outputNodes[i].outputNode == _copiedNodes[n])
						{
							_copiedNodes[n].inputNodes[o].inputNode.outputNodes[i].guid = _copiedNodes[n].guid;
						}
					}
				}
			}
			
			
			for (int h = 0; h < _copiedNodes.Count; h ++)
			{
				if (_copiedNodes[h].nodeData.nodeType == NodeAttributes.NodeType.SubGraph)
				{				
					var _subGraphNode = _copiedNodes[h] as SubGraph;
					var _cs = _subGraphNode.subGraph.CopyEditorSubGraph(_rootGraph, _subGraph, settings);
					
					_subGraphNode.subGraph = _cs;
					
					_cs.subGraphNode = _subGraphNode;
				}
			}
			
			return _subGraph;
		}
	
		public static Graph CreateGraph(string _name, string _path)
		{
			Graph graph = CreateInstance<Graph>();
			graph.hideFlags = HideFlags.None;
			graph.name = _name;
			graph.isRoot = true;
			graph.isActive = true;
			graph.rootGraph = graph;
			graph.minimapSize = new Vector2(100, 100);
			graph.inspectorWidth = 400;
			
			graph.nodes = new List<Node>();
			graph.tmpNodes = new List<Node>();
			graph.subGraphs = new List<Graph>();
			
			graph.flowReactorComponents = new Dictionary<int, FlowReactorComponent>();
			graph.blackboards = new Dictionary<Guid, Blackboards>();
			graph.eventboards = new Dictionary<Guid, Eventboards>();
			graph.parentLevels = new List<ParentLevels>();
			
			
			AssetDatabase.CreateAsset(graph, _path);
			
			return graph;
		}
		
		public static Graph CreateSubGraph(string _name, Graph parentGraph, Graph _rootGraph)
		{
			Graph graph = CreateInstance<Graph>();
			graph.hideFlags = HideFlags.HideInHierarchy;
			graph.name = _name;
			graph.isRoot = false;
			graph.isActive = false;
			graph.rootGraph = _rootGraph;
			
			graph.nodes = new List<Node>();
			graph.tmpNodes = new List<Node>();
			graph.subGraphs = new List<Graph>();
			
			graph.flowReactorComponents = new Dictionary<int, FlowReactorComponent>();
			
			parentGraph.AddSubGraph(graph);
			
			return graph;
			
		}
		
		
		void AddSubGraph(Graph _graph)
		{
			AssetDatabase.AddObjectToAsset(_graph, this);
		}
		
		
		public void AddNode(Node node)
		{		
			node.graphOwner = this;
			nodes.Add(node);
			
			if (!node.isCopy)
			{
				AssetDatabase.AddObjectToAsset(node, this);
			}
		}
		#endif
		
		public void AssignNewRootGraph(Graph _rootGraph)
		{
					
			rootGraph = _rootGraph;
			
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].rootGraph = _rootGraph;	
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].AssignNewRootGraph(_rootGraph);	
			}
		}
		
	
		
		#if UNITY_EDITOR
		public void ReloadColors(FlowReactor.Editor.FREditorSettings _settings)
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].ReloadColor(_settings);	
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].ReloadColors(_settings);	
			}
		}
		
		
		public void ReInitializeNodes()
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].ReInitialize();	
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].ReInitializeNodes();	
			}
		}
		
		public void ExpandAllNodes()
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].OpenNodeVariableUI();
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].ExpandAllNodes();	
			}
		}
		
		public void CollapseAllNodes()
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].CloseNodeVariableUI();	
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].CollapseAllNodes();	
			}
		}
		
		public void RegisterINodeControllables(FlowReactorComponent _fr)
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].RegisterINodeControllables(_fr);
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null)
				{
					subGraphs[s].RegisterINodeControllables(_fr);
				}
			}
		}
		
		public bool FindINodeControllable(Node _node)
		{
			bool _return = false;
			for (int n = 0; n < nodes.Count; n ++)
			{
				if (_node == nodes[n])
				{
					return true;
				}
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null)
				{
					_return = subGraphs[s].FindINodeControllable(_node);
				}
			}
			
			return _return;
		}
		
		#endif
	
		// Traverse all subgraphs until searchforgraph has been found
		// used by graph explorer when jumping directly to a subgraph
		public bool TraverseGraph(Graph _searchForGraph, string _parent, out string _path, List<Graph> _graphs, out List<Graph> _graphsOutput)
		{
			if (string.IsNullOrEmpty(_parent))
			{
				_parent = this.name;
			}
			else
			{
				_parent = _parent + "/" +  this.name;
			}
			_path = _parent;
			_graphsOutput = _graphs;
			_graphsOutput.Add (this);
			
			if (_searchForGraph == this)
			{
				return true;
			}
			else
			{
				for (int s = 0; s < subGraphs.Count; s ++)
				{
					var _ok =  subGraphs[s].TraverseGraph(_searchForGraph, _parent, out _path, _graphsOutput, out _graphsOutput);
					if (_ok)
					{
						return true;
					}
				}
			}
			return false;
		}
	
		public void HasError(bool _errorState)
		{
			if (!isRoot)
			{
				subGraphNode.hasError = _errorState;
				subGraphNode.graphOwner.HasError(_errorState);
				if (_errorState)
				{
					isActive = false;
				}
			}
		}
		
		
		public void Deactivate()
		{
			isActive = false;
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				subGraphs[s].Deactivate();	
			}
		}
		
		public void CallOnDisable(FlowReactorComponent _fr)
		{
			for (int i = 0; i < nodes.Count; i ++)
			{
				if (nodes[i] != null)
				{
					nodes[i].OnNodeDisable(_fr);
				}
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null)
				{
					subGraphs[s].CallOnDisable(_fr);
				}
			}
		}
		
		public void CallOnInitialize(FlowReactorComponent _fr)
		{
			for (int i = 0; i < nodes.Count; i ++)
			{
				if (nodes[i] != null)
				{
					nodes[i].OnInitialize(_fr);
					_fr.NodeControllable_OnNodeInitialization(nodes[i]);
				}
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null)
				{
					subGraphs[s].CallOnInitialize(_fr);
				}
			}
		}
		
		public void OnUpdate(FlowReactorComponent _flowReactor)
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].OnUpdate(_flowReactor);
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null && subGraphs[s].isActive)
				{
					subGraphs[s].OnUpdate(_flowReactor);
				}
			}
		}
		
		
		public void OnLateUpdate(FlowReactorComponent _flowReactor)
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].OnLateUpdate(_flowReactor);
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null && subGraphs[s].isActive)
				{
					subGraphs[s].OnLateUpdate(_flowReactor);
				}
			}
		}
		
		
		public void OnFixedUpdate(FlowReactorComponent _flowReactor)
		{
			for (int n = 0; n < nodes.Count; n ++)
			{
				nodes[n].OnFixedUpdate(_flowReactor);
			}
			
			for (int s = 0; s < subGraphs.Count; s ++)
			{
				if (subGraphs[s] != null && subGraphs[s].isActive)
				{
					subGraphs[s].OnFixedUpdate(_flowReactor);
				}
			}
		}
		
		#if UNITY_EDITOR
		public void UpdateGraph(Graph graph)
		{
			GraphUpdater.UpdateGraph(graph);
		}
		
		public void DEBUG_UnhideAllNodes()
		{
			hideFlags = HideFlags.None;
			
			for (int i = 0; i < nodes.Count; i ++)
			{
				Debug.Log(nodes[i].name);
				nodes[i].hideFlags = HideFlags.None;
			}
			
			for (int g = 0; g < subGraphs.Count; g ++)
			{
				subGraphs[g].hideFlags = HideFlags.None;
				
				subGraphs[g].DEBUG_UnhideAllNodes();
			}
			
			AssetDatabase.Refresh();
		}
		
		public void DEBUG_HideAllNodes()
		{
			hideFlags = HideFlags.HideInHierarchy;
			
			for (int i = 0; i < nodes.Count; i ++)
			{
				nodes[i].hideFlags = HideFlags.HideInHierarchy;
			}
			
			for (int g = 0; g < subGraphs.Count; g ++)
			{
				subGraphs[g].hideFlags = HideFlags.HideInHierarchy;
				subGraphs[g].DEBUG_HideAllNodes();
			}
			
			AssetDatabase.Refresh();
		}
		#endif
	}
}