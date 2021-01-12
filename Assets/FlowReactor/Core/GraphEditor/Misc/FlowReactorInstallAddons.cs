//---------------------------------------------------------------------------------
//	FLOWREACTOR
//  (c) Copyright doorfortyfour OG, 2020. All rights reserved.
//---------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
  
/// <summary>
/// Add the custom flowreactor databox define symbol to the player settings
/// </summary>
 namespace FlowReactor.Editor
 {
	 public class FlowReactorInstallAddons : UnityEditor.Editor
	 {
	  
		 /// <summary>
		 /// Symbols that will be added to the editor
		 /// </summary>
		 public static readonly string [] Symbols0 = new string[] {
			 "FLOWREACTOR_DEBUG"
		 };
		 
		 public static readonly string [] Symbols1 = new string[] {
			 "FLOWREACTOR_DATABOX"
		 };
		 
		 public static readonly string [] Symbols2 = new string[] {
			 "FLOWREACTOR_DOTWEEN"
		 };
		 
	  
		 /// <summary>
		 /// Add databox define symbols.
		 /// </summary>
		 //[MenuItem("Tools/FlowReactor/Addons/Enable DEBUG MODE")]
		 public static void EnableDebugMode ()
		 {
		 	if (EditorUtility.DisplayDialog("Enable DEBUG MODE?", "Do you want to enable DEBUG MODE?", "Yes", "No"))
		 	{
				 string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( EditorUserBuildSettings.selectedBuildTargetGroup );
				 List<string> allDefines = definesString.Split ( ';' ).ToList ();
				 allDefines.AddRange ( Symbols0.Except ( allDefines ) );
				 PlayerSettings.SetScriptingDefineSymbolsForGroup (
					 EditorUserBuildSettings.selectedBuildTargetGroup,
					 string.Join ( ";", allDefines.ToArray () ) );
		 	}
		 }
		 
		 [MenuItem("Tools/FlowReactor/Addons/Enable Databox addon")]
		 public static void InstallDatabox ()
		 {
		 	if (EditorUtility.DisplayDialog("Enable Databox addon", "Do you want to install/enable the Databox addon for FlowReactor? Please make sure Databox is available in your project.", "Yes", "No"))
		 	{
				 string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( EditorUserBuildSettings.selectedBuildTargetGroup );
				 List<string> allDefines = definesString.Split ( ';' ).ToList ();
				 allDefines.AddRange ( Symbols1.Except ( allDefines ) );
				 PlayerSettings.SetScriptingDefineSymbolsForGroup (
					 EditorUserBuildSettings.selectedBuildTargetGroup,
					 string.Join ( ";", allDefines.ToArray () ) );
		 	}
		 }
	  
		 [MenuItem("Tools/FlowReactor/Addons/Enable DOTween addon")]
		 public static void InstallDOTween ()
		 {
		 	if (EditorUtility.DisplayDialog("Enable DOTween addon", "Do you want to install/enable the DOTween addon for FlowReactor? Please make sure DOTween is available in your project.", "Yes", "No"))
		 	{
				 string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( EditorUserBuildSettings.selectedBuildTargetGroup );
				 List<string> allDefines = definesString.Split ( ';' ).ToList ();
				 allDefines.AddRange ( Symbols2.Except ( allDefines ) );
				 PlayerSettings.SetScriptingDefineSymbolsForGroup (
					 EditorUserBuildSettings.selectedBuildTargetGroup,
					 string.Join ( ";", allDefines.ToArray () ) );
		 	}
		 }
	 }
 }
 #endif