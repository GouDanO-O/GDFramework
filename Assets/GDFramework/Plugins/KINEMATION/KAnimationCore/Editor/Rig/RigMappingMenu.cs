// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Runtime.Rig;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Editor.Rig
{
    public class StringListWidget
    {
        public List<string> list = new();
        private ReorderableList _reorderableOptionsList;

        public void Initialize(string listName)
        {
            _reorderableOptionsList = new ReorderableList(list, typeof(string), true,
                true, true, true);

            _reorderableOptionsList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, listName); };

            _reorderableOptionsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                list[index] = EditorGUI.TextField(rect, list[index]);
            };

            _reorderableOptionsList.onAddCallback = (ReorderableList list) => { this.list.Add(""); };

            _reorderableOptionsList.onRemoveCallback = (ReorderableList list) =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        public void Render()
        {
            _reorderableOptionsList.DoLayoutList();
        }
    }

    public class RigMappingWindow : EditorWindow
    {
        private KRig _rigAsset;
        private GameObject _root;
        private StringListWidget _entriesToAvoid;

        public static RigMappingWindow CreateWindow()
        {
            var window = GetWindow<RigMappingWindow>(false, "Rig Mapping", true);
            return window;
        }

        private static string GetProjectWindowFolder()
        {
            // Use reflection to access the internal ProjectWindowUtil.GetActiveFolderPath method
            var projectWindowUtilType = typeof(ProjectWindowUtil);

            var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (getActiveFolderPathMethod != null)
            {
                var result = getActiveFolderPathMethod.Invoke(null, null);
                if (result != null) return result.ToString();
            }

            return "No folder is currently opened.";
        }

        private void TraverseHierarchy(Transform root, ref KRigElementChain chain, KRig rig)
        {
            var element = rig.rigHierarchy.Find(item => item.name.Equals(root.name));
            chain.elementChain.Add(element);

            // Filter child bones from corrections, twists and IKs.
            var fkBoneIndexes = new List<int>();
            for (var i = 0; i < root.childCount; i++)
            {
                var childName = root.GetChild(i).name.ToLower();

                var bSkipIteration = false;
                foreach (var entry in _entriesToAvoid.list)
                {
                    if (string.IsNullOrEmpty(entry)) continue;
                    if (childName.Contains(entry.ToLower()))
                    {
                        bSkipIteration = true;
                        break;
                    }
                }

                if (bSkipIteration) continue;

                fkBoneIndexes.Add(i);
            }

            // If no extra branches, traverse the old chain.
            if (fkBoneIndexes.Count == 1)
            {
                TraverseHierarchy(root.GetChild(fkBoneIndexes[0]), ref chain, rig);
                return;
            }

            // If we have branches, create new chains and start traversing them.
            foreach (var boneIndex in fkBoneIndexes)
            {
                var chainName = root.GetChild(boneIndex).name;
                var newChain = new KRigElementChain()
                {
                    chainName = chainName,
                    elementChain = new List<KRigElement>()
                };

                rig.rigElementChains.Add(newChain);
                TraverseHierarchy(root.GetChild(boneIndex), ref newChain, rig);
            }
        }

        private void MapRigChains(GameObject root)
        {
            if (root == null)
            {
                Debug.LogWarning("RigMappingWindow: Selected GameObject is NULL!");
                return;
            }

            var rigComponent = root.GetComponent<KRigComponent>();
            if (rigComponent == null)
            {
                rigComponent = root.AddComponent<KRigComponent>();
                rigComponent.RefreshHierarchy();
            }

            if (_rigAsset == null)
                _rigAsset = CreateInstance<KRig>();
            else
                _rigAsset.rigElementChains.Clear();

            _rigAsset.ImportRig(rigComponent);

            var newChain = new KRigElementChain()
            {
                chainName = root.name,
                elementChain = new List<KRigElement>()
            };

            _rigAsset.rigElementChains.Add(newChain);
            TraverseHierarchy(root.transform, ref newChain, _rigAsset);

            if (EditorUtility.IsPersistent(_rigAsset))
            {
                EditorUtility.SetDirty(_rigAsset);
                AssetDatabase.SaveAssetIfDirty(_rigAsset);
                return;
            }

            Undo.RegisterCreatedObjectUndo(_rigAsset, "Create Rig Asset");
            var path = $"{GetProjectWindowFolder()}/Rig_{root.transform.root.name}.asset";
            AssetDatabase.CreateAsset(_rigAsset, AssetDatabase.GenerateUniqueAssetPath(path));
        }

        private void OnEnable()
        {
            _entriesToAvoid = new StringListWidget();
            _entriesToAvoid.Initialize("Bone Names to Avoid");

            _entriesToAvoid.list.Add("twist");
            _entriesToAvoid.list.Add("correct");

            _root = Selection.activeObject as GameObject;
        }

        public void OnGUI()
        {
            EditorGUILayout.HelpBox("If empty, a new asset will be created.", MessageType.Info);
            _rigAsset = (KRig)EditorGUILayout.ObjectField("Rig Asset", _rigAsset, typeof(KRig),
                false);

            _root = (GameObject)EditorGUILayout.ObjectField("Root Bone", _root, typeof(GameObject),
                true);

            _entriesToAvoid.Render();

            if (GUILayout.Button("Create Rig Mapping")) MapRigChains(_root);
        }
    }

    public class RigMappingMenu
    {
        private const string RigItemName = "GameObject/Auto Rig Mapping";

        [MenuItem(RigItemName, true)]
        private static bool ValidateCreateRigMapping()
        {
            return Selection.activeObject is GameObject;
        }

        [MenuItem(RigItemName)]
        private static void CreateRigMapping()
        {
            var window = RigMappingWindow.CreateWindow();
            window.Show();
        }
    }
}