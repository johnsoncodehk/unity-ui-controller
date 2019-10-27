using UnityEngine;
using UnityEditor;

namespace UIControllerEditor
{

    [CanEditMultipleObjects, CustomEditor(typeof(UIController), true)]
    public class UIControllerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIController controller = this.target as UIController;

            if (Application.isPlaying)
            {
                // Infos
                string info = "Current State: " + controller.currentState;
                EditorGUILayout.HelpBox(info, MessageType.Info);

                // Buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Show"))
                {
                    controller.Show();
                }
                if (GUILayout.Button("Hide"))
                {
                    controller.Hide();
                }
                GUI.enabled = false;
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
