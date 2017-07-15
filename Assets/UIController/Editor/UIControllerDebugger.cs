using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace UIControllerEditor {

	[InitializeOnLoad]
	public class UIControllerDebugger {

		static UIControllerDebugger() {
			EditorApplication.update += OnUpdate;
		}
		private static void OnUpdate() {
			EditorApplication.update -= OnUpdate;

			UIControllerSetting setting = UIControllerSetting.instance;
			FixAllControllers(setting);
			FixAllOverrideControllers(setting);
		}

		public static void FixAllControllers(UIControllerSetting setting) {
			int fixCount = 0;
			foreach (var controller in setting.controllers) {
				AnimatorController animator = controller as AnimatorController;
				if (animator.GetBehaviours<UIControllerStateMachine>().Length == 0) {
					var baseLayer = animator.layers[0];
					baseLayer.stateMachine.AddStateMachineBehaviour<UIControllerStateMachine>();
					fixCount++;
				}
			}
			if (fixCount > 0) {
				UIControllerDebugger.LogWarning(1, fixCount + " Controllers is Fix.");
			}
		}
		public static void FixAllOverrideControllers(UIControllerSetting setting) {
			List<AnimatorOverrideController> overrideControllers = FindAllOverrideControllers(setting);
			UIControllerDebugger.FixAnimationsNotSet(overrideControllers);
			UIControllerDebugger.FixAnimationsHide(overrideControllers);
			UIControllerDebugger.FixControllerMainObject(overrideControllers);
		}
		public static List<AnimatorOverrideController> FindAllOverrideControllers(UIControllerSetting setting) {
			var ocs = AssetDatabase.FindAssets("t:" + typeof(AnimatorOverrideController).Name);
			List<AnimatorOverrideController> cons = new List<AnimatorOverrideController>();
			foreach (var oc in ocs) {
				var controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(AssetDatabase.GUIDToAssetPath(oc));
				if (setting.controllers.Contains(controller.runtimeAnimatorController)) {
					cons.Add(controller);
				}
			}
			return cons;
		}

		private static void FixAnimationsNotSet(List<AnimatorOverrideController> overrideControllers) {
			int fixCount = 0;

			foreach (var overrideController in overrideControllers) {
				bool isFix = false;
				var overrides = overrideController.GetOverridesUnite();
				foreach (var clipPair in overrides) {
					if (clipPair.Key == clipPair.Value) {
						AnimatorOverrideControllerInspector.SetupController(overrideController);
						isFix = true;
						break;
					}
				}
				fixCount += isFix ? 1 : 0;
			}

			if (fixCount > 0) {
				UIControllerDebugger.LogWarning(2, fixCount + " Override Controllers is Fix.");
			}
		}
		private static void FixAnimationsHide(List<AnimatorOverrideController> overrideControllers) {
			int fixCount = 0;

			foreach (var overrideController in overrideControllers) {
#if !(UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4)
				List<AnimationClip> clips = AnimatorOverrideControllerInspector.GetIncludeAnimations(overrideController);
				foreach (AnimationClip clip in clips) {
					if (clip.hideFlags == HideFlags.None) {
						continue;
					}
					clip.hideFlags = HideFlags.None;
					EditorUtility.SetDirty(clip);
					fixCount++;
				}
#endif
			}

			if (fixCount > 0) {
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				UIControllerDebugger.LogWarning(3, fixCount + " Override Controllers is Fix.");
			}
		}
		private static void FixControllerMainObject(List<AnimatorOverrideController> overrideControllers) {
			int fixCount = 0;

			foreach (var overrideController in overrideControllers) {
#if !(UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4)
				if (!AssetDatabase.IsMainAsset(overrideController)) {
					AssetDatabase.SetMainObject(overrideController, AssetDatabase.GetAssetPath(overrideController));
					EditorUtility.SetDirty(overrideController);
					fixCount++;
				}
#endif
			}

			if (fixCount > 0) {
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				UIControllerDebugger.LogWarning(4, fixCount + " Override Controllers is Fix.");
			}
		}
		private static void LogWarning(int code, string message) {
			Debug.LogWarning(
				"[UI Controller Warning] "
				+ "Code: " + code
				+ ", Message: " + message
				+ "\nSupport: https://github.com/johnsoncodehk/unity-uicontroller/issues"
			);
		}
	}
}
