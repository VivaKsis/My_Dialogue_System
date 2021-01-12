//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public class GetAvailableVariableTypes
	{
		public static void GetAllFRVariablesOnNode(Node _node, out List<FRVariable> _variables)
		{
			_variables = new List<FRVariable>();
			
			var _fields = _node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic  | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToList();	
			for(int f = 0; f < _fields.Count; f ++)
			{			
				if(_fields[f].FieldType.BaseType == typeof(FRVariable))
				{
					_variables.Add(_fields[f].GetValue(_node) as FRVariable);
				}
			}
		}
		
		public static void ReturnExistingTypesOfType<T>(out Dictionary<string, Type> _returnTypes)
		{
			_returnTypes = new Dictionary<string, Type>();
			
			System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			//System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(T)) select type).ToArray();
			System.Type[] _found = System.Reflection.Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.IsSubclassOf (typeof(T)) || t.IsGenericType).ToArray();
				
		
			foreach(var currentType in _found)
			{
				var attributes = currentType.GetCustomAttributes(typeof(FRVariableAttribute), false);
				if(attributes.Length > 0)
				{
					var targetAttribute = attributes.First() as FRVariableAttribute;
					_returnTypes.Add(targetAttribute.Name, currentType);
					//_returnTypes.Add(attributes[0].ToString(), currentType);
				}
			}
		}
		
		
		public static void GetFlowReactorVariableTypes(out Dictionary<string, Type> _returnTypes)
		{
			_returnTypes = new Dictionary<string, Type>();
			
			//System.Type[] _types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			//System.Type[] _found = (from System.Type type in _types where type.IsSubclassOf(typeof(FRVariable)) select type).ToArray();
			
			//List<System.Type> _sortedList = new List<Type>();
			
			//foreach(var currentType in _found)
			//{
			//	_sortedList.Add(currentType);	
			//}
			
			//_sortedList = _sortedList.OrderBy(n => n.Name).ToList();
			
			//for (int i = 0; i < _sortedList.Count; i ++)
			//{
			//	var attributes = _sortedList[i].GetCustomAttributes(typeof(FRVariableAttribute), false);
			//	if(attributes.Length > 0)
			//	{
	
			//		var targetAttribute = attributes.First() as FRVariableAttribute;
			//		_returnTypes.Add(targetAttribute.Name, _sortedList[i]);
					
			//	}
			//}
			
			
			_returnTypes = new Dictionary<string, Type>();
			
			var _allVariableTypes = System.AppDomain.CurrentDomain.FlowReactorGetAllDerivedTypes(typeof(FRVariable)).ToList();
			_allVariableTypes = _allVariableTypes.OrderBy(n => n.Name).ToList();
			
			for(int i = 0; i < _allVariableTypes.Count; i ++)
			{
				var attributes = _allVariableTypes[i].GetCustomAttributes(typeof(FRVariableAttribute), false);
				if(attributes.Length > 0)
				{
					var targetAttribute = attributes.First() as FRVariableAttribute;
					_returnTypes.Add(targetAttribute.Name, _allVariableTypes[i]);
				}
			}
		}
		
	
	}
}
#endif