#if FLOWREACTOR_DATABOX
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Databox;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Nodes.Databox
{
	[NodeAttributes( "Databox/Events" , "Called after databox object has been saved" , "eventNodeColor" , 1 , NodeAttributes.NodeType.Event )]
	public class OnDataboxSaved : Node
	{
		public DataboxObject databoxObject;
		
	
		FlowReactorComponent flowReactor;
		
	
		#if UNITY_EDITOR
		// Editor node initialization
		public override void Init(Graph _graph, Node _node)
		{
			base.Init(_graph, _node);
			// Load custom icon
			icon = EditorHelpers.LoadIcon("databoxIcon.png");
	
			// possibility to hide the default node inspector. Set to false normally.
			disableDefaultInspector = true;
			disableVariableInspector = true;
			
			nodeRect = new Rect(nodeRect.x, nodeRect.y, 150, 60);
		}
		
		// Draw default node window
		public override void DrawGUI(string _title, int _id, Graph _graph, GUISkin _editorSkin)
		{		
			base.DrawGUI(nodeData.title, _id, _graph, _editorSkin);
		}
	
		public override void DrawCustomInspector()
		{
			GUILayout.Label("Databox Object:");
			databoxObject = (DataboxObject)EditorGUILayout.ObjectField(databoxObject, typeof(DataboxObject), false);
		}
		#endif
		
		public override void OnInitialize(FlowReactorComponent _flowReactor)
		{
			flowReactor = _flowReactor;
			databoxObject.OnDatabaseSaved += DatabaseSaved;
		}
		
		public override	void OnNodeDisable(FlowReactorComponent _flowReactor)
		{
			if (databoxObject != null)
			{
				databoxObject.OnDatabaseSaved -= DatabaseSaved;
			}
		}
		
		void DatabaseSaved()
		{
			ExecuteNext(0, flowReactor);
		}
	
	}
}
#endif