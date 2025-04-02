using GDFramework_Core.Command.Cheat;
using QFramework;
using UnityEngine;

namespace GDFramework_Core.Utility
{
    public class ToolsUtilityManager : MonoSingleton<ToolsUtilityManager>,IController
    {
        [SerializeField] private bool willShowCheatWindow = false;
        
        [SerializeField] private bool willShowLogWindow = false;
        
        private bool canShowGUI = false;
        
        private Cheat_Utility cheatUtility;
        
        private Log_Utility logUtility;
        
        private Coroutine_Utility coroutineUtility;

        private Time_Utility timeUtility;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private void Awake()
        {
            InitData();
        }

        /// <summary>
        /// 初始化
        /// 根据所选项来选择性注册工具
        /// </summary>
        protected void InitData()
        {
            if (willShowLogWindow)
            {
                logUtility = gameObject.AddComponent<Log_Utility>();
                GetArchitecture().RegisterUtility(logUtility);
                canShowGUI = true;
            }
            
            if (willShowCheatWindow)
            {
                cheatUtility = gameObject.AddComponent<Cheat_Utility>();
                GetArchitecture().RegisterUtility(cheatUtility);
                
                AddFreeVideoCheat_Command newCommand = new AddFreeVideoCheat_Command("是否开启免费广告模式", (() =>
                    {
                        return this.GetUtility<Sdk_Utility>().ChangeFreeVideoMod();
                    }));
                this.SendCommand(newCommand);
                canShowGUI = true;
            }
            
            coroutineUtility = gameObject.AddComponent<Coroutine_Utility>();
            GetArchitecture().RegisterUtility(coroutineUtility);
            timeUtility = new Time_Utility();
            GetArchitecture().RegisterUtility(timeUtility);
        }

        private void OnGUI()
        {
            DrawGUI();
        }

        protected void DrawGUI()
        {
            if(!canShowGUI)
                return;            
            int showCount = -1;
            if (willShowLogWindow)
            {
                showCount++;
                if (GUI.Button(new Rect(20+showCount*150,0,120,30),logUtility.isShowing ? "关闭日志系统" : "打开日志系统"))
                {
                    logUtility.CheckButtonWillShow();
                    if (logUtility.isShowing && cheatUtility)
                    {
                        if (cheatUtility.isShowing)
                        {
                            cheatUtility.CheckButtonWillShow();
                        }
                    }
                }
            }

            if (willShowCheatWindow)
            {
                showCount++;
                if (GUI.Button(new Rect(20+showCount*150,0,120, 30),cheatUtility.isShowing ? "关闭作弊系统" : "打开作弊系统"))
                {
                    cheatUtility.CheckButtonWillShow();
                    if (cheatUtility.isShowing && logUtility)
                    {
                        if (logUtility.isShowing)
                        {
                            logUtility.CheckButtonWillShow();
                        }
                    }
                } 
            }
        }
    }
}

