using Core.Game.Chunk.Universe;
using Core.Game.Procedure.Resource;
using GDFramework.Input;
using GDFramework.Procedure;
using GDFramework.Resource;
using GDFramework.Scene;
using GDFrameworkCore;
using GDFrameworkExtend.FSM;

namespace Core.Game.Procedure
{
    public class InitGameDataProcedure  : ProcedureBase
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

        public override void OnDeInit()
        {
        }
        
        /// <summary>
        /// 数据加载完成
        /// 开始根据数据进行初始化
        /// </summary>
        private void DataLoadComplete()
        {
            this.GetSystem<NewInputManager>().InitActionAsset();
            this.GetSystem<UniverseSystem>().InitManager();
        }
        
    }
}