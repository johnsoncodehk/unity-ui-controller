using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIControllerStateMachine : UnityEngine.StateMachineBehaviour {

	private Dictionary<int, float> normalizedTimes = new Dictionary<int, float> ();

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.normalizedTimes[stateInfo.fullPathHash] = 0;
		if (stateInfo.IsName ("Init")) {
			UIController uiController;
			if (this.TryGetComponent<UIController> (animator.gameObject, out uiController)) {
				uiController.animator.SetBool ("isShow", uiController.onHideAction != UIController.OnHideAction.None);
			}
		}
	}
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (this.normalizedTimes[stateInfo.fullPathHash] < 1 && stateInfo.normalizedTime >= 1) {
			UIController uiController;
			if (this.TryGetComponent<UIController> (animator.gameObject, out uiController)) {
				uiController.StartCoroutine (this.WaitForSendMessage (uiController, stateInfo));
			}
		}
		this.normalizedTimes[stateInfo.fullPathHash] = stateInfo.normalizedTime;
	}

	private bool TryGetComponent<T> (GameObject go, out T t) where T : Component {
		T outT = go.GetComponent<T> ();
		if (outT == null) {
			Debug.LogError (go.gameObject + " missing script: " + typeof (T));
		}
		t = outT;
		return t != null;
	}
	private IEnumerator WaitForSendMessage (UIController uiController, AnimatorStateInfo stateInfo) {
		yield return new WaitForEndOfFrame ();
		if (stateInfo.IsName ("Show")) {
			uiController.SendMessage ("OnShow");
		}
		else if (stateInfo.IsName ("Hide")) {
			uiController.SendMessage ("OnHide");
		}
	}
}
