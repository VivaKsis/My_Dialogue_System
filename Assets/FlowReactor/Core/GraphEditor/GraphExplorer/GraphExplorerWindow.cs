//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlowReactor;
using FlowReactor.Nodes;

namespace FlowReactor.Editor
{
	public class GraphExplorerWindow : EditorWindow
	{

		GraphEditor editor;

		
		public void OpenExplorer(GraphExplorerWindow _window, Graph _rootGraph, FREditorSettings _settings, GUISkin _editorSkin, GraphEditor _editor)
		{
			_editor.graphExplorer = new GraphExplorer(_rootGraph, _settings, _editorSkin, _editor);
			editor = _editor;
			_window.Show();
		}
		
		public void OnGUI()
		{
			editor.graphExplorer.DrawExplorer();
			
			Repaint();
		}
	}
}
#endif