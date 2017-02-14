using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace UIControllerEditor {

	[InitializeOnLoad]
	public class UIControllerDebugger {

		static UIControllerDebugger () {
			EditorApplication.update += OnUpdate;
		}
		private static void OnUpdate() {
			EditorApplication.update -= OnUpdate;

			UIControllerSetting setting = UIControllerSetting.instance;
			FixAllControllers (setting);
			FixAllOverrideControllers (setting);
		}

		public static void FixAllControllers (UIControllerSetting setting) {
			int fixCount = 0;
			foreach (var controller in setting.controllers) {
				AnimatorController animator = controller as AnimatorController;
				if (animator.GetBehaviours<UIControllerStateMachine> ().Length == 0) {
					var baseLayer = animator.layers[0];
					baseLayer.stateMachine.AddStateMachineBehaviour<UIControllerStateMachine> ();
					fixCount++;
				}
			}
			if (fixCount > 0) {
				Debug.LogWarning ("UIController: " + fixCount + " Controllers is Fix. (Error 1)");
			}
		}
		public static void FixAllOverrideControllers (UIControllerSetting setting) {
			int fixCount = 0;
			List<AnimatorOverrideController> all = FindAllOverrideControllers (setting);
			foreach (var a in all) {
				foreach (AnimationClipPair clipPair in a.clips) {
					if (clipPair.originalClip == clipPair.overrideClip) {
						AnimatorOverrideControllerInspector.SetupController (a);
						fixCount++;
						break;
					}
				}
			}
			if (fixCount > 0) {
				Debug.LogWarning ("UIController: " + fixCount + " Override Controllers is Fix. (Error 2)");
			}
		}
		public static List<AnimatorOverrideController> FindAllOverrideControllers (UIControllerSetting setting) {
			var ocs = AssetDatabase.FindAssets ("t:" + typeof (AnimatorOverrideController).Name);
			List<AnimatorOverrideController> cons = new List<AnimatorOverrideController> ();
			foreach (var oc in ocs) {
				var controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController> (AssetDatabase.GUIDToAssetPath (oc));
				if (setting.controllers.Contains (controller.runtimeAnimatorController)) {
					cons.Add (controller);
				}
			}
			return cons;
		}
	}
}
