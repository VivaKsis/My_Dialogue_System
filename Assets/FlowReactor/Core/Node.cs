//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//
//	Base Node class where all FlowReactor nodes derive from.
//
//---------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using FlowReactor.OdinSerializer;
using FlowReactor.Editor;
using FlowReactor;
using FlowReactor.NodeUtilityModules;

namespace FlowReactor.Nodes
{
	public class Node : SerializedScriptableObject
	{
		
		
		[HideInInspector]
		public bool isCopy;
		[HideInInspector]
		public string graphPath;
		[HideInInspector]
		public int nodeListIndex;
		[HideInInspector]
		public GUISkin editorSkin;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public Node node;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool runParallel;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public Graph rootGraph;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public Graph graphOwner;
		
		[HideInInspector]
		public Texture2D icon;
		
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public Branch branchNode;
		
		public enum NodeState
		{
			SUCCESS,
			FAILURE,
			RUNNING,
			PARALLEL,
			END
		}
		
		// Node Input and Outputs
		[System.Serializable]
		public class NodeOutput
		{
			public string id = "";
			public string guid;
			public string subGraphExitguid;
			public Node outputNode;
			public bool endlessLoop;
			
			public NodeOutput(){}
			public NodeOutput (string _id) {id = _id; outputNode = null; }
			public NodeOutput (string _id, string _subGraphExitGuid, bool _exitNode){id = _id; subGraphExitguid = _subGraphExitGuid; outputNode = null;}
			public NodeOutput (string _id, string _guID){id = _id; guid = _guID; outputNode = null;}
			public NodeOutput (Node _node){ id = ""; outputNode = _node; }
			public NodeOutput (string _id, Node _node) {id = _id; outputNode = _node; }
		}
		
		[System.Serializable]
		public class InputNode
		{
			public string id = "";
			public string guid;
			public Node inputNode;
			
			public InputNode (Node _node)
			{
				inputNode = _node; 
				guid = _node.guid;
			}
			
			public InputNode (string _id, Node _node) {id = _id; inputNode = _node; }
		}
		
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public List<InputNode> inputNodes = new List<InputNode>();
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public List<NodeOutput> outputNodes = new List<NodeOutput>();
	
		// NODE PROPERTIES
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public string guid;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool bypass;
		[HideInInspector]
		public int bypassOutput;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public Rect nodeRect;
		[HideInInspector]
		public string title;
		[HideInInspector]
		public bool breakpoint;
		[HideInInspector]
		public Color color;
		[HideInInspector]
		public Color groupColor;
		[HideInInspector]
		public bool hideInput;
		[HideInInspector]
		public bool guiDisabled;
		[HideInInspector]
		public bool disableDefaultInspector = false;
		[HideInInspector]
		public bool disableDrawCustomInspector = false;
		[HideInInspector]
		public bool disableVariableInspector = false;
		
		// Collected node information from node attributes
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public NodeCategoryTree.NodeData nodeData;
		
	
		// EDITOR PROPERTIES
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool isMouseOver = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool isOverOutput = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool canDrag = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool hasBeenAligned = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool hasError = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool showNodeVariables;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public float originalNodeWidth;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public float originalNodeHeight;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public float nodeVariablesFieldHeight;
	
		FieldInfo[] nodeVariableFields;
		//List<FRVariable> nodeVariables;
		
		float nodeFieldAttributesHeightAddition;
		//Dictionary<string, Type> variableLocalTypes;
		
		// class to store all available node variables with properties
		[System.Serializable]
		public class NodeVariableGroups
		{
			//public enum VariableAttributeType
			//{
			//	Title,
			//	HelpBox,
			//	OpenUrl,
			//	Hide,
			//	HideInNode
			//}
			
			[System.Serializable]
			public class NodeVariableAttribute{	}
			public class NodeVariableTitleAttribute : NodeVariableAttribute
			{
				public string attributeTitle;
				public NodeVariableTitleAttribute (string _title)
				{
					attributeTitle = _title;
				}
			}
			
			public class NodeVariableHelpBoxAttribute : NodeVariableAttribute
			{
				public string attributeMessage;
				public HelpBox.MessageType attributeMessageType;
				
				public NodeVariableHelpBoxAttribute (string _msg, HelpBox.MessageType _messageType)
				{
					attributeMessage = _msg;
					attributeMessageType = _messageType;
				}
			}
			
			public class NodeVariableOpenURLAttribute : NodeVariableAttribute
			{
				public string attributeUrlTitle;
				public string attributeUrl;
				public NodeVariableOpenURLAttribute(string _title, string _url)
				{
					attributeUrlTitle = _title;
					attributeUrl = _url;
				}
			}
			
			public class NodeVariableHideAttribute : NodeVariableAttribute
			{
				public bool attributeHide;
				public NodeVariableHideAttribute (bool _hide)
				{
					attributeHide = _hide;
				}
			}
			
			public class NodeVariableHideInNodeAttribute : NodeVariableAttribute
			{
				public bool attributeHideInNode;
				public NodeVariableHideInNodeAttribute (bool _hide)
				{
					attributeHideInNode = _hide;
				}
			}
			
			public bool foldout;
			
			public class Variables
			{
				public string variableName;
				public FRVariable variable;	
				public List<NodeVariableAttribute> variableAttributes;
				public float additionalGUIHeight;
				
				public Variables(FRVariable _variable, string _variableName, List<NodeVariableAttribute> _variableAttributes,  float _additionalGUIHeight)
				{
					variableName = _variableName;
					variable = _variable;
					additionalGUIHeight = _additionalGUIHeight;
					variableAttributes = _variableAttributes;
				}
			}
			public List<Variables> variables = new List<Variables>();
			
			public NodeVariableGroups()
			{
				variables = new List<Variables>();
			}
			public NodeVariableGroups (FRVariable _variable, string _variableName, List<NodeVariableAttribute> _variableAttributes, float _additionalGUIHeight)
			{
			
				variables = new List<Variables>();
				variables.Add(new Variables(_variable, _variableName, _variableAttributes, _additionalGUIHeight));
			}
		}
		
		[HideInInspector]
		public Dictionary<string, NodeVariableGroups> nodeVariableGroups = new Dictionary<string, NodeVariableGroups>();
		[HideInInspector]
		public int hiddenNodeFields = 0;
		
		// Is running is being used for the graph explorer
		// we need to add a slight delay for updating the value so
		// we can see it better in the explorer
		#if UNITY_EDITOR
		bool _isRunning = false;
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public bool isRunning
		{
			get
			{
				if (graphOwner.isActive)
				{
					if (EditorApplication.timeSinceStartup > highlightGraphStartTime && !_isRunning)
					{
						return false;
					}
					else 
					{
						if (EditorApplication.timeSinceStartup < highlightGraphStartTime)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
				}
				else
				{
					return false;
				}
			}
			set
			{
				_isRunning = value;
			}
		}
		#endif
			
			
		[HideInInspector]
		public double highlightStartTime = 0f;
		
		#if UNITY_EDITOR
		#if !FLOWREACTOR_DEBUG
		[HideInInspector]
		#endif
		public double highlightGraphStartTime;
		[HideInInspector]
		public Vector2 lastZoomCoordsOrigin;
		Color highlightColor;
	
		[HideInInspector]
		public float highlightTime = 0.5f;
		int undoAlignNodesGroup;

		private static Dictionary<string, System.Type> variableLocalTypes = new Dictionary<string, System.Type>();
		
		
		// Editor methods
		public static Node CreateNode(string _type, string _name)
		{
			Node node = null;
			try
			{
				#if FLOWREACTOR_DEBUG
				Debug.Log("create node: " + _type);
				#endif
			
				node = (Node)CreateInstance(_type);
				node.name = _name;
				node.hideFlags = HideFlags.HideInHierarchy;
			
			
			}
			catch
			{
				Debug.LogWarning("Node " + _type + " does not exist");
			}

			return node;
		}
		
		// Called upon node creation. If user setting createExpandedNodes is true we have to open the node variabl field
		public void SetupVariables()
		{
			FlowReactor.Editor.EditorCoroutines.Execute(SetupVariablesDelayed());
		}
		
		IEnumerator SetupVariablesDelayed()
		{
			yield return new WaitForSeconds(0.05f);
			var _settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			if (_settings.createExpandedNodes)
			{
				OpenNodeVariableUI();
			}
			else
			{
				BuildVariableGroups();
			}
		}

		
		public void RemoveInputNode(Node _removeNode)
		{
			var _inputNodes = node.inputNodes;
			
			for (int i = 0; i < _inputNodes.Count; i ++)
			{
				if (_inputNodes[i].inputNode == _removeNode)
				{
					_inputNodes.RemoveAt(i);
				}
			}
		}
		 
		//public void SnapToGrid(Vector2 _mousePosition)
		//{		
		//	float _gsize = 20;
		//	var _p =  new  Vector2( Mathf.Round(_mousePosition.x / _gsize ) * _gsize, (Mathf.Round( _mousePosition.y / _gsize ) * _gsize));
			

		//	this.nodeRect.position = _p;
		//}
		
		public void MoveChilds(Vector2 _delta, Graph _rootGraph, Node _startNode)
		{
			Node _lastOutputNode = null;
			
		
			
			for (int c = 0; c < outputNodes.Count; c ++)
			{
				if (outputNodes[c].outputNode != null && outputNodes[c].outputNode != _lastOutputNode)
				{	
					_lastOutputNode = outputNodes[c].outputNode;
						
					if (outputNodes[c].outputNode.inputNodes.Count > 1)
					{
						if (this != outputNodes[c].outputNode.inputNodes[0].inputNode)
							return;
					}
						
					if (outputNodes[c].outputNode != _startNode)
					{

						outputNodes[c].outputNode.nodeRect.position += _delta;
					
						outputNodes[c].outputNode.MoveChilds(_delta, _rootGraph, _startNode);
					}
				}
			}
		}

		public void DoHighlight()
		{		
			highlightStartTime = Mathf.Infinity;	
		}
		#endif

		
		// VIRTUAL METHODS
		//////////////////	
		#if UNITY_EDITOR
		public virtual void MoveDelta(Vector2 _delta)
		{
		
			Undo.RecordObject(this, "move node");
			this.nodeRect.position += _delta;

			if (Event.current.shift)
			{
			
				// first deselect all except first node
				if (rootGraph.selectedNodes.Keys.Count > 1)
				{
					rootGraph.selectedNodes = new Dictionary<int, Node>();
					rootGraph.selectedNodes.Add(this.GetInstanceID(), this);
					rootGraph.selectedNodeIndex = rootGraph.selectedNode.nodeListIndex;
				}
				
				MoveChilds(_delta, rootGraph, this);
			}
		
		}
		
		public virtual void Init(Graph _graph, Node _node) // int _nodeIndex, Node _node )
		{
			node = _node;
			highlightColor = new Color(255f/255f,255f/255f,255f/255f,0f/255f);	
		}
		
		// Override this method to create custom node inspector GUIs
		public virtual void DrawCustomInspector(){}
		
		// Default node GUI
		public virtual void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
		
			editorSkin = _editorSkin;
			title = _title;
			
			if (rootGraph == null)
			{
				rootGraph = graphOwner.rootGraph;
				nodeRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, nodeRect.height);
				Init(graphOwner.rootGraph, node);
			}

			nodeListIndex = _id;
		
			if (rootGraph == null )
			{
				var _instanceID = EditorPrefs.GetInt("FlowReactorInstanceID");
			
				rootGraph = EditorUtility.InstanceIDToObject(_instanceID) as Graph;
			}
			
			// change node position based on zooming and panning
			if (rootGraph.currentGraph.zoomCoordsOrigin != lastZoomCoordsOrigin)
			{
				nodeRect = new Rect(nodeRect.x - (rootGraph.currentGraph.zoomCoordsOrigin.x), nodeRect.y - (rootGraph.currentGraph.zoomCoordsOrigin.y), nodeRect.width, nodeRect.height);
				lastZoomCoordsOrigin = rootGraph.currentGraph.zoomCoordsOrigin;
			}
			
			if (!guiDisabled)
			{
				switch (nodeData.nodeType)
				{
				case NodeAttributes.NodeType.Group:
					GUI.color = groupColor;
					GUI.Box(nodeRect, "", _editorSkin.GetStyle("Group"));
					GUI.color = Color.white;
					break;
				case NodeAttributes.NodeType.Comment:
					//GUI.color = groupColor;
					GUI.Box(nodeRect, "", _editorSkin.GetStyle("CommentNode"));
					//GUI.color = Color.white;
				
					break;
				default:
					// Node background
					GUI.Box(nodeRect, "", _editorSkin.GetStyle("Node"));
					break;
				}
			}
			
		
			// Node color overlay
			if (nodeData.nodeType != NodeAttributes.NodeType.Group && nodeData.nodeType != NodeAttributes.NodeType.Comment)
			{
				var _ovc = color;
				if (guiDisabled)
				{
					_ovc = new Color(color.r, color.g, color.b, 120f/255f);
				}
				GUI.color = _ovc;
				if (nodeData.nodeType != NodeAttributes.NodeType.Coroutine)
				{
					GUI.Box(nodeRect, "", _editorSkin.GetStyle("NodeColorOverlay"));
				}
				else
				{
					GUI.Box(nodeRect, "", _editorSkin.GetStyle("NodeCoroutineColorOverlay"));
				}
				GUI.color = Color.white;
			}
			
			// Node Highlighting
			if (Application.isPlaying)
			{
				if (EditorApplication.timeSinceStartup > highlightStartTime)
				{
				
					highlightColor = new Color(255f/255f,255f/255f,255f/255f,0f/255f);
					
					if (nodeData.nodeType == NodeAttributes.NodeType.SubGraph)
					{
						var _s = node as SubGraph;
						if (_s.subGraph.isActive)
						{
						
							highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
						}
					}
					else if ( nodeData.nodeType == NodeAttributes.NodeType.SubGraphInstance)
					{
						var _s = node as SubGraphInstance;
						if (_s.graphCopy.isActive)
						{
						
							highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
						}
					}
				}
				else
				{
					//Debug.Log(highlightActive + " " + highlightStartTime);
					highlightColor = Color.Lerp(color, new Color(255f/255f,255f/255f,255f/255f,0f/255f), (float)(EditorApplication.timeSinceStartup - (highlightStartTime - highlightTime)));
				}
				
				GUI.color = highlightColor;
				GUI.Box(nodeRect, "", _editorSkin.GetStyle("Indicator"));
				GUI.color = Color.white;
			}
			
			// Draw header
			if (nodeData.nodeType != NodeAttributes.NodeType.Group && nodeData.nodeType != NodeAttributes.NodeType.Comment && !guiDisabled)
			{
				GUI.Label(new Rect(nodeRect.x + 5, nodeRect.y + 3, nodeRect.width - 10, 20), new GUIContent(" " + _title, icon));
			}
			
			// Check if user has released mouse over node when dragging a spline
			if (!rootGraph.nodeSelectionPanelOpen && !rootGraph.selectionBoxDragging &&
				!rootGraph.resizeMinimap &&
				!rootGraph.dragMinimap &&
				nodeData.nodeType != NodeAttributes.NodeType.Group &&
				nodeData.nodeType != NodeAttributes.NodeType.Comment)// && 
				//nodeData.nodeType != NodeAttributes.NodeType.Event)
			{
				
			
				Rect _check = new Rect(nodeRect.x * rootGraph.currentGraph.zoomFactor, nodeRect.y * rootGraph.currentGraph.zoomFactor, nodeRect.width * rootGraph.currentGraph.zoomFactor, nodeRect.height * rootGraph.currentGraph.zoomFactor);

				if (_check.Contains(Event.current.mousePosition * rootGraph.currentGraph.zoomFactor))
				{
					isMouseOver = true;
					
				
					// make sure node variable fields are loosing focus so values are being updated
					//GUI.FocusControl("NodeClear"); 
					//
					
					if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseUp &&
						rootGraph.lastSelectedNodeIndex > -1)
					{			
						// Endless loop check
						//rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes[rootGraph.lastSelectedOutput].endlessLoop = false;
						for (int o = 0; o < rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes.Count; o ++)
						{
							rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes[o].endlessLoop = false;
						}
						if (EndlessLoopCheck(rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex], 0))
						{
							rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes[rootGraph.lastSelectedOutput].endlessLoop = true;
						}
						
						// Do nothing
						if (rootGraph.currentGraph.nodes[_id].hideInput)
						{
							rootGraph.lastSelectedNodeIndex = -1;
						}
						// Connect to node
						else
						{
							//#if UNITY_EDITOR
							if (rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex] != this && nodeData.nodeType != NodeAttributes.NodeType.Event)
							{
								Undo.SetCurrentGroupName( "Connect Node" );
								int undoGroup = Undo.GetCurrentGroup();
								
								Undo.RecordObject(	rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex], "Connect Node");
								Undo.RecordObject(	rootGraph.currentGraph.nodes[_id], "Connect Node");
								rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes[rootGraph.lastSelectedOutput].outputNode
									= rootGraph.currentGraph.nodes[_id];
									
							
								rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex].outputNodes[rootGraph.lastSelectedOutput].guid = rootGraph.currentGraph.nodes[_id].node.guid;
									
								
								AddNodeToInput(_id, rootGraph.currentGraph, rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex]);
							
								Undo.CollapseUndoOperations( undoGroup );
								
								
								EditorUtility.SetDirty(this);
								EditorUtility.SetDirty(rootGraph);
							}
							//#endif
							rootGraph.lastSelectedNodeIndex = -1;
						}
						
						rootGraph.drawModeOn = false;
					}
					
				}
				else
				{
					if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseUp)
					{						
						isMouseOver = false;					
					}	
				}
			}
		
			
			// Check if user is holding mouse down over outputs. If yes then the node should not be draggable
			var _outputRectCheck = new Rect (nodeRect.x + (nodeRect.width - 30), nodeRect.y + 20, 30, nodeRect.height);
			if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown)
			{
				if (_outputRectCheck.Contains(Event.current.mousePosition))
				{
					isOverOutput = true;
				}
				else
				{
					isOverOutput = false;
				}
				
				var _checkRect = new Rect(nodeRect.position.x, nodeRect.position.y, nodeRect.width, originalNodeHeight == 0 ? nodeRect.height : originalNodeHeight);
				if (_checkRect.Contains(Event.current.mousePosition))
				{
					canDrag = true;
				}
				else
				{
					canDrag = false;
				}
			}
			else if (rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseUp)
			{
				isOverOutput = false;
			}
					
					
					
			// Draw Input and output slots
			//////////////////////////////
						
			// INPUT SLOT
			if (nodeData.nodeType != NodeAttributes.NodeType.Event)
			{
				bool _hasInputs = false;
				for (int i = 0; i < inputNodes.Count; i ++)
				{
					if (inputNodes[i].inputNode != null)
					{
						_hasInputs = true;
					}
				}
				
				if (!hideInput)
				{
					GUI.color = color;
					if (!_hasInputs)
					{
						if (GUI.Button(new Rect(nodeRect.x, nodeRect.y + 25, 20, 20), "", _editorSkin.GetStyle("Input"))){};
					}
					else
					{
						if (GUI.Button(new Rect(nodeRect.x, nodeRect.y + 25, 20, 20), "", _editorSkin.GetStyle("InputActive"))){};
					}
					GUI.color = Color.white;
				}
			}
			
			
			
			// OUTPUT SLOTS
			for (int o = 0; o < outputNodes.Count; o ++)
			{
				
				if (outputNodes[o] == null)
					continue;
						
				// Output name label
				GUI.Label(new Rect(nodeRect.x + 5, nodeRect.y + 25 + (o * 20), nodeRect.width - 35, 20), outputNodes[o].id, FlowReactor.Editor.FlowReactorEditorStyles.nodeOutputTextStyle);

				// Output inactive
				if (outputNodes[o].outputNode == null)
				{
					
						
					var _b = _editorSkin.GetStyle("Output");

					GUI.color = color;
					var _outputRect = new Rect (nodeRect.x + (nodeRect.width - 22), nodeRect.y + 25 + (o * 20), 20, 20);
					GUI.Box(_outputRect, "", _b);
					GUI.color = Color.white;
 
				
					if (Event.current.type == EventType.Repaint &&
						_outputRect.Contains(Event.current.mousePosition) && 
						!rootGraph.selectionBoxDragging &&
						(rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown || 
						rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDrag) 
						&& !rootGraph.resizeMinimap 
						&& !rootGraph.dragMinimap 
						&& !rootGraph.isNodeDragging 
						&& !rootGraph.drawModeOn 
						//&& rootGraph.currentGroupNodeDragging == null
						&& rootGraph.isMouseOverNode
						&& !BlackBoardVariableDragProperties.isDragging)
					{
					
						rootGraph.nodeSelectionPanelOpen = false;
						rootGraph.lastSelectedNodeIndex = _id;
						rootGraph.lastSelectedOutput = o;
						rootGraph.nodeConnectPoint = new Vector2(nodeRect.x + nodeRect.width + 25, nodeRect.y);
						
						rootGraph.drawModeOn = true;
						
						if (nodeData.nodeType != NodeAttributes.NodeType.Group)
						{
							rootGraph.isMouseOverNode = true;
						}
					}
				

					GUI.color = Color.white;
				}
				// Output active
				else
				{
					var _b = _editorSkin.GetStyle("OutputDelete");
					
					GUI.color = color;
					var _outputRect = new Rect (nodeRect.x + (nodeRect.width - 22), nodeRect.y + 25 + (o * 20), 20, 20);
					GUI.Box(_outputRect, "", _b);
					GUI.color = Color.white;
				
					//#if UNITY_EDITOR
					if (Event.current.type == EventType.Repaint && 
						_outputRect.Contains(Event.current.mousePosition) &&
						rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown && 
						rootGraph.globalMouseButton != 2 &&
						!rootGraph.resizeMinimap && !rootGraph.dragMinimap)
					{
						for (int i = 0; i < outputNodes[o].outputNode.inputNodes.Count; i ++)
						{
							if (outputNodes[o].outputNode.inputNodes[i].inputNode == this)
							{
								outputNodes[o].outputNode.inputNodes.RemoveAt(i);	
							}
						}
						
						outputNodes[o].outputNode = null;
					
						rootGraph.drawModeOn = false;
						rootGraph.lastSelectedNodeIndex = -1;
						
						EditorUtility.SetDirty(this);
						EditorUtility.SetDirty(graphOwner);
				
					}
					//#endif
				
				} 
			}
			//////////////////////////////////////
	
	 
			 
			if (showNodeVariables)
			{	
				DrawNodeVariables(false);
			}
			
			// Node right click context menu
			if (rootGraph.isMouseOverNode && nodeRect.Contains(Event.current.mousePosition) && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown && Event.current.button == 1)
			{
				if (nodeData.nodeType == NodeAttributes.NodeType.Normal || nodeData.nodeType == NodeAttributes.NodeType.Coroutine || nodeData.nodeType == NodeAttributes.NodeType.Portal || nodeData.nodeType == NodeAttributes.NodeType.Event)
				{
					NodeContextMenu(_graph);			
				}
				
				else if (nodeData.nodeType == NodeAttributes.NodeType.SubGraph || nodeData.nodeType == NodeAttributes.NodeType.SubGraphInstance)
				{
					SubGraphNodeContextMenu(_graph);
				}
			
				rootGraph.selectedNode = null;
				rootGraph.selectedNodes = new Dictionary<int, Node>();
				
				//GUIUtility.ExitGUI();
			}
			else if ((!rootGraph.isMouseOverNode && nodeRect.Contains(Event.current.mousePosition) && rootGraph.globalMouseEvents == Graph.GlobalMouseEvents.mouseDown && Event.current.button == 1))
			{
				if (nodeData.nodeType == NodeAttributes.NodeType.Group)
				{
					GroupNodeContextMenu(_graph);
				}
			
				rootGraph.selectedNode = null;
				rootGraph.selectedNodes = new Dictionary<int, Node>();
				
				//GUIUtility.ExitGUI();
			}
			
			if (bypass)
			{
			
				GUI.color = new Color(255f/255f, 255f/255f, 255f/255f, 180f/255f);
				GUI.Box(new Rect(nodeRect.x, nodeRect.y, nodeRect.width, nodeRect.height - 10), "", "Button");		
				GUI.color = Color.white;
				bypass = GUI.Toggle(new Rect( nodeRect.x + (nodeRect.width / 2) - 25, nodeRect.y +(nodeRect.height / 2) - 20, nodeRect.width, nodeRect.height), bypass, "Bypass");

			}
			
			if (nodeVariableGroups.Keys.Count > 0 && nodeVariableGroups[""].variables.Count > 0)
			{

				if (nodeVariableGroups.Keys.Count == 1 && nodeVariableGroups[""].variables.Count == hiddenNodeFields)
				{
					
				}
				else
				{
					if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 25, nodeRect.y-2, 25, 25),showNodeVariables ? "-" : "+", _editorSkin.GetStyle("ExpandNodeVariables")))
					{
						if (showNodeVariables)
						{
							CloseNodeVariableUI();		
						}
						else
						{
							OpenNodeVariableUI();			
						}
					}
				}
			}	
		}
		
		
		// Show variables on nodes
		public void DrawNodeVariables(bool _isInspector)
		{
			if (nodeVariableFields == null)
			{
				nodeVariableFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy); // | BindingFlags.NonPublic
			}
			//if (nodeVariables == null || nodeVariables.Count == 0)
			//{
			//	GetAvailableVariableTypes.GetAllFRVariablesOnNode(this, out nodeVariables);
			//}
			
			if (variableLocalTypes == null)
			{
				variableLocalTypes = new Dictionary<string, Type>();
			}
			if (variableLocalTypes.Keys.Count == 0 )
			{
				GetAvailableVariableTypes.ReturnExistingTypesOfType<FRVariable>(out variableLocalTypes);
			}
			
			if (nodeVariableGroups == null || nodeVariableGroups.Keys.Count == 0)
			{
				BuildVariableGroups();
			}


			if (_isInspector)
			{
				DrawVariableNodesInternal(_isInspector);
			}
			else
			{
				var _groupsCount =  nodeVariableGroups.Keys.Count;
		
				var _additionalGroupsHeight = 0f;
				var _visibleVariables = 0;
				var _hiddenVariables = 0;
				var _additionalVariableGUIHeight = 0f;
		
				int _gIndex = 0;
	
				foreach(var g in nodeVariableGroups.Keys)
				{
					if (_gIndex > 0)
					{
						if (nodeVariableGroups[g].foldout)
						{
						
							foreach(var v in nodeVariableGroups[g].variables)
							{
								int vi = 0;
								foreach (var a in v.variableAttributes)
								{
									if (a.GetType() == typeof(NodeVariableGroups.NodeVariableHideInNodeAttribute))
									{
										_hiddenVariables ++;
									}
								}
								vi ++;
								
								_visibleVariables++;
								_additionalVariableGUIHeight += v.additionalGUIHeight;
							}
						}
					}
					else
					{
					
						foreach(var v in nodeVariableGroups[g].variables)
						{
							int vi = 0;
							foreach (var a in v.variableAttributes)
							{
								if (a.GetType() == typeof(NodeVariableGroups.NodeVariableHideInNodeAttribute))
								{
									_hiddenVariables ++;
								}
							}
							vi ++;
							
							_visibleVariables++;
							
							_additionalVariableGUIHeight += v.additionalGUIHeight;
						}
					}
			
					_gIndex ++;
				}
		
				if (_visibleVariables - _hiddenVariables != 0)
				{
					_additionalGroupsHeight = _groupsCount * 22;
				}
				else
				{
					_additionalGroupsHeight = (_groupsCount - 1) * 22;
				}
				
				nodeVariablesFieldHeight = ((_visibleVariables - _hiddenVariables) * 18) + _additionalGroupsHeight + _additionalVariableGUIHeight;// - (hiddenNodeFields * 20); // + nodeFieldAttributesHeightAddition; //(_validFields * 18) + nodeFieldAttributesHeightAddition;
				nodeRect.height = originalNodeHeight + nodeVariablesFieldHeight;
		
				
				using (new GUILayout.AreaScope(new Rect(nodeRect.x + 3, (nodeRect.y + 32) + (outputNodes.Count * 20), nodeRect.width - 6, nodeVariablesFieldHeight)))
				{			

					GUILayout.Label("", editorSkin.GetStyle("LineVariablesShadow"), GUILayout.Width(nodeRect.width));
					GUILayout.Space(-5);
					//if (GraphEditor.nodeInspector != null)
					//{
					//	NodeDefaultInspector.DrawDefaultInspectorWithoutScriptField(GraphEditor.nodeInspector);
					//}
					
					DrawVariableNodesInternal(_isInspector);
				}
			}
		}
		
		void DrawVariableNodesInternal(bool _isInspector)
		{
			using (new GUILayout.VerticalScope())
			{							
				int _i = 0;
				
				foreach(var g in nodeVariableGroups.Keys)
				{
					// first group are nodes which are not assigned to any group
					if (_i > 0)
					{
						nodeVariableGroups[g].foldout = EditorGUILayout.Foldout(nodeVariableGroups[g].foldout, g);
								
						if (nodeVariableGroups[g].foldout)
						{ 
							for(int v = 0; v < nodeVariableGroups[g].variables.Count; v ++)
							{
								var _hide = false;
								for (int a = 0; a < nodeVariableGroups[g].variables[v].variableAttributes.Count; a ++)
								{
									if (nodeVariableGroups[g].variables[v].variableAttributes[a].GetType() == typeof(NodeVariableGroups.NodeVariableHideAttribute) && _isInspector)
									{
										_hide = true;
									}
									
									if (nodeVariableGroups[g].variables[v].variableAttributes[a].GetType() == typeof(NodeVariableGroups.NodeVariableHideInNodeAttribute) && !_isInspector)
									{
										_hide = true;
									}
								}
								
								if (_hide)
									continue;
					
								if (!_isInspector)
								{
									FRVariableGUIUtility.DrawVariableAttributesNode(nodeVariableGroups[g].variables[v].variableAttributes);
								}
								else
								{
									FRVariableGUIUtility.DrawVariableAttributesInspector(nodeVariableGroups[g].variables[v].variableAttributes);
								}
								
								FRVariableGUIUtility.DrawVariable(nodeVariableGroups[g].variables[v].variableName, nodeVariableGroups[g].variables[v].variable, this, false, editorSkin);
									
							}				
						}
					}
					else
					{
	
						for(int v = 0; v < nodeVariableGroups[g].variables.Count; v ++)
						{		
							var _hide = false;
							for (int a = 0; a < nodeVariableGroups[g].variables[v].variableAttributes.Count; a ++)
							{
							
								if (nodeVariableGroups[g].variables[v].variableAttributes[a].GetType() == typeof(NodeVariableGroups.NodeVariableHideAttribute) && _isInspector)
								{
									_hide = true;
								}
									
								if (nodeVariableGroups[g].variables[v].variableAttributes[a].GetType() == typeof(NodeVariableGroups.NodeVariableHideInNodeAttribute) && !_isInspector)
								{ 
									_hide = true;
								}
							}
							
							if (_hide)
								continue;
						
							if (!_isInspector)
							{
								FRVariableGUIUtility.DrawVariableAttributesNode(nodeVariableGroups[g].variables[v].variableAttributes);
							}
							else
							{
								FRVariableGUIUtility.DrawVariableAttributesInspector(nodeVariableGroups[g].variables[v].variableAttributes);
							}
							
							FRVariableGUIUtility.DrawVariable(nodeVariableGroups[g].variables[v].variableName, nodeVariableGroups[g].variables[v].variable, this, false, editorSkin);		
							
						}
					}
							
					_i ++;
				}
			}
		}
		
		
		public void OpenNodeVariableUI()
		{
			BuildVariableGroups();
		
			if (nodeVariableGroups.Keys.Count > 0 && nodeVariableGroups[""].variables.Count > 0)
			{
				//if (hiddenNodeFields != nodeVariableGroups.Keys.Count)
				//{
					showNodeVariables = true;
					ExpandNodeRect();
				//}
			}
		}
		
		public void CloseNodeVariableUI()
		{ 
			if (nodeVariableGroups.Keys.Count > 0 && nodeVariableGroups[""].variables.Count > 0)
			{
				//if (hiddenNodeFields != nodeVariableGroups.Keys.Count)
				//{
					showNodeVariables = false;	
					CollapseNodeRect();
				//}
			}
		}
		
	
		
		// Collect all node variables and put them into the groups defined by the attribute VariableGroup
		public void BuildVariableGroups()
		{

			hiddenNodeFields = 0;
			
			nodeVariableFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy); //| BindingFlags.NonPublic
			
		
			if (variableLocalTypes == null)
			{
				variableLocalTypes = new Dictionary<string, Type>();
			} 
			if (variableLocalTypes.Keys.Count == 0)
			{ 
				GetAvailableVariableTypes.ReturnExistingTypesOfType<FRVariable>(out variableLocalTypes);
			} 
			
			//var _test = new Dictionary<string, Type>();
			//GetAvailableVariableTypes.ReturnExistingTypesOfType<FRVariable>(out _test);
			//foreach(var field in nodeVariableFields)
			//{
			//	Debug.Log(field.FieldType);
			//}
			
			// First group contains all variables which are not in a group
			nodeVariableGroups = new Dictionary<string, NodeVariableGroups>();				
			nodeVariableGroups.Add("", new NodeVariableGroups());

		
			foreach(var field in nodeVariableFields)
			{
				foreach (var key in variableLocalTypes.Keys)
				{
					if (field.FieldType == variableLocalTypes[key] || (field.FieldType.IsGenericType && variableLocalTypes[key].IsGenericType && (field.FieldType.GetGenericTypeDefinition() == variableLocalTypes[key].GetGenericTypeDefinition())))
					{
						
						FieldInfo _fieldInfo = field;
						
						FRVariable _v = field.GetValue(this) as FRVariable;
			
						if (_v == null)
							continue;
						//FieldInfo field = _v.GetType().GetField(_v.name);
				
						// return additional attribute gui heights
						var _additionalGUIHeight = 0f;
						_additionalGUIHeight = FRVariableGUIUtility.GetVariableAttributeHeight(field);
						
					
							
						var _vHeight = _v.GetGUIHeight();
						if (_vHeight > 1)
						{
							if (_additionalGUIHeight + _v.GetGUIHeight() - 18 >= 0)
							{
								_additionalGUIHeight += _v.GetGUIHeight() - 18;
							}
						}
						
					
						// Collect all variable attributes
						object[] _attributes = field.GetCustomAttributes(false);
						List<NodeVariableGroups.NodeVariableAttribute> _variableNodeAttribute = new List<NodeVariableGroups.NodeVariableAttribute>();
						for (var a = 0; a < _attributes.Length; a ++)
						{
							// TITLE
							if (_attributes[a].GetType() == typeof(Title))
							{
								var _a = _attributes[a] as Title;
								_variableNodeAttribute.Add(new NodeVariableGroups.NodeVariableTitleAttribute(_a.title));
							}
							
							// HELPBOX
							if (_attributes[a].GetType() == typeof(HelpBox))
							{
								var _a = _attributes[a] as HelpBox;
								_variableNodeAttribute.Add(new NodeVariableGroups.NodeVariableHelpBoxAttribute(_a.message, _a.type));
							}
							
							// OpenUrl
							if (_attributes[a].GetType() == typeof(OpenURL))
							{
								var _a = _attributes[a] as OpenURL;
								_variableNodeAttribute.Add(new NodeVariableGroups.NodeVariableOpenURLAttribute(_a.title, _a.url));
							}
							
							// Hide
							if (_attributes[a].GetType() == typeof(Hide))
							{
								var _a = _attributes[a] as Hide;
								_variableNodeAttribute.Add(new NodeVariableGroups.NodeVariableHideAttribute(true));
							}
							
							// Hide in node
							if (_attributes[a].GetType() == typeof (HideInNode))
							{
								var _a = _attributes[a] as HideInNode;
								_variableNodeAttribute.Add(new NodeVariableGroups.NodeVariableHideInNodeAttribute(true));
								hiddenNodeFields ++;
							}
						}
						

							
						if(_attributes.Length > 0)
						{
							bool _group = false;
							for (int a = 0; a < _attributes.Length; a ++)
							{
								
								if (_attributes[a].GetType() == typeof(VariableGroup))
								{
									_group = true;
									
									var _g = _attributes[a] as VariableGroup;
									if (!nodeVariableGroups.ContainsKey(_g.groupTitle))
									{
										nodeVariableGroups.Add(_g.groupTitle, new NodeVariableGroups(_v, field.Name, _variableNodeAttribute, _additionalGUIHeight));
									}
									else
									{
										nodeVariableGroups[_g.groupTitle].variables.Add(new NodeVariableGroups.Variables(_v, field.Name, _variableNodeAttribute, _additionalGUIHeight));
									}
								}
								
							} 
							
							if (!_group)
							{
								nodeVariableGroups[""].variables.Add(new NodeVariableGroups.Variables(_v, field.Name, _variableNodeAttribute, _additionalGUIHeight));
							}
						}
						else
						{
							nodeVariableGroups[""].variables.Add(new NodeVariableGroups.Variables(_v, field.Name, _variableNodeAttribute, _additionalGUIHeight));
						}
					}
				}
			}
		}


	

		
		void ExpandNodeRect()
		{	
			if (originalNodeWidth == float.NaN || originalNodeWidth == 0f)
			{
				originalNodeWidth = nodeRect.width;
			}
			if (originalNodeHeight == float.NaN || originalNodeHeight == 0f)
			{
				originalNodeHeight = nodeRect.height;
			}
			
			// cycle through all node variables and get the 
			// most lengthy variable name
			int txtLength = 0;
			int _additionalWidth = 100;
			foreach(var field in nodeVariableFields)
			{
				foreach (var key in variableLocalTypes.Keys)
				{
					
					if (field.FieldType == variableLocalTypes[key])
					{
						
						if (field.Name.Length > txtLength)
						{
							txtLength = field.Name.Length;
						}
						
						// vector fields need more space
						if (field.FieldType == typeof(FRVector2) || field.FieldType == typeof(FRVector3) || field.FieldType == typeof(FRVector4))
						{
							_additionalWidth = 250;
						}
					}
				}
			}
				
			var _newWidth = (txtLength * 8) + _additionalWidth;
			if (_newWidth > nodeRect.width)
			{
				nodeRect.width = _newWidth;
			}
			
			nodeRect.height = originalNodeHeight + nodeVariablesFieldHeight;
		}
		
		void CollapseNodeRect()
		{
			if (originalNodeWidth > 0 )
			{
				nodeRect.width = originalNodeWidth;
			}
		
			if (originalNodeHeight > 0)
			{
				nodeRect.height = originalNodeHeight;
			}
		}
		
		// Add new output to node
		public void AddOutput(string _outputName)
		{
			nodeRect.height += 20;
			originalNodeHeight += 20;
			outputNodes.Add(new NodeOutput(_outputName));
		}
		
		public void RemoveOutput(int _outputIndex)
		{
			if (outputNodes.Count > 1)
			{
				nodeRect.height -= 20;
				originalNodeHeight -= 20;
				outputNodes.RemoveAt(_outputIndex);
			}
		}
		
		public void RemoveOutput(string _outputName)
		{
			if (outputNodes.Count > 1)
			{
				for (int i = 0; i < outputNodes.Count; i ++)
				{
					if (outputNodes[i].id == _outputName)
					{
						outputNodes.RemoveAt(i);
					}
				}
			}
		}
		
		public void RegisterINodeControllables(FlowReactorComponent _frComponent)
		{
			var _fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic  | BindingFlags.Instance).ToList();
		
			Dictionary<string, INodeControllable> _interfaces = new Dictionary<string, INodeControllable>();

			for (int i = 0; i < _fields.Count; i++)
			{
				
				if (_fields[i].FieldType == typeof (FRNodeControllable))
				{
					INodeControllable _inc = null;
					_interfaces.Add(_fields[i].Name, _inc);
				}
				//if (_fields[i].FieldType == typeof (INodeControllable))
				//{
				//	_interfaces.Add(_fields[i].Name, _fields[i] as INodeControllable);
				//}
			}
		
			if (_frComponent.nodeControllables == null)
			{
				_frComponent.nodeControllables = new Dictionary<Node, FlowReactorComponent.NodeControllables>();
			}
			
			if (!_frComponent.nodeControllables.ContainsKey(this))
			{
				if (_interfaces.Keys.Count > 0)
				{
					_frComponent.nodeControllables.Add(this, new FlowReactorComponent.NodeControllables(_interfaces));
				}
			}
			else
			{
				foreach (var i in _interfaces.Keys)
				{
					if (!_frComponent.nodeControllables[this].interfaces.ContainsKey(i))
					{
						if (_interfaces.Keys.Count > 0)
						{
							_frComponent.nodeControllables[this].interfaces.Add(i, _interfaces[i]);
						}
					}
				}
			
			
				// remove interface if _interfaces count is less than current interfaces count
				if (_frComponent.nodeControllables[this].interfaces.Count > _interfaces.Count)
				{
					List<string> _remInterfaces = new List<string>();
					foreach(var _existingKey in _frComponent.nodeControllables[this].interfaces.Keys)
					{
						bool _exists = false;
						foreach(var i in _interfaces.Keys)
						{
							if (i == _existingKey)
							{
								_exists = true;
							}
						}
						
						if (!_exists)
						{
							_remInterfaces.Add(_existingKey);
						}
					}
					
					for (int i = 0; i < _remInterfaces.Count; i ++)
					{
						_frComponent.nodeControllables[this].interfaces.Remove(_remInterfaces[i]);
					}
				}
				
				if (_interfaces.Count == 0)
				{
					_frComponent.nodeControllables.Remove(this);
				}
			}
		}
		
		#endif

		
		// Execute node
		public virtual void OnExecute(FlowReactorComponent _flowReactor){}
		
		// Execute node with additional generic parameters
		public virtual void OnExecute<T>(T _params, FlowReactorComponent _flowReactor){}
		
		// Special execute method which returns a node status.
		// This could be useful when implementing a behaviour tree
		//public virtual NodeState OnExecute(FlowReactorComponent _flowReactor)
		//{
		//	return NodeState.RUNNING;
		//}
		
		
		// Called by FlowReactorComponent OnDisable
		public virtual void OnNodeDisable(FlowReactorComponent _flowReactor){}
		
		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public virtual void OnInitialize(FlowReactorComponent _flowReactor)
		{ 
			#if UNITY_EDITOR 
			highlightStartTime=0f; 
			#endif
			branchNode = null;
			runParallel = false;
		}
		
		// OnGraphStart should be only used by the OnStart node. 
		// Unlike the OnInitialization, OnStart only gets called in the root graph and only if the graph is active.
		public virtual void OnGraphStart(FlowReactorComponent _flowReactor){}
		
		// Similar to the Monobehaviour Update method. Gets called on every frame.
		public virtual void OnUpdate(FlowReactorComponent _flowReactor){}
		
		// Similar to the Monobehaviour LateUpdate method.
		public virtual void OnLateUpdate(FlowReactorComponent _flowReactor){}
		
		// Similar to the Monobehaviour FixedUpdate method.
		public virtual void OnFixedUpdate(FlowReactorComponent _flowReactor){}
		
		// Gets called on application quit
		public virtual void OnApplicationExit(FlowReactorComponent _flowReactor){}
		
		// Editor method which gets called when the node has been selected. 
		public virtual void OnSelect(Graph _graph){}
		
		// Gets called when node gets deleted
		public virtual void OnDelete(){}
		
		
		/// NODE Additional methods
		/////////////////////////// 


		/// <summary>
		/// Execute the next connected node with custom enum.
		/// use it like this in your custom node:
		/*
		
		enum CustomOutputs
		{
		OutputA = 0,
		OutputB = 1
		}
				
		ExecuteNext(CustomOutputs.OutputA, _flowreactor);
		
		*/
		/// </summary>
		/// <param name="_outputEnum"></param>
		/// <param name="_flowReactor"></param>
		public void ExecuteNext(Enum _outputEnum, FlowReactorComponent _flowReactor)
		{
		
			if (!runParallel)
			{
				ExecuteNextNodeImmediate(Convert.ToInt32(_outputEnum), _flowReactor, branchNode);
			}
			else
			{
				EnqueueNextNode(Convert.ToInt32(_outputEnum), _flowReactor);
			}
		}
		
		/// <summary>
		/// Execute the next connected node immediately on same frame
		/// </summary>
		/// <param name="_output"></param>
		/// <param name="_flowReactor"></param>
		public void ExecuteNext(int _output, FlowReactorComponent _flowReactor)
		{
		
			if (!runParallel)
			{
				ExecuteNextNodeImmediate(_output, _flowReactor, branchNode);		
			}
			else
			{	
				EnqueueNextNode(_output, _flowReactor);
			}	
		}
		
		public void ExecuteNextBranch(int _output, FlowReactorComponent _flowReactor, Branch _branch)
		{
			if (!runParallel)
			{
				ExecuteNextNodeImmediate(_output, _flowReactor, _branch);		
			}
			else
			{	
				EnqueueNextNode(_output, _flowReactor);
			}	
		}


		// Enqueue next node to the parallel stack.
		// The parallel stack executes each node frame by frame.
		void EnqueueNextNode(int _output, FlowReactorComponent _flowReactor)
		{
			#if UNITY_EDITOR
			if (Application.isEditor)
			{
				// fade out highlight
				highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
			}
			#endif
			
			
			if (_output < outputNodes.Count && outputNodes[_output].outputNode != null && graphOwner.isActive)
			{
				#if UNITY_EDITOR
				if (Application.isEditor)
				{
					isRunning = false;
					outputNodes[_output].outputNode.isRunning = true;
					
					highlightColor = new Color(255f/255f, 255f/255f, 255f/255f, 0f/255f);
					
					outputNodes[_output].outputNode.DoHighlight();
				}
				#endif
			
			
				outputNodes[_output].outputNode.hasError = false;
				outputNodes[_output].outputNode.graphOwner.HasError(false);

				_flowReactor.parallelStackExecution.Enqueue(outputNodes[_output].outputNode);
			}
		}
		
	
		void ExecuteNextNodeImmediate(int _output, FlowReactorComponent _flowReactor, Branch _branchNode)
		{
			
			#if UNITY_EDITOR
		
			if (Application.isEditor)
			{
				// fade out highlight
				highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
			}
			#endif
			
			
			if (_output < outputNodes.Count && outputNodes[_output].outputNode != null && graphOwner.isActive)
			{
				// Highlight node and build debug graph tree
				#if UNITY_EDITOR
				if (Application.isEditor)
				{
					isRunning = false;
					outputNodes[_output].outputNode.isRunning = true;
					
					highlightColor = new Color(255f/255f, 255f/255f, 255f/255f, 0f/255f);
					
					outputNodes[_output].outputNode.DoHighlight();
				}
				#endif
			
				if (outputNodes[_output].outputNode.bypass)
				{
					try
					{
						Node outputNode = outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode;
						// Run coroutine node
						if (outputNode.nodeData.nodeType == NodeAttributes.NodeType.Coroutine)
						{
							outputNode.hasError = false;
							hasError = false;
							outputNode.runParallel = false;
							if (_branchNode != null)
							{
								outputNodes[_output].outputNode.branchNode = _branchNode;
							}
														
							_flowReactor.NodeControllable_OnNodeExecute(outputNode);
							_flowReactor.NodeControllable_OnNodeStopExecute(this);
							
							
							if ((outputNode as CoroutineNode).coroutine != null)
							{
								_flowReactor.StopCoroutine((outputNode as CoroutineNode).coroutine);
								_flowReactor.executingCoroutines.Remove(outputNode.GetInstanceID());
							}
							
							(outputNode as CoroutineNode).coroutine = (outputNode as CoroutineNode).OnExecuteCoroutine(_flowReactor);
							_flowReactor.executingCoroutines.Add(outputNode.GetInstanceID(), new FlowReactorComponent.CoroutineNodes((outputNode as CoroutineNode).coroutine, outputNode));
						
							_flowReactor.StartCoroutine((outputNode as CoroutineNode).coroutine);


						}
						else
						{
							hasError = false;
							if (_branchNode != null)
							{
								outputNodes[_output].outputNode.branchNode = _branchNode;
							}
							
							_flowReactor.NodeControllable_OnNodeExecute(outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode);
							_flowReactor.NodeControllable_OnNodeStopExecute(this);
							
							
							outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.hasError = false;
							outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.graphOwner.HasError(false);
							outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.runParallel = false;
							
							outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.OnExecute(_flowReactor);
							
						
							return;
						}
					}
					catch (Exception e)
					{
						var _node = outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode;
						Debug.LogError("FlowReactor: Node execution error in Graph: " +_node.graphOwner.name + " Node: " +  _node.name + "\n" + e.Message + "\n" + e.StackTrace);
						_node.hasError = true;
						_node.runParallel = false;
						_node.graphOwner.HasError(true);
					}
				}
				else
				{
					try
					{
						Node outputNode = outputNodes[_output].outputNode;
						// Run coroutine node
						if (outputNode.nodeData.nodeType == NodeAttributes.NodeType.Coroutine)
						{
							outputNode.hasError = false;
							hasError = false;
							outputNode.runParallel = false;
							if (_branchNode != null)
							{
								outputNode.branchNode = _branchNode;
							}
							
							
							_flowReactor.NodeControllable_OnNodeExecute(outputNode);
							_flowReactor.NodeControllable_OnNodeStopExecute(this);
								
								
							if ((outputNode as CoroutineNode).coroutine != null)
							{
								_flowReactor.StopCoroutine((outputNode as CoroutineNode).coroutine);
								_flowReactor.executingCoroutines.Remove(outputNode.GetInstanceID());
							}
							
							(outputNode as CoroutineNode).coroutine = (outputNode as CoroutineNode).OnExecuteCoroutine(_flowReactor);
							_flowReactor.executingCoroutines.Add(outputNode.GetInstanceID(), new FlowReactorComponent.CoroutineNodes((outputNode as CoroutineNode).coroutine, outputNode));
						
							_flowReactor.StartCoroutine((outputNode as CoroutineNode).coroutine);
	
						}
						else
						{
							hasError = false;
							if (_branchNode != null)
							{							
								outputNodes[_output].outputNode.branchNode = _branchNode;
							}
							
							_flowReactor.NodeControllable_OnNodeExecute(outputNodes[_output].outputNode);
							_flowReactor.NodeControllable_OnNodeStopExecute(this);
							
							
							outputNodes[_output].outputNode.hasError = false;
							outputNodes[_output].outputNode.graphOwner.HasError(false);
							outputNodes[_output].outputNode.runParallel = false;
							
							outputNodes[_output].outputNode.OnExecute(_flowReactor);			
	
							return;
						}
					}
					catch ( Exception e )
					{
						var _node = outputNodes[_output].outputNode;
						Debug.LogError("FlowReactor: Node execution error in Graph: " + _node.graphOwner.name + " Node: " +  _node.nodeData.title + "\n" + e.Message + "\n" + e.StackTrace);
						_node.hasError = true;
						_node.runParallel = false;
						_node.graphOwner.HasError(true);
					}
				}
			}
			else
			{			
				// We've reached the end of current flow
				// jump to next sequence in branch node
				if (_branchNode != null)
				{
				
					_branchNode.NextSequence(_flowReactor);
				}
			}
		}
		
		// Execute the next connected node. Generic version so we can pass additional parameters to the next node
		// WARNING: EXPERIMENTAL AND NOT UP TO DATE!!!!
		// 
		//public void ExecuteNext<T>(T _params, int _output, FlowReactorComponent _flowReactor)
		//{
		//	if (_output < outputNodes.Count && outputNodes[_output].outputNode != null && graphOwner.isActive)
		//	{
		//		// Highlight node and build debug graph tree
		//		#if UNITY_EDITOR
		//		if (Application.isEditor)
		//		{
		//			isRunning = false;
		//			outputNodes[_output].outputNode.isRunning = true;
					
		//			highlightColor = new Color(255f/255f, 255f/255f, 255f/255f, 0f/255f);
					
		//			outputNodes[_output].outputNode.DoHighlight();

		//		}
		//		#endif
				
		//		if (outputNodes[_output].outputNode.bypass)
		//		{
		//			try
		//			{
		//				Node outputNode = outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode;

		//				if (outputNode.nodeData.nodeType == NodeAttributes.NodeType.Coroutine)
		//				{
		//					_flowReactor.StartCoroutine((outputNode as CoroutineNode).OnExecuteCoroutine(_flowReactor));
		//				}
		//				else
		//				{
		//					outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.hasError = false;
		//					outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode.OnExecute(_params, _flowReactor);
		//				}
		//			}
		//				catch (Exception e)
		//				{
		//					var _node = outputNodes[_output].outputNode.outputNodes[outputNodes[_output].outputNode.bypassOutput].outputNode;
		//					Debug.LogError("FlowReactor: Node execution error in Graph: " +_node.graphOwner.name + " Node: " +  _node.name + "\n" + e.Message + "\n" + e.StackTrace);
		//					_node.hasError = true;
		//				}
		//		}
		//		else
		//		{
		//			try
		//			{
		//				Node outputNode = outputNodes[_output].outputNode;

		//				if (outputNode.nodeData.nodeType == NodeAttributes.NodeType.Coroutine)
		//				{
		//					_flowReactor.StartCoroutine((outputNode as CoroutineNode).OnExecuteCoroutine(_flowReactor));
		//				}
		//				else
		//				{
		//					outputNodes[_output].outputNode.hasError = false;
		//					outputNodes[_output].outputNode.graphOwner.HasError(false);
		//					outputNodes[_output].outputNode.OnExecute(_params, _flowReactor);
		//				}
		//			}
		//				catch ( Exception e )
		//				{
		//					Debug.LogError("FlowReactor: Node execution error in Graph: " + outputNodes[_output].outputNode.graphOwner.name + " Node: " +  outputNodes[_output].outputNode.name + "\n" + e.Message + "\n" + e.StackTrace);
		//					outputNodes[_output].outputNode.hasError = true;
		//					outputNodes[_output].outputNode.graphOwner.HasError(true);
		//				}
		//		}
		//	}
		//}
		
		#pragma warning disable 0162
		public bool EndlessLoopCheck(Node _startNode, int _endlessLoopCount)
		{
		
			if (_endlessLoopCount > 100)
				return true;
		
			switch(this.GetType().Name)
			{
			case "Repeat":
			case "LateRepeat":
			case "FixedRepeat":
			case "Wait":
			case "SubGraph":
			case "SubGraphInstance":
				return false;
				break;
			}
				
			if (this != _startNode)
			{
				for (int o = 0; o < outputNodes.Count; o ++)
				{
					if (outputNodes[o].outputNode != null)
					{
						outputNodes[o].endlessLoop = false;
						_endlessLoopCount ++;
						return outputNodes[o].outputNode.EndlessLoopCheck(_startNode, _endlessLoopCount);
					}
					else
					{
						continue;
					}
				}
			}
			else
			{
				return true;
			}
			
			return false;
		
		}
		#pragma warning restore 0162
		
		
		// NODE CONTEXT MENUs
		//////////////////////
		#if UNITY_EDITOR
		void NodeContextMenu(Graph _graph)
		{
		
			GenericMenu menu = new GenericMenu();

			for (int o = 0; o < outputNodes.Count; o ++)
			{
				menu.AddItem(new GUIContent("bypass/to output: " + o.ToString()), bypass && o == bypassOutput ? true : false, SetBypass, o);
			}
			menu.AddItem(new GUIContent("edit script"), false, EditNodeScript);
			menu.AddItem(new GUIContent("delete"), false, DeleteNodeDialog, _graph);
			
			menu.ShowAsContext();
			
			rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;
			rootGraph.globalMouseButton = -1;
			
			Event.current.Use();
		}
		
		void GroupNodeContextMenu(Graph _graph)
		{
			
			GenericMenu menu = new GenericMenu();
	
			menu.AddItem(new GUIContent("select nodes in group"), false, SelectNodesInGroup);
			menu.AddItem(new GUIContent("edit script"), false, EditNodeScript);
			menu.AddItem(new GUIContent("delete"), false, DeleteNodeDialog, _graph);
				
			menu.ShowAsContext();
			
			rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;
			rootGraph.globalMouseButton = -1;
			
			
			Event.current.Use();
		}
		
		void SubGraphNodeContextMenu(Graph _graph)
		{
			
			GenericMenu menu = new GenericMenu();
	
			menu.AddItem(new GUIContent("edit script"), false, EditNodeScript);
			menu.AddItem(new GUIContent("delete"), false, DeleteSubgraphDialog, _graph);
				
			menu.ShowAsContext();
			
			rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;
			rootGraph.globalMouseButton = -1;
			
			
			Event.current.Use();
		}
		#endif
		//////////////////////
		
		
		void AddNodeToInput(int _id, Graph _graph, Node _inputNode)
		{
			_graph.nodes[_id].inputNodes.Add(new InputNode(_inputNode));
		}
		
		void RemoveInputFromNode(Node _fromNode, Node _removeNode)
		{
			var _inputNodes = _fromNode.inputNodes;
			
			for (int i = 0; i < _inputNodes.Count; i ++)
			{
				if (_inputNodes[i].inputNode == _removeNode)
				{
					_inputNodes.RemoveAt(i);	
				}
			}
		}
		
		public void SelectNodesInGroup()
		{
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			
			var gnode = node as Group;
			for (int i = 0; i < gnode.nodeChilds.Count; i ++)
			{
				rootGraph.selectedNodes.Add(gnode.nodeChilds[i].node.GetInstanceID(), gnode.nodeChilds[i].node);
			}
		}
		
		#if UNITY_EDITOR
		public void DeleteSubgraphDialog(object _obj)
		{
			if (  EditorUtility.DisplayDialog("Delete Subgraph", "Are you sure you want to delete selected sub graph?\nWarning: This can't be undone!", "yes", "no"))
			{
				DeleteSubGraph(_obj);
			}
		}
		
		
		public void DeleteNodeDialog(object _obj)
		{
			if (  EditorUtility.DisplayDialog("Delete Node", "Are you sure you want to delete this node", "yes", "no"))
			{
				DeleteNode(_obj);
			}
		}
		
		public void DeleteNode(object _obj)
		{
			Undo.SetCurrentGroupName( "Delete Node" );
			int undoGroup = Undo.GetCurrentGroup();
			
			var _graph = (Graph)_obj;
			// remove this node from connected input nodes
			if (this.inputNodes != null)
			{
				for (int i = 0; i < this.inputNodes.Count; i ++)
				{
					if (this.inputNodes[i].inputNode != null)
					{
						for (int o = 0; o < this.inputNodes[i].inputNode.outputNodes.Count; o ++)
						{
							if (this.inputNodes[i].inputNode.outputNodes[o].outputNode == node)
							{
								Undo.RecordObject(this.inputNodes[i].inputNode.outputNodes[o].outputNode, "Delete Node");
								Undo.RecordObject(this.inputNodes[i].inputNode, "Delete Node");
								this.inputNodes[i].inputNode.outputNodes[o].outputNode = null;
							}
						}
					}
				}
			}
			
			if (this.outputNodes != null)
			{
				for (int j = 0; j < this.outputNodes.Count; j ++)
				{
					if (this.outputNodes[j].outputNode != null)
					{
						Undo.RecordObject(this.outputNodes[j].outputNode, "Delete Node");
						this.outputNodes[j].outputNode.RemoveInputNode(node);
					}
				}
			}
			
			
			#if FLOWREACTOR_DEBUG
			Debug.Log("on delete node " + this.GetInstanceID() + " " + this.name);
			#endif
			this.OnDelete();
			
			// Remove connected nodes from blackboard
			foreach (var board in rootGraph.blackboards.Keys)
			{
				rootGraph.blackboards[board].blackboard.RemoveConnectedNode(this);
			}
		
			// remove exposed variables from this node
			if (rootGraph.exposedNodeVariables.ContainsKey(this.nodeData.title))
			{
				rootGraph.exposedNodeVariables.Remove(this.nodeData.title);
			}
		
			Undo.RegisterCompleteObjectUndo(this, "Delete Node");
			AssetDatabase.RemoveObjectFromAsset(this);
			
			// Cleanup graph nodes list
			for (int g = graphOwner.nodes.Count - 1; g > -1; g --)
			{
				if (graphOwner.nodes[g] == this)
				{
					Undo.RecordObject(graphOwner, "Delete Node");
					graphOwner.nodes.RemoveAt(g);
				}
			}
			
			Undo.CollapseUndoOperations( undoGroup );
			
			EditorUtility.SetDirty(graphOwner);
			EditorUtility.SetDirty(rootGraph);
		}
	
		
		// Same as delete node but without Undo functionality
		public void DeleteSubGraph(object _obj)
		{
	
			var _graph = (Graph)_obj;
			// remove this node from connected input nodes
			if (this.inputNodes != null)
			{
				for (int i = 0; i < this.inputNodes.Count; i ++)
				{
					if (this.inputNodes[i].inputNode != null)
					{
						for (int o = 0; o < this.inputNodes[i].inputNode.outputNodes.Count; o ++)
						{
							if (this.inputNodes[i].inputNode.outputNodes[o].outputNode == node)
							{
								this.inputNodes[i].inputNode.outputNodes[o].outputNode = null;
							}
						}
					}
				}
			}
			
			if (this.outputNodes != null)
			{
				for (int j = 0; j < this.outputNodes.Count; j ++)
				{
					if (this.outputNodes[j].outputNode != null)
					{
						this.outputNodes[j].outputNode.RemoveInputNode(node);
					}
				}
			}
			
			
			if (this.GetType() == typeof(SubGraph))
			{
				var _n = this as SubGraph;
				
				for (int g = 0; g < _n.graphOwner.subGraphs.Count; g ++)
				{
					if (_n.graphOwner.subGraphs[g] == _n.subGraph)
					{
						_n.graphOwner.subGraphs.RemoveAt(g);
					}
				}
				
				if (_n.subGraph != null)
				{
				
					_n.subGraph.tmpNodes = new List<Node>();
					
					for (int s = 0; s < _n.subGraph.nodes.Count; s ++)
					{
						_n.subGraph.tmpNodes.Add(_n.subGraph.nodes[s]);
					}
					
					for (int s = 0; s < _n.subGraph.tmpNodes.Count; s ++)
					{
						if (_n.subGraph.tmpNodes[s] != null)
						{
							_n.subGraph.tmpNodes[s].DeleteNode((object)_graph);				
						}
					}
					
					for (int u = 0; u < _n.subGraph.subGraphs.Count; u ++)
					{
						_n.subGraph.subGraphs[u].subGraphNode.DeleteSubGraph((object)_n.subGraph.subGraphs[u]);
					}
					
					AssetDatabase.RemoveObjectFromAsset(_n.subGraph);
				}
			}
			
			this.OnDelete();

			AssetDatabase.RemoveObjectFromAsset(this);
			
			// Cleanup graph nodes list
			for (int g = graphOwner.nodes.Count - 1; g > -1; g --)
			{
				if (graphOwner.nodes[g] == this)
				{
					graphOwner.nodes.RemoveAt(g);
				}
			}
		}
		#endif
		
		void SetBypass(object _obj)
		{
			var _selectedOutput = (int)_obj;
			bypassOutput = _selectedOutput;
			bypass = !bypass;
		}

		void EditNodeScript()
		{
			#if UNITY_EDITOR
			var script = MonoScript.FromScriptableObject( node );
			var path = AssetDatabase.GetAssetPath( script );
				
			AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(path));
			#endif	
		}
		
		#if UNITY_EDITOR
		public void ReloadColor(FlowReactor.Editor.FREditorSettings _settings)
		{
			var _allNodes = CollectAvailableNodes.CollectNodes();
			var _nodeData = _allNodes.GetData(name);
		
			color = _settings.GetColor(_nodeData.color);
		}
	
		
		public void ReInitialize()
		{
			// Re-initialize
			Init(rootGraph, node);
		}
		#endif
		
		// Important when copying nodes at runtime (unique instance)
		public virtual void AssignNewGraphInstance(Graph _rootGraph, Graph _graphOwner){}
		
		// Method to align nodes horizontally based on their input node
		// gets called by the GraphEditor.Tools DistributeNodes() method
		#if UNITY_EDITOR
		public void AlignNode(float _xPos, float _yPos,  Node _firstNode, Node _lastNode, List<Node> _selection, GraphEditor.NodeAlignment _alignment, FREditorSettings _settings)
		{
			
			if (hasBeenAligned)
				return;
				
			hasBeenAligned = true;

			Undo.SetCurrentGroupName( "Align Nodes" );
			undoAlignNodesGroup = Undo.GetCurrentGroup();
			
			Undo.RecordObject(this, "Align Node");
			
			if (_alignment == GraphEditor.NodeAlignment.HorizontallyAndVertically)
			{
				nodeRect = new Rect(_xPos, _yPos, nodeRect.width, nodeRect.height);
			}
			
			switch (_alignment)
			{
			case GraphEditor.NodeAlignment.HorizontallyAndVertically:
				_yPos = _lastNode.nodeRect.y;
				_xPos += nodeRect.width + 50;
				break;
			case GraphEditor.NodeAlignment.Horizontally:
		
				break;
			case GraphEditor.NodeAlignment.Vertically:
			
				break;
			}

			
			for (int o = 0; o < outputNodes.Count; o ++)
			{	
				for (int s = 0; s < _selection.Count; s ++)
				{
					if (outputNodes[o].outputNode == _selection[s])
					{	
					
						
						//if (o - 1 >= 0)
						//{
						//	_l = outputNodes[o -1].outputNode.nodeRect.height;
						//}
						
						if (_alignment == GraphEditor.NodeAlignment.HorizontallyAndVertically)
						{
							// if node is connected to a higher output slot to 0
							// we have to keep the height of the output slot 0 into account
							var _l = 0f;
							if (o > 0)
							{
								for (int l = o - 1; l >= 0; l --)
								{
									_l += outputNodes[l].outputNode.nodeRect.height;
								}
							}
							
							_yPos = nodeRect.y + _l; //(o * outputNodes[o].outputNode.nodeRect.height ) + _l;
						}
					
						
						_selection[s].AlignNode(_xPos, _yPos, _firstNode, this, _selection, _alignment, _settings);
					}
				}
			}
			
			
			if (outputNodes.Count == 0)
			{
				Undo.CollapseUndoOperations(undoAlignNodesGroup);
			}
		}
		#endif
		
		// Assign new output and input nodes after we have copied a graph during runtime
		public void AssignNewOutAndInputs(Graph _copyGraph)
		{
			for (int o = 0; o < outputNodes.Count; o ++)
			{
				for (int n = 0; n < _copyGraph.nodes.Count; n ++)
				{
					if (_copyGraph.nodes[n].guid == outputNodes[o].guid)
					{
						outputNodes[o].outputNode = _copyGraph.nodes[n];
					}
				}
			}
			
			for (int o = 0; o < inputNodes.Count; o ++)
			{
				for (int n = 0; n < _copyGraph.nodes.Count; n ++)
				{
					if (_copyGraph.nodes[n].guid == inputNodes[o].guid)
					{
						inputNodes[o].inputNode = _copyGraph.nodes[n];
					}
				}
			}
			
			graphOwner = _copyGraph;
		}
		
		public void AssignNewOutAndInputsFromList(List<Node> _nodeList)
		{
		
			for (int o = 0; o < outputNodes.Count; o ++)
			{
				for (int n = 0; n < _nodeList.Count; n ++)
				{
					if (_nodeList[n].guid == outputNodes[o].guid)
					{
						outputNodes[o].outputNode = _nodeList[n];		
					}
				}
			}
			
			
			for (int o = 0; o < inputNodes.Count; o ++)
			{
				for (int n = 0; n < _nodeList.Count; n ++)
				{
					if (_nodeList[n].guid == inputNodes[o].guid)
					{
						inputNodes[o].inputNode = _nodeList[n];
					}
				}
			}			
			
			for (int o = 0; o < outputNodes.Count; o ++)
			{
				var _connectedOutput = false;
				for (int n = 0; n < _nodeList.Count; n ++)
				{
					if (_nodeList[n].guid == outputNodes[o].guid)
					{
						_connectedOutput = true;
					}
				}
				
				if (!_connectedOutput && outputNodes.Count - 1 > 1)
				{
					outputNodes.RemoveAt(o);
				}
			}
			
			for (int i = 0; i < inputNodes.Count; i ++)
			{
				var _connectedInput = false;
				for (int n = 0; n < _nodeList.Count; n ++)
				{
					if (_nodeList[n].guid == inputNodes[i].guid)
					{
						_connectedInput = true;
					}
				}
				
				if (!_connectedInput)
				{
					inputNodes.RemoveAt(i);
				}
			}
		}
		
	}
}