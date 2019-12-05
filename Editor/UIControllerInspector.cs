using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace UIControllerEditor
{

    [CanEditMultipleObjects, CustomEditor(typeof(UIController), true)]
    public class UIControllerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIController controller = target as UIController;

            Toggle("m_Show", true, "Active On Show");
            Toggle("m_OnHide", false, "Inactive On Hide");

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Current State: " + controller.currentState, MessageType.Info);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Show"))
                        controller.Show();
                    if (GUILayout.Button("Hide"))
                        controller.Hide();
                }
            }
        }

        void Toggle(string propertyName, bool active, string label)
        {
            SerializedProperty calls = serializedObject.FindProperty(propertyName).FindPropertyRelative("m_PersistentCalls.m_Calls");
            int callbackIndex = FindSetActiveCallbackIndex();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                bool setActiveOnShow = EditorGUILayout.Toggle(label, callbackIndex >= 0);
                if (check.changed)
                {
                    if (setActiveOnShow)
                        AddSetActiveCallback();
                    else
                        calls.DeleteArrayElementAtIndex(callbackIndex);

                    serializedObject.ApplyModifiedProperties();
                }
            }

            int FindSetActiveCallbackIndex()
            {
                for (int i = 0; i < calls.arraySize; i++)
                {
                    var call = calls.GetArrayElementAtIndex(i);
                    var callTarget = call.FindPropertyRelative("m_Target");
                    var methodName = call.FindPropertyRelative("m_MethodName");
                    var boolArgument = call.FindPropertyRelative("m_Arguments.m_BoolArgument");

                    if (callTarget.objectReferenceValue == (target as Component).gameObject && methodName.stringValue == "SetActive" && boolArgument.boolValue == active)
                        return i;
                }

                return -1;
            }

            void AddSetActiveCallback()
            {
                calls.InsertArrayElementAtIndex(0);

                var call = calls.GetArrayElementAtIndex(0);
                var callTarget = call.FindPropertyRelative("m_Target");
                var methodName = call.FindPropertyRelative("m_MethodName");
                var mode = call.FindPropertyRelative("m_Mode");
                var boolArgument = call.FindPropertyRelative("m_Arguments.m_BoolArgument");
                var callState = call.FindPropertyRelative("m_CallState");

                callTarget.objectReferenceValue = (target as Component).gameObject;
                methodName.stringValue = "SetActive";
                mode.enumValueIndex = (int)PersistentListenerMode.Bool;
                boolArgument.boolValue = active;
                callState.enumValueIndex = (int)UnityEventCallState.RuntimeOnly;
            }
        }
    }
}
