using Game.Procedure;
using GDFramework.Resource;
using GDFrameworkCore;

namespace GDFramework.Procedure
{
    public class InitialGameProcedure : ProcedureBase, ICanGetSystem, ICanSendEvent
    {
        private ResourcesManager _resourcesManager;
        
        private InitialGameDataLoader _initialGameDataLoader;
        
        public override void OnInit()
        {
            _resourcesManager = GetArchitecture().GetSystem<ResourcesManager>();
            _initialGameDataLoader = new InitialGameDataLoader();
        }
        
        public override void OnEnter()
        {
            _resourcesManager.StartLoadingResources(EResourcesLoaderType.InitialGame, _initialGameDataLoader,
                () =>
                {
                    DataLoadComplete();
                });
        }
        
        /// <summary>
        /// 初始数据加载完成
        /// </summary>
        private void DataLoadComplete()
        {
            this.SendEvent<SChangeProcedureEvent>(new SChangeProcedureEvent(EProcedureType.Launch));
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