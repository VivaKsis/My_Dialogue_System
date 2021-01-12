//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
/*
	FlowReactor singleton which runs an update loop on every registered FlowReactorController
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.Utils;

namespace FlowReactor
{
	public class FlowReactorGlobalUpdate : Singleton<FlowReactorGlobalUpdate>
	{
		public List<FlowReactorComponent> components = new List<FlowReactorComponent>();
	
	
		public void Register(FlowReactorComponent _fr)
		{
			components.Add(_fr);
		}
		
	    // Update is called once per frame
	    void Update()
	    {
		    for (int i = 0; i < components.Count; i ++)
		    {
		    	components[i].GlobalUpdate();
		    }
	    }
	    
		void LateUpdate()
		{
			for (int i = 0; i < components.Count; i ++)
			{
				components[i].GlobalLateUpdate();
			}
		}
		
		void FixedUpdate()
		{
			for (int i = 0; i < components.Count; i ++)
			{
				components[i].GlobalFixedUpdate();
			}
		}
	}
}