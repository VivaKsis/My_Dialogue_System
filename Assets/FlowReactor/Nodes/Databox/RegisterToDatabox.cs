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
using FlowReactor.Editor;

namespace FlowReactor.Nodes.Databox
{
	[NodeAttributes( "Databox" , "Registers a data entry to another Databox object" , "actionNodeColor" , 1 , NodeAttributes.NodeType.Normal )]
	public class RegisterToDatabox : Node
	{
	
		public DataboxObject databoxObject;
		public FRString tableID;
		public FRString entryID;
		public FRString newEntryID;
		public DataboxObject databoxObjectToRegister;
		
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
			disableDrawCustomInspector = false;
			
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
			
			FRVariableGUIUtility.DrawVariable("Table ID:", tableID, this, false, editorSkin);
			FRVariableGUIUtility.DrawVariable("Entry ID:", entryID, this, false, editorSkin);
			
			GUILayout.Label("Register to:");
			databoxObjectToRegister = (DataboxObject)EditorGUILayout.ObjectField(databoxObjectToRegister, typeof(DataboxObject), false);
			FRVariableGUIUtility.DrawVariable("New entry ID:", newEntryID, this, false, editorSkin);
		}
		#endif
	
		
		// Execute node
		public override void OnExecute(FlowReactorComponent _flowReactor)
		{
			////////////////////////
			databoxObject.RegisterToDatabase(databoxObjectToRegister, tableID.Value, entryID.Value, newEntryID.Value);
			
			ExecuteNext(0, _flowReactor);
		}
	}
}
#endif