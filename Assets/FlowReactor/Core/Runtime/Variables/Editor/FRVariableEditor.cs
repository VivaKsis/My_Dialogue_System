#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FlowReactor;

[System.Serializable]
[CustomEditor(typeof (FRVariable))]
public class FRVariableEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
	
}
#endif