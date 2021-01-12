//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;

namespace FlowReactor.Editor
{
	[System.Serializable]
	public class NodeCategoryTree
	{
		[System.Serializable]
		public class NodeData
		{
			public string title;
			public string typeName;
			/// <summary>
			/// Obsolete! Use title instead
			/// </summary>
			public string name;
			public string nameSpace;
			public string category;
			public string description;
			public string color;
			public int outputSlotCount; // Legacy way of adding outputs and therefore obsolete.
			public string[] nodeOutputs;
		     
			public NodeAttributes.NodeType nodeType;
		
			public NodeData(string _title, string _typeName, string _nameSpace, string _category, string _description, string _color, int _outputSlotCount, string[] _nodeOutputs, NodeAttributes.NodeType _nodeType)
			{
				title = _title;
				typeName = _typeName;
				nameSpace = _nameSpace;
				category = _category;
				description = _description;
				color = _color;
				outputSlotCount = _outputSlotCount;
				nodeOutputs = _nodeOutputs;
				nodeType = _nodeType;
			}
		}
		
		public Dictionary<string, NodeCategoryTree> categories = new Dictionary<string, NodeCategoryTree>();
		public List<NodeData> nodesInCategory = new List<NodeData>();
		public NodeCategoryTree parentGraphTree;
		
		public bool FoldOut;
		
		public string Path { get; set; }
		public string Name;
	
			
		public NodeCategoryTree (){}
		//public NodeCategoryTree (string _path, string _name)
		//{
		//	Path = _path;
		//	Name = _name;
		//}
		
		public void AddNode(string _title, string _typeName, string _nameSpace, string _category, string _description, string _color, int _outputSlotCount, string[] _nodeOutputs, NodeAttributes.NodeType _nodeType)
		{
			if (nodesInCategory == null)
			{
				nodesInCategory = new List<NodeData>();
			}
			
			nodesInCategory.Add(new NodeData(_title, _typeName, _nameSpace, _category, _description, _color, _outputSlotCount, _nodeOutputs, _nodeType));
		}
		
		public NodeData GetData(string _type)
		{
			NodeData _nodeData = null;
			TraverseData(_type, out _nodeData);
			
			return _nodeData;
		}
		
		protected virtual void TraverseData(string _type, out NodeData _nodeData )
		{
			_nodeData = null;
			
			for (int n = 0; n < nodesInCategory.Count; n ++)
			{
			
				if (nodesInCategory[n].typeName == _type)
				{
					_nodeData = new NodeData
					(
						nodesInCategory[n].title,
						nodesInCategory[n].typeName,
						nodesInCategory[n].nameSpace,
						nodesInCategory[n].category,
						nodesInCategory[n].description,
						nodesInCategory[n].color,
						nodesInCategory[n].outputSlotCount,
						nodesInCategory[n].nodeOutputs,
						nodesInCategory[n].nodeType
					);
	
				}
			}
	
			if (_nodeData == null)
			{
				foreach (var tree in this.categories.Keys)
				{
					if (_nodeData == null)
					{
						categories[tree].TraverseData(_type, out _nodeData);
					}
				}
			}
				
		}
		
		public void Traverse(Action<int, NodeCategoryTree> visitor)
		{
			this.traverse(-1, visitor);
		}
	
		protected virtual void traverse(int depth, Action<int, NodeCategoryTree> visitor)
		{
			
			visitor(depth, this);
			
			if (!FoldOut && depth > -1)
				return;
				
			foreach (var tree in this.categories.Keys)
				categories[tree].traverse(depth + 1, visitor);
		}
		
		public void ResetFoldout()
		{
			FoldOut = false;
			foreach(var tree in this.categories.Keys)
				categories[tree].ResetFoldout();
		}
			
		public NodeCategoryTree BuildTree(string _path, string _name)
		{
			//Debug.Log("build tree " + _path);
			// Parse into a sequence of parts.
			string[] parts = _path.Split("/"[0]);
	
			// The current tree.  Start with this.
			NodeCategoryTree current = this;
			
		
		
			// Iterate through the parts.
			foreach (string part in parts)
			{
	
				// The child GraphTree.
				NodeCategoryTree child;
	
				// Does the part exist in the current GraphTree?  If
				// not, then add.
				if (!current.categories.TryGetValue(part, out child))
				{
					//Debug.Log(part + " does not exist");
					
					var n = _name;
					//if (part != parts[parts.Length-1])
					//{
					//	n = part;
					//}
					
					// Add the child.
					child = new NodeCategoryTree {
						Path = part,
						Name = _name,
						parentGraphTree = current
	    			};
	
					// Add to the dictionary.
					current.categories[part] = child;
				}
				
				// Set the current to the child.
				current = child;
			}
			
			return current;
		}
	}
}
