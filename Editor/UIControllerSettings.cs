using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace UIControllerEditor {

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
		[System.Serializable]
		public class Inspector {
			public bool showInfos = false;
			public bool showButtons = true;
		}

		public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
		public Transition transition;
		public Inspector inspector;

		private Transition m_Transition;

		void OnValidate() {
			if (this.transition.exitTime != this.m_Transition.exitTime
				|| this.transition.duration != this.m_Transition.duration) {
				this.m_Transition.exitTime = this.transition.exitTime;
				this.m_Transition.duration = this.transition.duration;
				foreach (RuntimeAnimatorController controller in this.controllers) {
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
			}
			if (this.transition.canTransitionToSelf != this.m_Transition.canTransitionToSelf) {
				this.m_Transition.canTransitionToSelf = this.transition.canTransitionToSelf;
				foreach (RuntimeAnimatorController controller in this.controllers) {
					AnimatorController animator = controller as AnimatorController;
					this.SetAnimatorBool(animator, "Can Transition To Self", this.transition.canTransitionToSelf);
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
			foreach (var condition in transition.conditions) {
				if (condition.parameter == "Init") {
					transition.hasExitTime = false;
					transition.exitTime = 0;
					transition.hasFixedDuration = false;
					transition.duration = 0;
					return;
				}
			}
			transition.hasExitTime = this.transition.exitTime > 0;
			transition.exitTime = this.transition.exitTime > 0 ? this.transition.exitTime : 0;
			transition.hasFixedDuration = false;
			transition.duration = this.transition.duration;
		}
		private void SetAnimatorBool(AnimatorController animator, string name, bool val) {
			for (int i = 0; i < animator.parameters.Length; i++) {
				var par = animator.parameters[i];
				if (par.name == name) {
					animator.RemoveParameter(i);
				}
			}
			AnimatorControllerParameter newPar = new AnimatorControllerParameter();
			newPar.name = name;
			newPar.type = AnimatorControllerParameterType.Bool;
			newPar.defaultBool = val;
			animator.AddParameter(newPar);
		}
	}
}
