//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FlowReactor
{
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class FRVariableAttribute : Attribute
	{
		public string Name{get;set;}
	}
	
	// VARIABLE GUI DECORATORS
	///////////////////////////
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class Title : Attribute
	{
		public string title;
		
		public Title(string _title)
		{
			title = _title;
		}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class HelpBox : Attribute
	{
		public string message;
		public enum MessageType
		{
			none,
			info,
			warning
		}
		
		public MessageType type;
		
		public HelpBox(string _message, MessageType _type)
		{
			message = _message;
			type = _type;
		}
		
		public HelpBox(string _m)
		{
			message = _m;
			type = MessageType.none;
		}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class OpenURL : Attribute
	{
		public string title;
		public string url;
		
		public OpenURL(string _title, string _url)
		{
			title = _title;
			url = _url;
		}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class Hide : Attribute{}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class HideInNode : Attribute{}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class VariableGroup : Attribute
	{
		public string groupTitle;
		public bool foldout;
		
		public VariableGroup (string _groupTitle)
		{
			groupTitle = _groupTitle;
		}
	}
	///////////////////////////
	
	
	
	// VARIABLE ATTRIBUTE OVERRIDES
	////////////////////////////////
	// Attributes which are being bassed 
	// to the Draw() method of the FRVariable class
	// These attributes can be used to alter the GUI drawing
	// Example: FRFloat -> Normal float field or slider with min max values
 
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class FRFloatRange : Attribute
	{
		public float min;
		public float max;
		
		public FRFloatRange (float _min, float _max)
		{
			min = _min;
			max = _max;
		}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class FRIntRange : Attribute
	{
		public int min;
		public int max;
		
		public FRIntRange (int _min, int _max)
		{
			min = _min;
			max = _max;
		}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class SceneObjectOnly : Attribute{}
}