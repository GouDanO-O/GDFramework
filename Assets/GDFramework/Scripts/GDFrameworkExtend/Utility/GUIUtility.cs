using Game;
using GDFrameworkCore;
using UnityEngine;

namespace GDFramework.Utility
{
    public class GUIUtility : BasicToolMonoUtility,ICanGetUtility
    {
        private bool willShowLogWindow = GameManager.Instance.WillShowLogWindow;
        
        private bool willShowCheatWindow = GameManager.Instance.WillShowCheatWindow;

        private LogMonoUtility _logMonoUtility => this.GetUtility<LogMonoUtility>();
        
        private CheatMonoUtility _cheatMonoUtility => this.GetUtility<CheatMonoUtility>();
        
        protected override void InitUtility()
        {
            
        }

        protected override void DrawGUI()
        {
            base.DrawGUI();
            DrawUtilityWindow();
        }

        private void DrawUtilityWindow()
        {
            var showCount = -1;
            if (willShowLogWindow)
            {
                showCount++;
                if (GUI.Button(new Rect(20 + showCount * 150, 0, 120, 30),
                        _logMonoUtility.isShowing ? "关闭日志系统" : "打开日志系统"))
                {
                    _logMonoUtility.CheckButtonWillShow();
                    if (_logMonoUtility.isShowing && _cheatMonoUtility)
                        if (_cheatMonoUtility.isShowing)
                            _cheatMonoUtility.CheckButtonWillShow();
                }
            }
            
            if (willShowCheatWindow)
            {
                showCount++;
                if (GUI.Button(new Rect(20 + showCount * 150, 0, 120, 30),
                        _cheatMonoUtility.isShowing ? "关闭作弊系统" : "打开作弊系统"))
                {
                    _cheatMonoUtility.CheckButtonWillShow();
                    if (_cheatMonoUtility.isShowing && _logMonoUtility)
                        if (_logMonoUtility.isShowing)
                            _logMonoUtility.CheckButtonWillShow();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    }
}