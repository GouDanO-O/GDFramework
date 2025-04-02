using GDFramework_Core.Game.Multilingual;
using GDFramework_Core.Game.Resource;
using GDFramework_Core.Game.SDK;
using GDFramework_Core.Models;
using GDFramework_Core.Models.Resource;
using GDFramework_Core.Procedure;
using GDFramework_Core.Scene;
using GDFramework_Core.Utility;
using GDFramework_Core.View;
using QFramework;

namespace GDFramework
{
    public class Main : Architecture<Main>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            Regiest_Utility();
            Regiest_Model();
            Regiest_System();
    
            Regiest_Event();
            UIRoot.Instance.OnSingletonInit();
        }
    
        /// <summary>
        /// 注册System
        /// </summary>
        protected void Regiest_System()
        {
            this.RegisterSystem(new ResourcesManager());
            this.RegisterSystem(new SceneManager());
            this.RegisterSystem(new MultilingualManager());
            this.RegisterSystem(new ViewManager());
            this.RegisterSystem(new SdkManager());
        }
    
        /// <summary>
        /// 注册Model
        /// </summary>
        protected void Regiest_Model()
        {
            this.RegisterModel(new LaunchResourcesData_Model());
            this.RegisterModel(new MultilingualData_Model());
            this.RegisterModel(new CheatData_Model());
        }
    
        /// <summary>
        /// 注册Utility
        /// </summary>
        protected void Regiest_Utility()
        {
            this.RegisterUtility(new Resouces_Utility());
            this.RegisterUtility(new Multilingual_Utility());
            this.RegisterUtility(new Sdk_Utility());
        }
    
        /// <summary>
        /// 注册事件
        /// </summary>
        protected void Regiest_Event()
        {
            
        }
    
        /// <summary>
        /// 注销事件
        /// </summary>
        protected void UnRegiest_Event()
        {
            
        }
    }
}
