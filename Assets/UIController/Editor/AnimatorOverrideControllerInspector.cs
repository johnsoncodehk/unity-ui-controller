using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace UIControllerEditor {
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorOverrideController))]
	public class AnimatorOverrideControllerInspector : DecoratorEditor {

		public AnimatorOverrideControllerInspector() : base("AnimatorOverrideControllerInspector") { }

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			var overrideController = this.target as AnimatorOverrideController;

#if UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4
			List<AnimationClip> clips = AnimatorOverrideControllerInspector.GetIncludeAnimations(overrideController);
			if (clips.Count > 0) {
				string names = clips[0].name;
				for (int i = 1; i < clips.Count; i++) {
					names += ", " + clips[i].name;
				}
				GUILayout.Label("Include Animations: " + names);
			}
#endif

			foreach (var controller in UIControllerSetting.instance.controllers) {
				if (GUILayout.Button("Setup " + controller.name.Replace("_", "->"))) {
					AnimatorOverrideControllerInspector.SetupController(overrideController, controller);
				}
			}
			GUILayout.Label("");

			List<AnimationClip> unusedClips = AnimatorOverrideControllerInspector.GetUnusedAnimations(overrideController);
			GUI.enabled = unusedClips.Count > 0;
			if (GUILayout.Button("Delete Unused Animations (" + unusedClips.Count + ")")) {
				foreach (AnimationClip clip in unusedClips) {
					Object.DestroyImmediate(clip, true);
				}
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			GUI.enabled = true;

			GUI.enabled = overrideController.runtimeAnimatorController != null;
			if (GUILayout.Button("Export to Animator")) {
				AnimatorOverrideControllerInspector.ExportController(overrideController);
			}
			GUI.enabled = true;
		}

		public static void SetupController(AnimatorOverrideController overrideController, RuntimeAnimatorController controller = null) {
			if (controller != null) {
				overrideController.runtimeAnimatorController = controller;
			}

			var overrides = overrideController.GetOverridesUnite();
			foreach (var clipPair in overrides) {
				string overrideClipName = clipPair.Key.name.Replace("Original", "");

				List<AnimationClip> clips = AnimatorOverrideControllerInspector.GetIncludeAnimations(overrideController);
				foreach (AnimationClip clip in clips) {
					if (clip.name == overrideClipName) {
						overrideController[clipPair.Key] = clip;
						break;
					}
				}

				if (overrideController[clipPair.Key] == clipPair.Key) {
					AnimationClip overrideClip = new AnimationClip();
					EditorUtility.CopySerialized(clipPair.Key, overrideClip);
					overrideClip.name = overrideClipName;
					overrideClip.hideFlags = UIControllerSetting.instance.overrideAnimationsHideFlags;
					AssetDatabase.AddObjectToAsset(overrideClip, overrideController);
					overrideController[clipPair.Key] = overrideClip;
				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		private static void ExportController(AnimatorOverrideController overrideController) {
			string overrideControllerPath = AssetDatabase.GetAssetPath(overrideController);
			string controllerPath = AssetDatabase.GetAssetPath(overrideController.runtimeAnimatorController);
			string copyControllerPath = overrideControllerPath.Replace(".overrideController", ".controller");
			if (AssetDatabase.CopyAsset(controllerPath, copyControllerPath)) {
				RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copyControllerPath);
				AnimatorController animator = controller as AnimatorController;
				AnimatorControllerLayer baseLayer = animator.layers[0];
				var overrides = overrideController.GetOverridesUnite();
				foreach (var clipPair in overrides) {
					AnimationClip overrideClip = new AnimationClip();
					EditorUtility.CopySerialized(clipPair.Value, overrideClip);
					overrideClip.hideFlags = HideFlags.None;
					AssetDatabase.AddObjectToAsset(overrideClip, controller);
					for (int i = 0; i < baseLayer.stateMachine.states.Length; i++) {
						AnimatorState state = baseLayer.stateMachine.states[i].state;
						if (state.motion == clipPair.Key) {
							state.motion = overrideClip;
						}
					}
				}
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		public static List<AnimationClip> GetIncludeAnimations(Object obj) {
			System.Collections.Generic.List<AnimationClip> clips = new System.Collections.Generic.List<AnimationClip>();
			Object[] subs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
			foreach (Object sub in subs) {
				AnimationClip clip = sub as AnimationClip;
				if (clip != null) {
					clips.Add(clip);
				}
			}
			return clips;
		}
		private static List<AnimationClip> GetUnusedAnimations(AnimatorOverrideController controller) {
			List<AnimationClip> unusedClips = new List<AnimationClip>();

			List<AnimationClip> includeClips = AnimatorOverrideControllerInspector.GetIncludeAnimations(controller);
			foreach (AnimationClip includeClip in includeClips) {
				if (!new List<AnimationClip>(controller.animationClips).Contains(includeClip)) {
					unusedClips.Add(includeClip);
				}
			}

			return unusedClips;
		}
	}
}
