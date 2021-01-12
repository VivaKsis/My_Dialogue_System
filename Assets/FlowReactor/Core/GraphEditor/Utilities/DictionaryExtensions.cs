using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowReactor.Editor
{
	public static class DictionaryExtensions
	{
		public static void UpdateKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
		{
			TValue value = dic[fromKey];
			dic.Remove(fromKey);
			dic[toKey] = value;
		}
		
	}
}
