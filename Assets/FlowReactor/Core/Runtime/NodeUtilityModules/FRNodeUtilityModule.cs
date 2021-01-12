using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowReactor.NodeUtilityModules
{
	[System.Serializable]
	public class FRNodeUtilityModule {}
	
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class NodeModule : Attribute
	{
		public string description;
		public string customDrawInspectorCode;
		public string moduleVariablesCode;
		
		public NodeModule(string _description)
		{
			description = _description;
		}
		
		public NodeModule(string _description, string _moduleVariablesCode, string _customDrawInspectorCode)
		{
			description = _description;
			customDrawInspectorCode = _customDrawInspectorCode;
			moduleVariablesCode = _moduleVariablesCode;
		}
		
	}
	 
}