using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace UIControllerEditor {

	public class UIControllerSetting : ScriptableObject {

		public static UIControllerSetting instance {
			get {
				var settings = AssetDatabase.FindAssets("t:" + typeof(UIControllerSetting));
				if (settings.Length == 0) {
					return null;
				}
				return AssetDatabase.LoadAssetAtPath<UIControllerSetting>(AssetDatabase.GUIDToAssetPath(settings[0]));
			}
		}

		[System.Serializable]
		public struct Transition {
			[Range(0, 1)] public float exitTime, duration;
		}
		
		public HideFlags overrideAnimationsHideFlags {
			get {
#if UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4
				return HideFlags.HideInHierarchy;
#else
				return this.hideOverrideAnimations ? HideFlags.HideInHierarchy : HideFlags.None;
#endif
			}
		}

		public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
		public Transition transition;
		public bool hideOverrideAnimations;

		private Transition m_Transition;
		private bool m_HideOverrideAnimations;

		void OnValidate() {
			if (this.transition.exitTime != this.m_Transition.exitTime || this.transition.duration != this.m_Transition.duration) {
				this.m_Transition = this.transition;
				foreach (RuntimeAnimatorController controller in this.controllers) {
					this.UpdateControllerTransition(controller);
				}
			}
			if (this.hideOverrideAnimations != this.m_HideOverrideAnimations) {
				this.m_HideOverrideAnimations = this.hideOverrideAnimations;
				List<AnimatorOverrideController> overrideControllers = UIControllerDebugger.FindAllOverrideControllers(this);
				UIControllerSetting.UpdateAnimationsHideFlags(overrideControllers, this.overrideAnimationsHideFlags);
				UIControllerDebugger.FixControllerMainObject(overrideControllers);
			}
		}

		private void UpdateControllerTransition(RuntimeAnimatorController controller) {
			AnimatorController animator = controller as AnimatorController;

			var baseLayer = animator.layers[0];
			foreach (var state in baseLayer.stateMachine.states) {
				if (state.state.nameHash == Animator.StringToHash("Init")) {
					continue;
				}
				foreach (var transition in state.state.transitions) {
					if (transition.conditions.Length == 0) {
						transition.hasExitTime = true;
						transition.exitTime = 1;
						transition.hasFixedDuration = false;
						transition.duration = 0;
						continue;
					}
					transition.hasExitTime = this.transition.exitTime > 0;
					transition.exitTime = this.transition.exitTime > 0 ? this.transition.exitTime : 1;
					transition.hasFixedDuration = false;
					transition.duration = this.transition.duration;
				}
			}
		}
		private static void UpdateAnimationsHideFlags(List<AnimatorOverrideController> overrideControllers, HideFlags hideFlags) {
			int fixCount = 0;

			foreach (var overrideController in overrideControllers) {
#if !(UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4)
				List<AnimationClip> clips = AnimatorOverrideControllerInspector.GetIncludeAnimations(overrideController);
				foreach (AnimationClip clip in clips) {
					if (clip.hideFlags == hideFlags) {
						continue;
					}
					clip.hideFlags = hideFlags;
					EditorUtility.SetDirty(clip);
					fixCount++;
				}
#endif
			}

			if (fixCount > 0) {
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				UIControllerDebugger.LogMessage(3, fixCount + " override animations Hide Flags is set to " + hideFlags + ".");
			}
		}
	}
}
