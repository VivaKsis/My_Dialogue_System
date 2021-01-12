using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.NodeUtilityModules;
using FlowReactor.Nodes;

public class NodeModulesExample : MonoBehaviour, INodeControllable
{
	bool eventHasBeenCalled = false;
	
	public void OnNodeInitialize(Node _node)
	{
		// Called when node is being initialized
	}
	
	public void OnNodeExecute()
	{
		// Called when node is being executed
		Debug.Log("Node controlled object: Node has been executed");
		
		// Start coroutine to set back the eventHasBeenCalled boolean
		eventHasBeenCalled = true;
		StartCoroutine(Reset());
	}
	
	public void OnNodeStopExecute()
	{
		// Called when node has stopped executing
	}
	
	public void OnNode(Node _node, params object[] _parameters)
	{
		// Custom method called from node
	}
	
	IEnumerator Reset()
	{
		yield return new WaitForSeconds(0.5f);
		
		eventHasBeenCalled = false;
	}
	
	
	void OnGUI()
	{
		using (new GUILayout.VerticalScope("Box"))
		{
			
			GUILayout.Label("Event dispatcher module:");
			GUILayout.Label("Hit Space key to raise custom event from the example event dispatcher node");
			
		}
		
		if (eventHasBeenCalled)
		{
			using (new GUILayout.VerticalScope("Box"))
			{
				GUI.color = Color.green;
				GUILayout.Label("EVENT HAS BEEN CALLED");
				GUI.color = Color.white;
			}
		}
	}
}
