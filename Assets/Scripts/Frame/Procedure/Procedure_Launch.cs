using Frame.Game.Resource;
using QFramework;
using UnityEngine.Events;

namespace Frame.Procedure
{
    /**
     * 登录流程
     * 开始初始化,加载数据
     */
    public class Procedure_Launch : ProcedureBase,ICanGetSystem
    {
        private ResourcesManager _resourcesManager;
        
        public override void OnInit()
        {
            _resourcesManager = GetArchitecture().GetSystem<ResourcesManager>();
        }

        public override void OnEnter()
        {
            _resourcesManager.StartLaunchLoad(() =>
            {
                DataLoadComplete();
            });
        }
        
        /// <summary>
        /// 初始数据加载完成
        /// </summary>
        private void DataLoadComplete()
        {
            ProcedureManager.Instance.ChangeProcedure<Procedure_MainMenu>();
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