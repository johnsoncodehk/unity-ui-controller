using UnityEngine;
using UnityEditor;

namespace JohnsonCodeHK.OverrideControllerToolsEditor {

	public enum AnimationsHideFlags {
		None,
		HideInHierarchy,
	}

	public class OverrideControllerToolsSetting : ScriptableObject {

		public static OverrideControllerToolsSetting instance {
			get {
				var settings = AssetDatabase.FindAssets("t:" + typeof(OverrideControllerToolsSetting));
				if (settings.Length == 0) {
					return null;
				}
				return AssetDatabase.LoadAssetAtPath<OverrideControllerToolsSetting>(AssetDatabase.GUIDToAssetPath(settings[0]));
			}
		}

		[System.Serializable]
		public struct StringReplace {
			public string oldValue, newValue;
		}

		public RuntimeAnimatorController[] quickSetupControllers = new RuntimeAnimatorController[0];
		public StringReplace[] animationNameReplaces = new StringReplace[0];

#if UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4
		public HideFlags animationsHideFlags {
			get {
				return HideFlags.HideInHierarchy;
			}
		}
#else
		public AnimationsHideFlags defaultAnimationsHideFlags = AnimationsHideFlags.None;
		public HideFlags animationsHideFlags {
			get {
				return this.defaultAnimationsHideFlags == AnimationsHideFlags.HideInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;
			}
		}
#endif
	}
}
