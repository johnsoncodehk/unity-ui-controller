using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace JohnsonCodeHK.UIControllerEditor {

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
				UIControllerDebugger.LogMessage(1, fixCount + " controllers UIControllerStateMachine missing is fix.");
			}
		}
		public static void FixAllOverrideControllers(UIControllerSetting setting) {
			List<AnimatorOverrideController> overrideControllers = FindAllOverrideControllers(setting);
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

		public static void FixControllerMainObject(List<AnimatorOverrideController> overrideControllers) {
#if !(UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4)
			int fixCount = 0;

			foreach (var overrideController in overrideControllers) {
				if (AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(overrideController)) == null) {
					AssetDatabase.SetMainObject(overrideController, AssetDatabase.GetAssetPath(overrideController));
					EditorUtility.SetDirty(overrideController);
					AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(overrideController));
					assetImporter.SaveAndReimport();
					fixCount++;
				}
			}

			if (fixCount > 0) {
				UIControllerDebugger.LogMessage(4, fixCount + " override controllers Main Object is set.");
			}
#endif
		}
		public static void LogMessage(int code, string message) {
			Debug.Log(
				"[UI Controller] "
				+ "Code: " + code
				+ ", Message: " + message
				+ "\nSupport: https://github.com/johnsoncodehk/unity-uicontroller/issues"
			);
		}
	}
}
