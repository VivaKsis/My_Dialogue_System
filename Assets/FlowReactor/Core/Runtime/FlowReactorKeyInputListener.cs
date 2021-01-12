//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	Singleton class which listens to all key inputs and send the event back to the registered OnKeyInput nodes
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor.Nodes;
using FlowReactor.Nodes.Unity;
using FlowReactor.Utils;

namespace FlowReactor
{
	public class FlowReactorKeyInputListener : Singleton<FlowReactorKeyInputListener>
	{
		
		public List<OnKeyInput> onKeyPressedInputs = new List<OnKeyInput>();
		public List<OnKeyInput> onKeyDownInputs = new List<OnKeyInput>();
		public List<OnKeyInput> onKeyUpInputs = new List<OnKeyInput>();
		
		public List<OnMouseInput> onMouseInput = new List<OnMouseInput>();
		public List<OnMouseInput> onMouseDownInput = new List<OnMouseInput>();
		public List<OnMouseInput> onMouseUpInput = new List<OnMouseInput>();
		
		KeyCode[] keys;
		
		public void RegisterKeyPressed(OnKeyInput _node)
		{
			if (onKeyPressedInputs == null)
			{
				onKeyPressedInputs = new List<OnKeyInput>();
			}
			
			onKeyPressedInputs.Add(_node);
		}
		
		public void RegisterKeyDown(OnKeyInput _node)
		{
			if (onKeyDownInputs == null)
			{
				onKeyDownInputs = new List<OnKeyInput>();
			}
			
			onKeyDownInputs.Add(_node);
		}
		
		public void RegisterKeyUp(OnKeyInput _node)
		{
			if (onKeyUpInputs == null)
			{
				onKeyUpInputs = new List<OnKeyInput>();
			}
			
			onKeyUpInputs.Add(_node);
		}
		
		public void RegisterMousePressed(OnMouseInput _node)
		{
			if (onMouseInput == null)
			{
				onMouseInput = new List<OnMouseInput>();
			}
			
			onMouseInput.Add(_node);
		}
		
		public void RegisterMouseDown(OnMouseInput _node)
		{
			if (onMouseDownInput == null)
			{
				onMouseDownInput = new List<OnMouseInput>();
			}
			
			onMouseDownInput.Add(_node);
		}
		
		public void RegisterMouseUp(OnMouseInput _node)
		{
			if (onMouseUpInput == null)
			{
				onMouseUpInput = new List<OnMouseInput>();
			}
			
			onMouseUpInput.Add(_node);
		}
		
		void Start()
		{
			keys = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
		}
		
	    void Update()
	    {
		    for(int k = 0; k < keys.Length; k ++)
		    {
			    if(Input.GetKey(keys[k]))
			    {
			    	for (int p = 0; p < onKeyPressedInputs.Count; p ++)
			    	{
			    		onKeyPressedInputs[p].OnKey(keys[k]);
			    	}
			    }
			    
			    if (Input.GetKeyDown(keys[k]))
			    {
			    	for (int d = 0; d < onKeyDownInputs.Count; d ++)
			    	{
			    		onKeyDownInputs[d].OnKey(keys[k]);
			    	}
			    }
			    
			    if (Input.GetKeyUp(keys[k]))
			    {
			    	for (int u = 0; u < onKeyUpInputs.Count; u ++)
			    	{
				    	onKeyUpInputs[u].OnKey(keys[k]);
			    	}
			    }
		    }
		    
		    if (Input.GetMouseButton(0))
		    {
		    	for (int p = 0; p < onMouseInput.Count; p ++)
		    	{
		    		onMouseInput[p].OnMouse(0);
		    	}
		    }
		    if (Input.GetMouseButton(1))
		    {
			    for (int p = 0; p < onMouseInput.Count; p ++)
		    	{
		    		onMouseInput[p].OnMouse(1);
		    	}
		    }
		    if (Input.GetMouseButton(2))
		    {
			    for (int p = 0; p < onMouseInput.Count; p ++)
		    	{
		    		onMouseInput[p].OnMouse(2);
		    	}
		    }
		    if (Input.GetMouseButtonDown(0))
		    {
			    for (int p = 0; p < onMouseDownInput.Count; p ++)
		    	{
		    		onMouseDownInput[p].OnMouse(0);
		    	}
		    }
		    if (Input.GetMouseButtonDown(1))
		    {
			    for (int p = 0; p < onMouseDownInput.Count; p ++)
		    	{
		    		onMouseDownInput[p].OnMouse(1);
		    	}
		    }
		    if (Input.GetMouseButtonDown(2))
		    {
			    for (int p = 0; p < onMouseDownInput.Count; p ++)
		    	{
		    		onMouseDownInput[p].OnMouse(2);
		    	}
		    }
		    if (Input.GetMouseButtonUp(0))
		    {
			    for (int p = 0; p < onMouseUpInput.Count; p ++)
		    	{
		    		onMouseUpInput[p].OnMouse(0);
		    	}
		    }
		    if (Input.GetMouseButtonUp(1))
		    {
			    for (int p = 0; p < onMouseUpInput.Count; p ++)
		    	{
		    		onMouseUpInput[p].OnMouse(1);
		    	}
		    }
		    if (Input.GetMouseButtonUp(2))
		    {
			    for (int p = 0; p < onMouseUpInput.Count; p ++)
		    	{
		    		onMouseUpInput[p].OnMouse(2);
		    	}
		    }
		    
		    
	    }
	}
}