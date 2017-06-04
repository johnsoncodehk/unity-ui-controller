using UnityEngine;
using System.Collections.Generic;

public static class AnimatorOverrideControllerExtensions {

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
}
