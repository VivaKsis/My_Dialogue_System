using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.Editor;

namespace FlowReactor.Nodes
{
	[NodeAttributes( "FlowReactor/Flow" , "Can run two or more flows sequential, parallel or random." , "flowNodeColor" , new string[]{"", ""} , NodeAttributes.NodeType.Normal )]
	public class Branch : Node
	{
		
		public enum RunMode
		{
			Sequential,
			Parallel,
			Select,
			Random,
			RandomWeightBased
		}
		
		public RunMode runMode;
		public RunMode lastRunMode;
		
		[HideInNode]
		public FRInt selectedOutput;
		[HideInNode]
		public List<FRFloat> weights = new List<FRFloat>();
	
		int currentOutput = 0;
		
		public Texture2D flowIconSequential;
		public Texture2D flowIconParallel;
		public Texture2D flowIconSelect;
		public Texture2D flowIconRandom;
		public Texture2D flowIconRandomWB;
		
		public Texture2D currentFlowIcon;
		
		#pragma warning disable 0414
		string sequentialHelpText = "Sequential mode runs one flow output after another.";
		string parallelHelpText = "Parallel mode runs all flow outputs simultaneously.";
		string selectHelpText = "Select mode runs only one specific flow output specified by the output index.";
		string randomHelpText = "Random mode runs only one randomly chosen flow output.";
		string randomWeightBasedHelpText = "Random weight based runs only one randomly chosen flow output influenced by their weight value.";
		#pragma warning restore 0414
		
		Color selectedOutputColor = new Color(125f/255f, 190f/255f, 160f/255, 255f/255f);
		
		// Editor node methods
		#if UNITY_EDITOR
		// Node initialization called upon node creation
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			
			flowIconRandom = EditorHelpers.LoadIcon("flowRandomIcon.png");
			flowIconRandomWB = EditorHelpers.LoadIcon("flowRandomWBIcon.png");
			flowIconParallel = EditorHelpers.LoadIcon("flowParallelIcon.png");
			flowIconSelect = EditorHelpers.LoadIcon("flowSelectIcon.png");
			flowIconSequential = EditorHelpers.LoadIcon("flowSequentialIcon.png");
			currentFlowIcon = flowIconSequential;
			
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			disableDrawCustomInspector = false;
			
			weights = new List<FRFloat>();
			weights.Add(new FRFloat());
			weights.Add(new FRFloat());

			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
			
			if (currentFlowIcon != null)
			{
				GUI.Label(new Rect(nodeRect.x + (nodeRect.width / 2) - (currentFlowIcon.width / 2), nodeRect.y + (nodeRect.height / 2) - (currentFlowIcon.height / 2), currentFlowIcon.width, currentFlowIcon.height), currentFlowIcon);
			}
		}
	
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Run mode:");
			runMode = (RunMode) EditorGUILayout.EnumPopup(runMode);
			
			switch (runMode)
			{
				case RunMode.Sequential:
					currentFlowIcon = flowIconSequential;
					
					EditorGUILayout.HelpBox(sequentialHelpText, MessageType.Info);
					
					break;
				case RunMode.Parallel:
					currentFlowIcon = flowIconParallel;

					EditorGUILayout.HelpBox(parallelHelpText, MessageType.Info);
					
					break;
				case RunMode.Select:
					currentFlowIcon = flowIconSelect;

					EditorGUILayout.HelpBox(selectHelpText, MessageType.Info);
					
					GUILayout.Label("Selected output index");
					
					selectedOutput.Value = EditorGUILayout.IntSlider(selectedOutput.Value, 0, outputNodes.Count-1);
					FRVariableGUIUtility.DrawVariable(selectedOutput, this, false, editorSkin);
					
					for (int i = 0; i < outputNodes.Count; i ++)
					{
						if (selectedOutput.Value == i)
						{
							outputNodes[i].id = ">>";
						}
						else
						{
							outputNodes[i].id = "";
						}
					}
					
					break;
				case RunMode.Random:
					currentFlowIcon = flowIconRandom;
					EditorGUILayout.HelpBox(randomHelpText, MessageType.Info);
					break;
				case RunMode.RandomWeightBased:
					currentFlowIcon = flowIconRandomWB;				
					EditorGUILayout.HelpBox(randomWeightBasedHelpText, MessageType.Info);
					
					lastRunMode = runMode;
					// assign weight values to output ids
					for (int i = 0; i < outputNodes.Count; i ++)
					{
						outputNodes[i].id = weights[i].Value.ToString();
					}
					
					break;
			}
			
			// reset output ids
			if (lastRunMode != runMode &&( runMode != RunMode.RandomWeightBased || runMode != RunMode.Select ))
			{
				lastRunMode = runMode;
				for (int i = 0; i < outputNodes.Count; i ++)
				{
					outputNodes[i].id = "";
				}
			}
			
			GUILayout.Space(10);
			EditorHelpers.DrawUILine();
			
			if (GUILayout.Button("Add Output"))
			{
				AddOutput("");
				weights.Add(new FRFloat());
			}
			
			
			for (int i = 0; i < outputNodes.Count; i ++)
			{
				if (selectedOutput.Value == i && runMode == RunMode.Select)
				{
					GUI.color = selectedOutputColor;
				}
				else
				{
					GUI.color = Color.white;
				}
				
				using (new GUILayout.HorizontalScope("Box"))
				{
					GUILayout.Label("output: " + i.ToString());
					
					if (runMode == RunMode.RandomWeightBased)
					{
						weights[i].Value = EditorGUILayout.Slider(weights[i].Value, 0f, 1f);
						GUILayout.FlexibleSpace();
						FRVariableGUIUtility.DrawVariable(weights[i], this, false, editorSkin);
					
					}
					
					if (i > 1)
					{
						if (GUILayout.Button("x", GUILayout.Width(20)))
						{
							RemoveOutput(i);
							weights.RemoveAt(i);
						}
					}
				}
				
				GUI.color = Color.white;
			}
		}

		#endif


		// Similar to the Monobehaviour Awake. 
		// Gets called on initialization on every node in all graphs and subgraphs. 
		// (No matter if the sub-graph is currently active or not)
		public override void OnInitialize(FlowReactorComponent _flowRector)
		{
			currentOutput = -1;
			branchNode = null;
		}
		
		public void NextSequence(FlowReactorComponent _flowReactor)
		{
			if (runMode == RunMode.Sequential)
			{
				if (currentOutput + 1 < outputNodes.Count)
				{
					currentOutput ++;
					
					runParallel = false;

					ExecuteNextBranch(currentOutput, _flowReactor, this);
				}
				else
				{
					// this branch node is connected to another branch node
					// so we need to call the NextSequence method of it's connected branch node
					if(branchNode != this && branchNode != null)
					{
						branchNode.NextSequence(_flowReactor);
					}
				}
			}
			
		}
		
		// Execute coroutine
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			// Enter your code here!
			////////////////////////
			if (currentOutput == outputNodes.Count - 1)
			{
				currentOutput = -1;
			}
			
			switch(runMode)
			{
				case RunMode.Parallel:
					
					runParallel = true;
				
					for (int i = 0; i < outputNodes.Count; i ++)
					{
						ExecuteNext(i, _flowReactor);
					}
					
					break;
				case RunMode.Sequential:
					
					runParallel = false;
					
					currentOutput ++;
					
					ExecuteNextBranch(currentOutput, _flowReactor,  this);
					
					break;
				case RunMode.Select:
						
					runParallel = false;
					
					ExecuteNext(selectedOutput.Value, _flowReactor);
					break;
				case RunMode.Random:
					
					runParallel = false;
					
					var _rndOutput = Random.Range(0, outputNodes.Count);
					ExecuteNext(_rndOutput, _flowReactor);
					break;
				case RunMode.RandomWeightBased:
				
					runParallel = false;
					var _tmpWeights = weights.Select(x => x.Value).ToList();
					
					var _rndIndex = GetRandomWeightedIndex(_tmpWeights);
					if (_rndIndex == -1)
					{
						_rndIndex = 0;
					}
					ExecuteNext(_rndIndex, _flowReactor);
					break;
			}
		}
		
		
		public int GetRandomWeightedIndex(List<float> _weights)
		{
			if (_weights == null || _weights.Count == 0) return -1;
	 
			float w;
			float t = 0;
			int i;
			for (i = 0; i < _weights.Count; i++)
			{
				w = _weights[i];
	 
				if (float.IsPositiveInfinity(w))
				{
					return i;
				}
				else if (w >= 0f && !float.IsNaN(w))
				{
					t += _weights[i];
				}
			}
	 
			float r = Random.value;
			float s = 0f;
	 
			for (i = 0; i < _weights.Count; i++)
			{
				w = _weights[i];
				if (float.IsNaN(w) || w <= 0f) continue;
	 
				s += w / t;
				if (s >= r) return i;
			}
	 
			return -1;
		}
	}
}