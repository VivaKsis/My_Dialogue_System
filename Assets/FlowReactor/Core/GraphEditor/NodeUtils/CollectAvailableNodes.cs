//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	
	public class CollectAvailableNodes
	{

		public static NodeCategoryTree collectedNodes = new NodeCategoryTree();
		
	    
		public class CollectedNodesData
		{
			public string title;
			public string typeName;
			public string name;
			public string nameSpace;
			public string category;
			public string description;
			public string color;
			public int outputCount;
			public string[] nodeOutputs;
			public NodeAttributes.NodeType nodeType;
			
			public CollectedNodesData(string _title, string _typeName, string _nameSpace, string _category, string _description, string _color, int _output, string[] _nodeOutputs, NodeAttributes.NodeType _type)
			{
				title = _title;
				typeName = _typeName;
				//name = _name;
				nameSpace = _nameSpace;
				category = _category;
				description = _description;
				color = _color;
				outputCount = _output;
				nodeOutputs = _nodeOutputs;
				nodeType = _type;
			}
		}
	
	
		public static NodeCategoryTree CollectNodes()
		{
			
			collectedNodes = new NodeCategoryTree();
		
			var _found = System.AppDomain.CurrentDomain.FlowReactorGetAllDerivedTypes(typeof(Node));
			
			//System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			//System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(Node)) select type).ToArray();
			
	
			List<CollectedNodesData> collected = new List<CollectedNodesData>();
		
			for (int i = 0; i < _found.Length; i++)
			{
			
				NodeAttributes _nodeAttributes = System.Attribute.GetCustomAttribute(_found[i], typeof(NodeAttributes)) as NodeAttributes;
				
				if (_nodeAttributes != null)
				{
					collected.Add(new CollectedNodesData(
						_found[i].Name.ToString(), // Title
						_found[i].Name.ToString(), // type name
						_found[i].Namespace,
						_nodeAttributes.category,
						_nodeAttributes.description,
						_nodeAttributes.color, 
						_nodeAttributes.outputSlotCount, 
						_nodeAttributes.nodeOutputs,
						_nodeAttributes.nodeType));   
				}
			}
			
			// Sort by categories
			collected = collected.OrderBy(c => c.category).ToList();
			
			// Add favorit nodes
			var _settings = (FREditorSettings)FREditorSettings.GetOrCreateSettings();
			if (_settings.favoredNodes == null)
			{
				_settings.favoredNodes = new List<NodeCategoryTree.NodeData>();
			}
			
			for (int f = 0; f < _settings.favoredNodes.Count; f ++)
			{
	
				collected.Insert(0, new CollectedNodesData
				(
					//_settings.favoredNodes[f].name,
					_settings.favoredNodes[f].title,
					_settings.favoredNodes[f].typeName,
					_settings.favoredNodes[f].nameSpace,
					"Favorites",
					_settings.favoredNodes[f].description,
					_settings.favoredNodes[f].color,
					_settings.favoredNodes[f].outputSlotCount,
					_settings.favoredNodes[f].nodeOutputs,
					_settings.favoredNodes[f].nodeType
				
				));
				
			}
			
			for (int c = 0; c < collected.Count; c ++)
			{
				var _child = collectedNodes.BuildTree(collected[c].category, collected[c].name);
				
				_child.AddNode 
				(
					//collected[c].name,
					collected[c].title,
					collected[c].typeName,
					collected[c].nameSpace,
					collected[c].category,
					collected[c].description,
					collected[c].color,
					collected[c].outputCount,
					collected[c].nodeOutputs,
					collected[c].nodeType
				);
			}
			
			return collectedNodes;
		}
	}
}
#endif