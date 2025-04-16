using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace GDFramework.Mission
{
    public abstract class MissionRequirementTemplate : MissionRequirement<object>
    {
        public abstract class MissionRequireTemplateHandle : MissionRequirementHandle<object>
        {
            protected MissionRequireTemplateHandle(MissionRequirementTemplate require) : base(require)
            {
                
            }
        }

#if UNITY_EDITOR
        public bool _unfolded;
        
        private string _title;
        
        private string _summary;
        
        private string _description;
        
        public MissionNode _node;
        
        public virtual string Title
        {
            get
            {
                _title ??= this.FetchAttribute<NameAttribute>(out var attr)
                    ? attr.name
                    : GetType().Name.SplitCamelCase();
                return _title;
            }
        }
        
        public virtual string Summary
        {
            get
            {
                _summary ??= this.FetchAttribute<DescriptionAttribute>(out var attr)
                    ? $"\"{attr.description}\""
                    : "无汇总信息";
                return _summary;
            }
        }

        public virtual string Description
        {
            get
            {
                _description ??= this.FetchAttribute<DescriptionAttribute>(out var attr)
                    ? attr.description
                    : "没有描述信息";
                return _description;
            }
        }

        public void DrawInspectorGUI()
        {
            GUILayout.BeginVertical();
            DrawTitleBar();
            if (_unfolded)
            {
                EditorGUILayout.HelpBox(Description, MessageType.None);
                OnInspectorGUI();
            }
            GUILayout.EndVertical();
        }

        protected void DrawTitleBar()
        {
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.black.WithAlpha(0.3f) : Color.white.WithAlpha(0.5f);
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                string _summaryInfo = $"<size=12><color=#a9a9a9>{Summary}</color></size>";
                GUILayout.Label(
                    "<b>" + (_unfolded ? "▼ " : "► ") + _summaryInfo + "</b>"
                    , Styles.leftLabel);
                
                if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
                    EditorUtils.OpenScriptOfType(this.GetType());
                
                if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20), GUILayout.Height(20)))
                    GetContextMenu().ShowAsContext();
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            /* unfold control */
            var titleRect = GUILayoutUtility.GetLastRect();
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && titleRect.Contains(e.mousePosition))
            {
                _unfolded = !_unfolded;
                e.Use();
            }
        }

        protected GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("复制"), false, () => CopyBuffer.SetCache(this));
            menu.AddItem(new GUIContent("重设"), false, Reset);
            if(CopyBuffer.TryGetCache<MissionRequirementTemplate>(out var cache) && cache != this && cache.GetType() == GetType())
                menu.AddItem(new GUIContent("粘贴"), false, () => MissionAttributes.CopyObjectFrom(this, cache));
            else
                menu.AddDisabledItem(new GUIContent("粘贴"));

            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("删除"), false, () => _node.DeleteRequire(this));
            menu = OnCreateContextMenu(menu);
            return menu;
        }

        /// <summary>
        /// 如果您尝试覆盖此函数
        /// 手动重置需求模板
        /// </summary>
        protected virtual void Reset()
        {
            UndoUtility.RecordObject(_node.graph, "重设请求");
            MissionAttributes.ResetObject(this);
        }

        /// <summary>
        /// 如果您尝试覆盖此函数
        /// 在上下文菜单中添加更多选项
        /// </summary>
        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;

        /// <summary> draw inspector gui </summary>
        protected virtual void OnInspectorGUI() {}
#endif
        
    }
}