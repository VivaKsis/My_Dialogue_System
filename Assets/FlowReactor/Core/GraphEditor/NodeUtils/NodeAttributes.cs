//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FlowReactor
{
    [AttributeUsage(AttributeTargets.Class)]
	public class NodeAttributes : Attribute
	{
		public string name;
		public string category;
		public string color = "actionNodeColor";
        public string description;
		public int outputSlotCount;
		public string[] nodeOutputs;
		
	    public NodeType nodeType;
	    
	    public enum NodeType 
	    {
	    	Normal,
	    	Event,
	    	Group,
	    	Portal,
	    	SubGraph,
	    	SubGraphInstance,
	    	Comment,
	    	Coroutine
	    }
	    
        /// <summary>
        /// default constructor
        /// </summary>
		public NodeAttributes ()
        {
            category = "uncategorized";
            description = "";
	        outputSlotCount = 1;
	        nodeType = NodeType.Normal;
        }
        
		public NodeAttributes(string _category, string _description, string _color, string[] _nodeOutputs, NodeType _nodeType)
		{
			category = _category;
			description = _description;
			color = _color;
			nodeOutputs = _nodeOutputs;
			nodeType = _nodeType;
		}
	    
	    
		// OBSOLETE ATTRIBUTES!!!
		// we do not need output slot count anymore
		/////////////////////////
		public NodeAttributes(string _category, string _description, string _color, int _outputSlotCount)
		{
			category = _category;
			description = _description;
			color = _color;
			outputSlotCount = _outputSlotCount;
			nodeType = NodeType.Normal;
		}
		
		public NodeAttributes(string _category, string _description, int _outputSlotCount)
		{
			category = _category;
			description = _description;
			outputSlotCount = _outputSlotCount;
			nodeType = NodeType.Normal;
		}
		
		public NodeAttributes(string _category, string _description, int _outputSlotCount, NodeType _nodeType)
		{
			category = _category;
			description = _description;
			outputSlotCount = _outputSlotCount;
			nodeType = _nodeType;
		}
		
		public NodeAttributes(string _category, string _description, string _color, int _outputSlotCount, NodeType _nodeType)
		{
			category = _category;
			description = _description;
			color = _color;
			outputSlotCount = _outputSlotCount;
			nodeType = _nodeType;
		}
	    
		public NodeAttributes(string _name, string _category, string _description, string _color, int _outputSlotCount, NodeType _nodeType)
		{
			name = _name;
		    category = _category;
			description = _description;
			color = _color;
			outputSlotCount = _outputSlotCount;

		    nodeType = _nodeType;
	    }
		/////////////////////////
		
		
	}
}
