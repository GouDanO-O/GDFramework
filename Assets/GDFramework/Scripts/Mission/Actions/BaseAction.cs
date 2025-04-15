using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace GDFramework.Mission
{
    public abstract class BaseAction : MissionChainObject
    {
        /// <summary>
        /// 使用当前参数执行操作
        /// </summary>
        public abstract void Execute();
        
#if UNITY_EDITOR
        public ActionNode _node;

        protected override GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("打开脚本"), false, () => EditorUtils.OpenScriptOfType(this.GetType()));
            menu.AddItem(new GUIContent("复制"), false, () => { CopyBuffer.SetCache(this); });
            if (CopyBuffer.TryGetCache<BaseAction>(out var copiedAction) &&
                this.GetType().IsInstanceOfType(copiedAction))
                menu.AddItem(new GUIContent("粘贴"), false, () =>
                {
                    UndoUtility.RecordObject(_node.graph, "粘贴行动");
                    MissionAttributes.CopyObjectFrom(this, copiedAction);
                });
            menu.AddItem(new GUIContent("重置"), false, () =>
            {
                UndoUtility.RecordObject(_node.graph, "重置动作");
                Reset();
            });
            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("删除"), false, () =>
            {
                UndoUtility.RecordObject(_node.graph, "删除动作");
                _node.DeleteAction(this);
            });
            menu.AddSeparator("/");
            return OnCreateContextMenu(menu);
        }

        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;
#endif
    }
}