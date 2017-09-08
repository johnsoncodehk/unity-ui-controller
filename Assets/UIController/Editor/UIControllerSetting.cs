using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace JohnsonCodeHK.UIControllerEditor {

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

		public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
		public Transition transition;

		private Transition m_Transition;

		void OnValidate() {
			if (this.transition.exitTime != this.m_Transition.exitTime || this.transition.duration != this.m_Transition.duration) {
				this.m_Transition = this.transition;
				foreach (RuntimeAnimatorController controller in this.controllers) {
					this.UpdateControllerTransition(controller);
				}
			}
		}

		private void UpdateControllerTransition(RuntimeAnimatorController controller) {
			AnimatorController animator = controller as AnimatorController;

			var baseLayer = animator.layers[0];
			foreach (var transition in baseLayer.stateMachine.anyStateTransitions) {
				this.SetStateTransitions(transition);
			}
			foreach (var state in baseLayer.stateMachine.states) {
				foreach (var transition in state.state.transitions) {
					this.SetStateTransitions(transition);
				}
			}
		}
		private void SetStateTransitions(AnimatorStateTransition transition) {
			if (transition.conditions.Length == 0) {
				transition.hasExitTime = true;
				transition.exitTime = 1;
				transition.hasFixedDuration = false;
				transition.duration = 0;
				return;
			}
			transition.hasExitTime = this.transition.exitTime > 0;
			transition.exitTime = this.transition.exitTime > 0 ? this.transition.exitTime : 1;
			transition.hasFixedDuration = false;
			transition.duration = this.transition.duration;
		}
	}
}
