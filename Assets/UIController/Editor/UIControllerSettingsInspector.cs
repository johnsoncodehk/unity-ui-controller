using UnityEngine;
using UnityEditor;

namespace JohnsonCodeHK.UIControllerEditor {

	[CanEditMultipleObjects, CustomEditor(typeof(UIControllerSettings), true)]
	public class UIControllerSettingsInspector : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			GUILayout.Label("");

			string info = "Version: 2.2.3";
			info += "\n" + "License: Pro";
			EditorGUILayout.HelpBox(info, MessageType.Info);
		}
	}
}
