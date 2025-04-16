using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter.Tree;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace GDFramework.Mission
{
 [ParadoxNotion.Design.Icon("Action"), Color("b1d480"), Name("Mission")]
    [Description("setup a new mission")]
    public class MissionNode : BaseNode
    {
        public override bool allowAsPrime => true;

        [SerializeField] private readonly List<MissionRequirementTemplate> _requires =
            new List<MissionRequirementTemplate>();
        
        [SerializeField] private EMissionRequireMode _mode;

        /// <summary>
        /// create mission prototype
        /// </summary>
        /// <returns></returns>
        public MissionPrototype<object> MissionProto
        {
            get
            {
                var proto = new MissionPrototype<object>(MissionId, _requires.ToArray(), null, _mode);
                return proto;
            }
        }

        public string MissionId => $"{graph.name}.{base.UID}";

#if UNITY_EDITOR

        /// <summary>
        /// remove given require from current list
        /// </summary>
        public void DeleteRequire(MissionRequirementTemplate require)
        {
            /* do safe check before record undo action */
            if(_requires.Contains(require))
            {
                UndoUtility.RecordObject(graph, "Require Deleted");
                _requires.Remove(require);
            }
        }

        /// <summary>
        /// add new require to current list
        /// </summary>
        public void AddRequire(MissionRequirementTemplate require)
        {
            if(require is null || _requires.Contains(require)) return;
            UndoUtility.RecordObject(graph, "Require Added");
            require._node = this;
            _requires.Add(require);
        }

        protected string RequireModeLabel
        {
            get{
                return _mode switch{
                    EMissionRequireMode.Any => "Complete Any Require",
                    EMissionRequireMode.All => "Complete All Requires",
                    _ => string.Empty
                };
            }
        }

        protected override void OnNodeGUI()
        {
            GUILayout.BeginVertical(Styles.roundedBox);
            if (_requires.Count == 0)
            {
                GUILayout.Label("No Requires");
            }
            else
            {
                if(_requires.Count > 1){
                    GUILayout.Label($"<i><color=#969696>{RequireModeLabel}</color></i>");
                }
                var builder = new StringBuilder();
                foreach (var require in _requires)
                    builder.AppendLine(require.Summary);
                GUILayout.Label(builder.ToString().Trim('\n'));
            }
            GUILayout.EndVertical();
        }

        protected override void OnNodeInspectorGUI()
        {            
            /* draw requires */
            GUILayout.Label("<color=#fffde3><size=12><b>Requires List</b></size></color>");
            GUILayout.BeginVertical("box");
            EditorUtils.ReorderableList(_requires, (index, picked) =>
            {
                var require = _requires[index];
                require.DrawInspectorGUI();
            });
            if(_requires.Count > 1)
            {
                _mode = (EMissionRequireMode)EditorGUILayout.EnumPopup("Require Mode", _mode);
            }
            GUILayout.EndVertical();
            
            
            /* add new require */
            GUI.backgroundColor = Colors.lightBlue;
            if (GUILayout.Button("添加需求"))
            {
                Action<Type> OnTypeSelected = type =>
                {
                    var require = (MissionRequirementTemplate)Activator.CreateInstance(type);
                    AddRequire(require);
                };

                var menu = EditorUtils.GetTypeSelectionMenu(typeof(MissionRequirementTemplate), OnTypeSelected);
                if(CopyBuffer.TryGetCache<MissionRequirementTemplate>(out var cache))
                {
                    menu.AddSeparator("/");
                    menu.AddItem(new GUIContent($"粘贴 {cache.Title}"), false, () => { AddRequire(MissionAttributes.CopyObject(cache)); });
                }

                menu.ShowAsBrowser("选择需求", typeof(MissionRequirementTemplate));
            }  
            GUI.backgroundColor = Color.white;
        }
#endif
    }
}