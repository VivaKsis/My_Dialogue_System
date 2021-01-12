#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Editor
{
	public static class GraphUpdater
	{
		public static void UpdateGraph(Graph graph)
		{
			if (graph.version != EditorHelpers.GetEditorVersion() || string.IsNullOrEmpty(graph.version))
			{
				if (EditorHelpers.GetEditorVersion() == "version 1.2.0" ||
					EditorHelpers.GetEditorVersion() == "version 1.2.0p1" || 
					EditorHelpers.GetEditorVersion() == "version 1.2.0p2")
				{
					// update nodes name to title
					//////////////////////////////
					for (int n = 0; n < graph.nodes.Count; n ++)
					{
						if (string.IsNullOrEmpty(graph.nodes[n].nodeData.title))
						{
							graph.nodes[n].nodeData.title = graph.nodes[n].nodeData.name;
							graph.nodes[n].nodeData.typeName = graph.nodes[n].nodeData.name;
						}
					}
					
			
					
					// check if nodevariables connected to blackboard variables are correctly registered to the connected node list
					//////////////////////////////
					for (int n = 0; n < graph.nodes.Count; n ++)
					{
					
						List<FRVariable> _nodeVariables;
						GetAvailableVariableTypes.GetAllFRVariablesOnNode(graph.nodes[n], out _nodeVariables);
					
						for (int v = 0; v < _nodeVariables.Count; v ++)
						{
							if (_nodeVariables[v].type == FRVariable.VariableType.blackboard)
							{
							
								// loop through all blackboards in graph and check if node is assigned to the connected nodes list of the blackboard variable
								foreach (var bbKey in graph.rootGraph.blackboards.Keys)
								{
									
									foreach(var bbVarKey in graph.rootGraph.blackboards[bbKey].blackboard.variables.Keys)
									{
									
										if (_nodeVariables[v].variableGuid == bbVarKey.ToString())
										{
										
											bool _assigned = false;
											if (graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes == null)
											{
												graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes = new List<Nodes.Node>();
											}
											
											for (int c = 0; c < graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes.Count; c ++)
											{
												if (graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes[c] == null)
												{
													graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes.RemoveAt(c);
												}
												else
												{
													if (graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes[c].node == graph.nodes[n])
													{
														_assigned = true;
													}
												}
											}
											
											if (!_assigned)
											{
												graph.rootGraph.blackboards[bbKey].blackboard.variables[bbVarKey].connectedNodes.Add(graph.nodes[n]);
											}
											
											
										}
									}
									
								
								}
							}
						}
					}
					
					
					
					
					
					for (int s = 0; s < graph.subGraphs.Count; s ++)
					{
						graph.subGraphs[s].UpdateGraph(graph.subGraphs[s]);
					}
					
					graph.version = EditorHelpers.GetEditorVersion();
					
					#if FLOWREACTOR_DEBUG
					Debug.Log("FlowReactor: Graph - " + graph.name + " has been updated to: " + graph.version);
					#endif
				}
			}
		}
	}
}
#endif