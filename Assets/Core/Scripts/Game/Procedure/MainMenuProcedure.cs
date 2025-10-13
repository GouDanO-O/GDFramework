using GDFramework.Input;
using GDFrameworkCore;
using GDFramework.Procedure;
using GDFramework.Scene;
using GDFramework.Utility;
using GDFramework.View;
using GDFrameworkExtend.FSM;


namespace Core.Game.Procedure
{
    /// <summary>
    /// 主界面流程
    /// </summary>
    public class MainMenuProcedure : ProcedureBase
    {
        public override void OnEnter()
        {
            StartLoadMenu();
            //StartTestScene();
        }

        private void StartLoadMenu()
        {
            SceneLoaderKit sceneLoaderKit = this.GetSystem<SceneLoaderKit>();
            sceneLoaderKit.onLoadScene.Invoke(ESceneName.Menu);

            sceneLoaderKit.OnSceneLoadStart += LoadMenuSceneStart;
            sceneLoaderKit.OnSceneLoadComplete += LoadMenuSceneComplete;
        }

        private void StartTestScene()
        {
            this.SendEvent(new SChangeProcedureEvent(typeof(GameSceneProcedure)));
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