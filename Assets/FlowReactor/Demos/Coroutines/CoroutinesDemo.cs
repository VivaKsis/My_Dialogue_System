using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.BlackboardSystem;

public class CoroutinesDemo : MonoBehaviour
{
	
	public BlackBoard blackboard;
	
	float weight1 = 0.5f;
	float weight2 = 0.5f; 
	float weight3 = 0.5f;
	
	
	void OnGUI()
	{
		using (new GUILayout.VerticalScope("Box"))
		{

			GUILayout.Label("Press space key to move sphere to random box position");
		
		
			GUILayout.Label("random weight 1: " + weight1.ToString());
			weight1 = GUILayout.HorizontalSlider(weight1, 0f, 1f);
			
			GUILayout.Label("random weight 2: " + weight2.ToString());
			weight2 = GUILayout.HorizontalSlider(weight2, 0f, 1f);
			
			GUILayout.Label("random weight 3: " + weight3.ToString());
			weight3 = GUILayout.HorizontalSlider(weight3, 0f, 1f);
	
			if (GUILayout.Button("assign weights"))
			{
				var _w1 = blackboard.GetVariableByName<FRFloat>("Weight1");
				var _w2 = blackboard.GetVariableByName<FRFloat>("Weight2");
				var _w3 = blackboard.GetVariableByName<FRFloat>("Weight3");
				
				_w1.Value = weight1;
				_w2.Value = weight2;
				_w3.Value = weight3; 
			}
		}
	}
}
