using System.Collections.Generic;
using GDFramework.Command.Cheat;
using GDFramework.Models;
using QFramework;
using UnityEngine;

namespace GDFramework.Utility
{
    public class Cheat_Utility : BasicTool_Utility_Mono,ICanGetModel
    {
        private CheatData_Model _cheatDataModel;

        // GUI布局滚动条
        private Vector2 scrollPosition;

        protected override void InitUtility()
        {
            Main.Interface.RegisterUtility(this);
            _cheatDataModel = GetArchitecture().GetModel<CheatData_Model>();
        }

        protected override void DrawGUI()
        {
            if(!isShowing)
                return;
            
            GUILayout.BeginArea(new Rect(10, 40, 300, 400), GUI.skin.box);
            GUILayout.Label("作弊系统", GUI.skin.label);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

            Dictionary<string,AddCheat_Command> cheaters = _cheatDataModel.GetCheaterDatas();
            foreach (var cheat in cheaters)
            {
                GUILayout.BeginHorizontal();

                AddCheat_Command thisCommand = cheat.Value;
                
                // 显示作弊功能的名称和描述
                GUILayout.Label(thisCommand.Name, GUILayout.Width(200));

                if (thisCommand is AddFreeVideoCheat_Command)
                {
                    AddFreeVideoCheat_Command newCommand= (AddFreeVideoCheat_Command)thisCommand;
                    string isActivite = newCommand.IsActive ? "关闭" : "开启";
                    // 按钮执行作弊功能
                    if (GUILayout.Button(isActivite))
                    {
                        newCommand.Execute();
                    }
                }
                else
                {
                    // 按钮执行作弊功能
                    if (GUILayout.Button("激活"))
                    {
                        cheat.Value.Execute();
                    }
                }
                
                
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    } 
}

