using UnityEngine;
using UnityEditor;

namespace JohnsonCodeHK.UIControllerEditor {

	[CanEditMultipleObjects, CustomEditor(typeof(UIController), true)]
	public class UIControllerInspector : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			UIController t = this.target as UIController;

			if (UIControllerSettings.instance.inspector.showInfos) {
				string info = "";
				info += "Is Show: " + t.isShow;
				info += "\n" + "Is Playing: " + t.isPlaying;
				EditorGUILayout.HelpBox(info, MessageType.Info);
			}

			if (UIControllerSettings.instance.inspector.showButtons) {
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Show") && ValidateEditorPlaying()) {
					t.Show();
				}
				if (GUILayout.Button("Hide") && ValidateEditorPlaying()) {
					t.Hide();
				}
				GUI.enabled = false;
				EditorGUILayout.EndHorizontal();
			}
		}
		private static bool ValidateEditorPlaying() {
			if (!Application.isPlaying) {
				Debug.Log("Animation can only be play in playing mode.");
				return false;
			}
			return true;
		}
	}
}
