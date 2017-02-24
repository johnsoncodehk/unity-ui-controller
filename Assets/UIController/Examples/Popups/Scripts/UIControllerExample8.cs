using UnityEngine;

namespace UIControllerExamples.Popup {

	public class UIControllerExample8 : UIControllerExample {

		public override void Show () {
			base.Show ();
			foreach (UIController child in this.GetComponentsInChildren<UIController> ()) {
				if (child != this) {
					child.gameObject.SetActive (false);
				}
			}
		}

		private void ShowChild (string path) {
			UIController child = this.GetChild (path);
			if (child == null) {
				return;
			}
			child.Show ();
		}
		private void HideChild (string path) {
			UIController child = this.GetChild (path);
			if (child == null) {
				return;
			}
			child.Hide ();
		}
		private UIController GetChild (string path) {
			Transform child = this.transform.FindChild (path);
			if (child == null) {
				return null;
			}
			return child.GetComponent<UIController> ();
		}
	}
}
