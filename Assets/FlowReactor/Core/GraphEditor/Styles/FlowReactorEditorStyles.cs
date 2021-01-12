//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FlowReactor.Editor
{
	public static class FlowReactorEditorStyles
	{
		// FLOWREACTOR EDITOR GUI STYLES
		
	
		// Node description text style
		///////////////////////////////
		public static GUIStyle nodeDescriptionStyle = new GUIStyle()
		{
			wordWrap = true,
			normal = new GUIStyleState
			{
				textColor = Color.grey
			}
		};
		
		// Node output text style
		public static GUIStyle nodeOutputTextStyle = new GUIStyle("Label")
		{
			alignment = TextAnchor.MiddleRight
		};
		
		// Reorderable dictionary styles
		///////////////////////////////
		static GUIStyle dictionaryListElementBackground	= new GUIStyle("RL Element");
			
		public static GUIStyle dictionaryElement = new GUIStyle("Button")
		{
			hover = dictionaryListElementBackground.onHover,
			active = dictionaryListElementBackground.onActive,
				
			normal = new GUIStyleState
			{
			background = dictionaryListElementBackground.onNormal.background,
			textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
				
			alignment = TextAnchor.MiddleLeft,
			//overflow = new RectOffset (0, 0, 0, 2),
			padding = new RectOffset(20, 0, 0, 0)
		};
			
		public static GUIStyle boardElement = new GUIStyle("Button")
		{
			hover = dictionaryListElementBackground.onHover,
			active = dictionaryListElementBackground.onActive,
				
			normal = new GUIStyleState
			{
			background = dictionaryListElementBackground.onNormal.background,
			textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
				
			fontStyle = FontStyle.Bold,
				
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(3, 3, 3, 3),
			contentOffset = new Vector2(0, 0),
		};
			
		public static GUIStyle dictionaryElementDragHandle = new GUIStyle("RL DragHandle");
		///////////////////////////////
		
		
		// Node panel styles
		///////////////////////////////
		public static GUIStyle rightArrow = "ArrowNavigationRight";
		public static GUIStyle leftArrow = "ArrowNavigationLeft";
		public static GUIStyle elementBackground ="DD ItemStyle";
		public static GUIStyle normalButton = "Button";
		
		public static GUIStyle elementButton = new GUIStyle(elementBackground)
		{
					
			//onNormal = EditorStyles.textArea.normal,
			//normal = EditorStyles.textArea.onNormal,
			//hover = elementBackground.onHover,
			//hover = elementBackground.onNormal,
			active = elementBackground.onActive,
			//onFocused =  elementBackground.onHover,
					
			normal = new GUIStyleState 
			{ 
			//background = EditorStyles.textArea.normal.background, //elementBackground.onNormal.background,
			background = normalButton.onHover.background, //elementBackground.onNormal.background,
			textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
			
			hover = new GUIStyleState
			{
			background = elementBackground.onHover.background,
				textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
					
			alignment = TextAnchor.MiddleLeft,
										
			//overflow = new RectOffset(25, 131, 1, 3),
			padding = new RectOffset(10, 0, 0, 0),
			margin = new RectOffset(-10, -10, 0, 0),
			fixedWidth = 250,
			fixedHeight = 30
		};
				
		public static GUIStyle elementButtonBold = new GUIStyle("Button")
		{
				
			hover = elementBackground.onHover,
			active = elementBackground.onActive,
									
			normal = new GUIStyleState 
			{ 
			background = elementBackground.onNormal.background,
			textColor = Color.white,		
			},
					
			fontStyle = FontStyle.Bold,
			alignment = TextAnchor.MiddleLeft,
										
			overflow = new RectOffset(25, 131, 1, 3),
			padding = new RectOffset(10, 0, 0, 0)
		};
					
					
		public static GUIStyle elementButtonBack = new GUIStyle("Button")
		{
					
			hover = elementBackground.onHover,
			active = elementBackground.onActive,
									
			normal = new GUIStyleState 
			{ 
			background = elementBackground.onNormal.background,
			textColor = Color.white,	
			},
					
			fontStyle = FontStyle.Bold,
					
			alignment = TextAnchor.MiddleCenter,
										
			overflow = new RectOffset(25, 131, 1, 3),
			padding = new RectOffset(10, 0, 0, 0)
		};
		///////////////////////////////
		
		// Overflow button style
		public static GUIStyle overflowButton = new GUIStyle("Button")
		{
			padding = new RectOffset(20, 0, 0, 0)
		};
		
		////////////////////////////////
		
		// Graph Explorer element
		public static GUIStyle graphExplorerButton = new GUIStyle(elementBackground)
		{
					
			//onNormal = EditorStyles.textArea.normal,
			//normal = EditorStyles.textArea.onNormal,
			//hover = elementBackground.onHover,
			//hover = elementBackground.onNormal,
			active = elementBackground.onActive,
			//onFocused =  elementBackground.onHover,
					
			normal = new GUIStyleState 
			{ 
			//background = EditorStyles.textArea.normal.background, //elementBackground.onNormal.background,
			background = normalButton.onHover.background, //elementBackground.onNormal.background,
			textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
			
			hover = new GUIStyleState
			{
			//background = elementBackground.onHover.background,
			background = elementBackground.onNormal.background,
			textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
			},
					
			alignment = TextAnchor.MiddleLeft,
										
			overflow = new RectOffset(0, -5, -3, 0),
			padding = new RectOffset(0, 0, 0, 0),
			margin = new RectOffset(0, 0, 0, 0),
			fixedHeight = 20,
		};
		
	}
}
#endif