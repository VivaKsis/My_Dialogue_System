using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	public class TriggerExitListener : MonoBehaviour
	{
		public List<OnTriggerExit> collisionTriggers = new List<OnTriggerExit>();
		
		public void OnTriggerExit(Collider _col)
		{
			for (int i = 0; i < collisionTriggers.Count; i ++)
			{
				collisionTriggers[i].Trigger(_col);
			}
		}
	}
}
