using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BSS.PoseBlender
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(IKController))]
    public class PoseBlenderLite : MonoBehaviour
    {
        [System.Serializable]
        public struct BoneRotationSettings
        {
            public string boneName;
            // The bone to rotate.
            public Transform bone;
            // Blend weight (0 to 1) for how strongly to apply the recorded rotation.
            [Range(0f, 1f)]
            public float blendWeight;
            // Per-bone rotation offset (applied during recorded pose processing).
            public Vector3 rotationOffset;
        }

        [System.Serializable]
        public class BoneChain
        {
            public string chainName;
            // Chain blend weight (0 to 1) that controls the overall blending for all bones in this chain.
            [Range(0f, 1f)]
            public float blendWeight = 1f;

            public List<BoneRotationSettings> bones = new List<BoneRotationSettings>();
        }

        [System.Serializable]
        public class AnimationOverride
        {
            public string animationName;
            [Range(0f, 1f)]
            public float blendWeight = 1f; // Overall influence
            public AnimationPoseDataSO poseData;
            public List<BoneChain> boneChains = new List<BoneChain>();

        }

        [System.Serializable]
        public struct RootSpaceRotation
        {
            public string boneName;
            public Transform bone;
            public Vector2 xMinMax; // Minimum and maximum rotation for the x-axis.
            public Vector2 yMinMax; // Minimum and maximum rotation for the y-axis.
            public Vector2 zMinMax; // Minimum and maximum rotation for the z-axis.
            [Range(0f, 1f)]
            public float blendWeight;

            // This field will store the computed rotation (for debugging or reference).
            public Vector3 currentRootSpaceRotation;
        }

        [Header("Preview")]
        [SerializeField] public bool previewInEditor = false;

        [Header("Setup Character")]
        // The root transform used when recording the pose.
        public Animator animator;
        public Transform animationRoot;

        [Header("Character Offsets")]
        public bool enableLookAtMode = false;
        public Transform lookAtTarget;
        // Input offsets (expected between -90 and 90) coming from your player controller.
        [Range(-90, 90)] public float lookVerticalOffset = 0;
        [Range(-90, 90)] public float lookHorizontalOffset = 0;
        [Range(-90, 90)] public float leaningOffset = 0;

        // Additional per-bone root-space rotations (applied after the main recorded pose).
        public List<RootSpaceRotation> lookOffsetBones = new List<RootSpaceRotation>();

        [Header("Character Overlay Poses")]
        public List<AnimationOverride> overlayPoses = new List<AnimationOverride>();

        // Cached recorded data and bone hierarchy.
        private Dictionary<Transform, List<Transform>> boneChildren = new Dictionary<Transform, List<Transform>>();
        private List<Transform> rootBones = new List<Transform>();

        [SerializeField] bool initialized = false;

        // Store the active blend coroutine
        private Coroutine activeBlendCoroutine = null;

        /// <summary>
        /// Sets the look offsets for vertical, horizontal, and leaning angles, clamping each value to the range -90 to 90.
        /// </summary>
        /// <param name="vertical">The vertical look offset.</param>
        /// <param name="horizontal">The horizontal look offset.</param>
        /// <param name="leaning">The leaning offset.</param>
        public void SetLookOffsets(float vertical, float horizontal, float leaning)
        {
            lookVerticalOffset = Mathf.Clamp(vertical, -90f, 90f);
            lookHorizontalOffset = Mathf.Clamp(horizontal, -90f, 90f);
            leaningOffset = Mathf.Clamp(leaning, -90f, 90f);
        }

        /// <summary>
        /// Automatically configures the character by setting the animation root and animator references.
        /// If the animation root is not assigned, it attempts to find it using the GetRoot() method.
        /// </summary>
        public void AutoSetupCharacter()
        {
            if(animationRoot == null)
                animationRoot = GetRoot();
            
            if(animator == null)
                animator = GetComponent<Animator>();

            if(animationRoot != null && animator != null)
                initialized = true;
        }
        /// <summary>
        /// Resets the character configuration by clearing the animation root and animator references, 
        /// marking the component as uninitialized.
        /// </summary>
        public void ResetCharacter()
        {
            animationRoot = null;
            animator = null;

            initialized = false;
        }

        /// <summary>
        /// Searches among the child transforms for one named "Root" or "root" and returns it.
        /// Returns null if no matching transform is found.
        /// </summary>
        /// <returns>The root transform if found; otherwise, null.</returns>
        public Transform GetRoot()
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();

            foreach (Transform t in transforms)
            {
                if (t.name == "Root" ||
                    t.name == "root")
                {
                    return t;
                }
            }
            return null;
        }

#if UNITY_EDITOR
        private float lastEditorTime = 0f;

        void OnEnable()
        {
            // Only subscribe in edit mode
            if (!Application.isPlaying)
            {
                lastEditorTime = (float)EditorApplication.timeSinceStartup;
                EditorApplication.update += EditorUpdate;
            }
        }

        void OnDisable()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
            }
        }
        /// <summary>
        /// Called on each editor update tick when previewing in the editor.
        /// It updates the animator based on elapsed time, processes overlay poses and root rotations,
        /// and repaints the Scene view for immediate visual feedback.
        /// </summary>
        void EditorUpdate()
        {
            if (!previewInEditor)
                return;

            float currentTime = (float)EditorApplication.timeSinceStartup;
            float dt = currentTime - lastEditorTime;
            lastEditorTime = currentTime;

            if(animator)
                animator.Update(dt);
            ProcessOverlayPoses();
            ProcessRootRotation();
            SceneView.RepaintAll();
        }
#endif

        // This LateUpdate will run during play mode (and even in the editor if playing)
        void LateUpdate()
        {
            if (Application.isPlaying)
            {
                if (previewInEditor) previewInEditor = false;

                ProcessOverlayPoses();
                ProcessRootRotation();
            }
        }

        private void ProcessOverlayPoses()
        {
            Quaternion poseRootRotation = animationRoot.rotation;

            // Loop through each overlay pose.
            foreach (var overlay in overlayPoses)
            {
                // Validate the overlay pose data.
                if (overlay.poseData == null || overlay.poseData.frames.Length == 0)
                    continue;

                // Here you might add interpolation between frames if needed.
                // For simplicity, we'll use the first frame:
                var frame = overlay.poseData.frames[0];

                // Process each bone chain in this overlay pose.
                foreach (var chain in overlay.boneChains)
                {
                    foreach (var boneSettings in chain.bones)
                    {
                        Transform bone = boneSettings.bone;
                        if (bone == null)
                            continue;

                        // Find the bone data using the relative path.
                        string relativePath = GetRelativePath(animationRoot, bone);
                        var boneData = frame.boneTransforms.Find(b => b.bonePath == relativePath);
                        if (boneData == null)
                            continue;

                        // Apply per-bone offset in local space first
                        Quaternion recordedLocalRotation = boneData.localRotation;
                        Quaternion perBoneOffset = Quaternion.Euler(boneSettings.rotationOffset);
                        recordedLocalRotation = perBoneOffset * recordedLocalRotation;

                        // Then convert to global space
                        Quaternion recordedGlobalRotation = poseRootRotation * recordedLocalRotation;

                        // Compute the effective blend weight.
                        float effectiveBlend = boneSettings.blendWeight * chain.blendWeight * overlay.blendWeight;

                        // Blend the current bone rotation with the overlay pose rotation.
                        bone.rotation = Quaternion.Slerp(bone.rotation, recordedGlobalRotation, effectiveBlend);
                    }
                }
            }
        }


        /// <summary>
        /// Maps an input value (expected in the range -90 to 90) to a target rotation value defined by the given min–max range.
        /// </summary>
        /// <param name="input">The input value (-90 to 90).</param>
        /// <param name="minMax">The target rotation range.</param>
        /// <returns>The mapped rotation value.</returns>
        float MapInputToBoneRotation(float input, Vector2 limits)
        {
            // 'limits.x' is assumed to be the negative limit (a negative value)
            // and 'limits.y' is the positive limit.
            if (input >= 0f)
            {
                float t = input / 90f; // Map [0, 90] to [0, 1]
                return Mathf.Lerp(0f, limits.y, t);
            }
            else
            {
                float t = (-input) / 90f; // Map [0, 90] to [0, 1] (input is negative)
                return Mathf.Lerp(0f, limits.x, t);
            }
        }


        /// <summary>
        /// Processes each pose override in the overrides list.
        /// For each override, it calculates the current frame based on its fps and elapsed time,
        /// then interpolates between frames and applies the computed rotations to the bones
        /// defined in the override's bone chains.
        /// </summary>
        private void ProcessRootRotation()
        {
            if (lookOffsetBones.Count == 0 || animationRoot == null)
                return;

            // 1) Compute raw vertical & horizontal offsets
            float vertical = lookVerticalOffset;
            float horizontal = lookHorizontalOffset;

            if (enableLookAtMode && lookAtTarget != null)
            {
                // Use the last look‑offset bone as the origin (fallback to animationRoot)
                Transform lookOrigin = lookOffsetBones[lookOffsetBones.Count - 1].bone ?? animationRoot;
                Vector3 dir = lookAtTarget.position - lookOrigin.position;
                // Transform into root‑local space
                Vector3 localDir = animationRoot.InverseTransformDirection(dir.normalized);

                horizontal = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
                vertical = Mathf.Asin(-localDir.y) * Mathf.Rad2Deg;

                horizontal = Mathf.Clamp(horizontal, -90f, 90f);
                vertical = Mathf.Clamp(vertical, -90f, 90f);

                lookVerticalOffset = vertical;
                lookHorizontalOffset = horizontal;
            }

            // 2) Apply to each look‑offset bone
            for (int i = 0; i < lookOffsetBones.Count; i++)
            {
                var rs = lookOffsetBones[i];
                if (rs.bone == null) continue;

                float mappedX = MapInputToBoneRotation(vertical, rs.xMinMax);
                float mappedY = MapInputToBoneRotation(horizontal, rs.yMinMax);
                float mappedZ = enableLookAtMode
                                  ? 0f
                                  : MapInputToBoneRotation(-leaningOffset, rs.zMinMax);

                Vector3 targetEuler = new Vector3(mappedX, mappedY, mappedZ);
                rs.currentRootSpaceRotation = targetEuler;

                Quaternion offsetQ = animationRoot.rotation
                                     * Quaternion.Euler(targetEuler)
                                     * Quaternion.Inverse(animationRoot.rotation);

                Quaternion blended = Quaternion.Slerp(
                    rs.bone.rotation,
                    offsetQ * rs.bone.rotation,
                    rs.blendWeight
                );

                rs.bone.rotation = blended;
                lookOffsetBones[i] = rs;
            }
        }

        /// <summary>
        /// Recursively processes a bone and its children to apply blended pose rotations.
        /// For bones found in the provided bone settings map, it computes a blended local rotation 
        /// by interpolating between the current and recorded rotations (with an applied global offset).
        /// </summary>
        /// <param name="bone">The current bone transform to process.</param>
        /// <param name="poseRootRotation">The root rotation of the recorded pose.</param>
        /// <param name="hipDeltaYaw">A yaw adjustment for the hip (unused in the provided snippet but may be relevant).</param>
        /// <param name="globalOffset">A global rotation offset applied to the recorded rotation.</param>
        /// <param name="boneSettingsMap">A mapping from bone transforms to their rotation settings.</param>
        /// <param name="chainBlendMap">A mapping from bone transforms to their chain blend weight values.</param>
        private void ProcessBoneAndChildren(
            Transform bone,
            Quaternion poseRootRotation,
            Quaternion hipDeltaYaw,
            Quaternion globalOffset,
            Dictionary<Transform, BoneRotationSettings> boneSettingsMap,
            Dictionary<Transform, float> chainBlendMap)
        {
            if (bone == null || !boneSettingsMap.ContainsKey(bone))
                return;

            BoneRotationSettings boneSettings = boneSettingsMap[bone];
            string relativePath = GetRelativePath(animationRoot, bone);

            // Use the first overlay pose from the overlayPoses list (if available).
            var boneData = (overlayPoses.Count > 0)
                ? overlayPoses[0].poseData.frames[0].boneTransforms.Find(b => b.bonePath == relativePath)
                : null;

            if (boneData != null)
            {
                Quaternion originalLocalRotation = bone.localRotation;
                Quaternion recordedLocalRotation = boneData.localRotation;
                Quaternion perBoneOffset = Quaternion.Euler(boneSettings.rotationOffset);
                recordedLocalRotation = perBoneOffset * recordedLocalRotation;

                recordedLocalRotation = globalOffset * recordedLocalRotation;
                float chainBlend = chainBlendMap.ContainsKey(bone) ? chainBlendMap[bone] : 1.0f;
                float effectiveBlend = boneSettings.blendWeight * chainBlend;
                Quaternion blendedLocalRotation = Quaternion.Slerp(originalLocalRotation, recordedLocalRotation, effectiveBlend);
                bone.localRotation = blendedLocalRotation;
            }

            if (boneChildren.TryGetValue(bone, out List<Transform> children))
            {
                foreach (Transform child in children)
                {
                    ProcessBoneAndChildren(child, poseRootRotation, hipDeltaYaw, globalOffset, boneSettingsMap, chainBlendMap);
                }
            }
        }



        /// <summary>
        /// Computes the relative path (hierarchical name) from the provided root transform to the target transform.
        /// Useful for matching recorded bone data with scene bones.
        /// </summary>
        /// <param name="root">The root transform from which the path should be computed.</param>
        /// <param name="target">The target transform whose relative path is desired.</param>
        /// <returns>The relative path string. Returns an empty string if the target is the root.</returns>
        private string GetRelativePath(Transform root, Transform target)
        {
            if (target == root)
                return "";
            string path = target.name;
            Transform current = target.parent;
            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
        }

        /// <summary>
        /// Starts a new blend coroutine, stopping any existing one.
        /// </summary>
        /// <param name="overlayName">The name of the overlay pose to blend.</param>
        /// <param name="targetBlend">The target blend weight (0 to 1).</param>
        /// <param name="duration">The duration of the blend in seconds.</param>
        public void BlendOverlay(string overlayName, float targetBlend, float duration)
        {
            // Stop any existing blend coroutine
            if (activeBlendCoroutine != null)
            {
                StopCoroutine(activeBlendCoroutine);
                activeBlendCoroutine = null;
            }

            // Start a new coroutine
            activeBlendCoroutine = StartCoroutine(BlendOverlayPoseCoroutine(overlayName, targetBlend, duration));
        }

        /// <summary>
        /// Starts a new blend coroutine for all overlays, stopping any existing one.
        /// </summary>
        /// <param name="targetBlend">The target blend weight (0 to 1).</param>
        /// <param name="duration">The duration of the blend in seconds.</param>
        public void BlendAllOverlays(float targetBlend, float duration)
        {
            // Stop any existing blend coroutine
            if (activeBlendCoroutine != null)
            {
                StopCoroutine(activeBlendCoroutine);
                activeBlendCoroutine = null;
            }

            // Start a new coroutine
            activeBlendCoroutine = StartCoroutine(BlendAllOverlaysCoroutine(targetBlend, duration));
        }

        /// <summary>
        /// Coroutine that smoothly blends an overlay pose to a target blend weight over time.
        /// </summary>
        /// <param name="overlayName">The name of the overlay pose to blend.</param>
        /// <param name="targetBlend">The target blend weight (0 to 1).</param>
        /// <param name="duration">The duration of the blend in seconds.</param>
        private System.Collections.IEnumerator BlendOverlayPoseCoroutine(string overlayName, float targetBlend, float duration)
        {
            // Find the overlay with the matching name
            AnimationOverride targetOverlay = overlayPoses.Find(o => o.animationName == overlayName);

            // If no matching overlay was found, exit
            if (targetOverlay == null)
            {
                Debug.LogWarning($"Overlay pose '{overlayName}' not found.");
                activeBlendCoroutine = null;
                yield break;
            }

            // Store initial blend weight
            float initialBlend = targetOverlay.blendWeight;
            float currentTime = 0f;

            // Clamp target blend to valid range
            targetBlend = Mathf.Clamp01(targetBlend);

            // Blend over time
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float t = Mathf.Clamp01(currentTime / duration);

                // Linear interpolation between initial and target blend weights
                targetOverlay.blendWeight = Mathf.Lerp(initialBlend, targetBlend, t);

                yield return null;
            }

            // Ensure we end exactly at the target blend
            targetOverlay.blendWeight = targetBlend;
            activeBlendCoroutine = null;
        }

        /// <summary>
        /// Coroutine that blends all overlay poses to a specified target weight over time.
        /// </summary>
        /// <param name="targetBlendWeight">The target blend weight for all overlays.</param>
        /// <param name="duration">The duration of the blend in seconds.</param>
        private System.Collections.IEnumerator BlendAllOverlaysCoroutine(float targetBlendWeight, float duration)
        {
            // Exit early if no overlays exist
            if (overlayPoses.Count == 0)
            {
                activeBlendCoroutine = null;
                yield break;
            }

            // Store initial blend weights
            Dictionary<AnimationOverride, float> initialBlends = new Dictionary<AnimationOverride, float>();

            foreach (var overlay in overlayPoses)
            {
                initialBlends.Add(overlay, overlay.blendWeight);
            }

            float currentTime = 0f;

            // Clamp target blend to valid range
            targetBlendWeight = Mathf.Clamp01(targetBlendWeight);

            // Blend all overlays over time
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float t = Mathf.Clamp01(currentTime / duration);

                foreach (var overlay in overlayPoses)
                {
                    overlay.blendWeight = Mathf.Lerp(initialBlends[overlay], targetBlendWeight, t);
                }

                yield return null;
            }

            // Ensure we end exactly at the target blend for all overlays
            foreach (var overlay in overlayPoses)
            {
                overlay.blendWeight = targetBlendWeight;
            }

            activeBlendCoroutine = null;
        }

        /// <summary>
        /// Sets the vertical look offset angle of the character.
        /// This controls the up/down orientation of the character's head and upper body.
        /// </summary>
        /// <param name="offset">The vertical angle in degrees. Positive values look up, negative values look down.</param>
        public void SetVerticalOffset(float offset)
        {
            lookVerticalOffset = offset;
        }

        /// <summary>
        /// Sets the horizontal look offset angle of the character.
        /// This controls the left/right orientation of the character's head and upper body.
        /// </summary>
        /// <param name="offset">The horizontal angle in degrees. Positive values look right, negative values look left.</param>
        public void SetHorizontalOffset(float offset)
        {
            lookHorizontalOffset = offset;
        }

        /// <summary>
        /// Sets the leaning offset angle of the character.
        /// This controls how much the character's body leans to the sides.
        /// </summary>
        /// <param name="offset">The leaning angle in degrees. Positive values lean right, negative values lean left.</param>
        public void SetLeaningOffset(float offset)
        {
            leaningOffset = offset;
        }
    }
}