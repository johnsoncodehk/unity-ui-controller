using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

namespace JohnsonCodeHK.OverrideControllerToolsEditor {

	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorOverrideController))]
	public class AnimatorOverrideControllerInspector : DecoratorEditor {

		public AnimatorOverrideControllerInspector() : base("AnimatorOverrideControllerInspector") { }

		private bool quickSetupFlagsFoldOut = true;
		private bool mainFoldOut = false;
		private bool hideFlagsFoldOut = false;
		private bool moreFoldOut = false;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			AnimatorOverrideController overrideController = this.target as AnimatorOverrideController;

			if (this.quickSetupFlagsFoldOut = EditorGUILayout.Foldout(this.quickSetupFlagsFoldOut, "Quick Setup")) {
				EditorGUI.indentLevel++;

				string lastFolder = "";

				foreach (var controller in OverrideControllerToolsSetting.instance.quickSetupControllers) {
					if (controller == null) {
						continue;
					}

					string folder = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(controller));
					if (folder != lastFolder) {
						EditorGUILayout.LabelField(folder + "/");
						lastFolder = folder;
					}

					this.DrawButton(
						true,
						controller.name,
						() => {
							overrideController.runtimeAnimatorController = controller;
							overrideController.CreateOverrideAnimations(overrideController.GetOverridesUnite().FindAll(cp => cp.Value == null).Select(cp => cp.Key).ToArray());
						}
					);
				}
				EditorGUI.indentLevel--;
			}

			if (this.mainFoldOut = EditorGUILayout.Foldout(this.mainFoldOut, "Override Controller Tools")) {
				EditorGUI.indentLevel++;

				var unSetupClips = overrideController.GetOverridesUnite().FindAll(cp => cp.Value == null).Select(cp => cp.Key).ToArray();
				string[] unSetupClipsName = unSetupClips.Select(c => c.name).ToArray();
				this.DrawButton(
					unSetupClipsName.Length > 0,
					"Cteate Override Animations" + (unSetupClipsName.Length > 0 ? " (" + string.Join(", ", unSetupClipsName) + ")" : ""),
					() => {
						overrideController.CreateOverrideAnimations(unSetupClips);
					}
				);
				List<AnimationClip> unusedClips = overrideController.GetUnusedAnimations();
				this.DrawButton(
					unusedClips.Count > 0,
					"Delete Unused Animations" + (unusedClips.Count > 0 ? " (" + string.Join(", ", unusedClips.Select(c => c.name).ToArray()) + ")" : ""),
					() => {
						foreach (AnimationClip clip in unusedClips) {
							Object.DestroyImmediate(clip, true);
						}
						AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(overrideController));
						assetImporter.SaveAndReimport();
					}
				);
				this.DrawButton(
					overrideController.runtimeAnimatorController != null,
					"Export to Animator",
					() => {
						overrideController.ExportController();
					}
				);
				if (this.hideFlagsFoldOut = EditorGUILayout.Foldout(this.hideFlagsFoldOut, "Set Include Animations Hide Flags")) {
					EditorGUI.indentLevel++;
					this.DrawButton(
						true,
						HideFlags.None.ToString(),
						() => {
							overrideController.SetAnimationsHideFlags(HideFlags.None);
						}
					);
					this.DrawButton(
						true,
						HideFlags.HideInHierarchy.ToString(),
						() => {
							overrideController.SetAnimationsHideFlags(HideFlags.HideInHierarchy);
						}
					);
					EditorGUI.indentLevel--;
				}
				if (this.moreFoldOut = EditorGUILayout.Foldout(this.moreFoldOut, "More...")) {
					EditorGUI.indentLevel++;
					string url = "http://u3d.as/U4e";
					this.DrawButton(
						true,
						"Asset Inspector - " + url,
						() => {
							System.Diagnostics.Process.Start(url);
						}
					);
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
		}

		private void DrawButton(bool enable, string name, System.Action onClick) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(EditorGUI.indentLevel * 15);
			GUI.enabled = enable;
			if (GUILayout.Button(name)) {
				onClick();
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
		}
	}
}
