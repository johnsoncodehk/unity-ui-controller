using UnityEngine;
using UnityEditor;

namespace JohnsonCodeHK.UIControllerEditor {

	[CanEditMultipleObjects, CustomEditor(typeof(UIControllerSettings), true)]
	public class UIControllerSettingsInspector : Editor {

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			base.OnInspectorGUI();
			GUI.enabled = true;
			GUILayout.Label("");

			string info = "Version: 2.4";
			info += "\n" + "License: Free";
			info += "\n" + "";
			info += "\n" + "This free version already contains most of the features, and the full version of the difference just is can use settings, so if you buy the full version, a greater degree is to give me support!";
			EditorGUILayout.HelpBox(info, MessageType.Info);
			if (GUILayout.Button("Get Full Version")) {
				System.Diagnostics.Process.Start("http://u3d.as/xWy");
			}
		}
	}
}
