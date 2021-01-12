using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowReactor.Nodes.Unity
{
	public class CollisionExitListener : MonoBehaviour
	{
		public List<OnCollisionExit> collisionTriggers = new List<OnCollisionExit>();
		
		public void OnCollisionExit(Collision _col)
		{
			for (int i = 0; i < collisionTriggers.Count; i ++)
			{
				collisionTriggers[i].Collision(_col);
			}
		}
	}
}
