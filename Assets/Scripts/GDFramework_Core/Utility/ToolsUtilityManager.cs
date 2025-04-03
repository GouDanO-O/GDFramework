using GDFramework_Core.Cheater;
using QFramework;
using UnityEngine;

namespace GDFramework_Core.Utility
{
    public class ToolsUtilityManager : MonoSingleton<ToolsUtilityManager>,IController
    {
        [SerializeField] private bool willShowCheatWindow = false;
        
        [SerializeField] private bool willShowLogWindow = false;
        
        private bool canShowGUI = false;
        
        private CheatMonoUtility _cheatMonoUtility;
        
        private LogMonoUtility _logMonoUtility;
        
        private CoroutineMonoUtility _coroutineMonoUtility;

        private TimeUtility timeUtility;
        
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
                _logMonoUtility = gameObject.AddComponent<LogMonoUtility>();
                GetArchitecture().RegisterUtility(_logMonoUtility);
                canShowGUI = true;
            }
            
            if (willShowCheatWindow)
            {
                _cheatMonoUtility = gameObject.AddComponent<CheatMonoUtility>();
                GetArchitecture().RegisterUtility(_cheatMonoUtility);
                
                AddFreeVideoCheatCommand newCommand = new AddFreeVideoCheatCommand("是否开启免费广告模式", (() =>
                    {
                        return this.GetUtility<Sdk_Utility>().ChangeFreeVideoMod();
                    }));
                this.SendCommand(newCommand);
                canShowGUI = true;
            }
            
            _coroutineMonoUtility = gameObject.AddComponent<CoroutineMonoUtility>();
            GetArchitecture().RegisterUtility(_coroutineMonoUtility);
            timeUtility = new TimeUtility();
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
                if (GUI.Button(new Rect(20+showCount*150,0,120,30),_logMonoUtility.isShowing ? "关闭日志系统" : "打开日志系统"))
                {
                    _logMonoUtility.CheckButtonWillShow();
                    if (_logMonoUtility.isShowing && _cheatMonoUtility)
                    {
                        if (_cheatMonoUtility.isShowing)
                        {
                            _cheatMonoUtility.CheckButtonWillShow();
                        }
                    }
                }
            }

            if (willShowCheatWindow)
            {
                showCount++;
                if (GUI.Button(new Rect(20+showCount*150,0,120, 30),_cheatMonoUtility.isShowing ? "关闭作弊系统" : "打开作弊系统"))
                {
                    _cheatMonoUtility.CheckButtonWillShow();
                    if (_cheatMonoUtility.isShowing && _logMonoUtility)
                    {
                        if (_logMonoUtility.isShowing)
                        {
                            _logMonoUtility.CheckButtonWillShow();
                        }
                    }
                } 
            }
        }
    }
}

