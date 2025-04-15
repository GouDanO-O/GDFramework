using UnityEngine;
using NodeCanvas.Framework;
using System;
using ParadoxNotion.Design;

namespace GDFramework.Mission
{
    public class BaseConnection : Connection
    {
        [SerializeField] private bool hasCondition;
        
        [SerializeField] private BaseCondition _condition;
        
        public bool IsAvailable
        {
            get
            {
                if (!isActive) return false;
                if (!hasCondition || _condition == null) return true;
                return _condition.IsConditionMet;
            }
        }
        
#if UNITY_EDITOR
        protected override string GetConnectionInfo()
        {
            if(!hasCondition)return string.Empty;
            return _condition == null ? "没有条件" : _condition.Summary;
        }

        protected override void OnConnectionInspectorGUI()
        {
            base.OnConnectionInspectorGUI();
            hasCondition = UnityEditor.EditorGUILayout.Toggle("条件", hasCondition);
            if(!hasCondition)return;
            
            // Draw the condition field
            if(_condition == null)
            {
                if(GUILayout.Button("添加条件"))
                {
                    Action<Type> OnConditionSelected = (type) =>
                    {
                        UndoUtility.RecordObject(graph, "添加条件");
                        _condition = (BaseCondition)Activator.CreateInstance(type);
                    };

                    var menu = EditorUtils.GetTypeSelectionMenu(typeof(BaseConnection), OnConditionSelected);
                    menu.ShowAsBrowser("选择条件");
                }
            }
            else
            {
                _condition.DrawInspector();
                if(GUILayout.Button("移除条件"))
                {
                    _condition = null;
                }
            }
        }
#endif
    }
}