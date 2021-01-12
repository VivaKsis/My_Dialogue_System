#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FlowReactor.Editor
{
	public class FlowReactorPostProcess : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			var _settings = FREditorSettings.GetOrCreateSettings();
			_settings.errorReport = "";
			
			//foreach (string str in importedAssets)
			//{
			//	Debug.Log("Reimported Asset: " + str);
			//}
			
			foreach (string str in deletedAssets)
			{
				_settings.errorReport += "deleted file: " + str + "\n";
			}
	
			for (int i = 0; i < movedAssets.Length; i++)
			{
				_settings.errorReport += "old file: " + movedFromAssetPaths[i] + " new file: " + movedAssets[i] + "\n";
			}
		}
	}
}
#endif
