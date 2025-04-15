using System;
using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion;

namespace GDFramework.Mission
{
    public class ActionNode : BaseNode
    {
        public override bool allowAsPrime => false;
        
        /// <summary>
        /// 禁止超过连接数量
        /// </summary>
        public override int maxOutConnections => 0;

        public override Alignment2x2 commentsAlignment
        {
            get
            {
                return Alignment2x2.Bottom;
            }
        }

        public override Alignment2x2 iconAlignment
        {
            get
            {
                return Alignment2x2.Default;
            }
        }

        [SerializeField] private BaseAction action;
        
        /// <summary>
        /// 执行此节点
        /// </summary>
        public void Execute() =>
            action?.Execute();

#if UNITY_EDITOR
        
        /// <summary>
        /// 删除给定动作
        /// </summary>
        /// <param name="other"></param>
        public void DeleteAction(BaseAction other)
        {
            /* remove action */
            if (other == action)
            {
                UndoUtility.RecordObject(graph, "移除行为");
                action = null;
                return;
            }
            
            /* remove action from group */
            if (action is ActionGroup group)
            {
                UndoUtility.RecordObject(graph, "移除行为");
                group.RemoveAction(other);
                if (group.Count == 1)
                    action = group.First;
            }
            
            /* do nothing */
        }
        
        /// <summary>
        /// 添加行为
        /// </summary>
        /// <param name="newAction"></param>
        public void AddAction(BaseAction newAction)
        {
            if (action == null)
            {
                UndoUtility.RecordObject(graph, "指定行为");
                action = newAction;
            }
            else
            {
                UndoUtility.RecordObject(graph, "行为组为组合");
                if (action is ActionGroup group)
                    group.AddAction(newAction);
                else
                {
                    var newGroup = new ActionGroup
                    {
                        _node = this,
                        _unfolded = true
                    };
                    newGroup.AddAction(action);
                    newGroup.AddAction(newAction);
                    action = newGroup;
                }
            }
        }

        public void UnGrouped()
        {
            if (action is ActionGroup group)
            {
                UndoUtility.RecordObject(graph, "行为取消组合");
                action = group.allActions.First();
                action._node = this;
            }
        }


        protected override void OnNodeInspectorGUI()
        {
            GUI.backgroundColor = Colors.lightBlue;
            var baseType = typeof(BaseAction);
            var label = "指定 " + baseType.Name.SplitCamelCase();
            if ( GUILayout.Button(label) ) 
            {
                Action<Type> TaskTypeSelected = (t) =>
                {
                    var newAction = (BaseAction)Activator.CreateInstance(t);
                    newAction._node = this;
                    AddAction(newAction);
                };

                var menu = EditorUtils.GetTypeSelectionMenu(baseType, TaskTypeSelected);
                if (CopyBuffer.TryGetCache<BaseAction>(out var copiedAction))
                {
                    menu.AddSeparator("/");
                    menu.AddItem(new GUIContent($"粘贴 {copiedAction.Summary}"), false, () => {
                        AddAction(MissionAttributes.CopyObject(copiedAction));
                    });
                }
                    
                menu.ShowAsBrowser(label, baseType);
            }

            GUI.backgroundColor = Color.white;
            
            if (action != null)
            {
                action.DrawInspector();
            }
        }

        protected override void OnNodeGUI()
        {
            GUILayout.BeginVertical(Styles.roundedBox);
            if (action is null)
            {
                GUILayout.Label("<i><color=#969696>没有指定行为</color></i>");
            }
            else
            {
                GUILayout.Label(action.Summary);
            }
            GUILayout.EndVertical();
        }
#endif
    }
}