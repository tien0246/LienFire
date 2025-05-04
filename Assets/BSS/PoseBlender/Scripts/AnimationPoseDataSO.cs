using System.Collections.Generic;
using UnityEngine;

namespace BSS.PoseBlender
{
    [CreateAssetMenu(fileName = "NewAnimationPoseData", menuName = "BSS/Animation/Animation Pose Data")]
    public class AnimationPoseDataSO : ScriptableObject
    {
        [System.Serializable]
        public class BoneTransformData
        {
            public string boneName;
            public string bonePath; // Relative path from the recording root.
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;

            public bool isPose = false;

            [Range(0f, 1f)]
            public float resetBlendWeight = 1f;
        }


        public bool loop = false;

        public int fps = 30;


        [System.Serializable]
        public class FrameData
        {
            public List<BoneTransformData> boneTransforms = new List<BoneTransformData>();
        }

        public FrameData[] frames = new FrameData[0];
    }
}