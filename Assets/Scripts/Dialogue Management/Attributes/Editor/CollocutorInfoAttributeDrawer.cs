using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CollocutorInfoSelectionAttribute))]
public class CollocutorInfoAttributeDrawer : PropertyDrawer
{
	private const string SELECT_COLLOCUTOR_BUTTON_LABEL_TAG = "Select Collocutor",
						 NONE_MENU_ITEM_TAG = "None",
						 COLLOCUTOR_INFO_TYPE_FILTER_TAG = "t:CollocutorInfo";


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		position = EditorGUI.PrefixLabel(position, label);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect buttonRect = position;
		buttonRect.width = 155;

		string buttonLabel = SELECT_COLLOCUTOR_BUTTON_LABEL_TAG;
		CollocutorInfo currentCollocutorInfo = property.objectReferenceValue as CollocutorInfo;
		if (currentCollocutorInfo != null) buttonLabel = currentCollocutorInfo.name;
		if (GUI.Button(buttonRect, buttonLabel))
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent(NONE_MENU_ITEM_TAG), currentCollocutorInfo == null, () => SetCollocutorInfo(property, null));
			string[] guids = AssetDatabase.FindAssets(COLLOCUTOR_INFO_TYPE_FILTER_TAG);
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				CollocutorInfo collocutorInfo = AssetDatabase.LoadAssetAtPath(path, typeof(CollocutorInfo)) as CollocutorInfo;
				if (collocutorInfo != null)
				{
					GUIContent content = new GUIContent(collocutorInfo.name);
					menu.AddItem(content, collocutorInfo == currentCollocutorInfo, () => SetCollocutorInfo(property, collocutorInfo));
				}
			}
			menu.ShowAsContext();
		}

		position.x += buttonRect.width + 4;
		position.width -= buttonRect.width + 4;
		EditorGUI.ObjectField(position, property, typeof(CollocutorInfo), GUIContent.none);

		if (EditorGUI.EndChangeCheck())
			property.serializedObject.ApplyModifiedProperties();

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	private void SetCollocutorInfo(SerializedProperty collocutorInfoProperty, CollocutorInfo collocutorInfo)
	{
		collocutorInfoProperty.objectReferenceValue = collocutorInfo;
		collocutorInfoProperty.serializedObject.ApplyModifiedProperties();
	}
}
