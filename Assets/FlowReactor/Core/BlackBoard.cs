//---------------------------------------------------------------------------------
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//	FLOWREACTOR - Blackboard
//
//	Blackboard scriptable object class.
//	Stores all variables and handles save and load
//
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using FlowReactor.OdinSerializer;
using FlowReactor.OrderedDictionary;

#if FLOWREACTOR_DATABOX
using Databox;
#endif 

namespace FlowReactor.BlackboardSystem
{
	[CreateAssetMenu(fileName = "Blackboard", menuName = "FlowReactor/New Blackboard", order = 1)]
	public class BlackBoard : SerializedScriptableObject
	{
		
		// EDITOR SETTINGS
		public bool isDragging;
		public Guid dragEntry;
		public float variableFieldWidth = 120;
		public bool dragVariableFieldSize = false;
		
		// DATABOX
		public bool showDataboxInfo;
		public bool useDatabox;
		#if FLOWREACTOR_DATABOX
		public DataboxObjectManager databoxObjectManager;
		#endif

		public OrderedDictionary<Guid, FRVariable> variables = new OrderedDictionary<Guid, FRVariable>();
		public List<VariablesData> tempVariablesList = new List<VariablesData>();
		
		[System.Serializable]
		public class VariablesData
		{
			public string id;
		
			
			public VariablesData (string _id)
			{
				id = _id;
			}
		}
		
		public bool foldout;
		
		public enum SavePath
		{
			PlayerPrefs,
			PersistentPath,
			StreamingAssets,
		}
		
		public enum SaveFormat
		{
			json,
			binary
		}
		
		
		/// <summary>
		/// Return blackboard variable of type T by name.
		/// Make sure that variable names are unique.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public T GetVariableByName<T> (string name) where T : FRVariable
		{
			
			foreach (var variable in variables)
			{
				if (variable.Value.name == name)
				{
					return (T)variable.Value;
				}
			}
			
			return null;
		}
		
		
		// Called by FRVariable on get or set
		public T GetData<T>(Guid id) where T : FRVariable
		{
			if (!variables.ContainsKey(id))
				return null;
			
			if (variables[id].overrideVariable)
			{
				return null;
			}
			else
			{
				return (T)variables[id];
			}
			
		}
		
		#if FLOWREACTOR_DATABOX
		public T GetDataboxData<T>(Guid id) where T : DataboxType
		{
			return databoxObjectManager.GetDataboxObject(variables[id].databoxID).GetData<T>(variables[id].tableID, variables[id].entryID, variables[id].valueID);
		}
		#else
		public T GetDataboxData<T>(Guid id)
		{
			Debug.LogWarning("Databox not installed");
			return default(T);
		}
		#endif
		
		public byte[] SerializeBlackboard(SaveFormat _format)
		{
			byte[] bytes = null;
			DataFormat _f = DataFormat.JSON;
			switch(_format)
			{
			case SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
			
			bytes = SerializationUtility.SerializeValue(variables, _f);
			return bytes;
		}
		
		public void DeserializeBlackboard(byte[] _bytes, SaveFormat _format)
		{
			DataFormat _f = DataFormat.JSON;
			switch(_format)
			{
			case SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
			
			variables = SerializationUtility.DeserializeValue<OrderedDictionary<Guid, FRVariable>>(_bytes, _f);
		}
		
		public void SaveToFile(string _fileName, SavePath _savePath, SaveFormat _format)
		{
			var _path = "";
			DataFormat _f = DataFormat.JSON;
			byte[] bytes = null;
			
			switch(_format)
			{
			case SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
			
			switch (_savePath)
			{
			case SavePath.PlayerPrefs:
				
				bytes = SerializationUtility.SerializeValue(variables, _f);
				string _saveString = System.Convert.ToBase64String(bytes);
				PlayerPrefs.SetString(_fileName, _saveString);
				
				break;
			case SavePath.PersistentPath:
				_path = System.IO.Path.Combine(Application.persistentDataPath, _fileName);
				
				bytes = SerializationUtility.SerializeValue(variables, _f);
				File.WriteAllBytes(_path, bytes);
				break;
			case SavePath.StreamingAssets:
				_path = System.IO.Path.Combine(Application.streamingAssetsPath, _fileName);
				
				bytes = SerializationUtility.SerializeValue(variables, _f);
				File.WriteAllBytes(_path, bytes);
				break;
			}
		}
		
		public void LoadFromFile(string _fileName, SavePath _savePath, SaveFormat _format)
		{
			var _path = "";
			DataFormat _f = DataFormat.JSON;
			byte[] bytes = null;
			
			switch(_format)
			{
			case SaveFormat.json:
				_f = DataFormat.JSON;
				break;
			case SaveFormat.binary:
				_f = DataFormat.Binary;
				break;
			}
			
			switch (_savePath)
			{
			case SavePath.PlayerPrefs:
				
				string _loadString = PlayerPrefs.GetString(_fileName);
				bytes = System.Convert.FromBase64String(_loadString);
				variables = SerializationUtility.DeserializeValue<OrderedDictionary<Guid, FRVariable>>(bytes, _f);
				
				break;
			case SavePath.PersistentPath:
			
				_path = System.IO.Path.Combine(Application.persistentDataPath, _fileName);
				bytes = File.ReadAllBytes(_path);
				variables = SerializationUtility.DeserializeValue<OrderedDictionary<Guid, FRVariable>>(bytes, _f);
				
				break;
			case SavePath.StreamingAssets:
			
				_path = System.IO.Path.Combine(Application.streamingAssetsPath, _fileName);		
				bytes = File.ReadAllBytes(_path);
				variables = SerializationUtility.DeserializeValue<OrderedDictionary<Guid, FRVariable>>(bytes, _f);
				
				break;
			}
			
		}
		
		#if UNITY_EDITOR
		public void RemoveConnectedNode(FlowReactor.Nodes.Node _node)
		{
			foreach (var v in variables.Keys)
			{
				if (variables[v].connectedNodes != null)
				{
					bool _hasOtherSceneRef = false;
					for (int n = 0; n < variables[v].connectedNodes.Count; n ++)
					{
						
						if (variables[v].connectedNodes[n] != null && variables[v].connectedNodes[n] == _node)
						{
						
							variables[v].connectedNodes.RemoveAt(n);
						}
						else if (variables[v].connectedNodes[n] == null)
						{
						
							variables[v].connectedNodes.RemoveAt(n);
						}
					
						//// If this variable was set to scene ref only
						//// then check if there are other variables with scene ref only from connected nodes
						//// if no, then we set it back to false for this bb variable
						if (variables[v].sceneReferenceOnly && n < variables[v].connectedNodes.Count)
						{
						
							// check if other connected nodes has scene reference only attribute
							if (variables[v].connectedNodes[n] != _node)
							{
							
								List<FRVariable> _allNodeVariables;
								FlowReactor.Editor.GetAvailableVariableTypes.GetAllFRVariablesOnNode(variables[v].connectedNodes[n], out _allNodeVariables);
							
								for (int i = 0; i < _allNodeVariables.Count; i ++)
								{
									if (_allNodeVariables[i].sceneReferenceOnly)
									{
										_hasOtherSceneRef = true;
									}
								}
							}
						}
					}
					
					if (!_hasOtherSceneRef && variables[v].sceneReferenceOnly)
					{
						variables[v].sceneReferenceOnly = false;
					}
				}
			}
		}
		
		public void RemoveAllConnectedNode(Graph _rootGraph)
		{
			foreach (var v in variables.Keys)
			{
				if (variables[v].connectedNodes != null)
				{
					for (int n = 0; n < variables[v].connectedNodes.Count; n ++)
					{
						if (variables[v].connectedNodes[n] != null && variables[v].connectedNodes[n].rootGraph == _rootGraph)
						{
							variables[v].connectedNodes.RemoveAt(n);
						}
						else if (variables[v].connectedNodes[n] == null)
						{
							variables[v].connectedNodes.RemoveAt(n);
						}
					}
				}
			}
		}
		#endif
	}
}
