using GDFramework_Core.Procedure;
using GDFramework_Core.Resource;
using GDFramework_General.Resource;
using QFramework;

namespace GDFramework_General.Procedure
{
    /// <summary>
    /// 登录流程
    /// 开始初始化,加载数据
    /// </summary>
    public class LaunchProcedure : ProcedureBase, ICanGetSystem, ICanSendEvent
    {
        private ResourcesManager _resourcesManager;

        private LaunchResourcesLoader _launchResourcesLoader;

        public override void OnInit()
        {
            _resourcesManager = GetArchitecture().GetSystem<ResourcesManager>();
            _launchResourcesLoader = new LaunchResourcesLoader();
        }

        public override void OnEnter()
        {
            _resourcesManager.StartLoadingResources(EResourcesLoaderType.Launch, _launchResourcesLoader,
                () => { DataLoadComplete(); });
        }

        /// <summary>
        /// 初始数据加载完成
        /// </summary>
        private void DataLoadComplete()
        {
            this.SendEvent<SChangeProcedureEvent>(new SChangeProcedureEvent(EProcedureType.MainMenu));
        }

        public override void OnUpdate()
        {
        }

        public override bool OnExitCondition()
        {
            return _resourcesManager.CheckIsLoadComplete();
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