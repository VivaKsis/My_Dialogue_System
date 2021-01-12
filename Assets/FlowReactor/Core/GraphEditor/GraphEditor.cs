//---------------------------------------------------------------------------------
//	(c) Copyright doorfortyfour OG, 2020. All rights reserved.
//	
//	Main FlowReactor graph editor class.
//	Responsible for handling user inputs, node drawing and all editor related methods.
//
//---------------------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.AnimatedValues;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;

#if FLOWREACTOR_DATABOX
using Databox;
#endif
using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.OdinSerializer;

namespace FlowReactor.Editor
{
	public partial class GraphEditor : EditorWindow
	{
	
		public static GraphEditor window;
		// Graph explorer
		public GraphExplorer graphExplorer;
		// flowreactor editor settings
		public static FREditorSettings settings;
		// the root graph
		public Graph rootGraph;
		// all available nodes
		public static NodeCategoryTree collectedNodes;
	
		Color bgGrey
		{
			get 
			{
				bool _isPro = EditorGUIUtility.isProSkin;
				if (_isPro)
				{
					return new Color(80f / 255f, 80f / 255f, 80f / 255f); //new Color(160f / 255f, 160f / 255f, 160f / 255f);
				}
				else
				{
					return new Color (220f/255f, 220f/255f, 220f/255f);
				}
			}
		}
		
		Color bgGreyRunning = new Color(80f / 255f, 80f / 255f, 80f / 255f);
		
		Color colGrid1
		{
			get
			{
				bool _isPro = EditorGUIUtility.isProSkin;
				if (_isPro)
				{
					return new Color(75f / 255f, 75f / 255f, 75f / 255f);
				}
				else
				{
					return new Color(170f / 255f, 170f / 255f, 170f / 255f);
				}
			}
		}
		
		Color colGrid2
		{
			get
			{
				bool _isPro = EditorGUIUtility.isProSkin;
				if (_isPro)
				{
					return new Color(65f / 255f, 65f / 255f, 65f / 255f);
				}
				else
				{
					return new Color(180f / 255f, 180f / 255f, 180f / 255f);
				} 
			}
		}
	 
	
	    private Color colGreen = new Color(100f / 255f, 140f / 255f, 120f / 255f);
		private Color colOrange = new Color(225f / 255f, 140f / 255f, 90f / 255f);
	    private Color colBlue = new Color(90f / 255f, 150f / 255f, 200f / 255f);
	    private Color colRed = new Color(140f / 255f, 100f / 255f, 100f / 255f);
		private Color colInspectorBG 
		{
			get 
			{
				if (EditorGUIUtility.isProSkin)
				{
					return new Color (194f / 255f, 194f / 255f, 194f / 255f);
				}
				else
				{
					return new Color (255f / 255f, 255f / 255f, 255f / 255f);
				}
			}
		}
		

	
		private static GUIStyle breadCrumbRight;
		private static GUIStyle breadCrumbRightOn;
		private static GUIStyle breadCrumbMid;
		private static GUIStyle breadCrumbMidOn;
		private static GUIStyle inspectorButtonStyle;
	
		public static UnityEditor.Editor nodeInspector;
		private UnityEditor.Editor lastNodeInspector;
	
	
	
		private string [] inspectorPanelOptions = new string[] {"Inspector", "Blackboard", "Events", "Graph Explorer", "Settings", "?"};
		//private int selectedInspectorPanel = 0;
		
		public static GUISkin editorSkin;
		public GUISkin EditorSKin
		{
			get
			{
				return editorSkin;
			}
		}
		
		private Event currentEvent;
		private Rect nodeCanvas;
		private Rect selectionBox;
	    private float zoomFactor = 1f;
		private float topCanvasSpace = 77;
		private float toolbarButtonSize = 30;
		private int copyPastNodeOffsetPositionMultiplier = 0;
		
		// Node drag properties
		private Rect buttonRect = new Rect(0,0,100,50);
		private bool buttonPressed;
		private bool isDragging = false;
		private Vector2 mousePosition;
		private Vector2 mouseDragStart;
		
		private Rect splitterRect;
		private bool splittDrag = false;
		private Vector2 nodeSelectionMousePos;

	
		// Resources
		private static bool resLoaded = false;
		private static Texture2D logo;
		private static Texture2D gizmoIcon;
		private static Texture2D subGraphIcon;
		private static Texture2D windowLogoIcon;
		private static Texture2D commentIcon;
		private static Texture2D groupIcon;
		private static Texture2D focusIcon;
		private static Texture2D inspectorIcon;
		private static Texture2D createSubGraphIcon;
		private static Texture2D blackBoardIcon;
		private static Texture2D eventBoardIcon;
		private static Texture2D canvasBackgroundTexture;
		private static Texture2D minimapIcon;
		private static Texture2D minimapResizeIcon;
		private static Texture2D alignLeftIcon;
		private static Texture2D alignRightIcon;
		private static Texture2D alignTopIcon;
		private static Texture2D alignBottomIcon;
		private static Texture2D distributeHorizontalIcon;
		private static Texture2D nodeRunningIcon;
		private static Texture2D nodeNotRunningIcon;
		private static Texture2D nodeErrorIcon;
		private static Texture2D expandNodesIcon;
		private static Texture2D collapseNodesIcon;
		private static Texture2D editIcon;
		private static Texture2D okIcon;
		private static Texture2D cancelIcon;
		
		Rect indicatorRect = new Rect(0,0,0,0);

		public static Guid selectedBlackboardGuid;
		public static Guid selectedVariableGuid;
		public static string selectedFieldName = "";
		
		private bool playModeStateChanged = false;
		public bool drawCurserHandle = false;
		
		public static Dictionary<string, System.Type> variableLocalTypes = new Dictionary<string, System.Type>();
		
		private FieldInfo[] nodeVariableFields;
		 
		private static Node lastSelectedNode;
	 
		double highlightStartTime; 
		float highlightTime = 1f;
		Color highlightColor = Color.white;
		Vector2 scrollPos;
		
		//Func<EditorWindow, bool> isWindowFocused = (Func<EditorWindow, bool>)Delegate.CreateDelegate( typeof( Func<EditorWindow, bool> ), typeof( EditorWindow ).GetProperty( "hasFocus", BindingFlags.NonPublic | BindingFlags.Instance ).GetGetMethod( true ) );

		void OnEnable()
		{
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
		}
		
		void OnDisable() 
		{
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
		}
		
		void PlayModeStateChanged(PlayModeStateChange state)
		{
			playModeStateChanged = !playModeStateChanged;
			highlightStartTime = EditorApplication.timeSinceStartup + highlightTime;
			
			// Save
			if (EditorUtility.IsDirty(rootGraph))
			{
				EditorUtility.SetDirty(rootGraph.currentGraph);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			
			if (!Application.isPlaying)
			{
				if (rootGraph.isCopy)
				{
					window.Close();
				}
			}
		}
		
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() 
		{
			// reload settings
			if (settings == null)
			{
				settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			}
			
			lastSelectedNode = null;

			LoadEditorResources();
		}

		public void Init(GraphEditor _editor, Graph _graph)
		{
			window = _editor;
		
			rootGraph = _graph;
			rootGraph.currentGraph = _graph;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			
			rootGraph.lastSelectedNodeIndex = -1;
			
			rootGraph.parentLevels = new List<Graph.ParentLevels>();
			rootGraph.parentLevels.Add(new Graph.ParentLevels(rootGraph.name, _graph));
			
			//rootGraph.currentGraph.zoomFactor = 1f;
			//rootGraph.zoomFactor = 1f;
			
			EditorPrefs.SetInt("FlowReactorInstanceID", rootGraph.GetInstanceID());
			
			if (!resLoaded)
			{
				resLoaded = true;
				LoadEditorResources();
			}
			
			SetupStyles();
			
			collectedNodes = CollectAvailableNodes.CollectNodes();

			// Get FlowReactor settings
			settings = (FREditorSettings)FREditorSettings.GetSerializedSettings().targetObject as FREditorSettings;
			 
			_editor.titleContent = new GUIContent("Graph: " + _graph.name, windowLogoIcon);
			_editor.minSize = new Vector2(100, 100);
			_editor.Show();
			

			if (EditorPrefs.GetBool("FLOWREACTOR_SHOW_WELCOME") && !FlowReactorWelcomeWindow.IsOpen)
			{
				FlowReactorWelcomeWindow.Init();
			} 
			
			// Graph is empty
			if (rootGraph.nodes == null || rootGraph.nodes.Count == 0 || rootGraph.currentGraph == null)
			{
				CreateGraph();
			}
		}
	
			
		static void SetupStyles()
		{
			breadCrumbRight = editorSkin.GetStyle("BreadCrumbRight");
			breadCrumbMid = editorSkin.GetStyle("BreadCrumbMiddle");
		
			breadCrumbRightOn = editorSkin.GetStyle("BreadCrumbRightOn");
			breadCrumbMidOn = editorSkin.GetStyle("BreadCrumbMiddleOn");
		
	
			inspectorButtonStyle = new GUIStyle("toolbarButton");
			inspectorButtonStyle.alignment = TextAnchor.LowerLeft;
		}
		
		public void FocusPosition(Vector2 targetPos)
		{
			var _pos = targetPos;
			_pos -= new Vector2(nodeCanvas.width / 2,  nodeCanvas.height / 2);
			rootGraph.currentGraph.zoomCoordsOrigin = _pos;
		}		
		
		void FocusCanvas(bool resetZoom)
		{		
			var arr1 = new Rect[rootGraph.currentGraph.nodes.Count];
			for ( var i = 0; i < rootGraph.currentGraph.nodes.Count; i++ ) 
			{
				arr1[i] = rootGraph.currentGraph.nodes[i].nodeRect;
			}

			var nBounds = GetRectBoundRect(arr1);
			
			rootGraph.currentGraph.zoomCoordsOrigin = new Vector2(nBounds.x - ((position.width / 2) - (nBounds.width / 2)), nBounds.y - ((position.height / 2) - (nBounds.height / 2) - 100));
			
			if(resetZoom)
			{
				zoomFactor = 1f;		
			}
		}
		
		
		
		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{ 
			return (screenCoords - (nodeCanvas.TopLeft())) / zoomFactor + rootGraph.currentGraph.zoomCoordsOrigin;
		}
		
#region ongui
	
		public void OnGUI()
		{  
			SetupStyles();
			
			// load editor resources if missing
			if (logo == null || editorSkin == null)
			{
				LoadEditorResources();
			}
			
			if (rootGraph == null)
				return;
			
			currentEvent = Event.current;
		
		    EditorGUI.BeginChangeCheck ();
		   
		    bool didPan = false;
		   	
  	
			if (rootGraph.currentGraph)
		    {
				// USER INPUT EVENTS
				//==================
				
				var _inspectorRect = new Rect(0, 0, rootGraph.tmpInspectorWidth + 5, position.height);
				var _toolbarRect = new Rect(rootGraph.tmpInspectorWidth, 35, rootGraph.showInspector ? position.width - rootGraph.tmpInspectorWidth  : position.width, 40);
				var _miniMapRect = new Rect(position.width - rootGraph.minimapSize.x, position.height - rootGraph.minimapSize.y, rootGraph.minimapSize.x, rootGraph.minimapSize.y);
				
			    switch (currentEvent.type)
			    {
			    	// MOUSE DOWN
			    	/////////////
				    case EventType.MouseDown:

					    //if ( isWindowFocused( GetWindow<EditorWindow>() ) )
					    //{
					    //	Debug.Log("window is focused");
					    //}
					    
					    // Keep zoomcords updated
					    //Vector2 _zoomCoordsOriginDelta = Event.current.delta;
					    //_zoomCoordsOriginDelta /= zoomFactor;
					    //rootGraph.currentGraph.zoomCoordsOrigin = new Vector2(-_zoomCoordsOriginDelta.x, -_zoomCoordsOriginDelta.y);		
					  
					  
						rootGraph.globalMouseButton = currentEvent.button;
					    rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.mouseDown;
					    
					    if (splitterRect.Contains(Event.current.mousePosition))
					    {
						    splittDrag = true;  	
					    }
					    
					    for (int i = 0; i < rootGraph.inspectors.Count; i ++)
					    {
					    	rootGraph.inspectors[i].isDragging = rootGraph.inspectors[i].splitRect.Contains(Event.current.mousePosition);
					    }
					    
					   
					    if(_inspectorRect.Contains(Event.current.mousePosition))
					    {
					    	rootGraph.mouseDownOnInspector = true;
					    	rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.ignore;

						  
						    break;
					    }   
					    
					    
					    if (rootGraph.showMinimap && _miniMapRect.Contains(Event.current.mousePosition))
					    {
					    	break;
					    }
					    
		    
				    	// Check if mouse is over any node
					    rootGraph.isMouseOverNode = false;
					    
					    int _selectedNodeIndex = -1;
					    
					    if (rootGraph.currentGraph != null)
					    {
						    for (int n = 0; n < rootGraph.currentGraph.nodes.Count; n ++)
						    {
						    	if (rootGraph.currentGraph.nodes[n] == null)
							    	continue;
						    	if (rootGraph.currentGraph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Group)
							    	continue;
							    	
							    // node has been selected
						    	var _rect = new Rect(rootGraph.currentGraph.nodes[n].nodeRect.position.x * zoomFactor, rootGraph.currentGraph.nodes[n].nodeRect.position.y * zoomFactor, rootGraph.currentGraph.nodes[n].nodeRect.width * zoomFactor, (rootGraph.currentGraph.nodes[n].nodeRect.height) * zoomFactor);
						    	if (_rect.Contains(new Vector2(Event.current.mousePosition.x - rootGraph.tmpInspectorWidth, Event.current.mousePosition.y - topCanvasSpace)))
						    	{		
						    	
						    		GUI.FocusControl(null);
						    		
						    		editNodeTitle = false;
							    	
						    		_selectedNodeIndex = n;
						    		
							    	rootGraph.isMouseOverNode = true;
							    	
							    	
							    	Node _tmpN = null;
							    	if (!rootGraph.selectedNodes.TryGetValue(rootGraph.currentGraph.nodes[n].GetInstanceID(), out _tmpN))
							    	{
				
								    	if (!Event.current.shift)
								    	{
									    	rootGraph.selectedNodes = new Dictionary<int, Node>();
								    	}
								    
								    	// Add Selected node to selected nodes list
								    	rootGraph.selectedNodes.Add(rootGraph.currentGraph.nodes[n].GetInstanceID(), rootGraph.currentGraph.nodes[n]);
								    	rootGraph.selectedNodeIndex = rootGraph.currentGraph.nodes[n].nodeListIndex;
						
							    	}
	
						    		if (lastSelectedNode != rootGraph.currentGraph.nodes[n] || rootGraph.selectedNode == null)
							    	{
							    		
								    	// Create new inspector editor in a coroutine. This prevents the GUILayout group error
								    	// "Scope.Finalize()"
								    	
								    	FlowReactor.Editor.EditorCoroutines.Execute(CreateNodeInspectorEditor(rootGraph.currentGraph.nodes[n], Event.current.shift));
								    }
						    	}
						    }
					    
						
						    if (!_toolbarRect.Contains(Event.current.mousePosition) && !_inspectorRect.Contains(Event.current.mousePosition) && Event.current.button != 2)// || splittDrag)
						    {
							    // Clear multiple selected nodes
							    if (rootGraph.currentGraph != null)
							    {
								    if (!rootGraph.isMouseOverNode )
								    {
									    rootGraph.selectedNode = null;
								    	rootGraph.selectedNodes = new Dictionary<int, Node>();
								    	selectionBox = new Rect(0,0,0,0);
									    
								    }
								    else
								    {
								    	if (rootGraph.currentGraph.nodes[_selectedNodeIndex] != null)
								    	{
								    		Node _nt = null;
									    	if (!rootGraph.selectedNodes.TryGetValue(rootGraph.currentGraph.nodes[_selectedNodeIndex].GetInstanceID(), out _nt))
									    	{	
										    	rootGraph.selectedNodes = new Dictionary<int, Node>();
										    	selectionBox = new Rect(0,0,0,0);
									    	}
								    	}
								    }
							    }
						    }
					    
						    mouseDragStart = Event.current.mousePosition;
					    
					    }
					    
					    //currentEvent.Use();
					    //Repaint();
					    //Event.current.Use();
					
					    break;
				    case EventType.MouseDrag:
					    				    
					    rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.mouseDrag;
					    
					    if (rootGraph.drawModeOn)
					    {
					    	rootGraph.selectedNodes = new Dictionary<int, Node>();	
					    }
					    
					    // panning
					    if (currentEvent.button == 2 || (currentEvent.button == 1 && currentEvent.alt) || (currentEvent.button == 0 && currentEvent.alt))
					    {
						    //Vector2 delta = Event.current.delta;
						    //delta /= zoomFactor;
						    //fr.zoomCoordsOrigin += delta;
						    
						    Vector2 _delta = Event.current.delta;
						    _delta /= zoomFactor;
						    rootGraph.currentGraph.zoomCoordsOrigin = new Vector2(-_delta.x, -_delta.y);				    
	
						    
						    didPan = true;
					    }
					    else
					    {
					    
					    
						    // split view
						    if (splittDrag && rootGraph.showInspector)
						    {
							    rootGraph.tmpInspectorWidth += Event.current.delta.x;
						    }
						    else
						    {
							 
					    		if (!rootGraph.isMouseOverNode && !rootGraph.drawModeOn && !_toolbarRect.Contains(mouseDragStart) && !_inspectorRect.Contains(mouseDragStart) && !rootGraph.mouseClickInGroupNode && !rootGraph.resizeMinimap && !rootGraph.dragMinimap)
							    {
								    rootGraph.selectionBoxDragging = true;
							    }
						    }
						    
						       
						    for (int i = 0; i < rootGraph.inspectors.Count; i ++)
						    {
							    if (rootGraph.inspectors[i].isDragging)
							    {
								    rootGraph.inspectors[i].height += Event.current.delta.y;
							    }
							    
							    if (rootGraph.inspectors[i].height <= 5)
							    {
							    	rootGraph.inspectors[i].height = 50;
							    }
						    }
						    
						    
						    var _nc = new Rect(rootGraph.tmpInspectorWidth, 16, position.width - (rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0), position.height);
						    
						    if (_nc.Contains(Event.current.mousePosition)  && !_toolbarRect.Contains(mouseDragStart) && !_inspectorRect.Contains(mouseDragStart) && !rootGraph.dragMinimap && !rootGraph.resizeMinimap && !BlackBoardVariableDragProperties.isDragging)
						    {					    	
							    isDragging = true; 
						    }
					    }
					    
					    
					    if (rootGraph.selectedNodes.Keys.Count > 0 && isDragging && !rootGraph.mouseDownOnInspector)
					    {
					    	var _canDrag = false;
					    	foreach(var _no in  rootGraph.selectedNodes.Keys)
						    {
						    	if (rootGraph.selectedNodes.ContainsKey(_no))
						    	{
						    		if (rootGraph.selectedNodes[_no].canDrag)
						    		{
						    			_canDrag = true;
						    		}
						    	}
						    }
						    
						    foreach(var _no in  rootGraph.selectedNodes.Keys)
						    {
						    	if (rootGraph.selectedNodes.ContainsKey(_no))
						    	{
							    	if (rootGraph.selectedNodes[_no].isOverOutput)
								    	break;
							    	if (_canDrag)
							    	{    	
								        rootGraph.selectedNodes[_no].MoveDelta(currentEvent.delta / zoomFactor);	        
								    	rootGraph.isNodeDragging = true;
							    	}
						    	}
						    }
					    }
					 
					    break;
				    case EventType.MouseUp:
					    
					    rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.mouseUp;
					  
					    rootGraph.isNodeDragging = false;
					    rootGraph.mouseDownOnInspector = false;
					    
					    if (didPan)
					    {
						    Vector2 _zoomCoordsOriginDelta = Event.current.delta;
						    _zoomCoordsOriginDelta /= zoomFactor;
						    rootGraph.currentGraph.zoomCoordsOrigin = new Vector2(-_zoomCoordsOriginDelta.x, -_zoomCoordsOriginDelta.y);			
					    }
					    
					    // stop splitt drag
					    if (splittDrag)
					    {
						    if (rootGraph.tmpInspectorWidth < 6)
						    {
							    rootGraph.tmpInspectorWidth = 5;
						    }
						    
						    splittDrag = false;
					    }
					   
				
					    // right click
					    if (currentEvent.button == 1 && !currentEvent.alt && !rootGraph.drawModeOn && !rootGraph.nodeSelectionPanelOpen && !_miniMapRect.Contains(Event.current.mousePosition) && !rootGraph.isMouseOverNode)
					    {   	
				
						    mousePosition = Event.current.mousePosition;
						    
						   
						    var _testRect = new Rect(rootGraph.tmpInspectorWidth, 18, position.width, position.height);
					    	if (_testRect.Contains(Event.current.mousePosition))
					    	{
						    	// open node panel on right click
							    if (!rootGraph.nodeSelectionPanelOpen)
							    {
								    rootGraph.nodeSelectionPanelOpen = true;
							    	nodeSelectionMousePos = Event.current.mousePosition;
					
							    	NodePanel(nodeSelectionMousePos);
						    	}
						    	else
						    	{
						    		rootGraph.nodeSelectionPanelOpen = false;
							    	isDragging = false;
						    	}
						    	
						    	// set focus to node filter textfield
						    	//if (rootGraph.nodeSelectionPanelOpen)
						    	//{
						    	//	EditorGUI.FocusTextInControl ("FilterTextField");			    
						    	//}
					    	}
					    	
						  
					    	
					    	//Repaint();
					    	return;
					    }
					    else if(currentEvent.button == 1 && rootGraph.drawModeOn)
					    {   
					    	mousePosition = Event.current.mousePosition;
					    	rootGraph.drawModeOn = false;
						  
					    }
					  
					    
					    // Add nodes to selection box
					    if (isDragging && rootGraph.selectionBoxDragging && rootGraph.currentGraph != null) // && fr.graph != null) // && fr.graphs.Count > 0) //fr.isDragging
					    {
					    	//Debug.Log("clear");
					    	rootGraph.selectedNodes = new Dictionary<int, Node>();
					    	selectionBox = new Rect(selectionBox.position.x - (rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0), selectionBox.position.y - topCanvasSpace, selectionBox.width , selectionBox.height );
					    	if (selectionBox.width < 0)
					    	{
					    		selectionBox.width = 0 - selectionBox.width;
					    		selectionBox.x = selectionBox.x - selectionBox.width;
					    	}
						    if (selectionBox.height < 0)
					    	{
					    		selectionBox.height = 0 - selectionBox.height;
					    		selectionBox.y = selectionBox.y - selectionBox.height;
					    	}
					    
					    	for (int g = 0; g < rootGraph.currentGraph.nodes.Count; g ++)
					    	{
						    	if (selectionBox.Contains(rootGraph.currentGraph.nodes[g].nodeRect.position * zoomFactor))
						    	{			   
							    	Node _n = null;
							    	if (!rootGraph.selectedNodes.TryGetValue(rootGraph.currentGraph.nodes[g].GetInstanceID(), out _n))
							    	{
								    	rootGraph.selectedNodes.Add(rootGraph.currentGraph.nodes[g].GetInstanceID(), rootGraph.currentGraph.nodes[g]);
							    	}
						    	}
					    	}
					    	
					    	
					    	rootGraph.selectionBoxDragging = false;
					    }
					    
					  
					    
					    isDragging = false;

				    	if (rootGraph.selectedNode != null)
				    	{
					    
					    	if (drawCurserHandle && !rootGraph.nodeSelectionPanelOpen)
					    	{
					    	
						    	nodeSelectionMousePos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y - topCanvasSpace);
						    	bool _abort = false;
						    	for (int n = 0; n < rootGraph.currentGraph.nodes.Count; n ++)
						    	{
						    		if (rootGraph.currentGraph.nodes[n].nodeData.nodeType == NodeAttributes.NodeType.Group)
							    		continue;
							    		
						    		var nodeSelectionMousePos2 = new Vector2((nodeSelectionMousePos.x - (rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0)) / zoomFactor, nodeSelectionMousePos.y / zoomFactor);
							    	
							    	if (rootGraph.currentGraph.nodes[n].nodeRect.Contains(nodeSelectionMousePos2))
						    		{
						    			_abort = true;
						    		}
						    	}
						    
						    	if (!_abort && currentEvent.button == 0)
						    	{
						    		rootGraph.nodeSelectionPanelOpen = true;
						    		
						    		NodePanel(nodeSelectionMousePos);
						    	}
						    	
					    		//if (rootGraph.nodeSelectionPanelOpen)
						    	//{
							    //	EditorGUI.FocusTextInControl ("FilterTextField");			    
						    	//}
						    
						    
						    	return;
	
					    	}
					    	else if (!rootGraph.nodeSelectionPanelOpen)
					    	{
					    		rootGraph.lastSelectedNodeIndex = -1;
						    	rootGraph.drawModeOn = false;
					    	}
				    	}
	    	
				    	//currentEvent.Use();
					    break;
				    case EventType.ScrollWheel:
					    
					    rootGraph.globalMouseEvents = Graph.GlobalMouseEvents.scrollWheel;			    
					    rootGraph.currentGraph.zoomCoordsOrigin = Vector2.zero;
					    
  
					    if(!_inspectorRect.Contains(Event.current.mousePosition))
					    {			    
						    Vector2 _screenCoordsMousePos = Event.current.mousePosition;
						    Vector2 _delta2 = Event.current.delta;
					    	Vector2 _zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(_screenCoordsMousePos);
						    
						    float _zoomDelta = -_delta2.y / 10.0f;
					    	float _oldZoom = zoomFactor; // zoom on mouse position -> node window bug?!
						    float _targetZoom = zoomFactor + _zoomDelta;
						    
					    	zoomFactor += _zoomDelta;
						    zoomFactor = Mathf.Clamp(zoomFactor, 0.3f, 1f);
						     _delta2 /= zoomFactor;
						   
						    //float _oldZoom = zoomFactor;
						    
						   
						    rootGraph.currentGraph.zoomCoordsOrigin += (_zoomCoordsMousePos - rootGraph.currentGraph.zoomCoordsOrigin) - (_oldZoom / zoomFactor) * (_zoomCoordsMousePos - rootGraph.currentGraph.zoomCoordsOrigin);
					    
					    	rootGraph.currentGraph.zoomFactor = zoomFactor;
					    
					    }
					    
					    Repaint();
					    
					    break;
				    case EventType.KeyDown:
					    
					    
					    if (_inspectorRect.Contains(currentEvent.mousePosition))
						    break;
						    
					    // Delete selected nodes
					    if (currentEvent.keyCode == KeyCode.Delete)      		    
					    {
					  
					    	if (rootGraph.selectedNodes.Keys.Count > 0)
					    	{
					    		// Check if nodes or subgraphs are selected
					    		bool _nodesSelected = false;
					    		bool _subGraphNodeSelected = false;
					    		foreach (var node in rootGraph.selectedNodes.Keys)
					    		{
						    		if (rootGraph.selectedNodes[node].GetType() != typeof(Comment) && rootGraph.selectedNodes[node].GetType() != typeof(Group) && rootGraph.selectedNodes[node].GetType() != typeof(SubGraph))
						    		{
							    		_nodesSelected = true;
						    		}
						    		
						    		if (rootGraph.selectedNodes[node].GetType() == typeof(SubGraph))
						    		{
						    			_subGraphNodeSelected = true;
						    		}
					    		}
					    		
					    		if (_nodesSelected)
					    		{
						    		if (  EditorUtility.DisplayDialog("Delete Node", "Are you sure you want to delete selected nodes?", "yes", "no"))
						    		{    	
							    		foreach (var node in rootGraph.selectedNodes.Keys)
							    		{
							    			if (rootGraph.selectedNodes[node].GetType() != typeof(Comment) && rootGraph.selectedNodes[node].GetType() != typeof(Group))
							    			{
								    			if (rootGraph.selectedNodes[node].GetType() != typeof(SubGraph))
								    			{
									    			rootGraph.selectedNodes[node].DeleteNode(rootGraph.currentGraph);
									    			rootGraph.selectedNode = null;
								    			}
							    			}
							    		}
						    		}
					    		}
					    		
					    		if (_subGraphNodeSelected)
					    		{
						    		if (  EditorUtility.DisplayDialog("Delete Subgraph", "Are you sure you want to delete selected sub graph?\nWarning: This can't be undone!", "yes", "no"))
						    		{  
						    			foreach (var node in rootGraph.selectedNodes.Keys)
						    			{
							    			if (rootGraph.selectedNodes[node].GetType() != typeof(Comment) && rootGraph.selectedNodes[node].GetType() != typeof(Group))
							    			{
								    			if (rootGraph.selectedNodes[node].GetType() == typeof(SubGraph))
								    			{
									    			rootGraph.selectedNodes[node].DeleteSubGraph(rootGraph.currentGraph);
								    			}
							    			}
						    			}
					    			}
					    		}
					    	}
					    	
					    	currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == settings.keyFocus && currentEvent.control)
					    {
					    	FocusCanvas(true);
					    }
					    
					    if (currentEvent.keyCode == settings.keyCreateComment && currentEvent.control)
					    {
					    	CreateComment();
					    }
					    
					    if (currentEvent.keyCode == settings.keyCreateGroup && currentEvent.control)
					    {
					    	if(rootGraph.selectedNodes.Count > 0)
					    	{
					    		CreateGroupFromSelected();
					    	}
					    	
					    	currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == settings.keyCreateSubGraph && currentEvent.control && currentEvent.alt)
					    {
					    	if (rootGraph.selectedNodes.Count > 0)
					    	{
					    		CreateSubGraphFromSelected();
					    	}
					    	
					    	currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == settings.keyExpandNodes && currentEvent.control)
					    {
						    ExpandAllNodes();	    
					    	currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == settings.keyCollapseNodes && currentEvent.control)
					    {
					    	CollapseAllNodes();
					    	currentEvent.Use();
					    }
					    
					    // Copy / Paste selected nodes
					    if (currentEvent.keyCode == KeyCode.C && currentEvent.control)
					    {
					    	copyPastNodeOffsetPositionMultiplier = 0;
					    	settings.tmpSelectedNodes = rootGraph.selectedNodes;
					    	currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == KeyCode.V && currentEvent.control)
					    {
						    if (settings.tmpSelectedNodes != null && settings.tmpSelectedNodes.Keys.Count > 0)
						    {	    
							    CopyPasteSelectedNodes(settings.tmpSelectedNodes);
						    }
						    
						    currentEvent.Use();
					    }
					    
					    if (currentEvent.keyCode == settings.keyAlignNodesLeft && currentEvent.control)
					    {
					    	AlignLeft();
					    }
					    
					    if (currentEvent.keyCode == settings.keyAlignNodesRight && currentEvent.control)
					    {
					    	AlignRight();
					    }
					    
					    if (currentEvent.keyCode == settings.keyAlignNodesTop && currentEvent.control)
					    {
					    	AlignTop();
					    }
					    
					    if (currentEvent.keyCode == settings.keyAlignNodesBottom && currentEvent.control)
					    {
					    	AlignBottom();
					    }
					    
					    if (currentEvent.keyCode == settings.keyAlignNodesAutomatically && currentEvent.control)
					    {
					    	DistributeNodes(GraphEditor.NodeAlignment.HorizontallyAndVertically);
					    }
					    
					    if (currentEvent.keyCode == settings.keyGotoParentGraph && currentEvent.control)
					    {
					    	GotoParentGraph();
					    	currentEvent.Use();
					    }
					   
					    Repaint();
					    
					    break;    
				 
			    }
	    
		   
		    }

			// Graph editor GUI modules
			// =========================
			DrawBackground();
		   
		    TopBar();
			Toolbar();
		   
			//==========================
			    
			if (rootGraph.showInspector)
			{
				if (rootGraph.inspectors == null || rootGraph.inspectors.Count == 0)
				{
					rootGraph.inspectors = new List<Graph.GraphInspectors>();
					rootGraph.inspectors.Add(new Graph.GraphInspectors(0, 200));
				}
				
				//if (GUILayout.Button("AASD"))
				//{
				//	rootGraph.inspectors = new List<Graph.GraphInspectors>();
				//	rootGraph.inspectors.Add(new Graph.GraphInspectors(0, 200));
				//}
				
			
				using (new GUILayout.AreaScope(new Rect(0, 0, rootGraph.tmpInspectorWidth, position.height)))
				{
					
					var _h = 0f;
					for (int c = 0; c < rootGraph.inspectors.Count; c ++)
					{
						
						var _iHeight = rootGraph.inspectors[c].height;
						
						if (c > 0)
						{
							_h += rootGraph.inspectors[c-1].height;
						}
						if (c == rootGraph.inspectors.Count-1)
						{
							_iHeight = position.height;
						}
						
					
						using (new GUILayout.AreaScope(new Rect(0, _h, rootGraph.tmpInspectorWidth-5, _iHeight)))
						{	
							//using (new GUILayout.HorizontalScope())
							//{
								switch (rootGraph.inspectors[c].selectedTab)
								{
								case 0:
									GUI.color = settings.GetInspectorColor("inspectorColor");
									break;
								case 1:
									GUI.color = settings.GetInspectorColor("blackboardColor");
									break;
								case 2:
									GUI.color = settings.GetInspectorColor("eventboardColor");
									break;
								default:
									GUI.color = settings.GetInspectorColor("");
									break;
								}
							
							
								using (new GUILayout.VerticalScope(editorSkin.GetStyle("InspectorLine")))
								{
									GUI.color = Color.white;
								
									CustomInspector(c);
									
								
							
									if (c < rootGraph.inspectors.Count-1)
									{
										rootGraph.inspectors[c].splitRect = new Rect(0, _h + rootGraph.inspectors[c].height - 5, rootGraph.tmpInspectorWidth, 5);
										EditorGUIUtility.AddCursorRect(new Rect(rootGraph.inspectors[c].splitRect.x, rootGraph.inspectors[c].splitRect.y - _h, rootGraph.tmpInspectorWidth, 5), MouseCursor.ResizeVertical);
									}
								
								}
							//}
						}
						
						if (c < rootGraph.inspectors.Count-1)
						{
							using (var areaScope = new GUILayout.AreaScope(rootGraph.inspectors[c].splitRect))
							{
								if (GUILayout.Button("","TextArea", GUILayout.Height(6), GUILayout.MaxHeight(6), GUILayout.MinWidth(6), GUILayout.ExpandWidth(true))){}
							}
						}
					
					}
					
					// splitt view
					splitterRect = new Rect(rootGraph.tmpInspectorWidth - 5, -1, 10, position.height);
					EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);

					using (var areaScope = new GUILayout.AreaScope(splitterRect))
					{
						if (GUILayout.Button("","TextArea", GUILayout.Width(6), GUILayout.MaxWidth(6), GUILayout.MinWidth(6), GUILayout.ExpandHeight(true))){}
					}
				}
				
				
			
			}
			
			// Selection box dragging
			nodeCanvas = new Rect(rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0, topCanvasSpace, position.width - (rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0), position.height);

			if (nodeCanvas.Contains (Event.current.mousePosition) && isDragging && !rootGraph.isMouseOverNode && rootGraph.selectedNodes.Keys.Count == 0 && !rootGraph.drawModeOn && rootGraph.selectionBoxDragging  && !rootGraph.dragNodeFromNodeMenu) // && !showNodePanelMouse)//&& fr.selectedNode == null
			{
			    // Drag selection box
			    selectionBox = new Rect(mouseDragStart.x, mouseDragStart.y, Event.current.mousePosition.x - mouseDragStart.x,  Event.current.mousePosition.y - mouseDragStart.y);
				GUI.Box(selectionBox, "", editorSkin.GetStyle("SelectionBox"));
		    }
			
			// Graph title	    
			GUI.skin.label.richText = true;
			using (new GUILayout.AreaScope(new Rect((rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0) + 20, 80, position.width, 100)))
			{
				using (new GUILayout.HorizontalScope())
				{
					if (rootGraph.currentGraph.isCopy)
					{
						GUILayout.Label("<size=22><b>" + rootGraph.currentGraph.name + "</b> (Instance)</size>");
					}
					else
					{
						if (rootGraph.parentLevels.Count > 0)
						{
							//string _levelNames = "";
							for (int p = 0; p < rootGraph.parentLevels.Count; p ++)
							{
								if (rootGraph.parentLevels[p].graph == rootGraph.currentGraph)
								{
									if (GUILayout.Button("<color=#A58CCD><b>" + rootGraph.parentLevels[p].name + " </b></color>", editorSkin.GetStyle("ButtonLargeText")))
									{
										MoveToGraph(p);
									}
								} 
								else
								{
									if (GUILayout.Button( rootGraph.parentLevels[p].name + " / " , editorSkin.GetStyle("ButtonLargeText")))
									{
										MoveToGraph(p);
									}
									var _r = GUILayoutUtility.GetLastRect();
									if (_r.Contains(Event.current.mousePosition))
									{
										if (GUI.Button( _r, "<color=#FFFFFF>" + rootGraph.parentLevels[p].name + "</color> / " , editorSkin.GetStyle("ButtonLargeText")))
										{
											MoveToGraph(p);
										}
									}
								}
								
								//if (rootGraph.parentLevels[p].graph == rootGraph.currentGraph)
								//{
								//	_levelNames +=  "<size=22><b><color=#A58CCD>" + rootGraph.parentLevels[p].name + " </color>/ </b></size>";
								//}
								//else
								//{
								//	_levelNames +=  "<size=22>" + rootGraph.parentLevels[p].name + " / </size>";
								//}
							}
							
							//GUILayout.Label(_levelNames);
							
						
						}
						else
						{
							GUILayout.Label("<size=22><b>" + rootGraph.name + "</b></size>");
						}
						
					}
					GUILayout.FlexibleSpace();
				}
			}
			
			GUI.color = Color.white;
	
	
			//////////////////////////////////////////
		    // Start node canvas
			//////////////////////////////////////////	   	    
		    EditorZoomArea.Begin(zoomFactor, nodeCanvas);

			if (rootGraph.currentGraph == null)
			    return;
			    
	        // Show all nodes
			//===============	
			Exception _nodeException = null;
			Node _exceptionNode = null;
			
			if (rootGraph.currentGraph != null)
			{
				// First draw only group nodes
				for (int o = 0; o < rootGraph.currentGraph.nodes.Count; o ++)
				{
					var _node = rootGraph.currentGraph.nodes[o];
				
					if (_node.guiDisabled)
					{
						GUI.color = new Color(255f/255f, 255f/255f, 255f/255f, 20f/255f);
					}
					
					try
					{
						if (_node.nodeData.nodeType == NodeAttributes.NodeType.Group)
						{
							DrawNode(_node, o);
						}
					}
					catch	{}
					
					GUI.color = Color.white;
				}
				
				// Draw all other nodes
				
				for (int o = 0; o < rootGraph.currentGraph.nodes.Count; o ++)
				{

					var _node = rootGraph.currentGraph.nodes[o];
				
					if (_node.guiDisabled)
					{
						GUI.color = new Color(255f/255f, 255f/255f, 255f/255f, 20f/255f);
					}
					
					try
					{
						if (_node.nodeData.nodeType !=  NodeAttributes.NodeType.Group)
						{
							DrawNode(_node, o);
						}
					}
					catch (Exception e)
					{
						// Node class doesn't exist anymore
						_nodeException = e;
						_exceptionNode = _node;
					}
					
					GUI.color = Color.white;
				
				}
				
				
				if (rootGraph.currentGraph.isCopy)
				{
					ShowNotification(new GUIContent("Warning: Graph instance should not be edited"));
				}
				
			}    
			//=========================
		    
		    
			// Draw handle on mouse position
			if (rootGraph.lastSelectedNodeIndex > -1)
		    { 
		    	var _mousePos = Event.current.mousePosition;
				if (rootGraph.nodeSelectionPanelOpen)
		    	{
					_mousePos = new Vector2((nodeSelectionMousePos.x - (rootGraph.showInspector ? rootGraph.tmpInspectorWidth : 0)) / zoomFactor, (nodeSelectionMousePos.y) / zoomFactor);
		    	}

				DrawHandlesCursor(new Rect(rootGraph.nodeConnectPoint.x - 45, rootGraph.nodeConnectPoint.y, 20,0), _mousePos, rootGraph.currentGraph.nodes[rootGraph.lastSelectedNodeIndex]);		    
		    }
		    else
		    {
		    	drawCurserHandle = false;
		    }
		   
		    
			if (Application.isPlaying && rootGraph.currentNode != null) //  && fr.currentLevel == fr.currentNode.nodeLevel)//&& !fr.currentNode.isHidden
		    {
				indicatorRect = new Rect(rootGraph.currentNode.nodeRect.x, rootGraph.currentNode.nodeRect.y, rootGraph.currentNode.nodeRect.width, rootGraph.currentNode.nodeRect.height);
		    	GUI.color = new Color(255f/255f, 255f/255f, 255f/255f, 120f/255f);
		    	GUI.Box(indicatorRect, "", editorSkin.GetStyle("Indicator"));
		    	GUI.color = Color.white;
		    }
	
		    EditorZoomArea.End();
		
			if (_nodeException != null)
			{
				DrawFaultyNode(_nodeException, _exceptionNode);
			}
		
			// MINIMAP
			// ===========
			if (rootGraph.showMinimap)
			{
				MiniMap();
			}
			
			
			// Node side panel
			// ===============
			//if (rootGraph.showNodeSidePanel)
			//{
				//NodeSidePanel();
			//}
			
			
			// Blackboard variable drag UI element
			BlackboardVariableDragGUI();
		    
	        // Abort draw handles on right click
	        if (Event.current.type == EventType.ContextClick)
	        {
		        rootGraph.lastSelectedNodeIndex = -1;
	        }
	
		    if (didPan) Repaint();
		    if (isDragging) Repaint();
			if (rootGraph.isMouseOverNode) Repaint();
		    
	       
		    if (!EditorApplication.isPlaying)
		    {
		    	try
		    	{
			        if (EditorGUI.EndChangeCheck())
			        {
					    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			        }
			    }
		    	catch{}
		    }
		    
	    }
#endregion ongui
		

		void BlackboardVariableDragGUI()
		{
			Event _evt = Event.current;
			if (BlackBoardVariableDragProperties.isDragging)
			{
				// Bug Fix workaround. We need to close all blackboard foldouts
				// when user draggs a variable
				foreach (var b in rootGraph.blackboards.Keys)
				{
					if (b != BlackBoardVariableDragProperties.blackboardGuid)
					{
						rootGraph.blackboards[b].foldout = false;	
					}
				}
				
				GUI.Box(new Rect(_evt.mousePosition.x, _evt.mousePosition.y, 200, 25 ), BlackBoardVariableDragProperties.variable.name, editorSkin.GetStyle("Box"));
				//var _startPos = new Vector3(BlackBoardVariableDragProperties.startDragPosition.x + 5, BlackBoardVariableDragProperties.startDragPosition.y + 28, 0);
				//var _endPos = new Vector3(_evt.mousePosition.x, _evt.mousePosition.y, 0);
				//Handles.DrawBezier(_startPos, _endPos, _startPos + Vector3.right * 20, _endPos + Vector3.left * 20, Color.white, null, 2);
			}
			
			if (_evt.type == EventType.MouseUp)
			{
				BlackBoardVariableDragProperties.isDragging = false;		
				
				//_evt.Use();
			}
		}
		
		
		IEnumerator CreateNodeInspectorEditor(Node _selectedNode, bool _shift)
		{
		
			nodeInspector = UnityEditor.Editor.CreateEditor(_selectedNode);
		
			//GetAvailableNodeVariables(_selectedNode);
			if (_selectedNode != lastSelectedNode)
			{
				_selectedNode.BuildVariableGroups();
			}
			
			yield return new WaitForSeconds(0.1f);		

			lastSelectedNode = _selectedNode;
			rootGraph.selectedNode = _selectedNode;
			 
			rootGraph.isMouseOverNode = true;						    
							    			    	
			rootGraph.selectedNode.OnSelect(rootGraph.currentGraph);
							    	
			Repaint();
		}
	

#region copyPasteNodes

		void CopyPasteSelectedNodes(Dictionary<int, Node> _selectedNodes)
		{
			copyPastNodeOffsetPositionMultiplier ++;
			
			var _selectedNodesList = _selectedNodes.Keys.ToList();
			var _copiedNodes = new List<Node>();
			
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			
			for (int n = 0; n < _selectedNodesList.Count; n ++)
			{
				SerializationUtility.SerializeValue(_selectedNodes[_selectedNodesList[n]], DataFormat.Binary, out settings.objectReferences);
				var _original1 = settings.objectReferences[0] as Node;
				var _newCopy = NodeCreator.CreateNodeFromCopy(_original1.nodeData, _original1, rootGraph, rootGraph.currentGraph, _original1.nodeData.typeName, copyPastNodeOffsetPositionMultiplier);
				
				
				_newCopy.outputNodes = new List<Node.NodeOutput>();
				_newCopy.inputNodes = new List<Node.InputNode>();
				
				for (int o = 0; o < _original1.outputNodes.Count; o ++)
				{
					_newCopy.outputNodes.Add(new Node.NodeOutput(_original1.outputNodes[o].id, _original1.outputNodes[o].outputNode));
					_newCopy.outputNodes[_newCopy.outputNodes.Count-1].guid = _original1.outputNodes[o].guid;
				}
				
				
				for (int i = 0; i < _original1.inputNodes.Count; i ++)
				{
					_newCopy.inputNodes.Add(new Node.InputNode(_original1.inputNodes[i].inputNode));
					_newCopy.inputNodes[_newCopy.inputNodes.Count-1].guid = _original1.inputNodes[i].guid;
				}
				
			
				
				rootGraph.selectedNodes.Add(_newCopy.GetInstanceID(), _newCopy);
				
		
				
				_copiedNodes.Add(_newCopy);
				 
				// check if copied node has exposed variables
				//var _fields = _newCopy.GetType().GetFields((BindingFlags.Public | BindingFlags.Instance));
				List<FRVariable> _fields = new List<FRVariable>();
				GetAvailableVariableTypes.GetAllFRVariablesOnNode(_newCopy, out _fields);
				for(int f = 0; f < _fields.Count; f ++)
				{
					//if (_fields[f].FieldType.BaseType == typeof(FRVariable))
					//{
					//	var _variable = _fields[f].GetValue(_newCopy) as FRVariable;
					//	if (_variable == null)
					//		continue;
							
						if (_fields[f].type == FRVariable.VariableType.exposed)
						{
							// register exposed variable
							//Debug.Log("exposed variable");
							
							_fields[f].nodeOwner = _newCopy;
							
							FRVariableGUIUtility.ExposeVariable(rootGraph, _newCopy, _fields[f], _fields[f].name);
						}
					//}
				}
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
			
			for (int n = 0; n < _copiedNodes.Count; n ++)
			{
				// Add blackboard connected node variables to blackboard connected nodes list
				List<FRVariable> _allNodeVariables;
				GetAvailableVariableTypes.GetAllFRVariablesOnNode(_copiedNodes[n], out _allNodeVariables);
				for (int v = 0; v < _allNodeVariables.Count; v ++)
				{
					if (_allNodeVariables[v] == null)
						continue;
						
					if (_allNodeVariables[v].type == FRVariable.VariableType.blackboard)
					{
						if (rootGraph.blackboards[Guid.Parse(_allNodeVariables[v].blackboardGuid)].blackboard.variables[Guid.Parse(_allNodeVariables[v].variableGuid)].connectedNodes == null)
						{
							rootGraph.blackboards[Guid.Parse(_allNodeVariables[v].blackboardGuid)].blackboard.variables[Guid.Parse(_allNodeVariables[v].variableGuid)].connectedNodes = new List<FlowReactor.Nodes.Node>();
						}
															
						rootGraph.blackboards[Guid.Parse(_allNodeVariables[v].blackboardGuid)].blackboard.variables[Guid.Parse(_allNodeVariables[v].variableGuid)].connectedNodes.Add(_copiedNodes[n]);
					
					}
				}
			}
			
			// Copy selected subgraphs
			for (int h = 0; h < _copiedNodes.Count; h ++)
			{
				if (_copiedNodes[h].nodeData.nodeType == NodeAttributes.NodeType.SubGraph)
				{
					var _subGraphNode = _copiedNodes[h] as SubGraph;
					var _cs = _subGraphNode.subGraph.CopyEditorSubGraph(rootGraph, rootGraph.currentGraph, settings);
					
					_subGraphNode.subGraph = _cs;
					
					_cs.subGraphNode = _subGraphNode;
				}
			}
		}
		
#endregion copyPasteNodes
		
		void CleanUpBrokenNodes()
		{
			for (int i = 0; i < rootGraph.currentGraph.nodes.Count; i ++)
			{
				if (rootGraph.currentGraph.nodes[i] == null)
				{
					rootGraph.currentGraph.nodes.RemoveAt(i);
				}
			}
		}

		// Draw node window on canvas
		// ==========================
		
		void DrawNode(Node _node, int _n)
		{
			if (editorSkin == null)
			{
				LoadEditorResources();
			}
			
			if (_node.nodeData.nodeType != NodeAttributes.NodeType.Group && _node.nodeData.nodeType != NodeAttributes.NodeType.Comment)
			{
				GUI.depth = 0;
				
				// Node description
				if (!string.IsNullOrEmpty(_node.nodeData.description))
				{
					GUI.Label (
						new Rect(_node.nodeRect.x,
						_node.nodeRect.y + _node.nodeRect.height, // + (_node.showNodeVariables ? _node.nodeVariablesFieldHeight : 0),
						_node.nodeRect.width, 200), 
						_node.nodeData.description, FlowReactorEditorStyles.nodeDescriptionStyle);
				}
				
				// draw portal icon if it is a portal node		
				if (_node.nodeData.nodeType == NodeAttributes.NodeType.Portal)
				{
					var _portalIcon = editorSkin.GetStyle("PortalIcon");
					var _pRect = _node.nodeRect;
					GUI.Box(new Rect(_pRect.x + _pRect.width, _pRect.y + (_pRect.height / 2) - 10, 30, 30), "", _portalIcon);
				}

			}
			else
			{
				GUI.depth = 1;
			}
			
			
			/// DRAW NODE GUI
			_node.DrawGUI("", _n, rootGraph.currentGraph, editorSkin);
			
			// Node selection box  
			if (_node.nodeData.nodeType != NodeAttributes.NodeType.Group && _node.nodeData.nodeType != NodeAttributes.NodeType.Comment)
			{
				
				var _startEndStyle = editorSkin.GetStyle("StartEnd");
				Node _isNodeSelected = null;
				if (rootGraph.selectedNodes.TryGetValue(_node.GetInstanceID(), out _isNodeSelected))
				{
					var _selectedBox = new Rect(_node.nodeRect.x - 5, _node.nodeRect.y - 5, _node.nodeRect.width + 10, _node.nodeRect.height + 10);
					GUI.Box(_selectedBox, "", _startEndStyle);
				}
			}
				
			// Draw Error box
			if (_node.hasError)
			{
				var _startEndStyle = editorSkin.GetStyle("StartEnd");
				var _errorBox = new Rect(_node.nodeRect.x - 5, _node.nodeRect.y - 5, _node.nodeRect.width + 10, _node.nodeRect.height + 10);
				GUI.color = Color.red;
				GUI.Box(_errorBox, "", _startEndStyle);
				GUI.color = Color.white;
				
				GUI.Label(new Rect(_node.nodeRect.x - 5, _node.nodeRect.y - 30, 20, 20), nodeErrorIcon);
			}
			
			Rect _startRect = new Rect(0, 0, 0, 0);
			Rect _endRect = new Rect(0, 0, 0, 0);
			
			// Draw Handles
			for (int e = 0; e < _node.outputNodes.Count; e++)
			{           	
				if (_node.outputNodes[e] != null && _node.outputNodes[e].outputNode != null)
				{
					var _drawHandlesOff = false;
			            
					var _highlightHandle = false;
		                       
					if (_node.isMouseOver)
					{
						_highlightHandle = true;
					}
			             	
					_startRect = _node.nodeRect;
					_startRect = new Rect(_startRect.x, _startRect.y + (e * 20), _startRect.width, _startRect.height);
				            	
				
					_endRect = _node.outputNodes[e].outputNode.nodeRect;
	
			            
					if (!_drawHandlesOff)
					{     
						DrawHandles(_startRect, _endRect, _node.nodeRect, _highlightHandle, e, _node, _node.outputNodes[e].outputNode);   
					}
				}
			}
		}
		
		void DrawFaultyNode(Exception exception, Node _node)
		{
			
			using (new GUILayout.AreaScope(new Rect(rootGraph.tmpInspectorWidth, position.height - 120, position.width - rootGraph.tmpInspectorWidth, 70)))
			{
				using (new GUILayout.VerticalScope("TextArea"))
				{
					using (var scrollView = new GUILayout.ScrollViewScope(scrollPos))
					{
						scrollPos = scrollView.scrollPosition;
						EditorGUILayout.HelpBox("Warning a node could not be drawn, this is probably because you have renamed a node class. You can either rename the class back or click on clenaup to remove the broken node from your graph.\n" +
							settings.errorReport + "\n" + exception.StackTrace, MessageType.Error);
					}
				}
			}
			
			if (GUI.Button(new Rect(rootGraph.tmpInspectorWidth, position.height - 50, position.width - rootGraph.tmpInspectorWidth, 50), "Cleanup broken node")) //
			{
				CleanUpBrokenNodes();
			}
		}
		
		
		void NodePanel(Vector2 _pos)
		{
			rootGraph.selectedNode = null;
			Selection.activeGameObject = null;
			var _nodePanelRect = new Rect(_pos.x, _pos.y + 350, 270, 350);
			isDragging = false;
			Event.current.Use();
			FlowReactor.Editor.EditorCoroutines.Execute(OpenNodePanelDelayed(_pos));
		}   
		
		IEnumerator OpenNodePanelDelayed(Vector2 _pos)
		{
			var _nodePanelRect = new Rect(_pos.x, _pos.y + 350, 270, 350);
			//yield return new WaitForSeconds(0.1f);
			double _startTime = EditorApplication.timeSinceStartup;
			while (EditorApplication.timeSinceStartup < _startTime + 0.01f)
			{
				yield return null;
			}
			PopupWindow.Show(_nodePanelRect, new NodePanelPopup.NodePanel(_nodePanelRect, rootGraph, this, editorSkin, settings));				
		}
		
		void DrawBackground()
		{ 
			var _topSpace = 18;

			GUI.color = bgGrey;
			GUI.Box(new Rect(rootGraph.tmpInspectorWidth, _topSpace, position.width, position.height), "");
			GUI.color = Color.white;

			GUI.DrawTextureWithTexCoords(new Rect(rootGraph.tmpInspectorWidth, _topSpace, position.width, position.height), canvasBackgroundTexture, new Rect(0, 0, position.width / canvasBackgroundTexture.width, position.height / canvasBackgroundTexture.height));
		}
		
		void CreateGraph()
		{
			
			rootGraph.showInspector = false;
			rootGraph.tmpInspectorWidth = 0;	
			
			// Set default graph values
			rootGraph.isActive = true;
			rootGraph.isRoot = true;
			rootGraph.isCopy = false;
			
			rootGraph.nodes = new List<Node>();
			rootGraph.tmpNodes = new List<Node>();
			rootGraph.subGraphs = new List<Graph>();
				
			rootGraph.currentGraph = rootGraph;
			rootGraph.rootGraph = rootGraph;
			rootGraph.currentGraph.minimapSize = new Vector2(170, 100);
			rootGraph.currentGraph.minimapMinSize = new Vector2(170, 100);
			rootGraph.inspectorWidth = 400;
			rootGraph.eventboards = new Dictionary<Guid, Graph.Eventboards>();
			rootGraph.blackboards = new Dictionary<Guid, Graph.Blackboards>();
			
			collectedNodes = CollectAvailableNodes.CollectNodes();
			
			// Better than AssetDatabase SaveAsset or Refresh for refreshing node
			//Undo.SetCurrentGroupName( "Create Node" );
			//int undoGroup = Undo.GetCurrentGroup();
			//Undo.RecordObject(	rootGraph, "Create Node");
			NodeCreator.CreateDefaultNodes(collectedNodes, rootGraph);
			//Undo.CollapseUndoOperations( undoGroup );
		}
	
#region drawHandles

		void DrawHandles(Rect _startWindowRect, Rect _endWindowRect, Rect _nodeWindow, bool _highlightHandle, int _outputCount, Node _startNode, Node _endNode )
		{
			float _width = 3f;
			float _startSpace = 35;
			float _endSpace = 35;
			
			
		    Vector3 _startPos = new Vector3(_startWindowRect.x + _startWindowRect.width + 10, _startWindowRect.y + _startSpace, 0);
		    Vector3 _endPos = Vector3.zero;
	
		    Vector3 _startTan = _startPos + Vector3.right * 20;
		   
	        Vector3 _endTan = Vector3.zero;
		    
		
			Color _splineColor = _startNode.color; // Color.grey;
			
			if (_startNode.outputNodes[_outputCount].endlessLoop)
			{
				_splineColor = Color.red;
			}
			
			if (_startNode.guiDisabled)
			{
				_splineColor = new Color(_startNode.color.r, _startNode.color.g, _startNode.color.b, 20f/255f);
			}
			
			if (_startNode.bypass)
			{
				if (_outputCount != _startNode.bypassOutput)
				{
					_splineColor = new Color(_startNode.color.r, _startNode.color.g, _startNode.color.b, 20f/255f);
				}
			}
	
	        
			_endPos = new Vector3(_endWindowRect.x - 10, _endWindowRect.y + _endSpace, 0);
			_endTan = _endPos + Vector3.left * 20;

		    if (_highlightHandle)
		    {
		    	_splineColor = Color.white;
		    }
		    
		
			if ((_startNode.nodeData.nodeType == NodeAttributes.NodeType.Portal && _highlightHandle) || (_startNode.nodeData.nodeType != NodeAttributes.NodeType.Portal))
			{
			    //draw loop handles
				if (_startNode == _endNode)
			    {
			    	_splineColor = new Color(255f/255f, 80f/255f, 0f/255f);
			    	
			    	Vector3 _startPos2 = new Vector3(_startWindowRect.x + _startWindowRect.width, _startWindowRect.y + _startSpace, 0);
			    	Vector3 _endPos2 = new Vector3(_endWindowRect.x, _endWindowRect.y + _endSpace, 0);
			    	var _pos1 = new Vector3( _nodeWindow.x + _nodeWindow.width, _nodeWindow.y - 10, 0);
			    	var _pos2 = new Vector3( _nodeWindow.x, _nodeWindow.y - 10, 0);
			    	Handles.DrawBezier(_startPos2, _pos1, _startPos2 + Vector3.right * 20, _pos1 + Vector3.right * 20, _splineColor, null, _width);
			    	Handles.DrawBezier(_pos1, _pos2, _pos1 + Vector3.left * 20, _pos2 + Vector3.right * 20, _splineColor, null, _width);
			    	Handles.DrawBezier(_pos2, _endPos2, _pos2 + Vector3.left * 20, _endPos2 + Vector3.left * 20, _splineColor, null, _width);
			    }
				else if (_startPos.x - 30 > _endPos.x)
				{
					
					var _offset = _outputCount * 5;
					var _xoffset = _outputCount * 5;
					
					Vector3 _startPos2 = new Vector3(_startWindowRect.x + _startWindowRect.width  + _xoffset, _startWindowRect.y + _startSpace, 0);
					Vector3 _endPos2 = new Vector3(_endWindowRect.x - _xoffset, _endWindowRect.y + _endSpace, 0);
					Vector3 _pos1 = Vector3.zero;
					Vector3 _pos2 = Vector3.zero;
					
					// middle bezier line position
					if (_startPos.y + ((_nodeWindow.height) * 0.5f) < _endPos.y) //((_nodeWindow.height + (_startNode.showNodeVariables ? _startNode.nodeVariablesFieldHeight : 0)) * 0.5f) < _endPos.y)
					{
						// calculate y position
						var _a =_nodeWindow.y + _nodeWindow.height;
						var _b =_endWindowRect.y - 20;
						var _c = (_b - _a) * 0.5f;
					
						_pos1 = new Vector3( _nodeWindow.x + _nodeWindow.width + _xoffset, _a + _c + _offset, 0);
						_pos2 = new Vector3( _endWindowRect.x - _xoffset, _a + _c + _offset, 0);
						//_pos1 = new Vector3( _nodeWindow.x + _nodeWindow.width + _xoffset, ((_nodeWindow.y  + _nodeWindow.height + (_startNode.showNodeVariables ? _startNode.nodeVariablesFieldHeight : 0) ) + (_endWindowRect.y - (_nodeWindow.y  + _nodeWindow.height )) / 2.2f) + _offset , 0);
						//_pos2 = new Vector3( _endWindowRect.x - _xoffset, ((_nodeWindow.y  + _nodeWindow.height) + (_endWindowRect.y - (_nodeWindow.y  + _nodeWindow.height )) / 2.2f) + _offset , 0);
					}
					else
					{
						var _a =_nodeWindow.y + _nodeWindow.height;
						var _b =_endWindowRect.y + _endWindowRect.height;
						//var _c = (_b - _a) * 0.5f;
						if (_nodeWindow.y + _nodeWindow.height > _endWindowRect.y + _endWindowRect.height)
						{
							_pos1 = new Vector3(_nodeWindow.x + _nodeWindow.width + _xoffset, _a + _offset, 0);
							_pos2 = new Vector3(_endWindowRect.x - + _xoffset, _a + _offset, 0);
						}
						else
						{
							_pos1 = new Vector3(_nodeWindow.x + _nodeWindow.width + _xoffset, _b + _offset, 0);
							_pos2 = new Vector3(_endWindowRect.x - + _xoffset, _b + _offset, 0);
						}
						
						//_pos1 = new Vector3(_nodeWindow.x + _nodeWindow.width + _xoffset, _nodeWindow.y + _nodeWindow.height + 30 + _offset , 0);
						//_pos2 = new Vector3(_endWindowRect.x - + _xoffset, _nodeWindow.y + _nodeWindow.height + 30 + _offset, 0);
					}
					
					// additional line between start and curved start line. Only when there's more than one output
					if (_outputCount > 0)
					{
						Handles.DrawBezier(new Vector3(_startPos2.x - _xoffset, _startPos2.y, _startPos2.z), _startPos2, new Vector3(_startPos2.x - _xoffset, _startPos2.y, _startPos2.z), _startPos2, _splineColor, null, _width);
						Handles.DrawBezier(new Vector3(_endPos2.x + _xoffset, _endPos2.y, _endPos2.z), _endPos2, new Vector3(_endPos2.x - _xoffset, _endPos2.y, _endPos2.z), _endPos2, _splineColor, null, _width);
					}
					
					Handles.DrawBezier(_startPos2, _pos1, _startPos2 + Vector3.right * 30, _pos1 + Vector3.right * 30, _splineColor, null, _width);
					Handles.DrawBezier(_pos1, _pos2, _pos1 + Vector3.left * 30, _pos2 + Vector3.right * 30, _splineColor, null, _width);
					Handles.DrawBezier(_pos2, _endPos2, _pos2 + Vector3.left * 30, _endPos2 + Vector3.left * 30, _splineColor, null, _width);
					
					if (_startNode.outputNodes[_outputCount].endlessLoop)
					{
						var _rect = new Rect(((_pos1.x + 20 + (_pos2.x - _pos1.x ) / 2) - 150) - (_xoffset + (_outputCount + 1) * 10), (_pos1.y + (_pos2.y - _pos1.y) / 2), 300, 20);	
						GUI.Label(_rect, new GUIContent("endless loop - use repeat or wait node", nodeErrorIcon));

					}
					//GUI.color = _splineColor;
					//if (GUI.Button(_btnRect, "", editorSkin.GetStyle("DeleteHandle")))
					//{
					//	var _i = _startNode.outputNodes[_outputCount].outputNode;
					//	_i.RemoveInputNode(_startNode); //(_i, _startNode);
					//	//_startNode.nextNode[_outputCount].inputNode = null;
					//	_startNode.outputNodes[_outputCount] = null;
					//}
					GUI.color = Color.white;
				}
			    else
			    {
			    	var _pos1 = new Vector3(_startWindowRect.x + _startWindowRect.width, _startWindowRect.y + _startSpace, 0);
			    	var _pos2 = new Vector3(_endWindowRect.x, _endWindowRect.y + _endSpace, 0);
			    	
			    	
			    	Handles.DrawBezier(_pos1, _startPos, _pos1 + Vector3.right, _startPos + Vector3.left, _splineColor, null, _width);
			    	Handles.DrawBezier(_endPos, _pos2, _endPos + Vector3.right, _pos2 + Vector3.left, _splineColor, null, _width);
			    	Handles.DrawBezier(_startPos, _endPos, _startTan, _endTan, _splineColor, null, _width);
			    	
			    	
				    if (_startNode.outputNodes[_outputCount].endlessLoop)
				    {
					    var _rect = new Rect((_startPos.x + (_endPos.x - _startPos.x ) / 2) - 150, (_startPos.y + (_endPos.y - _startPos.y) / 2), 300, 20);
					    GUI.Label(_rect, new GUIContent("endless loop - use repeat or wait node", nodeErrorIcon));
				    }
				    
				    //GUI.color = _splineColor;
			    	//if (GUI.Button(new Rect((_startPos.x + (_endPos.x - _startPos.x ) / 2) - 10, (_startPos.y + (_endPos.y - _startPos.y) / 2) - 10, 20, 20), "", editorSkin.GetStyle("DeleteHandle")))
			    	//{
			    	//	var _i = _startNode.outputNodes[_outputCount].outputNode;
				    //	_i.RemoveInputNode(_startNode); //(_i, _startNode);
			    	//	//_startNode.nextNode[_outputCount].inputNode = null;
				    //	_startNode.outputNodes[_outputCount] = null;			    	
			    	//}
			    	GUI.color = Color.white;
			    }
			}
			
		}
		
		void DrawHandlesCursor(Rect _startWindowRect, Vector2 _mousePos, Node _startNode)
		{
			float _startSpace = 35;
		
			Vector3 _startPos = new Vector3(_startWindowRect.x + _startWindowRect.width, (_startWindowRect.y + _startSpace) + (rootGraph.lastSelectedOutput * 20), 0);
			Vector3 _endPos = new Vector3(_mousePos.x, _mousePos.y, 0);
	        Vector3 _startTan = _startPos + Vector3.right * 50;
	        Vector3 _endTan = _endPos + Vector3.left * 50;
	        
			Handles.DrawBezier(_startPos, _endPos, _startTan, _endTan, Color.grey, null, 3);
	        
			drawCurserHandle = true;
		}
		
#endregion drawHandles
		
		void OnInspectorUpdate()
		{
			//Repaint();
		}
	    
		// Load all editor icons and graphics
		static void LoadEditorResources()
		{
	
			subGraphIcon = EditorHelpers.LoadGraphic("subGraphIcon.png");
			commentIcon = EditorHelpers.LoadGraphic("commentIcon.png");
			groupIcon = EditorHelpers.LoadGraphic("groupIcon.png");
			focusIcon = EditorHelpers.LoadGraphic("focusIcon.png");
			inspectorIcon = EditorHelpers.LoadGraphic("inspectorIcon.png");
			blackBoardIcon = EditorHelpers.LoadGraphic("blackBoardIcon.png");
			eventBoardIcon = EditorHelpers.LoadGraphic("eventBoardIcon.png");
			minimapIcon = EditorHelpers.LoadGraphic("minimapIcon.png");
			minimapResizeIcon = EditorHelpers.LoadGraphic("minimapResizeIcon.png");
			
			expandNodesIcon = EditorHelpers.LoadGraphic("expandNodesIcon.png");
			collapseNodesIcon = EditorHelpers.LoadGraphic("collapseNodesIcon.png");
			
			alignLeftIcon = EditorHelpers.LoadGraphic("alignLeftIcon.png");
			alignRightIcon = EditorHelpers.LoadGraphic("alignRightIcon.png");
			alignTopIcon = EditorHelpers.LoadGraphic("alignTopIcon.png");
			alignBottomIcon = EditorHelpers.LoadGraphic("alignBottomIcon.png");
			distributeHorizontalIcon = EditorHelpers.LoadGraphic("horizontalIcon.png");
	
			createSubGraphIcon = EditorHelpers.LoadGraphic("createSubGraphIcon.png");
			canvasBackgroundTexture = EditorHelpers.LoadGraphic("canvasBackground.png");
			
			nodeRunningIcon = EditorHelpers.LoadGraphic("nodeRunningIcon.png");
			nodeNotRunningIcon = EditorHelpers.LoadGraphic("nodeNotRunningIcon.png");
			nodeErrorIcon = EditorHelpers.LoadGraphic("errorIcon.png");
			
			logo = EditorHelpers.LoadGraphic("logo.png");
			gizmoIcon = EditorHelpers.LoadGraphic("gizmo.png");
			windowLogoIcon = EditorHelpers.LoadGraphic("windowLogoIcon.png");
			
			editIcon = EditorHelpers.LoadIcon("editIcon.png");
			okIcon = EditorHelpers.LoadIcon("checkmarkIcon.png");
			cancelIcon = EditorHelpers.LoadIcon("cancelIcon.png");
			
			editorSkin = EditorHelpers.LoadSkin();
		}
	    
		void OnDestroy()
		{
			if (rootGraph == null)
				return;
				
			rootGraph.selectedNode = null;
			rootGraph.selectedNodes = new Dictionary<int, Node>();
			
			// Cleanup graph editor window
			var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
			
			_settings.CleanupGraphWindows(rootGraph);
			
		
		}
	}
}
#endif
