using UnityEngine;
using System.Collections.Generic;

public class UIControllerStateMachine : StateMachineBehaviour {

	private Dictionary<int, float> normalizedTimes = new Dictionary<int, float>();

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.normalizedTimes[stateInfo.fullPathHash] = 0;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (this.normalizedTimes[stateInfo.fullPathHash] < 1 && stateInfo.normalizedTime >= 1) {
			if (stateInfo.IsName("Show")) {
				animator.SendMessage("OnShow");
			}
			else if (stateInfo.IsName("Hide")) {
				if (!animator.GetBool("Is Show")) {
					animator.SendMessage("OnHide");
				}
			}
		}
		this.normalizedTimes[stateInfo.fullPathHash] = stateInfo.normalizedTime;
	}
}
