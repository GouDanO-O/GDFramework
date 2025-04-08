using Game.Models.Resource;
using GDFramework_Core.Models;
using GDFramework_Core.Multilingual;
using GDFramework_Core.Procedure;
using GDFramework_Core.Resource;
using GDFramework_Core.Scene;
using GDFrameworkCore;
using GDFramework_Core.SDK;
using GDFramework_Core.Utility;
using GDFramework_Core.View;
using GDFrameworkExtend.UIKit;

namespace GDFramework
{
    public class Main : Architecture<Main>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            Register_Utility();
            Register_Model();
            Register_System();
            Register_Event();
            
            UIRoot.Instance.OnSingletonInit();
        }
    
        /// <summary>
        /// 注册System
        /// </summary>
        protected void Register_System()
        {
            this.RegisterSystem(new ResourcesManager());
            this.RegisterSystem(new SceneManager());
            this.RegisterSystem(new MultilingualManager());
            this.RegisterSystem(new ViewManager());
            this.RegisterSystem(new SdkManager());
            this.RegisterSystem(new ProcedureManager());
        }
    
        /// <summary>
        /// 注册Model
        /// </summary>
        protected void Register_Model()
        {
            this.RegisterModel(new LaunchResourcesDataModel());
            this.RegisterModel(new GameSceneResourcesDataModel());
            this.RegisterModel(new MultilingualDataModel());
            this.RegisterModel(new CheatDataModel());
            this.RegisterModel(new GameDataModel());
        }

        /// <summary>
        /// 注册Model
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        public void Register_Model<T>(T model) where T : AbstractModel
        {
            this.RegisterModel(model);
        }
    
        /// <summary>
        /// 注册Utility
        /// </summary>
        protected void Register_Utility()
        {
            this.RegisterUtility(new ResoucesUtility());
            this.RegisterUtility(new MultilingualUtility());
            this.RegisterUtility(new Sdk_Utility());
        }
    
        /// <summary>
        /// 注册事件
        /// </summary>
        protected void Register_Event()
        {
        }
    
        /// <summary>
        /// 注销事件
        /// </summary>
        protected void UnRegister_Event()
        {
        }
    }
}
