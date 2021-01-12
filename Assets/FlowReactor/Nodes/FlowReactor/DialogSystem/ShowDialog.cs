using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
namespace FlowReactor.Nodes
{
	
	[NodeAttributes( "FlowReactor/DialogSystem" , "The FlowReactor dialog node. See the dialog example scene for more information." , "#ffef94" , 1)]
	public class ShowDialog : Node
	{
		
		public FRString dialogText;
		public FRGameObject uiText;
		public FRGameObject uiButton;
		
		[SceneObjectOnly]
		public FRGameObject uiButtonContainer;
		
		[Hide]
		public FRBoolean doNotWaitForUser;
		
		[SerializeField]
		List<string> options = new List<string>();
		
		FlowReactorComponent flowReactor;
		
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("dialogIcon.png");
			
			disableDefaultInspector = true;
			disableVariableInspector = false;
			disableDrawCustomInspector = false;
			
			options.Add("");
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 250, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		
		public override void DrawCustomInspector()
		{
			
			nodeData.description = dialogText.Value;
			
			
			doNotWaitForUser.Value = GUILayout.Toggle(doNotWaitForUser.Value, "Don't wait for user");
			
			
			if (!doNotWaitForUser.Value)
			{
				if (GUILayout.Button("Add Option"))
				{
					AddNewOption();
				}
				
				for (int i = 0; i < options.Count; i ++)
				{
					if (i >= outputNodes.Count)
						break;
					outputNodes[i].id = options[i];
					
					using (new GUILayout.HorizontalScope(editorSkin.GetStyle("Box")))
					{
						options[i] = GUILayout.TextField(options[i]);
						
						if (i > 0)
						{
							if (GUILayout.Button("x", GUILayout.Width(20)))
							{
								RemoveOption(i);
							}
						}
					}
					
					
				}
			}
			else
			{
				if (outputNodes.Count == 0)
				{
					AddNewOption();
				}
			}
		}
	
	
		void AddNewOption()
		{
			options.Add("");
			AddOutput("");
		}
		
		void RemoveOption(int _index)
		{
			if (options.Count - 1 > 0)	
			{			
				options.RemoveAt(_index);
				RemoveOutput(_index);
			}
		}
		
		#endif
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			
			////////////////////////
			
			
			// Show Dialog text
			uiText.Value.GetComponent<Text>().text = dialogText.Value;
			
			// Destroy all old buttons in button container
			var _oldButtons = uiButtonContainer.Value.GetComponentsInChildren<Button>();
			for (int b = 0; b < _oldButtons.Length; b ++)
			{
				Destroy(_oldButtons[b].gameObject);
			}
		
			if (!doNotWaitForUser.Value)
			{
				// build button options
				for (int i = 0; i < options.Count; i ++)
				{
					
					var _b = Instantiate(uiButton.Value);
					_b.transform.SetParent(uiButtonContainer.Value.transform, false);
					
					_b.transform.GetComponentInChildren<Text>().text = options[i];
					
					var _index = i;
					
					_b.transform.GetComponent<Button>().onClick.AddListener(() => { ExecuteOption(_index);});
					
				}
			}
			else
			{
				ExecuteNext(0, flowReactor);
			}
			
			flowReactor = _flowReactor;
			
			
		}
		
		public void ExecuteOption(int _option)
		{
			ExecuteNext(_option, flowReactor);
		}
	}

}