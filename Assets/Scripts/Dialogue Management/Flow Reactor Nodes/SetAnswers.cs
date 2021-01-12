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

namespace FlowReactor.Nodes
{
	[NodeAttributes( "Dialogue Management" , "Sets answers in a frame. Add OnAnswerChosen as Input to navigate to an answer" , "#ff7300", null, NodeAttributes.NodeType.Normal )]
	public class SetAnswers : SetFrame
	{
		[SerializeField, HideInInspector]
		private GameObject _frame;
		public GameObject Frame => _frame;
		private int id;

		[SerializeField] private List<string> _answers;
		public List<string> Answers => _answers;

		[HideInNode]
		public FRInt IndexAnswerChosen;

		private FrameAnswers _frameAnswers;
		private bool nodeIsFinished, frameIsSceneObject;

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
						FrameAnswers frameAnswers = frameObject.GetComponent<FrameAnswers>();
						if (frameAnswers != null)
                        {
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _frame, () =>
							{
								_frame = frameObject;
								id = frameAnswers._Id;
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
						FrameAnswers frameAnswers = frameObject.GetComponent<FrameAnswers>();
						if (frameAnswers != null)
						{
							menu.AddItem(new GUIContent(frameObject.name), frameObject == _frame, () =>
							{
								_frame = frameObject;
								id = frameAnswers._Id;
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

		public override void DrawCustomInspector()
		{
			if (GUILayout.Button("Renew Scene Objects in the BlackBoard"))
			{
				RenewObjectFromSceneToBlackboard();
			}

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			DrawFrame();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			GUILayout.Label("Press this button after you've typed answers");
			if (GUILayout.Button("Add Outputs"))
			{
				for (int a = 0; a < _answers.Count + 1; a++)
                {
					RemoveOutput(a);
                }
				for (int a = 0; a < _answers.Count; a++)
                {
					AddOutput(_answers[a]);
				}
			}
		}

#endif
		public override void SetFrameField()
		{
			if (_frame == null)
			{
				CheckIfBlackBoardVariablesComplete();

				for (int a = 0; a < sceneFrameList.Count; a++)
				{
					GameObject frameObject = sceneFrameList[a];
					FrameAnswers frameAnswers = frameObject.GetComponent<FrameAnswers>();
					if (frameAnswers == null)
					{
						continue;
					}
					if (frameAnswers._Id == id)
					{
						_frame = frameObject;
					}
				}
			}
		}

		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			nodeIsFinished = false;
			IndexAnswerChosen.Value = -1;

			// Do not remove this line:
			base.OnInitialize(_flowRector);
		}

		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			if (!nodeIsFinished)
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

				_frameAnswers = frameObject.GetComponent<FrameAnswers>();
				_frameAnswers.Show();
				frameObject.transform.localPosition = _frameAnswers.Position;
				_frameAnswers.SetAnswerButtons(_answers);

				nodeIsFinished = true;
			}
            else
            {
				if (IndexAnswerChosen.Value >= 0 || IndexAnswerChosen.Value < _answers.Count - 1)
                {
                    if (frameIsSceneObject)
                    {
						_frameAnswers.Hide();
                    }
                    else
                    {
						_frameAnswers.ClearFrame();
					}
					ExecuteNext(IndexAnswerChosen.Value, _flowReactor);
				}
                else
                {
					Debug.LogError("An answer with such INDEX - " + IndexAnswerChosen.Value + " - doesn't exist. Maybe you forgot to connect 'IndexAnswerChosen' to the blackboard");
                }
            }
		}
	}
}