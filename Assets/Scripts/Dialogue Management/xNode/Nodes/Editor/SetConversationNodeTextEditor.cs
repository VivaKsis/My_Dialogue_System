using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SetConversationTextNode))]
public class SetConversationNodeTextEditor : NodeEditor
{
    private GameObject _textFrame;
    private SerializedProperty _serializedPropertyTextFrame, _serializedPropertyNameFrame;
    private FrameCollocutorName _collocutorNameFrame;

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(BaseDialogueNode._INPUT_TAG), GUILayout.MinWidth(0));
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(BaseDialogueNode._SEQUEL_TAG), GUILayout.MinWidth(0));
        GUILayout.EndHorizontal();

        _serializedPropertyTextFrame = serializedObject.FindProperty(SetConversationTextNode._TEXT_FRAME_TAG);
        _serializedPropertyNameFrame = serializedObject.FindProperty(SetConversationTextNode._NAME_FRAME_TAG);

        EditorGUILayout.PropertyField(_serializedPropertyTextFrame, GUIContent.none);
        
        _textFrame = _serializedPropertyTextFrame.objectReferenceValue as GameObject;

        if (_textFrame != null)
        {
            _collocutorNameFrame = _textFrame.GetComponentInChildren<FrameCollocutorName>();
        }

        if (_collocutorNameFrame == null)
        {
            GUILayout.Label("This frame doesn't have space for collocutor name");
            GUILayout.Label("Select separate name frame");
        }
        else
        {
            GUILayout.Label("This frame has space for collocutor name");
            GUILayout.Label("Select separate name frame to override");
        }

        EditorGUILayout.PropertyField(_serializedPropertyNameFrame, GUIContent.none);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(SetConversationTextNode._COLLOCUTOR_INFO_TAG), GUIContent.none);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(SetConversationTextNode._SENTENCES_TAG), GUIContent.none);

        serializedObject.ApplyModifiedProperties();
    }

    public override int GetWidth()
    {
        return 350;
    }
}
