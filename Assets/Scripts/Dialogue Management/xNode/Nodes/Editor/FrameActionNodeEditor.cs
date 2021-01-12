using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(FrameActionNode))]
public class FrameActionNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(BaseDialogueNode._INPUT_TAG), GUILayout.MinWidth(0));
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(BaseDialogueNode._SEQUEL_TAG), GUILayout.MinWidth(0));
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(FrameActionNode._FRAME_TAG), GUIContent.none);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(FrameActionNode._DO_ACTION_TAG), GUIContent.none);

        serializedObject.ApplyModifiedProperties();
    }

    public override int GetWidth()
    {
        return 350;
    }
}
