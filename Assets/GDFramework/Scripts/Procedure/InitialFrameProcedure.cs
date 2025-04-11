using Game.Procedure;
using GDFramework.Input;
using GDFramework.Resource;
using GDFrameworkCore;
using GDFrameworkExtend.FSM;
using GDFrameworkExtend.YooAssetKit;

namespace GDFramework.Procedure
{
    /// <summary>
    /// 框架初始化流程
    /// 主要初始化一些框架内容配置
    /// </summary>
    public class InitialFrameProcedure : ProcedureBase, ICanGetSystem, ICanSendEvent
    {
        private ResourcesManager _resourcesManager;
        
        private InitialFrameDataLoader _initialFrameDataLoader;
        
        public override void OnInit(FsmManager  fsmManager)
        {
            base.OnInit(fsmManager);
            _resourcesManager = GetArchitecture().GetSystem<ResourcesManager>();
            _initialFrameDataLoader = new InitialFrameDataLoader();
        }
        
        public override void OnEnter()
        {
            _resourcesManager.StartLoadingResources(typeof(InitialFrameDataLoader), _initialFrameDataLoader,
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
            this.SendEvent(new SChangeProcedureEvent(typeof(LaunchProcedure)));
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