using GDFramework_Core.Procedure;
using GDFramework_Core.Scene;
using GDFramework_Core.Utility;
using GDFramework_Core.View;
using QFramework;

namespace GDFramework_General.Procedure
{
    /// <summary>
    /// 主界面流程
    /// </summary>
    public class MainMenuProcedure : ProcedureBase,ICanGetSystem
    {
        public override void OnInit()
        {
            
        }

        public override void OnEnter()
        {
            SceneManager sceneManager = this.GetSystem<SceneManager>();
            sceneManager.onLoadScene.Invoke(ESceneName.Menu);

            sceneManager.OnSceneLoadStart += LoadMenuSceneStart;
            sceneManager.OnSceneLoadComplete += LoadMenuSceneComplete;
        }

        /// <summary>
        /// 开始加载菜单场景
        /// </summary>
        private void LoadMenuSceneStart()
        {
            Log_Utility.AddLog("LoadMenuSceneStart");
        }
        
        /// <summary>
        /// 加载菜单场景完成
        /// </summary>
        /// <param name="sceneName"></param>
        private void LoadMenuSceneComplete(ESceneName sceneName)
        {
            Log_Utility.AddLog("LoadMenuSceneComplete:"+sceneName);
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