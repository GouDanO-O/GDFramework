// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Editor.Misc;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Editor.Rig
{
    public delegate void OnTreeItemClicked(string displayName, int index);

    public delegate void OnSelectionChanged(List<(string, int)> selectedItems);

    public class RigTreeView : TreeView
    {
        public OnTreeItemClicked onItemClicked;
        public OnSelectionChanged onSelectionChanged;

        public float singleRowHeight = 0f;
        public bool drawToggleBoxes;

        private List<TreeViewItem> _treeItems;
        private (string, int)[] _originalItems;

        public RigTreeView(TreeViewState state) : base(state)
        {
            _treeItems = new List<TreeViewItem>();
            Reload();
        }

        public void InitializeTreeItems(ref (string, int)[] items)
        {
            _treeItems.Clear();

            var count = items.Length;
            _originalItems = new (string, int)[count];

            var depthOffset = drawToggleBoxes ? 1 : 0;
            for (var i = 0; i < count; i++)
                _treeItems.Add(new TreeViewItem(i + 1, items[i].Item2 + depthOffset, items[i].Item1));

            items.CopyTo(_originalItems, 0);
        }

        public void Filter(string query)
        {
            var depthOffset = drawToggleBoxes ? 1 : 0;

            _treeItems.Clear();
            query = query.ToLower().Trim();

            var count = _originalItems.Length;
            for (var i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(query))
                {
                    _treeItems.Add(new TreeViewItem(i + 1, _originalItems[i].Item2 + depthOffset,
                        _originalItems[i].Item1));
                    continue;
                }

                if (!_originalItems[i].Item1.ToLower().Trim().Contains(query)) continue;

                _treeItems.Add(new TreeViewItem(i + 1, depthOffset, _originalItems[i].Item1));
            }

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            // 0 is the root ID, -1 means the root has no parent
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            // Utility method to setup the parent/children relationship
            SetupParentsAndChildrenFromDepths(root, _treeItems);

            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var rect = args.rowRect;

            var darkGrey = new Color(0.2f, 0.2f, 0.2f);
            var lightGrey = new Color(0.25f, 0.25f, 0.25f);
            var blue = new Color(115f / 255f, 147f / 255f, 179f / 255f, 0.25f);

            var color = args.selected ? blue : args.row % 2 == 0 ? lightGrey : darkGrey;

            EditorGUI.DrawRect(rect, color);

            if (drawToggleBoxes)
            {
                GUI.enabled = false;
                EditorGUI.Toggle(rect, args.selected);
                GUI.enabled = true;
            }

            singleRowHeight = rowHeight;

            if (!drawToggleBoxes)
            {
                var buttonRect = args.rowRect;
                var indent = GetContentIndent(args.item);
                buttonRect.x += indent;

                if (GUI.Button(buttonRect, args.item.displayName, EditorStyles.label))
                {
                    var displayName = _originalItems[args.item.id - 1].Item1;
                    var index = args.item.id - 1;
                    onItemClicked?.Invoke(displayName, index);
                }

                return;
            }

            base.RowGUI(args);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (!drawToggleBoxes) return;

            var selectedItems = new List<(string, int)>();

            foreach (var selectedId in selectedIds)
            {
                var displayName = _originalItems[selectedId - 1].Item1;
                var index = selectedId - 1;
                selectedItems.Add((displayName, index));
            }

            onSelectionChanged?.Invoke(selectedItems);
        }
    }

    public class RigTreeWidget : IEditorTool
    {
        public RigTreeView rigTreeView;
        private TreeViewState _rigTreeViewState;

        public RigTreeWidget()
        {
            _rigTreeViewState = new TreeViewState();
            rigTreeView = new RigTreeView(_rigTreeViewState);
        }

        public void Refresh(ref (string, int)[] items)
        {
            rigTreeView.InitializeTreeItems(ref items);
            rigTreeView.Reload();
            rigTreeView.ExpandAll();
        }

        public void Render()
        {
            var maxHeight = rigTreeView.singleRowHeight + rigTreeView.totalHeight;
            var height = Mathf.Max(rigTreeView.singleRowHeight * 2f, maxHeight);

            EditorGUILayout.BeginHorizontal();
            var parentRect = GUILayoutUtility.GetRect(0f, 0f, 0f, height);
            EditorGUILayout.EndHorizontal();

            var padding = 7f;

            GUI.Box(parentRect, "", EditorStyles.helpBox);

            parentRect.x += padding;
            parentRect.y += padding;

            parentRect.width -= 2f * padding;
            parentRect.height -= 2f * padding;

            rigTreeView.OnGUI(parentRect);
        }
    }
}