﻿using GDFrameworkCore;
using GDFramework.Procedure;
using GDFramework.Scene;
using GDFramework.Utility;
using GDFramework.View;
using GDFrameworkExtend.FSM;


namespace Game.Procedure
{
    /// <summary>
    /// 主界面流程
    /// </summary>
    public class MainMenuProcedure : ProcedureBase, ICanGetSystem
    {
        public override void OnInit(FsmManager  fsmManager)
        {
            base.OnInit(fsmManager);
        }

        public override void OnEnter()
        {
            SceneLoaderKit sceneLoaderKit = this.GetSystem<SceneLoaderKit>();
            sceneLoaderKit.onLoadScene.Invoke(ESceneName.Menu);

            sceneLoaderKit.OnSceneLoadStart += LoadMenuSceneStart;
            sceneLoaderKit.OnSceneLoadComplete += LoadMenuSceneComplete;
        }

        /// <summary>
        /// 开始加载菜单场景
        /// </summary>
        private void LoadMenuSceneStart()
        {
            LogMonoUtility.AddLog("LoadMenuSceneStart");
        }

        /// <summary>
        /// 加载菜单场景完成
        /// </summary>
        /// <param name="sceneName"></param>
        private void LoadMenuSceneComplete(ESceneName sceneName)
        {
            LogMonoUtility.AddLog("LoadMenuSceneComplete:" + sceneName);
            this.GetSystem<ViewManager>().EnterMenu();
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnDeinit()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    }
}