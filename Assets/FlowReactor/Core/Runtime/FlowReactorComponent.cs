//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	FlowReactor Runtime Graph Controller!
	Handles execution of the assigned graph, also stores scene override values.
*/
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if FLOWREACTOR_DATABOX
using Databox;
#endif

using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using FlowReactor.OdinSerializer;
using FlowReactor.OdinSerializer.Utilities;
using FlowReactor.Editor;

namespace FlowReactor
{  
	 
	[AddComponentMenu("FlowReactor/FlowReactorComponent")]
	public class FlowReactorComponent : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private SerializationData serializationData;
		public bool isSerializing;
		 
		
		public bool runUniqueInstance;
		public bool useGlobalUpdateLoop;
		
		public Graph graph;
		// keep the original graph when creating a unique instance
		public Graph originalGraph;
		
		[OdinSerialize]  
		public Dictionary<string, FRVariable> overrideSceneVariables = new Dictionary<string, FRVariable>();


		public Queue<Node> parallelStackExecution = new Queue<Node>();
		int currentFrameCount = 0;
		
		// track all executing coroutine nodes
		public class CoroutineNodes
		{
			public Node node;
			public IEnumerator coroutine;
			public CoroutineNodes (IEnumerator _coroutine, Node _node)
			{
				coroutine = _coroutine;
				node = _node;
			}
		}
		
		public Dictionary<int, CoroutineNodes> executingCoroutines = new Dictionary<int, CoroutineNodes>();
	
	
		// INodeControllable objects
		public class NodeControllables
		{
	 
			[OdinSerialize]
			public Dictionary<string, INodeControllable> interfaces;
		
			
			public NodeControllables(Dictionary<string, INodeControllable> _interfaces)
			{
				interfaces = new Dictionary<string, INodeControllable>();
				interfaces = _interfaces;
			}
		}
		
		
		public class ExposedVariables
		{
			public string variableName;
			[OdinSerialize]
			public FRVariable variable;
			
			public ExposedVariables(string _variableName, FRVariable _variable)
			{
				variableName = _variableName;
				variable = _variable;
			}
		}
		
		[OdinSerialize]
		public Dictionary<Node, NodeControllables> nodeControllables;
		[OdinSerialize]
		// <string = node name, string = variable name, FRVariable = node variable 
		public Dictionary<string, Dictionary<string, FRVariable>> exposedNodeVariables = new Dictionary<string, Dictionary<string, FRVariable>>();
		
		
		// Custom Odin serialization implementation
		// Instead of using SerializedMonobehaviour we're using the manual odin implementation so we can track the serialization state.
		// We do this because of a bug where exposedNodeVariables dictionary would loose all values when variables are accessing the dictionary
		// while Unity is serializing!
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			using (var cachedContext = Cache<DeserializationContext>.Claim())
			{
				cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
				UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData, cachedContext.Value);
			}
			
			isSerializing = true;
		}  

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				isSerializing = false;
				
				using (var cachedContext = Cache<SerializationContext>.Claim())
				{
					cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
					UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData, serializeUnityFields: true, context: cachedContext.Value);
				} 
			}
		} 
	
	
		public void NodeControllable_OnNodeExecute(Node _node)
		{
			if (!HasControllables(_node))
			{
				return;
			}
			
			foreach (var i in nodeControllables[_node].interfaces.Keys)
			{
				if (nodeControllables[_node].interfaces[i] != null)
				{
					nodeControllables[_node].interfaces[i].OnNodeExecute();
				}
			}
		}
		
		public void NodeControllable_OnNodeStopExecute(Node _node)
		{
			if (!HasControllables(_node))
			{
				return;
			}
			
			foreach (var i in nodeControllables[_node].interfaces.Keys)
			{
				if (nodeControllables[_node].interfaces[i] != null)
				{
					nodeControllables[_node].interfaces[i].OnNodeStopExecute();
				}
			}
		}
		
		public void NodeControllable_OnNodeInitialization(Node _node)
		{
			if (!HasControllables(_node))
			{
				return;
			}
				
			foreach (var i in nodeControllables[_node].interfaces.Keys)
			{
				if (nodeControllables[_node].interfaces[i] != null)
				{
					nodeControllables[_node].interfaces[i].OnNodeInitialize(_node);
				}
			}
		}
		
		public void NodeControllable_OnNode(Node _node, params object[] _parameters)
		{
			if (!HasControllables(_node))
			{
				return;
			}
				
			foreach (var i in nodeControllables[_node].interfaces.Keys)
			{
				if (nodeControllables[_node].interfaces[i] != null)
				{
					nodeControllables[_node].interfaces[i].OnNode(_node, _parameters);
				}
			}
		}
		
		bool HasControllables(Node _node)
		{
			var _key = _node;
			bool _exists = false;
			// Instead of using ContainsKey we have to iterate because
			// ContainsKey somehow returns false??!
			if (nodeControllables != null)
			{
				foreach(Node cont in nodeControllables.Keys)
				{
					if (cont == _key)
					{
						_exists = true;
					}
				} 
			}
			
			
			return _exists;
		} 
		 
		// Exposed variable methods
		/////////////////////////// 
		/// <summary>
		/// Return exposed variable of type FRVariable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="_nodeName"></param>
		/// <param name="_variableName"></param>
		/// <returns></returns>
		public T GetExposedVariable<T>(string _nodeName, string _variableName) where T : FRVariable
		{
			if (isSerializing)
			{
				return default(T);
			}
		
			if (exposedNodeVariables.ContainsKey(_nodeName))
			{
				if (exposedNodeVariables[_nodeName].ContainsKey(_variableName))
				{
					var _returnVariable = exposedNodeVariables[_nodeName][_variableName] as T;
					return (T) _returnVariable;
				}
			}
			
			Debug.LogWarning("FlowReactor: Exposed variable: " + _variableName + " in node: " + _nodeName + " not found");
			return default(T);
		}
		

		// object variable override methods
		///////////////////////////////////
		/// <summary>
		/// Set override variable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="_id"></param>
		public void SetOverrideVariable<T>(T value, string _variableName) where T : FRVariable
		{
		 
			string key = "";
			foreach(var k in overrideSceneVariables.Keys)
			{
				if (overrideSceneVariables[k].name == _variableName)
				{
					key = k;
					break;
				}
			}
			
			if (!string.IsNullOrEmpty(key))
			{
				var v = overrideSceneVariables[key] as T;
				v.GetType().GetProperty("Value").SetValue(v, value.GetType().GetProperty("Value").GetValue(value));
				overrideSceneVariables[key] = v as T;
			}
			else
			{
				Debug.LogWarning("Variable not found: " + _variableName + "- Is it overridable?");
				
				// create new item and copy it to the override variables
				//overrideSceneVariables.Add(key.ToString(), (FRVariable)Activator.CreateInstance(graph.blackboards[bb].blackboard.variables[key].GetType()));
				//fr.overrideSceneVariables[key.ToString()].overrideVariable = true; 
				//fr.overrideSceneVariables[key.ToString()].name = v.name;
											
				//v.overrideVariable = false;// blackboard is always false
			}
		}
		
		/// <summary>
		/// Return override variable with name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="_id"></param>
		/// <returns></returns>
		public T GetOverrideVariable<T>(string _variableName) where T : FRVariable
		{
			if (isSerializing)
			{
				return default(T);
			}
			
			string key = "";
			foreach(var k in overrideSceneVariables.Keys)
			{
				if (overrideSceneVariables[k].name == _variableName)
				{
					key = k;
					break;
				}
			}
			if (!string.IsNullOrEmpty(key))
			{
				var v = overrideSceneVariables[key] as T;
				return (T)v;
			}
			else
			{
				Debug.LogWarning("Variable not found: " + _variableName + "- Is it overridable?");
				return default(T);
			}
		}
		
		/// <summary>
		/// Only for internal use!
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="_variableGuid"></param>
		/// <returns></returns>
		public T GetOverrideVariableByGuid<T>(string _variableGuid) where T : FRVariable
		{
			if (isSerializing)
			{
				return default(T);
			}
			
			FRVariable _ov;
			if (overrideSceneVariables.TryGetValue(_variableGuid, out _ov))
			{
				return (T) _ov;
			}
			
			return default(T);
		}
		 
		/// <summary>
		/// Enables an override for blackboard variable
		/// </summary>
		/// <param name="_blackboardName"></param>
		/// <param name="_variableName"></param>
		public void EnableOverrideForVariable(string _blackboardName, string _variableName)
		{
			if (overrideSceneVariables == null)
			{
				overrideSceneVariables = new Dictionary<string, FRVariable>();	
			}
			
			foreach(var blackboard in graph.blackboards.Keys)
			{
				if (graph.blackboards[blackboard].blackboard.name == _blackboardName)
				{
					foreach (var variable in graph.blackboards[blackboard].blackboard.variables.Keys)
					{
						if (graph.blackboards[blackboard].blackboard.variables[variable].name == _variableName)
						{
						
							var _newOverrideVariable = (FRVariable)Activator.CreateInstance(graph.blackboards[blackboard].blackboard.variables[variable].GetType());
							_newOverrideVariable.overrideVariable = true;
							_newOverrideVariable.name = graph.blackboards[blackboard].blackboard.variables[variable].name;
												
							overrideSceneVariables.Add(variable.ToString(), _newOverrideVariable);
							overrideSceneVariables[variable.ToString()].overrideVariable = true; 
							overrideSceneVariables[variable.ToString()].name = _newOverrideVariable.name;
						}
					}
				}
			}
		}
		
		
		
		public Node currentNode;
		
		// Inspector property
		public bool showInspectorVariables;
		
		
		public void OnDisable()
		{
			if (graph == null)
			{
				Debug.LogWarning("No graph assigned to: " + this.gameObject.name);
				return;
			}
			else
			{
				graph.CallOnDisable(this);
			}
		
		}
		
		public void Awake()
		{		

			if (graph == null)
			{
				Debug.LogWarning("No graph assigned to: " + this.gameObject.name);
				return;
			}
			
			isSerializing = false;
			
			executingCoroutines = new Dictionary<int, CoroutineNodes>();
			
			// Make sure exposed variable dictionary is up to date
			CollectAndUpdateAllExposedVariables();
			
			if (runUniqueInstance)
			{
				originalGraph = graph;
				
				var _uniqueGraph = graph.Copy(null, null, null);	
			
				graph = _uniqueGraph;
			}
			
			// Register this component to graph
			graph.flowReactorComponents.Add(this.GetInstanceID(), this);
		
			if (graph.flowReactorComponents.Keys.Count > 1)
			{
				Debug.LogWarning("FlowReactor: Multiple FlowReactor components are running the same graph. Please set it to unique instance to prevent errors. This graph will not run!", this.gameObject);
			}
			else
			{
				if (graph != null)
				{
					graph.CallOnInitialize(this);
				}
			}
		}
	    
		public void Start()
		{
			currentFrameCount = Time.frameCount;
			
			if (graph != null && graph.isActive)
			{
				for (int n = 0; n < graph.nodes.Count; n ++)
				{
					graph.nodes[n].OnGraphStart(this);
				}
			}
			
			if (useGlobalUpdateLoop)
			{
				FlowReactorGlobalUpdate.Instance.Register(this);
			}
		}

	    
	    public void Update()
		{
			
		
			// execute all nodes in the parallel execution stack
			if (parallelStackExecution.Count > 0)
			{
				// wait for one frame
				if (Time.frameCount > currentFrameCount + 1)
				{
					currentFrameCount = Time.frameCount;
					
					var _node = parallelStackExecution.Dequeue();
					
					_node.runParallel = true;
					_node.hasError = false;
				
					// Run coroutine node
					if (_node.nodeData.nodeType == NodeAttributes.NodeType.Coroutine)
					{
						StartCoroutine((_node as CoroutineNode).OnExecuteCoroutine(this));
					}
					else
					{
						_node.OnExecute(this);
					}
				}
			}
			
			
			if (useGlobalUpdateLoop)
				return;
				
		    if (graph != null && graph.isActive)
		    {
		    	graph.OnUpdate(this);
		    }
	    }
	    
		public void GlobalUpdate()
		{
			if (!useGlobalUpdateLoop)
				return;
				
			if (graph != null && graph.isActive)
			{
				graph.OnUpdate(this);
			}
		}
		
		public void LateUpdate()
		{
			if (useGlobalUpdateLoop)
				return;
				
			if (graph != null && graph.isActive)
			{
				graph.OnLateUpdate(this);
			}
		}
		
		public void GlobalLateUpdate()
		{
			if (!useGlobalUpdateLoop)
				return;
				
			if (graph != null && graph.isActive)
			{
				graph.OnLateUpdate(this);
			}
		}
		
		public void FixedUpdate()
		{
			if (useGlobalUpdateLoop)
				return;
				
			if (graph != null && graph.isActive)
			{
				graph.OnFixedUpdate(this);
			}
		}
		
		public void GlobalFixedUpdate()
		{
			if (!useGlobalUpdateLoop)
				return;
				
			if (graph != null && graph.isActive)
			{
				graph.OnFixedUpdate(this);
			}
		}
	    
		public void OnApplicationQuit()
		{
			if (graph != null && graph.isActive)
			{
				for (int n = 0; n < graph.nodes.Count; n ++)
				{
					graph.nodes[n].OnApplicationExit(this);
				}
			}
		}
		
		
		
		// Get and compare exposed variables with the exposed dictionary from the graph.
		// update the flowreactor component dictionary accordingly
		public void CollectAndUpdateAllExposedVariables()
		{
			if (exposedNodeVariables == null)
			{
				exposedNodeVariables = new Dictionary<string, Dictionary<string, FRVariable>>();
				
				foreach(string exposedNodeName in graph.exposedNodeVariables.Keys)
				{
					exposedNodeVariables.Add(exposedNodeName, new Dictionary<string, FRVariable>());
					
					foreach (string exvar in graph.exposedNodeVariables[exposedNodeName].variables.Keys)
					{
						var _type = graph.exposedNodeVariables[exposedNodeName].variables[exvar].GetType();
						var _newFR = System.Activator.CreateInstance(_type) as FRVariable;
						_newFR.exposedName = exvar;
						_newFR.nodeOwner = graph.exposedNodeVariables[exposedNodeName].node;
						_newFR.exposedNodeName = graph.exposedNodeVariables[exposedNodeName].node.nodeData.title;
						_newFR.type = FRVariable.VariableType.local;
							
							
						//Debug.Log(exvar + " - " + graph.exposedNodeVariables[exposedeNodeName].variables[exvar].exposedName);
						exposedNodeVariables[exposedNodeName].Add(graph.exposedNodeVariables[exposedNodeName].variables[exvar].exposedName, _newFR);
					}
				}
				
			}
			else
			{
				
				// loop through each node and variables and check if variables exposednodename != node name
				// if so, then node has been renamed and we dont have to create a new variable
				List<string> _nodeKeys = exposedNodeVariables.Keys.ToList();
				
				List<string> _oldKeys = new List<string>();
				List<string> _newKeys = new List<string>();
				
				for(int e = 0; e < _nodeKeys.Count; e ++)
				{
					foreach (var variable in exposedNodeVariables[_nodeKeys[e]].Keys)
					{
						
						if (exposedNodeVariables[_nodeKeys[e]][variable].nodeOwner == null)
							continue;
							
						if (exposedNodeVariables[_nodeKeys[e]][variable].nodeOwner.title != exposedNodeVariables[_nodeKeys[e]][variable].exposedNodeName)
						{
							var _oldName = exposedNodeVariables[_nodeKeys[e]][variable].exposedNodeName;
							var _newName = exposedNodeVariables[_nodeKeys[e]][variable].nodeOwner.title;
														
							exposedNodeVariables[_nodeKeys[e]][variable].exposedNodeName = _newName;

							_oldKeys.Add(_oldName);
							_newKeys.Add(_newName);
						}
					}
				}
							
				for (int o = 0; o < _oldKeys.Count; o ++)
				{
					exposedNodeVariables.UpdateKey(_oldKeys[o], _newKeys[o]);
				}
							
							
				// Update nodes and variables
				foreach (string graphNode in graph.exposedNodeVariables.Keys)
				{
					var _nodeExists = false;
					foreach (string node in exposedNodeVariables.Keys)
					{
						if (graphNode.Equals(node))
						{
							_nodeExists = true;
						}
					}
							
					// add graph node to frcomponent node
					if (!_nodeExists)
					{
						foreach(var graphVariable in graph.exposedNodeVariables[graphNode].variables.Keys)
						{
							if (!exposedNodeVariables.ContainsKey(graphNode))
							{
								var _type = graph.exposedNodeVariables[graphNode].variables[graphVariable].GetType();
								var _newFR = System.Activator.CreateInstance(_type) as FRVariable;
								_newFR.exposedName = graphVariable;
								_newFR.nodeOwner = graph.exposedNodeVariables[graphNode].node;
								_newFR.exposedNodeName = graph.exposedNodeVariables[graphNode].node.nodeData.title;
								_newFR.type = FRVariable.VariableType.local;
							

								
								var _tmp = new Dictionary<string, FRVariable>();
								_tmp.Add(_newFR.exposedName, _newFR);
								exposedNodeVariables.Add(graphNode,  _tmp);
							}
							else
							{
								var _newVariable = graph.exposedNodeVariables[graphNode].variables[graphVariable];
							
								exposedNodeVariables[graphNode].Add(_newVariable.exposedName, _newVariable);
							}
						}
					} 
					else 
					{
						// check if variables have changed
						foreach (var graphVariable in graph.exposedNodeVariables[graphNode].variables.Keys)
						{
							var _variableExists = false;
							foreach (var variable in exposedNodeVariables[graphNode].Keys)						
							{
								if (graphVariable.Equals(variable))								
								{
									_variableExists = true;
								}
							}
								
							// add variable to frcomponent variable
							if (!_variableExists)
							{
								var _type = graph.exposedNodeVariables[graphNode].variables[graphVariable].GetType();
								var _newFR = System.Activator.CreateInstance(_type) as FRVariable;
								_newFR.exposedName = graphVariable;
								_newFR.nodeOwner = graph.exposedNodeVariables[graphNode].node;
								_newFR.exposedNodeName = graph.exposedNodeVariables[graphNode].node.nodeData.title;
								_newFR.type = FRVariable.VariableType.local;
								

								exposedNodeVariables[graphNode].Add(graphVariable, _newFR);
							}
						}
					}
				}
					
				List<string> _exposedNodeKeys = exposedNodeVariables.Keys.ToList();
				foreach(string node in _exposedNodeKeys)
				{
					var _frNodeExists = false;
					foreach(string graphNode in graph.exposedNodeVariables.Keys)
					{
						if (node.Equals(graphNode))
						{
							_frNodeExists = true;
						
							List<string> _exposedVariableKeys = exposedNodeVariables[node].Keys.ToList();
							for (int v = 0; v < _exposedVariableKeys.Count; v ++)
							{	
		
								var _frVariableExists = false;
								foreach(var graphVariable in graph.exposedNodeVariables[node].variables.Keys)
								{
									if (_exposedVariableKeys[v] == graphVariable)			
									{
										_frVariableExists = true;
									}
								}
									
								if (!_frVariableExists)
								{
									exposedNodeVariables[node].Remove(_exposedVariableKeys[v]);
								}
							}
						}
					}
						
					if (!_frNodeExists)
					{
						exposedNodeVariables.Remove(node);
					}
				}
			}
		}
    }  
}
