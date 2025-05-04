#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace BSS.PoseBlender
{
    [CustomEditor(typeof(PoseBlenderLite))]
    public class PoseBlenderLiteEditor : Editor
    {
        // Serialized properties for easier management
        private SerializedProperty animatorProp;
        private SerializedProperty animationRootProp;
        private SerializedProperty previewInEditorProp;
        private SerializedProperty overlayPosesProp;
        private SerializedProperty lookOffsetBonesProp;
        private SerializedProperty lookVerticalOffsetProp;
        private SerializedProperty lookHorizontalOffsetProp;
        private SerializedProperty leaningOffsetProp;
        private SerializedProperty enableLookAtModeProp;
        private SerializedProperty lookAtTargetProp;

        // Property to track initialization status
        private SerializedProperty initializedProp;

        void OnEnable()
        {
            // Cache all serialized properties
            animatorProp = serializedObject.FindProperty("animator");
            animationRootProp = serializedObject.FindProperty("animationRoot");
            previewInEditorProp = serializedObject.FindProperty("previewInEditor");
            overlayPosesProp = serializedObject.FindProperty("overlayPoses");
            lookOffsetBonesProp = serializedObject.FindProperty("lookOffsetBones");
            lookVerticalOffsetProp = serializedObject.FindProperty("lookVerticalOffset");
            lookHorizontalOffsetProp = serializedObject.FindProperty("lookHorizontalOffset");
            leaningOffsetProp = serializedObject.FindProperty("leaningOffset");
            enableLookAtModeProp = serializedObject.FindProperty("enableLookAtMode"); 
            lookAtTargetProp = serializedObject.FindProperty("lookAtTarget"); 


            // Find the initialization status property
            initializedProp = serializedObject.FindProperty("initialized");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Update bone names based on referenced transforms
            UpdateBoneNames();
            UpdateOffsetBoneNames();

            // Custom header style
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField("Pose Blender Lite", headerStyle);
            EditorGUILayout.Space();

            // Check initialization status
            bool isInitialized = initializedProp.boolValue;

            // Always show initialization controls
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Auto Setup", EditorStyles.miniButton))
            {
                if (target is PoseBlenderLite blender)
                {
                    blender.AutoSetupCharacter();
                    serializedObject.Update(); // Refresh serialized object
                }
            }

            if (GUILayout.Button("Reset", EditorStyles.miniButton))
            {
                if (target is PoseBlenderLite blender)
                {
                    blender.ResetCharacter();
                    serializedObject.Update(); // Refresh serialized object
                }
            }
            EditorGUILayout.EndHorizontal();

            // Show initialization status
            if (!isInitialized)
            {
                EditorGUILayout.HelpBox("Character not initialized. Please set up Animator and Animation Root.", MessageType.Warning);
            }

            // Only show Animator and Animation Root when not initialized
            if (!isInitialized)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(animatorProp, new GUIContent("Animator", "Reference to the character's Animator component"));
                EditorGUILayout.PropertyField(animationRootProp, new GUIContent("Animation Root", "Root transform used when recording the pose"));
            }

            // If initialized, show the rest of the inspector
            if (isInitialized)
            {
                // Preview Section (Only in Edit Mode)
                if (!Application.isPlaying)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.HelpBox("Enable to see real-time pose blending in the Scene view.", MessageType.Info);
                    previewInEditorProp.boolValue = EditorGUILayout.Toggle("Preview In Editor", previewInEditorProp.boolValue);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                // If no offset bones have been added, warn the user
                if (lookOffsetBonesProp == null || lookOffsetBonesProp.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("No offset bones. Add at least one bone for offset rotation!", MessageType.Warning);
                }
                else
                {
                    // Helper Box: Calculate total offset values from all offset bones
                    float totalXMin = 0f, totalXMax = 0f;
                    float totalYMin = 0f, totalYMax = 0f;
                    float totalZMin = 0f, totalZMax = 0f;

                    // Gather all offset values
                    for (int i = 0; i < lookOffsetBonesProp.arraySize; i++)
                    {
                        SerializedProperty offsetBoneProp = lookOffsetBonesProp.GetArrayElementAtIndex(i);

                        // Assumes xMinMax, yMinMax, and zMinMax are Vector2 fields
                        SerializedProperty xMinMaxProp = offsetBoneProp.FindPropertyRelative("xMinMax");
                        float xMin = xMinMaxProp.vector2Value.x;
                        float xMax = xMinMaxProp.vector2Value.y;

                        SerializedProperty yMinMaxProp = offsetBoneProp.FindPropertyRelative("yMinMax");
                        float yMin = yMinMaxProp.vector2Value.x;
                        float yMax = yMinMaxProp.vector2Value.y;

                        SerializedProperty zMinMaxProp = offsetBoneProp.FindPropertyRelative("zMinMax");
                        float zMin = zMinMaxProp.vector2Value.x;
                        float zMax = zMinMaxProp.vector2Value.y;

                        totalXMin += xMin;
                        totalXMax += xMax;
                        totalYMin += yMin;
                        totalYMax += yMax;
                        totalZMin += zMin;
                        totalZMax += zMax;
                    }

                    // Check if everything is still zero, meaning no offsets are configured
                    bool anyNonZero =
                        Mathf.Abs(totalXMin) > 0.01f || Mathf.Abs(totalXMax) > 0.01f ||
                        Mathf.Abs(totalYMin) > 0.01f || Mathf.Abs(totalYMax) > 0.01f ||
                        Mathf.Abs(totalZMin) > 0.01f || Mathf.Abs(totalZMax) > 0.01f;

                    if (!anyNonZero)
                    {
                        // If all are zero, show an info box
                        EditorGUILayout.HelpBox("Offset Bone Totals: All offsets are 0. (No rotation configured yet.)", MessageType.Info);
                    }
                    else
                    {
                        // Evaluate each axis separately
                        bool xHasError = Mathf.Abs(totalXMin + 90f) > 0.1f || Mathf.Abs(totalXMax - 90f) > 0.1f;
                        bool yHasError = Mathf.Abs(totalYMin + 90f) > 0.1f || Mathf.Abs(totalYMax - 90f) > 0.1f;
                        bool zHasError = Mathf.Abs(totalZMin + 90f) > 0.1f || Mathf.Abs(totalZMax - 90f) > 0.1f;

                        // Show per-axis warnings if values are off
                        if (xHasError)
                        {
                            string xMsg = $"X-Axis Totals: \nMin = {totalXMin:F1}, \nMax = {totalXMax:F1} \nDesired: -90 / 90";
                            EditorGUILayout.HelpBox(xMsg, MessageType.Warning);
                        }
                        if (yHasError)
                        {
                            string yMsg = $"Y-Axis Totals \nMin = {totalYMin:F1} \nMax = {totalYMax:F1} \nDesired: -90 / 90";
                            EditorGUILayout.HelpBox(yMsg, MessageType.Warning);
                        }
                        if (zHasError)
                        {
                            string zMsg = $"Z-Axis Totals: \nMin = {totalZMin:F1}, \nMax = {totalZMax:F1} \nDesired: -90 / 90";
                            EditorGUILayout.HelpBox(zMsg, MessageType.Warning);
                        }

                        // If no errors, show a success message
                        if (!xHasError && !yHasError && !zHasError)
                        {
                            string successMsg = "Looks good! The total offsets match the desired -90/90 range.";
                            EditorGUILayout.HelpBox(successMsg, MessageType.Info);
                        }
                    }
                }

                // Offset Bones Setup
                EditorGUILayout.PropertyField(lookOffsetBonesProp, new GUIContent("Offset Bones Setup"), true);
                EditorGUILayout.Space();

                // Look At Target Mode
                EditorGUILayout.LabelField("Look At Target Mode", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(enableLookAtModeProp, new GUIContent("Enable Look At Target", "Enable to make the character look at a target transform"));
                if (enableLookAtModeProp.boolValue)
                {
                    EditorGUILayout.PropertyField(lookAtTargetProp, new GUIContent("Look At Target", "The transform the character should look at"));
                }
                EditorGUILayout.Space();

                // Character Offsets
                EditorGUILayout.LabelField("Character Offsets", EditorStyles.boldLabel);

                // Character Offsets
                EditorGUILayout.LabelField("Character Offsets", EditorStyles.boldLabel);
                EditorGUILayout.Slider(lookVerticalOffsetProp, -90f, 90f, new GUIContent("Look Vertical"));
                EditorGUILayout.Slider(lookHorizontalOffsetProp, -90f, 90f, new GUIContent("Look Horizontal"));
                EditorGUILayout.Slider(leaningOffsetProp, -90f, 90f, new GUIContent("Leaning"));
                EditorGUILayout.Space();

                // Overlay Poses
                EditorGUILayout.PropertyField(overlayPosesProp, new GUIContent("Overlay Poses"), true);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("You can easily create bone chains when you have added your first pose", MessageType.Info);

                // If there is at least one overlay pose, show the "Open Bone Chain Creator" button
                if (overlayPosesProp.arraySize > 0)
                {
                    if (GUILayout.Button("Open Bone Chain Creator"))
                    {
                        BoneChainCreatorWindow.OpenWindow((PoseBlenderLite)target);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Iterates through each overlay pose, then each bone chain, and each BoneRotationSettings,
        /// updating the boneName field to the name of the referenced bone transform.
        /// </summary>
        private void UpdateBoneNames()
        {
            if (overlayPosesProp == null) return;

            for (int i = 0; i < overlayPosesProp.arraySize; i++)
            {
                SerializedProperty poseProp = overlayPosesProp.GetArrayElementAtIndex(i);
                SerializedProperty boneChainsProp = poseProp.FindPropertyRelative("boneChains");
                if (boneChainsProp == null) continue;

                for (int j = 0; j < boneChainsProp.arraySize; j++)
                {
                    SerializedProperty chainProp = boneChainsProp.GetArrayElementAtIndex(j);
                    SerializedProperty bonesProp = chainProp.FindPropertyRelative("bones");
                    if (bonesProp == null) continue;

                    for (int k = 0; k < bonesProp.arraySize; k++)
                    {
                        SerializedProperty boneSettingProp = bonesProp.GetArrayElementAtIndex(k);
                        SerializedProperty boneProp = boneSettingProp.FindPropertyRelative("bone");
                        SerializedProperty boneNameProp = boneSettingProp.FindPropertyRelative("boneName");

                        if (boneProp != null && boneProp.objectReferenceValue != null)
                        {
                            Transform boneTransform = (Transform)boneProp.objectReferenceValue;
                            if (boneNameProp.stringValue != boneTransform.name)
                            {
                                boneNameProp.stringValue = boneTransform.name;
                            }
                        }
                        else
                        {
                            boneNameProp.stringValue = "";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Iterates through each offset bone in lookOffsetBones and updates its boneName.
        /// </summary>
        private void UpdateOffsetBoneNames()
        {
            if (lookOffsetBonesProp == null) return;

            for (int i = 0; i < lookOffsetBonesProp.arraySize; i++)
            {
                SerializedProperty offsetBoneProp = lookOffsetBonesProp.GetArrayElementAtIndex(i);
                SerializedProperty boneProp = offsetBoneProp.FindPropertyRelative("bone");
                SerializedProperty boneNameProp = offsetBoneProp.FindPropertyRelative("boneName");

                if (boneProp != null && boneProp.objectReferenceValue != null)
                {
                    Transform boneTransform = (Transform)boneProp.objectReferenceValue;
                    if (boneNameProp.stringValue != boneTransform.name)
                    {
                        boneNameProp.stringValue = boneTransform.name;
                    }
                }
                else
                {
                    boneNameProp.stringValue = "";
                }
            }
        }
    }
}
#endif
