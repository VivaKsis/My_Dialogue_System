using UnityEngine;
using System.Collections;

using FlowReactor;

namespace FlowReactor.Nodes
{
	[NodeAttributes("FlowReactor/Flow", "Connects a node with a hidden spline", "flowNodeColor", 1, NodeAttributes.NodeType.Portal)]
	public class Portal : Node
    {
	    
	    #if UNITY_EDITOR
	    public override void Init(Graph _graph, Node _node)
	    {
		    base.Init(_graph, _node);
		    icon = EditorHelpers.LoadIcon("portalIcon.png");
		    disableDefaultInspector = true;
		    disableVariableInspector = true;
		    disableDrawCustomInspector = true;
		    
		    nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
        }

	    public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
	    {		
		    base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
	    }
	   
	    
	    public override void DrawCustomInspector()
	    {
		   
		    if (outputNodes[0].outputNode != null)
		    {
			    GUILayout.Label("Connected to:");
			    
			    if (GUILayout.Button(outputNodes[0].outputNode.GetType().Name.ToString(), "miniButton", GUILayout.Width(100)))
	    		{
	    		    rootGraph.selectedNode = outputNodes[0].outputNode;//Selection.activeGameObject;
		    	    rootGraph.selectedNodes.Add(rootGraph.selectedNode.GetInstanceID(), rootGraph.selectedNode);
		    	    rootGraph.selectedNodeIndex = rootGraph.selectedNode.nodeListIndex;
	    		    rootGraph.selectedNode.OnSelect(rootGraph.currentGraph);
	    	    }
	        }  
		       
	    }
		#endif
	    
	    public override void OnExecute(FlowReactorComponent _flowReactor)
	    {
		    ExecuteNext(0, _flowReactor);
	    }
    }
}
