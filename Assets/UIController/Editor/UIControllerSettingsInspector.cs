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

			string version = "2.5";
			bool isPro = false;

			string info = "Version: " + version;
			info += "\n" + "License: " + (isPro ? "Pro" : "Free");
			info += "\n" + "Support Email: johnsoncodehk+support@gmail.com";
			if (!isPro) {
				info += "\n" + "";
				info += "\n" + "This free version already contains most of the features, if you like this plugin, you can buy the pro version to give a support.";
				info += "\n" + "";
				info += "\n" + "If you have create anything (game/example video...) using UIController, and share to Asset Store comment area. Please send me an email, I will give you a pro version voucher.";
			}
			EditorGUILayout.HelpBox(info, MessageType.Info);
			if (!isPro) {
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Get Pro Version")) {
					System.Diagnostics.Process.Start("http://u3d.as/xWy");
				}
				if (GUILayout.Button("Write Comment")) {
					System.Diagnostics.Process.Start("http://u3d.as/B5u");
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
