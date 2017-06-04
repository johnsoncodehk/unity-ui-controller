using UnityEngine;
using UnityEditor;

namespace UIControllerEditor {
	[CanEditMultipleObjects, CustomEditor(typeof(UIController), true)]
	public class UIControllerInspector : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			UIController t = this.target as UIController;
			if (GUILayout.Button("Show / Hide")) {
				if (!t.isShow) {
					t.Show();
				}
				else {
					t.Hide();
				}
			}
		}
	}
}
