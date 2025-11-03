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
        public override void OnInit(FsmManager fsmManager)
        {
            base.OnInit(fsmManager);
        }

        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void OnDeInit()
        {
            
        }
        
        private void ChangeToGameScene()
        {
            SceneLoaderKit sceneLoaderKit = this.GetSystem<SceneLoaderKit>();
            sceneLoaderKit.onLoadScene.Invoke(ESceneName.GameScene);
            sceneLoaderKit.OnSceneLoadComplete += LoadGameSceneComplete;
        }

        private void LoadGameSceneComplete()
        {
            GameManager.Instance.LoadGameSceneComplete();
            this.GetSystem<SceneLoaderKit>().OnSceneLoadComplete -= LoadGameSceneComplete;
        }
    }
}