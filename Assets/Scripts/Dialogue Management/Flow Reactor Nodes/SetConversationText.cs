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
	[NodeAttributes("Dialogue Management" , "Sets text in a selected frame" , "#ff00b3", new string[]{""} , NodeAttributes.NodeType.Normal)]
	public class SetConversationText : SetFrame
	{
		[SerializeField, HideInInspector]
		private GameObject _textFrame;
		public GameObject TextFrame => _textFrame;
		private int textId = -1;

		[SerializeField, HideInInspector]
		private GameObject _nameFrame;
		public GameObject NameFrame => _nameFrame;
		private int nameId = -1;

		[SerializeField, HideInInspector]
		private CollocutorInfo _collocutorInfo;
		public CollocutorInfo CollocutorInfo => _collocutorInfo;

		[SerializeField, TextArea] private List<string> _sentences;
		public List<string> _Sentences => _sentences;

		private FrameText _frameText;
		private FrameCollocutorName _collocutorNameInTextFrame, _collocutorNameSeparate, _collocutorNameFrame;

		private bool _frameWasSet, _nodeFinished, _textFrameIsSceneObject, _nameFrameIsSceneObject;

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
		
		private void DrawTextFrame()
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Select Frame From Prefabs"))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("None"), _textFrame == null, () =>
				{
					_textFrame = null;
					textId = -1;
				});
				string[] guids = AssetDatabase.FindAssets("frame t:GameObject", new[] { "Assets/Prefabs" });
				for (int a = 0; a < guids.Length; a++)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[a]);
					GameObject frameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

					if (frameObject != null)
					{
						FrameText frameText = frameObject.GetComponent<FrameText>();
						if(frameText != null)
                        {
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _textFrame, () =>
							{
								_textFrame = frameObject;
								textId = frameText._Id;
								_textFrameIsSceneObject = false;
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

				menu.AddItem(new GUIContent("None"), _textFrame == null, () =>
				{
					_textFrame = null;
					textId = -1;
				});

				for (int a = 0; a < sceneFrameList.Count; a++)
				{
					GameObject frameObject = sceneFrameList[a];
					if (frameObject != null)
					{
						FrameText frameText = frameObject.GetComponent<FrameText>();
						if (frameText != null)
						{
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _textFrame, () =>
							{
								_textFrame = frameObject;
								textId = frameText._Id;
								_textFrameIsSceneObject = true;
							});
						}
					}
				}
				menu.ShowAsContext();
			}

			GUILayout.EndHorizontal();

			Rect rt = GUILayoutUtility.GetAspectRect(23f);
			EditorGUI.ObjectField(rt, "", _textFrame, typeof(GameObject), true);
		}

		private void DrawNameFrame()
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Select Frame From Prefabs"))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("None"), _nameFrame == null, () =>
				{
					_nameFrame = null;
					nameId = -1;
					_nameFrameIsSceneObject = true;
				});
				string[] guids = AssetDatabase.FindAssets("frame t:GameObject", new[] { "Assets/Prefabs" });
				for (int a = 0; a < guids.Length; a++)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[a]);
					GameObject frameObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

					if (frameObject != null)
					{
						FrameCollocutorName frameName = frameObject.GetComponent<FrameCollocutorName>();
						if(frameName != null)
                        {
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _nameFrame, () =>
							{
								_nameFrame = frameObject;
								nameId = frameName._Id;
								_nameFrameIsSceneObject = false;
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

				menu.AddItem(new GUIContent("None"), _nameFrame == null, () =>
				{
					_nameFrame = null;
					nameId = -1;
					_nameFrameIsSceneObject = true;
				});

				for (int a = 0; a < sceneFrameList.Count; a++)
				{
					GameObject frameObject = sceneFrameList[a];
					if (frameObject != null)
					{
						FrameCollocutorName frameName = frameObject.GetComponent<FrameCollocutorName>();
						if (frameName != null)
						{
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _nameFrame, () =>
							{
								_nameFrame = frameObject;
								nameId = frameName._Id;
								_nameFrameIsSceneObject = true;
							});
						}
					}
				}
				menu.ShowAsContext();
			}

			GUILayout.EndHorizontal();

			Rect rt = GUILayoutUtility.GetAspectRect(23f);
			EditorGUI.ObjectField(rt, "", _nameFrame, typeof(GameObject), true);
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
			if (GUILayout.Button("Renew Scene Objects in the BlackBoard"))
			{
				RenewObjectFromSceneToBlackboard();
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			DrawTextFrame();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			if (_textFrame != null)
			{
				_collocutorNameFrame = _textFrame.GetComponentInChildren<FrameCollocutorName>();
				if (_collocutorNameFrame == null)
				{
					GUILayout.Label("This text frame doesn't have space for collocutor name");
					GUILayout.Label("Select separate name frame");
				}
				else
				{
					GUILayout.Label("This text frame has space for collocutor name");
					GUILayout.Label("Select separate name frame to override");
				}
			}
            else
            {
				GUILayout.Label("No text frame");
			}

			DrawNameFrame();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			DrawCollocutorInfo();
		}

		#endif


		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			_frameWasSet = false;
			_nodeFinished = false;
			// Do not remove this line
			base.OnInitialize(_flowRector);
		}
		public override void SetFrameField()
		{
			CheckIfBlackBoardVariablesComplete();
			if (_textFrame == null)
			{
				for (int a = 0; a < sceneFrameList.Count; a++)
				{
					GameObject frameObject = sceneFrameList[a];
					FrameText frameText = frameObject.GetComponent<FrameText>();
					if (frameText == null)
					{
						continue;
					}
					if (frameText._Id == textId)
					{
						_textFrame = frameObject;
					}
				}
			}
			if (_nameFrame == null)
			{
				for (int a = 0; a < sceneFrameList.Count; a++)
				{
					GameObject frameObject = sceneFrameList[a];
					FrameCollocutorName frameName = frameObject.GetComponent<FrameCollocutorName>();
					if (frameName == null)
					{
						continue;
					}
					if (frameName._Id == nameId)
					{
						_nameFrame = frameObject;
					}
				}
			}
		}

		private void SetFrame()
        {
			GameObject frameTextObject;

			if (!_textFrameIsSceneObject)
			{
				frameTextObject = ObjectPool.Instance.Aquire(_textFrame);
				frameTextObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);
			}
			else
			{
				frameTextObject = _textFrame;
			}

			_frameText = frameTextObject.GetComponent<FrameText>();
			_frameText.InsertInQueue(_sentences);
			_frameText.ShowAnimation();
			frameTextObject.transform.localPosition = _frameText.Position;

			_collocutorNameInTextFrame = frameTextObject.GetComponentInChildren<FrameCollocutorName>();

			if (_nameFrame != null) // collocutor name will be set in the separate frame
			{
				GameObject frameNameObject;

				if (!_nameFrameIsSceneObject)
				{
					frameNameObject = ObjectPool.Instance.Aquire(_nameFrame);
					frameNameObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);
				}
				else
				{
					frameNameObject = _nameFrame;
				}

				_collocutorNameSeparate = frameNameObject.GetComponent<FrameCollocutorName>();
				_collocutorNameSeparate.NameText.text = _collocutorInfo.name;
				_collocutorNameSeparate.ShowAnimation();
				frameNameObject.transform.localPosition = _collocutorNameSeparate.Position;

				if (_collocutorNameInTextFrame != null) // must hide name component in the text frame
				{
					_collocutorNameInTextFrame.Background.color = new Color(0f, 0f, 0f, 0f);
					_collocutorNameInTextFrame.NameText.text = "";
				}
			}
			else
			{
				// collocutor name will be set in the text frame
				if (_collocutorNameInTextFrame != null)
				{
					_collocutorNameInTextFrame.NameText.text = _collocutorInfo.name;
				}
				else
				{
					// no collocutor name at all
				}
			}
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			if (_nodeFinished)
            {
				return;
            }

			if (!_frameWasSet)
            {
				SetFrame();
				_frameWasSet = true;
			}
            else
            {
				if (_frameText._sentencesEnd)
				{
					_nodeFinished = true;
					ExecuteNext(0, _flowReactor);
				}
				_frameText.SetSentence();
			}
		}
	}
}