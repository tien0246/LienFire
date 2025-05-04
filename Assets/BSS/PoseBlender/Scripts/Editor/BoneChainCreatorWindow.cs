using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static BSS.PoseBlender.PoseBlenderLite;

namespace BSS.PoseBlender
{
    public class BoneChainCreatorWindow : EditorWindow
    {
        // Reference to your PoseBlenderLite component.
        private PoseBlenderLite targetScript;

        // The name for the new chain.
        private string newChainName = "New Bone Chain";

        // Dropdown index for selecting which overlay pose to add/edit the bone chain.
        private int selectedOverlayPoseIndex = 0;

        // Dropdown index for selecting an existing bone chain in editing mode.
        private int selectedBoneChainIndex = 0;

        // Action mode: 0 = Create New, 1 = Edit Existing.
        private int actionModeIndex = 0;
        private string[] actionModes = new string[] { "Create New Bone Chain", "Edit Existing Bone Chain" };

        // Foldout and selection states for the hierarchy.
        private Dictionary<Transform, bool> foldoutStates = new Dictionary<Transform, bool>();
        private Dictionary<Transform, bool> selectionStates = new Dictionary<Transform, bool>();

        // HashSet to track which bones have been toggled during the current mouse drag.
        private HashSet<Transform> toggledDragSet = new HashSet<Transform>();

        // Scroll position for the window.
        private Vector2 scrollPos;

        // A convenience property to quickly access the root transform.
        private Transform Root => targetScript != null ? targetScript.animationRoot : null;

        // Open the window from the Window menu.
        [MenuItem("Window/Pose Blender Lite/Bone Chain Creator Lite")]
        private static void ShowWindow()
        {
            var window = GetWindow<BoneChainCreatorWindow>("Bone Chain Creator Lite");

            // Try to auto-assign the targetScript if a GameObject is selected.
            if (Selection.activeGameObject != null)
            {
                var poseBlender = Selection.activeGameObject.GetComponent<PoseBlenderLite>();
                if (poseBlender != null)
                {
                    window.targetScript = poseBlender;
                }
            }
            window.Show();
        }

        // Alternatively, you can open it from your component's context menu:
        public static void OpenWindow(PoseBlenderLite script)
        {
            var window = GetWindow<BoneChainCreatorWindow>("Bone Chain Creator Lite");
            window.targetScript = script;
            window.Show();
        }

        private void OnGUI()
        {
            // Clear the drag tracking set when the mouse button is released.
            if (Event.current.type == EventType.MouseUp)
            {
                toggledDragSet.Clear();
            }

            if (targetScript == null)
            {
                EditorGUILayout.HelpBox("No target script assigned.\nSelect a GameObject with a PoseBlenderLite component.", MessageType.Error);
                return;
            }

            if (Root == null)
            {
                EditorGUILayout.HelpBox("No animationRoot assigned in the target script.", MessageType.Warning);
                return;
            }

            // Begin the scroll view.
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.LabelField("Bone Chain Creator / Editor", EditorStyles.boldLabel);

            // Action mode selection.
            actionModeIndex = EditorGUILayout.Popup("Action", actionModeIndex, actionModes);

            // Dropdown for overlay pose selection.
            if (targetScript.overlayPoses == null || targetScript.overlayPoses.Count == 0)
            {
                EditorGUILayout.HelpBox("No overlay poses available in the target script.", MessageType.Warning);
            }
            else
            {
                string[] overlayPoseNames = new string[targetScript.overlayPoses.Count];
                for (int i = 0; i < targetScript.overlayPoses.Count; i++)
                {
                    overlayPoseNames[i] = string.IsNullOrEmpty(targetScript.overlayPoses[i].animationName)
                        ? $"Overlay {i}"
                        : targetScript.overlayPoses[i].animationName;
                }
                selectedOverlayPoseIndex = EditorGUILayout.Popup("Select Overlay Pose", selectedOverlayPoseIndex, overlayPoseNames);
            }

            // If editing mode is selected, show existing bone chains.
            if (actionModeIndex == 1)
            {
                var boneChains = targetScript.overlayPoses[selectedOverlayPoseIndex].boneChains;
                if (boneChains == null || boneChains.Count == 0)
                {
                    EditorGUILayout.HelpBox("No existing bone chains in the selected overlay pose.", MessageType.Warning);
                }
                else
                {
                    string[] boneChainNames = new string[boneChains.Count];
                    for (int i = 0; i < boneChains.Count; i++)
                    {
                        boneChainNames[i] = boneChains[i].chainName;
                    }
                    selectedBoneChainIndex = EditorGUILayout.Popup("Select Bone Chain", selectedBoneChainIndex, boneChainNames);
                    if (GUILayout.Button("Load Selected Bone Chain"))
                    {
                        // Clear current selection states.
                        selectionStates.Clear();
                        // Load bone chain bones into selection states.
                        foreach (var chainBone in boneChains[selectedBoneChainIndex].bones)
                        {
                            if (chainBone.bone != null)
                            {
                                selectionStates[chainBone.bone] = true;
                            }
                        }
                        // Update chain name field.
                        newChainName = boneChains[selectedBoneChainIndex].chainName;
                    }
                }
            }

            // Chain name field.
            newChainName = EditorGUILayout.TextField("Chain Name", newChainName);

            EditorGUILayout.Space();

            // Toolbar for selection and foldout actions.
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                SelectAllBones(Root);
            }
            if (GUILayout.Button("Clear Selection"))
            {
                ClearSelection(Root);
            }
            if (GUILayout.Button("Expand All"))
            {
                SetAllFoldouts(Root, true);
            }
            if (GUILayout.Button("Collapse All"))
            {
                SetAllFoldouts(Root, false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button(actionModeIndex == 0 ? "Create Bone Chain from Selection" : "Update Bone Chain from Selection"))
            {
                ApplyBoneChainChanges();
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select Bones:", EditorStyles.boldLabel);

            // Draw the entire hierarchy under animationRoot.
            DrawBoneHierarchy(Root, 0);

            // End the scroll view.
            EditorGUILayout.EndScrollView();
        }

        private void DrawBoneHierarchy(Transform current, int indentLevel)
        {
            if (current == null)
                return;

            EditorGUILayout.BeginHorizontal();

            // Draw the toggle for selection at a fixed position on the left
            Rect toggleRect = GUILayoutUtility.GetRect(20, EditorGUIUtility.singleLineHeight, GUILayout.Width(20));
            bool toggled = selectionStates.ContainsKey(current) ? selectionStates[current] : false;
            Event e = Event.current;
            if (toggleRect.Contains(e.mousePosition))
            {
                // Toggle on mouse down.
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    toggled = !toggled;
                    toggledDragSet.Add(current);
                    e.Use();
                }
                // Toggle on mouse drag (only once per drag).
                else if (e.type == EventType.MouseDrag && e.button == 0 && !toggledDragSet.Contains(current))
                {
                    toggled = !toggled;
                    toggledDragSet.Add(current);
                    e.Use();
                }
            }
            toggled = EditorGUI.Toggle(toggleRect, toggled);
            selectionStates[current] = toggled;

            // Now add space for indentation of the bone name only
            GUILayout.Space(indentLevel * 15); // Adjust the multiplier as needed for proper spacing

            // Draw the foldout arrow and bone name with proper indentation
            EditorGUI.indentLevel = 0; // Reset indentation since we're handling it manually
            foldoutStates[current] = EditorGUILayout.Foldout(foldoutStates.ContainsKey(current) ? foldoutStates[current] : false, current.name, true);

            EditorGUILayout.EndHorizontal();

            if (foldoutStates[current])
            {
                for (int i = 0; i < current.childCount; i++)
                {
                    DrawBoneHierarchy(current.GetChild(i), indentLevel + 1);
                }
            }
        }


        private void ApplyBoneChainChanges()
        {
            // Gather selected bones.
            List<Transform> selectedBones = new List<Transform>();
            foreach (var kvp in selectionStates)
            {
                if (kvp.Value)
                    selectedBones.Add(kvp.Key);
            }

            if (selectedBones.Count == 0)
            {
                EditorUtility.DisplayDialog("No Bones Selected", "Please select at least one bone.", "OK");
                return;
            }

            if (targetScript.overlayPoses != null && targetScript.overlayPoses.Count > 0)
            {
                if (actionModeIndex == 0)
                {
                    // Create new bone chain.
                    var newChain = new BoneChain { chainName = newChainName };
                    foreach (var bone in selectedBones)
                    {
                        var settings = new BoneRotationSettings
                        {
                            boneName = bone.name,
                            bone = bone,
                            blendWeight = 1f,
                            rotationOffset = Vector3.zero
                        };
                        newChain.bones.Add(settings);
                    }
                    Undo.RecordObject(targetScript, "Add Bone Chain");
                    targetScript.overlayPoses[selectedOverlayPoseIndex].boneChains.Add(newChain);
                }
                else
                {
                    // Edit existing bone chain.
                    var boneChains = targetScript.overlayPoses[selectedOverlayPoseIndex].boneChains;
                    if (boneChains == null || boneChains.Count == 0 || selectedBoneChainIndex >= boneChains.Count)
                    {
                        EditorUtility.DisplayDialog("No Bone Chain", "No bone chain available for editing.", "OK");
                        return;
                    }
                    var chainToEdit = boneChains[selectedBoneChainIndex];
                    chainToEdit.chainName = newChainName;
                    chainToEdit.bones.Clear();
                    foreach (var bone in selectedBones)
                    {
                        var settings = new BoneRotationSettings
                        {
                            boneName = bone.name,
                            bone = bone,
                            blendWeight = 1f,
                            rotationOffset = Vector3.zero
                        };
                        chainToEdit.bones.Add(settings);
                    }
                    Undo.RecordObject(targetScript, "Update Bone Chain");
                }
                EditorUtility.SetDirty(targetScript);
            }
            else
            {
                EditorUtility.DisplayDialog("No Overlay Poses", "There are no overlay poses available to add or edit the bone chain.", "OK");
            }

            // Clear selection.
            selectionStates.Clear();

            // Optionally close the window.
            Close();
        }

        private void SelectAllBones(Transform current)
        {
            if (current == null)
                return;

            selectionStates[current] = true;
            for (int i = 0; i < current.childCount; i++)
            {
                SelectAllBones(current.GetChild(i));
            }
        }

        private void ClearSelection(Transform current)
        {
            if (current == null)
                return;

            selectionStates[current] = false;
            for (int i = 0; i < current.childCount; i++)
            {
                ClearSelection(current.GetChild(i));
            }
        }

        private void SetAllFoldouts(Transform current, bool state)
        {
            if (current == null)
                return;

            foldoutStates[current] = state;
            for (int i = 0; i < current.childCount; i++)
            {
                SetAllFoldouts(current.GetChild(i), state);
            }
        }
    }
}
