using UnityEngine;
using UnityEditor;

namespace OverrideControllerToolsEditor {

	public enum AnimationsHideFlags {
		None,
		HideInHierarchy,
	}

	public class OverrideControllerToolsSettings : ScriptableObject {

		public static OverrideControllerToolsSettings instance {
			get {
				var settings = AssetDatabase.FindAssets("t:" + typeof(OverrideControllerToolsSettings));
				if (settings.Length == 0) {
					return null;
				}
				return AssetDatabase.LoadAssetAtPath<OverrideControllerToolsSettings>(AssetDatabase.GUIDToAssetPath(settings[0]));
			}
		}

		[System.Serializable]
		public struct StringReplace {
			public string oldValue, newValue;
		}

		public RuntimeAnimatorController[] quickSetupControllers = new RuntimeAnimatorController[0];
		public StringReplace[] animationNameReplaces = new StringReplace[0];
		public StringReplace[] quickSetupControllerNameReplaces = new StringReplace[0];
		public AnimationsHideFlags overrideAnimationHideFlags = AnimationsHideFlags.None;
		public HideFlags animationHideFlags {
			get {
				return this.overrideAnimationHideFlags == AnimationsHideFlags.HideInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;
			}
		}
	}
}
