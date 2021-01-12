using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FrameAnswersSelectionAttribute))]
public class FrameAnswersSelectionAttributeDrawer : PropertyDrawer
{
	private const string SELECT_FRAME_BUTTON_LABEL_TAG = "Select Frame",
						 NONE_MENU_ITEM_TAG = "None",
						 FRAME_NAME_AND_GAMEOBJECT_TYPE_FILTER_TAG = "frame t:GameObject",
						 PREFAB_FOLDER_TAG = "Assets/Prefabs";


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		position = EditorGUI.PrefixLabel(position, label);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect buttonRect = position;
		buttonRect.width = 155;

		string buttonLabel = SELECT_FRAME_BUTTON_LABEL_TAG;
		GameObject currentFrame = property.objectReferenceValue as GameObject;
		if (currentFrame != null) buttonLabel = currentFrame.name;
		if (GUI.Button(buttonRect, buttonLabel))
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent(NONE_MENU_ITEM_TAG), currentFrame == null, () => SetFrame(property, null));
			string[] guids = AssetDatabase.FindAssets(FRAME_NAME_AND_GAMEOBJECT_TYPE_FILTER_TAG, new[] { PREFAB_FOLDER_TAG });
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				GameObject frame = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				if (frame != null && frame.GetComponent<FrameAnswers>() != null)
				{
					GUIContent content = new GUIContent(frame.name);
					menu.AddItem(content, frame == currentFrame, () => SetFrame(property, frame));
				}
			}
			menu.ShowAsContext();
		}

		position.x += buttonRect.width + 4;
		position.width -= buttonRect.width + 4;
		EditorGUI.ObjectField(position, property, typeof(GameObject), GUIContent.none);

		if (EditorGUI.EndChangeCheck())
			property.serializedObject.ApplyModifiedProperties();

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	private void SetFrame(SerializedProperty frameProperty, GameObject frame)
	{
		frameProperty.objectReferenceValue = frame;
		frameProperty.serializedObject.ApplyModifiedProperties();
	}
}
