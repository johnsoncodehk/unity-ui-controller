using UnityEngine;
using UnityEditor;

namespace JohnsonCodeHK.UIControllerEditor {

	[CanEditMultipleObjects, CustomEditor(typeof(UIController), true)]
	public class UIControllerInspector : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			UIController t = this.target as UIController;
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Show")) {
				t.Show();
			}
			if (GUILayout.Button("Hide")) {
				t.Hide();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
