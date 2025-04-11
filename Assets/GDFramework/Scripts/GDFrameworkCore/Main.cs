using Game.Models.Resource;
using GDFramework.Input;
using GDFramework.Models;
using GDFramework.Multilingual;
using GDFrameworkCore;
using GDFramework.Procedure;
using GDFramework.Resource;
using GDFramework.Scene;
using GDFramework.SDK;
using GDFramework.Utility;
using GDFramework.View;
using GDFrameworkExtend.UIKit;
using GDFrameworkExtend.YooAssetKit;

namespace GDFrameworkCore
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
            this.RegisterSystem(new YooAssetManager());
            
            this.RegisterSystem(new NewInputManager());
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
            this.RegisterUtility(new SdkUtility());
            
            
            LoadFrameSettingUtility();
        }

        /// <summary>
        /// 根据框架设置来添加模块
        /// </summary>
        protected void LoadFrameSettingUtility()
        {
            if (FrameManager.Instance.WillShowCheatWindow)
            {
                this.RegisterUtility(FrameManager.Instance.gameObject.AddComponent<CheatMonoUtility>());;
            }
            
            if (FrameManager.Instance.WillShowLogWindow)
            {
                this.RegisterUtility(FrameManager.Instance.gameObject.AddComponent<LogMonoUtility>());;
            }
            
            this.RegisterUtility(FrameManager.Instance.gameObject.AddComponent<CoroutineMonoUtility>());;
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
