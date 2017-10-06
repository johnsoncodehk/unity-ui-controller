using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace JohnsonCodeHK.UIControllerEditor {

	public class UIControllerSettings : ScriptableObject {

		public static UIControllerSettings instance {
			get {
				var settings = AssetDatabase.FindAssets("t:" + typeof(UIControllerSettings));
				if (settings.Length == 0) {
					return null;
				}
				return AssetDatabase.LoadAssetAtPath<UIControllerSettings>(AssetDatabase.GUIDToAssetPath(settings[0]));
			}
		}

		[System.Serializable]
		public struct Transition {
			[Range(0, 1)] public float exitTime, duration;
			public bool canTransitionToSelf;
		}

		public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
		public Transition transition;
	}
}
