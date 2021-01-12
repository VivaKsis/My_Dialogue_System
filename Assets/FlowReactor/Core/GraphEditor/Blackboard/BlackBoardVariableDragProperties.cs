#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FlowReactor.Editor
{
	public static class BlackBoardVariableDragProperties
	{
		public static bool isDragging;
		public static FRVariable variable;
		public static EventType eventType;
		public static Vector2 mousePosition;
		public static Vector2 startDragPosition;
		public static Guid blackboardGuid;
		public static Guid blackboardVariableGuid;
		public static BlackBoardEditor editor;
	}
}
#endif