using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(SetAnswersNode))]
public class SetAnswersNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(BaseDialogueNode._INPUT_TAG), GUILayout.MinWidth(0));

        EditorGUILayout.PropertyField(serializedObject.FindProperty(SetAnswersNode._FRAME_TAG), GUIContent.none);
        NodeEditorGUILayout.DynamicPortList(SetAnswersNode._ANSWERS_TAG, typeof(BaseDialogueNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);

        serializedObject.ApplyModifiedProperties();
    }

    public override int GetWidth()
    {
        return 350;
    }
}
