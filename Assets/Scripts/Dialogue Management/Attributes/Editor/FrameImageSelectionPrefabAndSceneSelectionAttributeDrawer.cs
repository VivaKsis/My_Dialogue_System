using UnityEditor;
using UnityEngine;
using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using FlowReactor.BlackboardSystem;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(FrameImagePrefabAndSceneSelectionAttribute))]
public class FrameImageSelectionPrefabAndSceneSelectionAttributeDrawer : PropertyDrawer
{
    private const string SELECT_FRAME_BUTTON_LABEL_TAG = "Select Frame",
                         NONE_MENU_ITEM_TAG = "None",
                         FRAME_NAME_AND_GAMEOBJECT_TYPE_FILTER_TAG = "frame t:GameObject",
                         PREFAB_FOLDER_TAG = "Assets/Prefabs",
                         BLACKBOARD_PATH_TAG = "Assets/Dialogues/Flow Reactor/Scene Frames.asset";

    private BlackBoard GetSceneFramesBlackBoard()
    {
        return AssetDatabase.LoadAssetAtPath(BLACKBOARD_PATH_TAG, typeof(BlackBoard)) as BlackBoard;
    }
    private void RenewObjectFromSceneToBlackboard(BlackBoard blackBoard)
    {
        Debug.LogWarning("Scene Frames Blackboard Renew");

        List<GameObject> sceneFrameList;
        Transform canvasTransform = GameObject.FindGameObjectWithTag("Main Canvas").transform;
        sceneFrameList = new List<GameObject>();

        for (int b = 0; b < canvasTransform.childCount; b++)
        {
            GameObject gameObject = canvasTransform.GetChild(b).gameObject;

            if (gameObject.GetComponent<Frame>() != null)
            {
                sceneFrameList.Add(gameObject);
            }
        }

        blackBoard.GetVariableByName<FRGameObjectList>("SceneFrames").Value = sceneFrameList;
    }

    private void CheckIfFramesBlackBoardVariablesComplete(BlackBoard blackBoard)
    {
        List<GameObject> sceneFrameList;
        sceneFrameList = blackBoard.GetVariableByName<FRGameObjectList>("SceneFrames").Value;
        for (int a = 0; a < sceneFrameList.Count; a++)
        {
            if (sceneFrameList[a] == null)
            {
                Debug.LogWarning("Scene Frames in the blackboard is not completed. Renew");
                RenewObjectFromSceneToBlackboard(blackBoard);
                break;
            }
        }
    }


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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Frame From Prefabs"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), currentFrame == null, () => SetFrame(property, null));
            string[] guids = AssetDatabase.FindAssets("frame t:GameObject", new[] { "Assets/Prefabs" });
            for (int a = 0; a < guids.Length; a++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[a]);
                GameObject frameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                if (frameObject != null && frameObject.GetComponent<FrameImage>() != null)
                {
                    GUIContent content = new GUIContent(frameObject.name);
                    menu.AddItem(content, frameObject == currentFrame, () => SetFrame(property, frameObject));
                }
            }
            menu.ShowAsContext();
        }

        if (GUILayout.Button(text: "Select Frame From The Scene"))
        {
            GenericMenu menu = new GenericMenu();

            BlackBoard sceneFramesBlackBoard = GetSceneFramesBlackBoard();

            if (sceneFramesBlackBoard == null)
            {
                menu.AddItem(new GUIContent("Scene Frames BlackBoard is not selected"), false, null);
                menu.ShowAsContext();
                return;
            }

            CheckIfFramesBlackBoardVariablesComplete(sceneFramesBlackBoard);

            List<GameObject> sceneFrameList = sceneFramesBlackBoard.GetVariableByName<FRGameObjectList>("SceneFrames").Value;

            menu.AddItem(new GUIContent("None"), currentFrame == null, () => SetFrame(property, null));

            for (int a = 0; a < sceneFrameList.Count; a++)
            {
                GameObject frameObject = sceneFrameList[a];
                if (frameObject != null && frameObject.GetComponent<FrameImage>() != null)
                {
                    GUIContent content = new GUIContent(frameObject.name);
                    menu.AddItem(content, frameObject == currentFrame, () =>
                    {
                        int id = frameObject.GetComponent<FrameImage>()._Id;
                        Debug.Log(id);
                    });
                }
            }
            menu.ShowAsContext();
        }

        GUILayout.EndHorizontal();

        position.x += buttonRect.width + 4;
        position.width -= buttonRect.width + 4;
        //EditorGUI.ObjectField(position, property, typeof(GameObject), GUIContent.none);
        EditorGUI.ObjectField(position, "", property.objectReferenceValue, typeof(GameObject), true);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        /*if (GUI.Button(buttonRect, buttonLabel))
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent(NONE_MENU_ITEM_TAG), currentFrame == null, () => SetFrame(property, null));
			string[] guids = AssetDatabase.FindAssets(FRAME_NAME_AND_GAMEOBJECT_TYPE_FILTER_TAG, new[] { PREFAB_FOLDER_TAG });
			for (int a = 0; a < guids.Length; a++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[a]);
				GameObject frame = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				if (frame != null && frame.GetComponent<FrameImage>() != null)
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
		EditorGUI.EndProperty();*/
    }

    private void SetFrame(SerializedProperty frameProperty, GameObject frame)
    {
        frameProperty.objectReferenceValue = frame;
        frameProperty.serializedObject.ApplyModifiedProperties();
    }
}
