using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.YooAssetKit;

namespace GDFramework.Procedure
{
    /// <summary>
    /// 加载资源流程
    /// 用来获取必要的初始化资源
    /// 和加载ab包
    /// </summary>
    public class LoadResProcedure : ProcedureBase,ICanGetSystem,ICanSendEvent
    {
        public override void OnEnter()
        {
            this.GetSystem<YooAssetManager>().InitYooAssetKit(() =>
            {
                ResourceLoadComplete();
            });
        }

        /// <summary>
        /// 资源加载完毕
        /// </summary>
        private void ResourceLoadComplete()
        {
            this.SendEvent(new SChangeProcedureEvent(typeof(InitialFrameProcedure)));
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