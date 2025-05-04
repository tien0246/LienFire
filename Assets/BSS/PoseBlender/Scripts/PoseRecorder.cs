using System.Collections.Generic;
using UnityEngine;
using static BSS.PoseBlender.AnimationPoseDataSO;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BSS.PoseBlender
{
    [ExecuteInEditMode]
    public class PoseRecorder : MonoBehaviour
    {
        [Tooltip("Reference to the Animator component.")]
        public Animator animator;
        public Transform recordingRoot;

        [Tooltip("The ScriptableObject asset that will hold the recorded pose data.")]
        public AnimationPoseDataSO poseDataAsset;

        public RuntimeAnimatorController controllerForPosing;
        public AnimatorOverrideController overrideControllerForPosing;

        [Tooltip("The AnimationClip to record.")]
        public AnimationClip clip;

        [SerializeField] bool Initialized;

        // Internal frame counter.
        int currentFrame = 0;
        // Normalized time step between frames.
        private float frameNormalizedStep = 0f;

        RuntimeAnimatorController cachedController;

        PoseBlenderLite poseBlenderLite;
        IKController ikController;


        public Transform GetRoot()
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();

            foreach (Transform t in transforms)
            {
                if (t.name == "Root")
                {
                    return recordingRoot = t;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets up the animator for posing using an override controller if provided.
        /// It creates a runtime instance of the override controller and swaps out
        /// the placeholder clip ("BasePoseAnimation") with the clip you want to record.
        /// </summary>
        /// 

        public void Initialize()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            if (recordingRoot == null)
                recordingRoot = GetRoot();

            Initialized = true;
        }

        public void Reset()
        {
            animator = null;
            recordingRoot = null;

            Initialized = false;
        }

        public void SetupPoseEditor()
        {
            if (animator == null)
            {
                Debug.LogError("Animator component not found on the character!");
                return;
            }

            // If an override controller is provided, create an instance and override the clip.
            if (overrideControllerForPosing != null)
            {
                // Create a runtime copy of the override controller.
                AnimatorOverrideController newOverride = new AnimatorOverrideController(overrideControllerForPosing);

                // Get the current overrides.
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                newOverride.GetOverrides(overrides);

                // Replace the desired clip in the override.
                // Assumes that your base controller has a clip named "BasePoseAnimation".
                for (int i = 0; i < overrides.Count; i++)
                {
                    if (overrides[i].Key.name == "Empty")
                    {
                        if (clip != null)
                        {
                            overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, clip);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Did not find animation called 'Empty' to replace, cancelling");
                        return;
                    }
                }
                newOverride.ApplyOverrides(overrides);
                animator.runtimeAnimatorController = newOverride;
            }
            else if (controllerForPosing != null)
            {
                animator.runtimeAnimatorController = controllerForPosing;
            }
            else
            {
                Debug.LogWarning("No Animator Controller assigned for posing.");
            }
        }

        // Records the entire animation clip, frame by frame.
        [ContextMenu("Record Animation")]
        public void RecordAnimationPose()
        {
            cachedController = animator.runtimeAnimatorController;

            if (TryGetComponent<PoseBlenderLite>(out poseBlenderLite))
                poseBlenderLite.enabled = false;

            if (TryGetComponent<IKController>(out ikController))
                ikController.enabled = false;

            SetupPoseEditor();

            if (clip == null)
            {
                Debug.LogError("Animation clip is not assigned!");
                return;
            }

#if UNITY_EDITOR
            if (poseDataAsset == null)
            {
                poseDataAsset = ScriptableObject.CreateInstance<AnimationPoseDataSO>();
                string assetPath = EditorUtility.SaveFilePanelInProject("Save Pose Data", "NewAnimationPoseData", "asset", "Enter a file name for the pose data.");
                if (!string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.CreateAsset(poseDataAsset, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("Pose data asset was not created. Aborting recording.");
                    return;
                }
            }
#endif

            // Calculate the total number of frames based on clip length and frame rate.
            int numFrames = Mathf.CeilToInt(clip.length * clip.frameRate);
            poseDataAsset.frames = new FrameData[numFrames];
            poseDataAsset.fps = Mathf.RoundToInt(clip.frameRate);

            // Loop through each frame, sample the animation, and record the pose.
            for (int i = 0; i < numFrames; i++)
            {
                float time = i / clip.frameRate;

                // Record the current pose.
                List<BoneTransformData> frameData = new List<BoneTransformData>();
                Transform[] allBones = recordingRoot.GetComponentsInChildren<Transform>();
                foreach (Transform bone in allBones)
                {
                    BoneTransformData data = new BoneTransformData();
                    data.boneName = bone.name;
                    data.bonePath = GetRelativePath(recordingRoot, bone);
                    if (bone == recordingRoot)
                    {
                        // For the root bone, record the world transform.
                        data.localPosition = bone.position;
                        data.localRotation = bone.rotation;
                        data.localScale = bone.lossyScale;
                    }
                    else
                    {
                        // Record the transform relative to the recording root.
                        data.localPosition = recordingRoot.InverseTransformPoint(bone.position);
                        data.localRotation = Quaternion.Inverse(recordingRoot.rotation) * bone.rotation;
                        Vector3 rootScale = recordingRoot.lossyScale;
                        Vector3 boneScale = bone.lossyScale;
                        data.localScale = new Vector3(
                            boneScale.x / rootScale.x,
                            boneScale.y / rootScale.y,
                            boneScale.z / rootScale.z
                        );
                    }
                    frameData.Add(data);
                    AdvanceOneFrame();
                }
                poseDataAsset.frames[i] = new FrameData();
                poseDataAsset.frames[i].boneTransforms = frameData;
            }
            Debug.Log("Animation recorded with " + numFrames + " frames.");

            // After recording is complete, before the method ends we mark the scriptable object as dirty.
            EditorUtility.SetDirty(poseDataAsset);
            AssetDatabase.SaveAssets();

            animator.runtimeAnimatorController = cachedController;
        }

        public void AdvanceOneFrame()
        {
            // Increment the current frame.
            currentFrame++;

            // Calculate the new normalized time.
            float normalizedTime = currentFrame * frameNormalizedStep;
            // Clamp the normalized time to a maximum of 1 (end of clip).
            normalizedTime = Mathf.Clamp01(normalizedTime);

            // Jump to the new normalized time. This method resets the animation state.
            animator.Play("Empty", 0, normalizedTime);
            animator.Update(0f);
        }

        // Helper: computes the relative path from the recording root to a given bone.
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
    }
}