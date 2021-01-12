using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	public class TriggerEnterListener : MonoBehaviour
	{
		public List<OnTriggerEnter> collisionTriggers = new List<OnTriggerEnter>();
		
		public void OnTriggerEnter(Collider _col)
		{
			for (int i = 0; i < collisionTriggers.Count; i ++)
			{
				collisionTriggers[i].Trigger(_col);
			}
		}
	}
}
