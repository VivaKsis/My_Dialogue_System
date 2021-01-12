using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SetCollocutorNode))]
public class SetCollocutorNodeEditor : NodeEditor
{
    private CollocutorInfo _collocutorInfo, _previousCollocutorInfo;
    private Sprite[] _collocutorSprites;
    private Sprite _sprite;
    private SerializedProperty _serializedPropertyCollocutorInfo, _serializedPropertySprite;

    private const string SELECT_COLLOCUTOR_HINT_TAG = "Select Collocutor to choose from only his sprites",
                         SPRITE_LABEL_TAG = "Sprite",
                         SELECT_SPRITE_BUTTON_TEXT_TAG = "Select Sprite",
                         NONE_MENU_ITEM_TAG = "None";

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetInputPort(BaseDialogueNode._INPUT_TAG), GUILayout.MinWidth(0));
        NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort(BaseDialogueNode._SEQUEL_TAG), GUILayout.MinWidth(0));
        GUILayout.EndHorizontal();

        _serializedPropertyCollocutorInfo = serializedObject.FindProperty(SetCollocutorNode._COLLOCUTOR_INFO_TAG);
        _serializedPropertySprite = serializedObject.FindProperty(SetCollocutorNode._SPRITE_TAG);

        EditorGUILayout.PropertyField(serializedObject.FindProperty(SetCollocutorNode._FRAME_TAG), GUIContent.none);
        EditorGUILayout.PropertyField(_serializedPropertyCollocutorInfo, GUIContent.none);

        _collocutorInfo = _serializedPropertyCollocutorInfo.objectReferenceValue as CollocutorInfo;
        _sprite = _serializedPropertySprite.objectReferenceValue as Sprite;

        if (_collocutorInfo == null)
        {
            GUILayout.Label(SELECT_COLLOCUTOR_HINT_TAG);

            GUILayout.BeginHorizontal();
            GUILayout.Label(SPRITE_LABEL_TAG);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SetCollocutorNode._SPRITE_TAG), GUIContent.none);
            GUILayout.EndHorizontal();
        }
        else
        {
            if (_previousCollocutorInfo == null)
            {
                _previousCollocutorInfo = _collocutorInfo;
            }
            else if (_previousCollocutorInfo != _collocutorInfo) // if collocutor changes, sprite resets
            {
                SetSprite(_serializedPropertySprite, null);
                _previousCollocutorInfo = _collocutorInfo;
            }

            _collocutorSprites = _collocutorInfo._CollocutorSprites;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(text: SELECT_SPRITE_BUTTON_TEXT_TAG))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(NONE_MENU_ITEM_TAG), _sprite == null, () => SetSprite(_serializedPropertySprite, null));
                string[] spriteNames = GetSpriteNames();
                for (int a = 0; a < spriteNames.Length; a++)
                {
                    Sprite spriteValue = _collocutorSprites[a];
                    if (spriteValue != null)
                    {
                        GUIContent content = new GUIContent(spriteValue.name);
                        menu.AddItem(content, spriteValue == _sprite, () => SetSprite(_serializedPropertySprite, spriteValue));
                    }
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SetCollocutorNode._SPRITE_TAG), GUIContent.none);
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private string[] GetSpriteNames()
    {
        string[] names = new string[_collocutorSprites.Length];
        for (int a = 0; a < _collocutorSprites.Length; a++)
        {
            names[a] = _collocutorSprites[a].name;
        }
        return names;
    }

    private void SetSprite(SerializedProperty spriteProperty, Sprite sprite)
    {
        spriteProperty.objectReferenceValue = sprite;
        spriteProperty.serializedObject.ApplyModifiedProperties();
    }

    public override int GetWidth()
    {
        return 400;
    }
}
