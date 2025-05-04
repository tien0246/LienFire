#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace BSS.PoseBlender
{
    [CustomEditor(typeof(PoseRecorder))]
    public class PoseRecorderEditor : Editor
    {
        // Cached serialized properties
        private SerializedProperty animatorProp;
        private SerializedProperty recordingRootProp;
        private SerializedProperty initializedProp;
        private SerializedProperty poseDataAssetProp;
        private SerializedProperty controllerForPosingProp;
        private SerializedProperty overrideControllerForPosingProp;
        private SerializedProperty clipProp;

        private void OnEnable()
        {
            animatorProp = serializedObject.FindProperty("animator");
            recordingRootProp = serializedObject.FindProperty("recordingRoot");
            initializedProp = serializedObject.FindProperty("Initialized");
            poseDataAssetProp = serializedObject.FindProperty("poseDataAsset");
            controllerForPosingProp = serializedObject.FindProperty("controllerForPosing");
            overrideControllerForPosingProp = serializedObject.FindProperty("overrideControllerForPosing");
            clipProp = serializedObject.FindProperty("clip");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // If in play mode, show an informational message and exit.
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Pose editor does not function during runtime. Please exit play mode to use the pose recorder.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Pose Recorder Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Always show Auto Setup and Reset buttons at the top.
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Auto Setup", EditorStyles.miniButton))
            {
                ((PoseRecorder)target).Initialize();
                serializedObject.Update(); // Refresh the serialized object after setup.
            }
            if (GUILayout.Button("Reset", EditorStyles.miniButton))
            {
                ((PoseRecorder)target).Reset();
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // Check initialization status.
            bool isInitialized = initializedProp.boolValue;

            if (!isInitialized)
            {
                EditorGUILayout.HelpBox("Character not initialized. Please set up Animator and Recording Root.", MessageType.Warning);
                EditorGUILayout.Space();
                // Show only the fields required for initialization.
                EditorGUILayout.PropertyField(animatorProp, new GUIContent("Animator", "Reference to the character's Animator component"));
                EditorGUILayout.PropertyField(recordingRootProp, new GUIContent("Recording Root", "Root transform used for recording poses"));
            }
            else
            {
                // Once initialized, show the rest of the properties
                EditorGUILayout.PropertyField(controllerForPosingProp, new GUIContent("Controller For Posing"));
                EditorGUILayout.PropertyField(overrideControllerForPosingProp, new GUIContent("Override Controller For Posing"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(poseDataAssetProp, new GUIContent("Pose Data Asset"));
                EditorGUILayout.PropertyField(clipProp, new GUIContent("Animation Clip"));

                // If no poseDataAsset is assigned, let the user know one will be created automatically
                if (poseDataAssetProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(
                        "No Pose Data Asset is currently assigned. " +
                        "A new one will be created automatically if you record an animation without assigning one.",
                        MessageType.Info
                    );
                }
            }

            EditorGUILayout.Space();

            if (initializedProp.boolValue == true)
                if (GUILayout.Button("Record Animation"))
                    ((PoseRecorder)target).RecordAnimationPose();
                

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}