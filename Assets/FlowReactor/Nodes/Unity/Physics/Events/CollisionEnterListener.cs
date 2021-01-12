using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	public class CollisionEnterListener : MonoBehaviour
	{
		public List<OnCollisionEnter> collisionTriggers = new List<OnCollisionEnter>();
		
		public void OnCollisionEnter(Collision _col)
		{
			for (int i = 0; i < collisionTriggers.Count; i ++)
			{
				collisionTriggers[i].Collision(_col);
			}
		}
	}
}
