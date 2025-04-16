using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDFramework.Mission
{
    [System.Serializable]
    public abstract class MissionChainObject
    {
        #if UNITY_EDITOR

        public bool _unfolded;
        private string _title;
        private string _summary;
        private string _description;

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
                    : "无描述信息";
                return _description;
            }
        }

        public virtual void DrawInspector()
        {
            DrawTitleBar();
            if (_unfolded)
            {
                OnInspectorGUI();
            }
        }

        protected virtual string TitleBarLabel
        {
            get
            {
                var summaryInfo = _unfolded ? string.Empty: $"\n{Summary}";
                return $"<size=12><b>{Title}</b></size><i>{summaryInfo}</i>";
            }
        }


        /// <summary>
        /// 绘制当前节点的标题栏
        /// </summary>
        protected virtual void DrawTitleBar()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("<b>" + (_unfolded ? "▼ " : "► ") + "</b>" + TitleBarLabel, Styles.leftLabel);
                
                if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
                    EditorUtils.OpenScriptOfType(this.GetType());
                
                if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20),
                        GUILayout.Height(20)))
                    GetContextMenu().ShowAsContext();
            }
            GUILayout.EndHorizontal();
            
            var titleRect = GUILayoutUtility.GetLastRect();
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && titleRect.Contains(e.mousePosition))
            {
                _unfolded = !_unfolded;
                e.Use();
            }
        }
        
        /// <summary>
        /// 重置当前节点的参数
        /// </summary>
        public virtual void Reset() => MissionAttributes.ResetObject(this);

        protected virtual void OnInspectorGUI(){}
        protected virtual GenericMenu GetContextMenu() => 
            new GenericMenu();
#endif
    }
}