#if FLOWREACTOR_DOTWEEN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

using DG.Tweening;
using FlowReactor;
using FlowReactor.EventSystem;
using FlowReactor.Editor;

namespace FlowReactor.Nodes.DOTween
{
	[NodeAttributes("DOTween", "DOSequence", 1, NodeAttributes.NodeType.Normal)]
	public class DOSequence : Node
	{
	
		[System.Serializable]
		public class Sequence
		{
			public FRFloat floatValue;
			public FRInt intValue;
			public FRImage image;
			public FRRectTransform rectTransform;
			public FRCanvasGroup canvasGroup;
			public FRGameObject gameObject;
			public UnityEvent callback;
			
			public enum TweenState
			{
				append,
				insert
			}
			
			public enum TweenObject
			{
				floatValue,
				intValue,
				image,
				rectTransform,
				canvasGroup,
				gameObject,
			}
			
			public enum TweenMethodVariables
			{
				DOValue,
			}
			
			public enum TweenMethodTransform
			{
				DOScale,
				DOScaleX,
				DOScaleY,
				DOScaleZ,
				DOMove,
				DOMoveX,
				DOMoveY,
				DOMoveZ,
			
				DOLocalMove,
				DOLocalMoveX,
				DOLocalMoveY,
				DOLocalMoveZ,
				DORotate,
				DOLocalRotate,
				DOSizeDelta,
				DOAnchoredPosition
			}
			
			public enum TweenMethodImage
			{
				DOColor,
				DOFill
			}
			
			public enum TweenMethodCanvas
			{
				DOFade
			}
		
			public TweenState tweenState;
			public TweenObject tweenObject;
			public TweenMethodVariables tweenMethodVariables;
			public TweenMethodTransform tweenMethodTransform;
			public TweenMethodImage tweenMethodImage;
			public TweenMethodCanvas tweenMethodCanvas;
			
			public Ease easeType = Ease.Linear;
			
			public FRFloat targetFloat;
			public FRInt targetInt;
			public FRVector3 target;
			public FRVector2 targetVector2;
			public FRColor targetColor;
			
			
			public float duration;
			public float insertTime;
			
			public Sequence()
			{
				floatValue = new FRFloat(0f);
				intValue = new FRInt(0);
				image = new FRImage();
				canvasGroup = new FRCanvasGroup();
				rectTransform = new FRRectTransform();
				gameObject = new FRGameObject();
				
				targetFloat = new FRFloat(0f);
				targetInt = new FRInt(0);
				target = new FRVector3(Vector3.zero);
				targetVector2 = new FRVector2(Vector2.zero);
				targetColor = new FRColor(Color.black);
			}
		}
		
		public List<Sequence> tweenSequence = new List<Sequence> ();
	
		
		// Event properties
		[SerializeField]
		int selectedEB;
		[SerializeField]
		string selectedEventID;
		[SerializeField]
		int selectedEventIDInt;
		#if UNITY_EDITOR
		EventBoardEditor editor;
		#endif
		public bool useOnTweenCompleteEvent;
		public EventBoard eventBoard;
		
		
		public string tweenID;
		
		
		// Editor Properties
		public static string selectedFieldName;
		public static Guid selectedBlackboardGuid;
		public static Guid selectedVariableGuid;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			icon = EditorHelpers.LoadIcon("doTweenIcon.png");
			disableDefaultInspector = true;
			disableVariableInspector = true;
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 155, 60);
		}
		
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}

	
		public override void DrawCustomInspector()
		{
			base.DrawCustomInspector();
			
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Tween ID:", "boldLabel");
				tweenID = GUILayout.TextField(tweenID, GUILayout.Height(18));
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("On Tween Complete Event", "boldLabel");
				
				useOnTweenCompleteEvent = GUILayout.Toggle(useOnTweenCompleteEvent, "Enable");
				
				if (useOnTweenCompleteEvent)
				{
				
					var eb = rootGraph.eventboards.Keys.Select(x => x.ToString()).ToArray();
					var ebName = rootGraph.eventboards.Values.Select(x => x.eventboard != null ? x.eventboard.name.ToString() : "empty").ToArray();
					
					GUILayout.Label("Eventboard:");
					
					selectedEB = EditorGUILayout.Popup(selectedEB, ebName);
					
					if (eb != null || eb.Length > 0)
					{
						if (selectedEB < eb.Length)
						{
							if (rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard != null)
							{
								if (editor == null || rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard != eventBoard)
								{
									eventBoard = rootGraph.eventboards[Guid.Parse(eb[selectedEB])].eventboard;
								
									editor = UnityEditor.Editor.CreateEditor(eventBoard) as EventBoardEditor;
								}
							
								if (editor != null)
								{
								
									editor.DrawNodeInspector(eventBoard, selectedEventID, out selectedEventID, out selectedEventIDInt);
									
								}
							}
						}
					
					}
					else
					{
						EditorGUILayout.HelpBox("Create or assign a new eventboard", MessageType.Info);
					}
				}
				
			}
		
			
			GUILayout.Label("Tween Sequence:");
			
			using (new GUILayout.VerticalScope("TextArea"))
			{
				if (GUILayout.Button("Add"))
				{
					tweenSequence.Add(new Sequence());
				}
				
				var _totalDuration = 0f;
				
				for (int s = 0; s < tweenSequence.Count; s ++)
				{
					using (new GUILayout.VerticalScope("Box"))
					{
						
						using (new GUILayout.HorizontalScope("Toolbar"))
						{
							if (s - 1 >= 0)
							{
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									GUILayout.Label("starts at: " + _totalDuration.ToString());
								}
								else
								{
									GUILayout.Label("starts at: " + tweenSequence[s].insertTime);
								}
							}
							else
							{
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									GUILayout.Label("starts at: 0");
								}
								else
								{
									GUILayout.Label("starts at: " + tweenSequence[s].insertTime);
								}
							}
							
							GUILayout.FlexibleSpace();
							
							if (GUILayout.Button("<", "ToolbarButton", GUILayout.Width(25)))
							{
								
							}
							
							if (GUILayout.Button(">", "ToolbarButton", GUILayout.Width(25)))
							{
								
							}
							
							if (GUILayout.Button("x", "ToolbarButton", GUILayout.Width(25)))
							{
								tweenSequence.RemoveAt(s);
								
								return;
							}
						}
						
						using (new GUILayout.HorizontalScope())
						{
							tweenSequence[s].tweenState = (Sequence.TweenState) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenState, GUILayout.Width(100));
							
							if (tweenSequence[s].tweenState == Sequence.TweenState.insert)
							{
								tweenSequence[s].insertTime = EditorGUILayout.FloatField(tweenSequence[s].insertTime);
							}
						}
							
						using (new GUILayout.HorizontalScope())
						{
							tweenSequence[s].tweenObject = (Sequence.TweenObject) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenObject, GUILayout.Width(100));
							
							switch(tweenSequence[s].tweenObject)
							{
							case Sequence.TweenObject.floatValue:
								var _varFloat = tweenSequence[s].GetType().GetField("floatValue").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("floatValue", _varFloat, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("floatValue"), tweenSequence[s].floatValue, this, editorSkin);
								break;
							case Sequence.TweenObject.intValue:
								var _varInt = tweenSequence[s].GetType().GetField("intValue").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("intValue", _varInt, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("intValue"), tweenSequence[s].intValue, this, editorSkin);
								break;
							case Sequence.TweenObject.gameObject:
								var _varGO = tweenSequence[s].GetType().GetField("gameObject").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("gameObject", _varGO, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("gameObject"), tweenSequence[s].gameObject, this, editorSkin);
								break;
							case Sequence.TweenObject.image:
								var _varImage = tweenSequence[s].GetType().GetField("image").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("image", _varImage, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("image"), tweenSequence[s].image, this, editorSkin);
								break;
							case Sequence.TweenObject.rectTransform:
								var _varRect = tweenSequence[s].GetType().GetField("rectTransform").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("rectTransform", _varRect, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("rectTransform"), tweenSequence[s].rectTransform, this, editorSkin);
								break;
							case Sequence.TweenObject.canvasGroup:
								var _varCG = tweenSequence[s].GetType().GetField("canvasGroup").GetValue(tweenSequence[s]) as FRVariable;
								FRVariableGUIUtility.DrawVariable("canvasGroup", _varCG, this, false, editorSkin);
								//FRVariableGUIUtility.DrawVariable(tweenSequence[s].GetType().GetField("canvasGroup"), tweenSequence[s].canvasGroup, this, editorSkin);
								break;
							}
						}
					
						
						using (new GUILayout.HorizontalScope())
						{		
							
							switch(tweenSequence[s].tweenObject)
							{
							
							case Sequence.TweenObject.floatValue:
								
								tweenSequence[s].tweenMethodVariables = (Sequence.TweenMethodVariables) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodVariables, GUILayout.Width(100));
							
								FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetFloat, this, false, editorSkin);
								
								break;
								
							case Sequence.TweenObject.intValue:
								
								tweenSequence[s].tweenMethodVariables = (Sequence.TweenMethodVariables) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodVariables, GUILayout.Width(100));
				
								FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetInt, this, false, editorSkin);
								
								break;
							
							case Sequence.TweenObject.gameObject:
								tweenSequence[s].tweenMethodTransform = (Sequence.TweenMethodTransform) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodTransform, GUILayout.Width(100));
							
								FRVariableGUIUtility.DrawVariable(tweenSequence[s].target, this, false, editorSkin);
								break;
							case Sequence.TweenObject.image:
								tweenSequence[s].tweenMethodImage = (Sequence.TweenMethodImage) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodImage, GUILayout.Width(100) );
										
								switch(tweenSequence[s].tweenMethodImage)
								{
								case Sequence.TweenMethodImage.DOColor:
			
									FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetColor, this, false, editorSkin);
									break;
								default:
								
									FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetFloat, this, false, editorSkin);
									break;
								}
										
										
								break;
							case Sequence.TweenObject.rectTransform:
								tweenSequence[s].tweenMethodTransform = (Sequence.TweenMethodTransform) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodTransform, GUILayout.Width(100) );
								//tweenSequence[s].target = EditorGUILayout.Vector3Field("",tweenSequence[s].target);
								
								switch (tweenSequence[s].tweenMethodTransform)
								{
								case Sequence.TweenMethodTransform.DOSizeDelta:
									FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetVector2, this, false, editorSkin);
									break;
								default:
									FRVariableGUIUtility.DrawVariable(tweenSequence[s].target, this, false, editorSkin);
									break;
								}
								break;
							case Sequence.TweenObject.canvasGroup:
								tweenSequence[s].tweenMethodCanvas = (Sequence.TweenMethodCanvas) EditorGUILayout.EnumPopup("", tweenSequence[s].tweenMethodCanvas, GUILayout.Width(100) );
								//tweenSequence[s].target.Value.x = EditorGUILayout.FloatField(tweenSequence[s].target.Value.x);
								FRVariableGUIUtility.DrawVariable(tweenSequence[s].targetFloat, this, false, editorSkin);
								break;
							}
						
						
						}
						
					
						using (new GUILayout.HorizontalScope())
						{		
							tweenSequence[s].easeType = (Ease) EditorGUILayout.EnumPopup("", tweenSequence[s].easeType, GUILayout.Width(100));
							tweenSequence[s].duration = EditorGUILayout.FloatField(tweenSequence[s].duration);
						}
						
					
					}
					
					if (tweenSequence[s].tweenState == Sequence.TweenState.append)
					{
						_totalDuration += tweenSequence[s].duration;
					}
					else
					{
						_totalDuration += tweenSequence[s].duration + tweenSequence[s].insertTime;
					}
				}
			}
		}
		#endif
		
		void UpdateFloat(int _sequenceIndex, float _f)
		{
			tweenSequence[_sequenceIndex].floatValue.Value = _f;
		}
		
		void UpdateInt(int _sequenceIndex, int _i)
		{
			tweenSequence[_sequenceIndex].intValue.Value = _i;
		}
		
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{	
			DG.Tweening.Sequence _seq = DG.Tweening.DOTween.Sequence();
			
			for (int s = 0; s < tweenSequence.Count; s ++)
			{
			
				switch(tweenSequence[s].tweenObject)
				{
					case Sequence.TweenObject.floatValue:
						
						switch(tweenSequence[s].tweenMethodVariables)
						{
						case Sequence.TweenMethodVariables.DOValue:
							if (tweenSequence[s].tweenState == Sequence.TweenState.append)
							{
								var _float = tweenSequence[s].floatValue.Value;
								var _target = tweenSequence[s].targetFloat.Value;
								var _index = s;
								_seq.Append(DG.Tweening.DOTween.To(x => _float = x, _float, _target, tweenSequence[s].duration)).OnUpdate(() => UpdateFloat(_index, _float));		
							}
							else
							{
								var _float = tweenSequence[s].floatValue.Value;
								var _target = tweenSequence[s].targetFloat.Value;
								var _index = s;
								_seq.Insert(tweenSequence[s].insertTime, DG.Tweening.DOTween.To(() => _float, x => _float = x, _target, tweenSequence[s].duration)).OnUpdate(() => UpdateFloat(_index, _float));
							}
							break;
						}
						
						break;
						
					case Sequence.TweenObject.intValue:
						
						switch(tweenSequence[s].tweenMethodVariables)
						{
						case Sequence.TweenMethodVariables.DOValue:
							if (tweenSequence[s].tweenState == Sequence.TweenState.append)
							{
								var _int = tweenSequence[s].intValue.Value;
								var _target = tweenSequence[s].targetInt.Value;
								var _index = s;
								_seq.Append(DG.Tweening.DOTween.To(() => _int, x => _int = x, _target, tweenSequence[s].duration)).OnUpdate(() => UpdateInt(_index, _int));
							}
							else
							{
								var _int = tweenSequence[s].intValue.Value;
								var _target = tweenSequence[s].targetInt.Value;
								var _index = s;
								_seq.Insert(tweenSequence[s].insertTime, DG.Tweening.DOTween.To(() => _int, x => _int = x, _target, tweenSequence[s].duration)).OnUpdate(() => UpdateInt(_index, _int));
							}
							break;
						}
						
						break;
					
					case Sequence.TweenObject.gameObject:
					
						switch(tweenSequence[s].tweenMethodTransform)
						{
							case Sequence.TweenMethodTransform.DOLocalMove:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOLocalMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOLocalMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveX:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOLocalMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOLocalMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveY:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOLocalMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOLocalMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveZ:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOLocalMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOLocalMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMove:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMoveX:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].gameObject.Value.transform.DOMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMoveY:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOMoveZ:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOScale:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOScale(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOScale(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOScaleX:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOScaleX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOScaleX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOScaleY:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOScaleY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOScaleY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOScaleZ:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOScaleZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOScaleZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DOLocalRotate:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DOLocalRotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DOLocalRotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodTransform.DORotate:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].gameObject.Value.transform.DORotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].gameObject.Value.transform.DORotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									break;
							}
								
							break;
							
						case Sequence.TweenObject.image:
						
							switch(tweenSequence[s].tweenMethodImage)
							{
								
								case Sequence.TweenMethodImage.DOColor:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].image.Value.DOColor(tweenSequence[s].targetColor.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].image.Value.DOColor(tweenSequence[s].targetColor.Value, tweenSequence[s].duration));
									}
									break;
								case Sequence.TweenMethodImage.DOFill:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].image.Value.DOFillAmount(tweenSequence[s].targetFloat.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].image.Value.DOFillAmount(tweenSequence[s].targetFloat.Value, tweenSequence[s].duration));
									}
									break;
							}
						
							break;
						case Sequence.TweenObject.rectTransform:
						
							switch(tweenSequence[s].tweenMethodTransform)
							{
							case Sequence.TweenMethodTransform.DOAnchoredPosition:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOAnchorPos(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOAnchorPos(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								
								break;
							case Sequence.TweenMethodTransform.DOLocalMove:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].rectTransform.Value.DOLocalMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOLocalMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
									}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveX:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOLocalMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOLocalMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveY:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOLocalMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOLocalMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalMoveZ:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOLocalMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOLocalMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMove:	
							
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOMove(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								
								break;
							case Sequence.TweenMethodTransform.DOMoveX:
								
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOMoveX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMoveY:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOMoveY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOMoveZ:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOMoveZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOScale:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOScale(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOScale(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOScaleX:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOScaleX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOScaleX(tweenSequence[s].target.Value.x, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOScaleY:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOScaleY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								else	
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOScaleY(tweenSequence[s].target.Value.y, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOScaleZ:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOScaleZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOScaleZ(tweenSequence[s].target.Value.z, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOLocalRotate:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOLocalRotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOLocalRotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DORotate:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DORotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DORotate(tweenSequence[s].target.Value, tweenSequence[s].duration));
								}
								break;
							case Sequence.TweenMethodTransform.DOSizeDelta:
								if (tweenSequence[s].tweenState == Sequence.TweenState.append)
								{
									_seq.Append(tweenSequence[s].rectTransform.Value.DOSizeDelta(tweenSequence[s].targetVector2.Value, tweenSequence[s].duration));
								}
								else
								{
									_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].rectTransform.Value.DOSizeDelta(tweenSequence[s].targetVector2.Value, tweenSequence[s].duration));
								}
								break;
							}
							
							break;
						case Sequence.TweenObject.canvasGroup:
						
							switch(tweenSequence[s].tweenMethodCanvas)
							{
								case Sequence.TweenMethodCanvas.DOFade:
									if (tweenSequence[s].tweenState == Sequence.TweenState.append)
									{
										_seq.Append(tweenSequence[s].canvasGroup.Value.DOFade(tweenSequence[s].targetFloat.Value, tweenSequence[s].duration));
									}
									else
									{
										_seq.Insert(tweenSequence[s].insertTime, tweenSequence[s].canvasGroup.Value.DOFade(tweenSequence[s].targetFloat.Value, tweenSequence[s].duration));
									}
									break;
							}
							
							break;
					}
					
					_seq.SetEase(tweenSequence[s].easeType);
				}
				
				_seq.OnComplete(() => OnComplete());
				_seq.SetId(tweenID);
				_seq.Play();

			ExecuteNext(0, _flowReactor);
		    	
		}
		
		void OnComplete()
		{
			if (!useOnTweenCompleteEvent)
				return;
			
			if (eventBoard != null)
			{
				if (eventBoard.events.ContainsKey(Guid.Parse(selectedEventID)))
				{
					eventBoard.events[Guid.Parse(selectedEventID)].Raise(null);
				}
			}
			else
			{
				Debug.LogError("EventBoard not existent");
			}
		}
		
		public override void AssignNewGraphInstance(Graph _rootGraph, Graph _graphOwner)
		{
			for (int t = 0; t < tweenSequence.Count; t ++)
			{
				tweenSequence[t].floatValue.graph = _graphOwner;
				tweenSequence[t].intValue.graph = _graphOwner;
				tweenSequence[t].image.graph = _graphOwner;
				tweenSequence[t].rectTransform.graph = _graphOwner;
				tweenSequence[t].gameObject.graph = _graphOwner;
				tweenSequence[t].canvasGroup.graph = _graphOwner;
				
				tweenSequence[t].target.graph = _graphOwner;
				tweenSequence[t].targetColor.graph = _graphOwner;
				tweenSequence[t].targetFloat.graph = _graphOwner;
				tweenSequence[t].targetInt.graph = _graphOwner;
				tweenSequence[t].targetVector2.graph = _graphOwner;
			}
		}
	}
}
#endif