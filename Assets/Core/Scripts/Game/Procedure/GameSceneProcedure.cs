using Core.Game.Procedure.Resource;
using Core.Game.View;
using GDFramework.Input;
using GDFramework.Procedure;
using GDFramework.Resource;
using GDFramework.Scene;
using GDFrameworkCore;
using GDFrameworkExtend.FSM;
using GDFrameworkExtend.UIKit;

namespace Core.Game.Procedure
{
    public class GameSceneProcedure : ProcedureBase
    {
        private ResourcesManager _resourcesManager;
        
        private GameSceneResourcesLoader _gameSceneResourcesLoader; 
        
        public override void OnInit(FsmManager  fsmManager)
        {
            base.OnInit(fsmManager);
            _resourcesManager = this.GetSystem<ResourcesManager>();
            _gameSceneResourcesLoader=new GameSceneResourcesLoader();
        }
        
        public override void OnEnter()
        {
            _resourcesManager.StartLoadingResources(typeof(GameSceneResourcesLoader), _gameSceneResourcesLoader,
                () =>
                {
                    DataLoadComplete();
                });
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
        
        /// <summary>
        /// 数据加载完成
        /// </summary>
        private void DataLoadComplete()
        {
            this.GetSystem<NewInputManager>().InitActionAsset();
            ChangeToTestScene();
        }
        
        private void ChangeToTestScene()
        {
            SceneLoaderKit sceneLoaderKit = this.GetSystem<SceneLoaderKit>();
            sceneLoaderKit.onLoadScene.Invoke(ESceneName.TestScene);
        }
    }
}