using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using FlowReactor.BlackboardSystem;
using System;

namespace FlowReactor.Nodes
{
    [NodeAttributes("Dialogue Management", "Set collocutor's image in the selected frame", "#0dff00", new string[] { "" }, NodeAttributes.NodeType.Normal)]
    public class SetCollocutor : SetFrame
    {
        [SerializeField, HideInInspector]
        private GameObject _frame;
        public GameObject _Frame => _frame;
        private int id = -1;

        [SerializeField, HideInInspector]
        private CollocutorInfo _collocutorInfo;
        public CollocutorInfo CollocutorInfo => _collocutorInfo;

        [SerializeField, HideInInspector]
        private Sprite _sprite;
        public Sprite _Sprite => _sprite;

        private FrameImage _frameImage;
        public bool frameIsSceneObject;

        #region Unity Editor

        private CollocutorInfo _previousCollocutorInfo;

        // Editor node methods
#if UNITY_EDITOR
        // Node initialization called upon node creation
        public override void Init(Graph _graph, Node _node)
        {
            base.Init(_graph, _node);

            // possibility to hide the default node inspector. Set to false normally.
            disableDefaultInspector = false;
            disableVariableInspector = false;
            disableDrawCustomInspector = false;

            nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
        }

        // Draw default node window
        public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
        {
            base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
        }

        private void DrawFrame()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Frame From Prefabs"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), _frame == null, () =>
                {
                    _frame = null;
                    id = -1;
                });
                string[] guids = AssetDatabase.FindAssets("frame t:GameObject", new[] { "Assets/Prefabs" });
                for (int a = 0; a < guids.Length; a++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[a]);
                    GameObject frameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                    if (frameObject != null)
                    {
                        FrameImage frameImage = frameObject.GetComponent<FrameImage>();
                        if (frameImage != null)
                        {
                            menu.AddItem(new GUIContent(frameObject.name), frameObject == _frame, () =>
                            {
                                _frame = frameObject;
                                id = frameImage._Id;
                                frameIsSceneObject = false;
                            });
                        }

                    }
                }
                menu.ShowAsContext();
            }

            if (GUILayout.Button(text: "Select Frame From The Scene"))
            {
                GenericMenu menu = new GenericMenu();

                if (SceneFramesBlackBoard == null)
                {
                    menu.AddItem(new GUIContent("Scene Frames BlackBoard is not selected"), false, null);
                    menu.ShowAsContext();
                    return;
                }

                CheckIfBlackBoardVariablesComplete();

                menu.AddItem(new GUIContent("None"), _frame == null, () =>
                {
                    _frame = null;
                    id = -1;
                });

                for (int a = 0; a < sceneFrameList.Count; a++)
                {
                    GameObject frameObject = sceneFrameList[a];
                    if (frameObject != null)
                    {
                        FrameImage frameImage = frameObject.GetComponent<FrameImage>();
                        if (frameImage != null)
                        {
                            menu.AddItem(new GUIContent(frameObject.name), frameObject == _frame, () =>
                            {
                                _frame = frameObject;
                                id = frameImage._Id;
                                frameIsSceneObject = true;
                            });
                        }
        
                    }
                }
                menu.ShowAsContext();
            }

            GUILayout.EndHorizontal();

            Rect rt = GUILayoutUtility.GetAspectRect(23f);
            EditorGUI.ObjectField(rt, "", _frame, typeof(GameObject), true);
        }

        private void DrawSprite()
        {

            //SerializedObject serializedObject = new SerializedObject(this);

            //SerializedProperty serializedPropertySprite = serializedObject.FindProperty("_sprite");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(text: "Select Sprite"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), _sprite == null, () => _sprite = null);
                string[] spriteNames = GetSpriteNames();
                for (int a = 0; a < spriteNames.Length; a++)
                {
                    Sprite spriteValue = _collocutorInfo._CollocutorSprites[a];
                    if (spriteValue != null)
                    {
                        GUIContent content = new GUIContent(spriteValue.name);
                        menu.AddItem(content, spriteValue == _sprite, () => _sprite = spriteValue);
                    }
                }
                menu.ShowAsContext();
            }

            //serializedPropertySprite.objectReferenceValue = _sprite;

            Rect rt = GUILayoutUtility.GetAspectRect(10f);
            if (_sprite != null)
            {

                //EditorGUI.ObjectField(rt, serializedPropertySprite, typeof(Sprite), GUIContent.none);
            }

            GUILayout.EndHorizontal();

            EditorGUI.ObjectField(rt, "", _sprite, typeof(GameObject), true);

            //serializedObject.ApplyModifiedProperties();
        }

        private void DrawCollocutorInfo()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Select Collocutor"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), _collocutorInfo == null, () => _collocutorInfo = null);
                string[] guids = AssetDatabase.FindAssets("t:CollocutorInfo");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    CollocutorInfo collocutorInfo = AssetDatabase.LoadAssetAtPath(path, typeof(CollocutorInfo)) as CollocutorInfo;
                    if (collocutorInfo != null)
                    {
                        GUIContent content = new GUIContent(collocutorInfo.name);
                        menu.AddItem(content, collocutorInfo == _collocutorInfo, () => _collocutorInfo = collocutorInfo);
                    }
                }
                menu.ShowAsContext();
            }

            Rect rt = GUILayoutUtility.GetAspectRect(10f);
            EditorGUI.ObjectField(rt, "", _collocutorInfo, typeof(GameObject), true);

            GUILayout.EndHorizontal();
        }

        public override void DrawCustomInspector()
        {
            if (_previousCollocutorInfo == null)
            {
                _previousCollocutorInfo = _collocutorInfo;
            }
            else if (_previousCollocutorInfo != _collocutorInfo) // if collocutor changes, sprite resets to null
            {
                _sprite = null;
                _previousCollocutorInfo = _collocutorInfo;
            }

            if (GUILayout.Button("Renew Scene Objects in the BlackBoard"))
            {
                RenewObjectFromSceneToBlackboard();
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            DrawFrame();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawCollocutorInfo();
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (_collocutorInfo == null)
            {
                GUILayout.Label("Select Collocutor to choose from only his sprites");
            }
            
            DrawSprite();
        }

        private string[] GetSpriteNames()
        {
            string[] names = new string[_collocutorInfo._CollocutorSprites.Length];
            for (int a = 0; a < _collocutorInfo._CollocutorSprites.Length; a++)
            {
                names[a] = _collocutorInfo._CollocutorSprites[a].name;
            }
            return names;
        }
#endif

        #endregion

        public override void OnInitialize(FlowReactorComponent _flowRector)
        {
            // Do not remove this line
            base.OnInitialize(_flowRector);
        }

        public override void SetFrameField()
        {
            if (_frame == null)
            {
                CheckIfBlackBoardVariablesComplete();

                for (int a = 0; a < sceneFrameList.Count; a++)
                {
                    GameObject frameObject = sceneFrameList[a];
                    FrameImage frameImage = frameObject.GetComponent<FrameImage>();
                    if (frameImage == null)
                    {
                        continue;
                    }
                    if (frameImage._Id == id)
                    {
                        _frame = frameObject;
                    }
                }
            }
        }

        // Execute node
        public override void OnExecute(FlowReactorComponent _flowReactor)
        {
            GameObject frameObject;

            if (!frameIsSceneObject)
            {
                frameObject = ObjectPool.Instance.Aquire(_frame);
                frameObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);
            }
            else
            {
                frameObject = _frame;
            }
            _frameImage = frameObject.GetComponent<FrameImage>();
            _frameImage.SetImageSprite(_sprite);
            _frameImage.ShowAnimation();
            frameObject.transform.localPosition = _frameImage.Position;

            ExecuteNext(0, _flowReactor);
        }
    }
}