#if UNITY_EDITOR
using UnityEngine ;
using UnityEditor ;
 
// Hides the "script" field in the defaul inspector view 
////////////////////////////////////////////////////////
namespace FlowReactor.Editor
{
	//[ CustomEditor ( typeof ( MonoBehaviour ) , true ) ]
	//public class DefaultInspector : Editor
	//{
	//	public override void OnInspectorGUI ( )
	//	{
	//		this . DrawDefaultInspectorWithoutScriptField ( ) ;
	//	}
	//}
	 
	public static class NodeDefaultInspector
	{
		public static bool DrawDefaultInspectorWithoutScriptField ( this UnityEditor.Editor _inspector )
		{
			EditorGUI.BeginChangeCheck();
	       
			_inspector.serializedObject.Update();
	       
			SerializedProperty Iterator = _inspector.serializedObject.GetIterator();
	       
			Iterator.NextVisible(true);
	       
			while ( Iterator.NextVisible(false))
			{
				#if FLOWREACTOR_DEBUG
				EditorGUILayout.PropertyField(Iterator, true);
				#else
				if (!Iterator.type.Contains("FR"))
				{
					EditorGUILayout.PropertyField(Iterator, true);
				}
				#endif
			}
	       
			_inspector.serializedObject.ApplyModifiedProperties();
	       
			return (EditorGUI.EndChangeCheck());
		}
	}
}
#endif