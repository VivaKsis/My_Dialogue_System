using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowReactor;

namespace FlowReactor.Nodes.Unity
{
	[NodeAttributes("Unity/Physics/Events", "On Trigger Exit", "eventNodeColor", 1, NodeAttributes.NodeType.Event)]
	public class OnTriggerExit : Node
	{
		[Title("Trigger listener")]
		public FRGameObject listener;
		
		[Title("Compare with")]
		public FRBoolean compareWithLayer = new FRBoolean(true);
		public FRLayerMask layerMask;
		public FRBoolean compareWithTag;
		public FRString tag;
		
		[Title("Store collider to:")]
		public FRGameObject collider;
		
		FlowReactorComponent flowReactor;
		
		#if UNITY_EDITOR
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			icon = EditorHelpers.LoadIcon("eventIcon.png");	
	
			// If hide input is true
			// no other node can be connected to this node
			hideInput = true;
			
			disableDefaultInspector = true;
			disableDrawCustomInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;
			
			if (listener.Value != null)
			{
				var _go = listener.Value;
					
				if (_go != null)
				{
					var _component = _go.GetComponent<TriggerExitListener>();
					if (_component != null)
					{
						_component.collisionTriggers.Add(this);
					}
					else
					{
						_go.AddComponent<TriggerExitListener>().collisionTriggers.Add(this);
					}
				}
			}
		}
		
		public void Trigger(Collider _collider)
		{
			bool _valid = false;
			if (graphOwner.isActive)
			{
				if (compareWithLayer.Value && !compareWithTag.Value)
				{
					if(((1<<_collider.gameObject.layer) & layerMask.Value) != 0 )
					{
						_valid = true;
					}
				}
				if (!compareWithLayer.Value && compareWithTag.Value)
				{
					if (_collider.gameObject.tag == tag.Value)
					{
						_valid = true;
					}
				}
				if (compareWithLayer.Value && compareWithTag.Value)
				{
					if(((1<<_collider.gameObject.layer) & layerMask.Value) != 0 && _collider.gameObject.tag == tag.Value)
					{
						_valid = true;
					}
				}
				if (!compareWithLayer.Value && !compareWithTag.Value)
				{
					_valid = true;
				}
			}
			
			
			if(_valid)
			{
				collider.Value = _collider.gameObject;
				
				ExecuteNext(0, flowReactor);
			}
		}

		
	}
}