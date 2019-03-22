using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Reflection;

namespace OverrideControllerToolsEditor {

	public static class AnimatorOverrideControllerExtensions {

		public static List<AnimationClip> GetUnusedAnimations(this AnimatorOverrideController controller) {
			List<AnimationClip> clips = new List<AnimationClip>();

			List<AnimationClip> includeClips = controller.LoadAllAsset<AnimationClip>();
			foreach (AnimationClip includeClip in includeClips) {
				if (!new List<AnimationClip>(controller.animationClips).Contains(includeClip)) {
					clips.Add(includeClip);
				}
			}

			return clips;
		}
		public static List<T> LoadAllAsset<T>(this Object obj) where T : Object {
			List<T> assets = new List<T>();

			Object[] subs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
			foreach (Object sub in subs) {
				T asset = sub as T;
				if (asset != null) {
					assets.Add(asset);
				}
			}
			return assets;
		}
		public static void ExportController(this AnimatorOverrideController overrideController) {
			string overrideControllerPath = AssetDatabase.GetAssetPath(overrideController);
			string controllerPath = AssetDatabase.GetAssetPath(overrideController.runtimeAnimatorController);
			string copyControllerPath = overrideControllerPath.Replace(".overrideController", ".controller");
			if (AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copyControllerPath) != null) {
				Debug.Log("File " + copyControllerPath + " already exists.");
				return;
			}
			if (AssetDatabase.CopyAsset(controllerPath, copyControllerPath)) {
				RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copyControllerPath);
				AnimatorController animator = controller as AnimatorController;
				List<AnimatorState> states = animator.LoadAllAsset<AnimatorState>();
				var overrides = overrideController.GetOverridesUnite();
				foreach (var clipPair in overrides) {
					if (clipPair.Value == null) {
						continue;
					}
					AnimationClip overrideClip = new AnimationClip();
					EditorUtility.CopySerialized(clipPair.Value, overrideClip);
					overrideClip.hideFlags = HideFlags.None;
					AssetDatabase.AddObjectToAsset(overrideClip, controller);
					foreach (AnimatorState state in states) {
						if (state.motion == null) {
							continue;
						}
						bool isSame = false;
						if (AssetDatabase.GetAssetPath(state.motion) == AssetDatabase.GetAssetPath(state)) {
							isSame = state.motion.GetFileId() == clipPair.Key.GetFileId();
						}
						else {
							isSame = state.motion == clipPair.Key;
						}
						if (isSame) {
							state.motion = overrideClip;
						}
					}
				}
				List<AnimationClip> clips = animator.LoadAllAsset<AnimationClip>();
				states = animator.LoadAllAsset<AnimatorState>();
				foreach (AnimationClip clip in clips) {
					bool isFound = false;
					foreach (AnimatorState state in states) {
						if (state.motion == clip) {
							isFound = true;
							break;
						}
					}
					if (!isFound) {
						Object.DestroyImmediate(clip, true);
					}
				}
			}

			AssetImporter assetImporter = AssetImporter.GetAtPath(copyControllerPath);
			assetImporter.SaveAndReimport();
		}
		public static void CreateOverrideAnimations(this AnimatorOverrideController overrideController, AnimationClip[] originalClips) {
#if UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4
			if (OverrideControllerToolsSetting.instance.animationsHideFlags == HideFlags.None) {
				Debug.LogWarning("Your HideFlag config is None, Unity 5.5.x lower can't display correct asset tree, you should config HideFlag to HideInHierarchy in setting: " + AssetDatabase.GetAssetPath(OverrideControllerToolsSetting.instance));
			}
#endif
			foreach (AnimationClip clip in originalClips) {
				string overrideClipName = clip.name;
				foreach (var strReplace in OverrideControllerToolsSettings.instance.animationNameReplaces) {
					overrideClipName = overrideClipName.Replace(strReplace.oldValue, strReplace.newValue);
				}
				AnimationClip overrideClip = new AnimationClip();
				EditorUtility.CopySerialized(clip, overrideClip);
				overrideClip.name = overrideClipName;
				overrideClip.hideFlags = OverrideControllerToolsSettings.instance.animationHideFlags;
				AssetDatabase.AddObjectToAsset(overrideClip, overrideController);
				overrideController[clip] = overrideClip;
			}

			AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(overrideController));
			assetImporter.SaveAndReimport();
		}
		public static List<KeyValuePair<AnimationClip, AnimationClip>> GetOverridesUnite(this AnimatorOverrideController overrideController) {
#if UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4
			List<AnimationClipPair> clips = new List<AnimationClipPair> (overrideController.clips);
			return clips.ConvertAll(
				new System.Converter<AnimationClipPair, KeyValuePair<AnimationClip, AnimationClip>>((clip) => {
					return new KeyValuePair<AnimationClip, AnimationClip>(clip.originalClip, clip.overrideClip);
				})
			);
#else
			var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
			overrideController.GetOverrides(overrides);
			return overrides;
#endif
		}
		public static void SetAnimationsHideFlags(this AnimatorOverrideController overrideController, HideFlags hideFlags) {
#if !(UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4)
			List<AnimationClip> clips = overrideController.LoadAllAsset<AnimationClip>();
			foreach (AnimationClip clip in clips) {
				if (clip.hideFlags == hideFlags) {
					continue;
				}
				clip.hideFlags = hideFlags;
				EditorUtility.SetDirty(clip);
			}
#endif

			AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(overrideController));
			assetImporter.SaveAndReimport();
		}

		private static long GetFileId(this Object obj) {
			SerializedObject serializedObject = new SerializedObject(obj);
			PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
			SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
			return localIdProp.longValue;
		}
	}
}
