using Game.Models.Resource;
using GDFramework.Input;
using GDFramework.LubanKit;
using GDFramework.Models;
using GDFramework.Multilingual;
using GDFramework.Resource;
using GDFramework.Scene;
using GDFramework.SDK;
using GDFramework.Utility;
using GDFramework.View;
using GDFramework.World;
using GDFramework.World.Object;
using GDFramework.YooAssetKit;

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

            LoadFrameSettingUtility();
        }
    
        /// <summary>
        /// 注册System
        /// </summary>
        protected void Register_System()
        {
            this.RegisterSystem(new ResourcesManager());
            this.RegisterSystem(new SceneLoaderKit());
            this.RegisterSystem(new MultilingualManager());
            this.RegisterSystem(new ViewManager());
            this.RegisterSystem(new SdkManager());
            this.RegisterSystem(new YooAssetManager());
            this.RegisterSystem(new LubanKit());
            
            this.RegisterSystem(new NewInputManager());

            this.RegisterSystem(new WorldObjectManager());
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
            
            this.RegisterModel(new WorldObjectModel());
        }
    
        /// <summary>
        /// 注册Utility
        /// </summary>
        protected void Register_Utility()
        {
            this.RegisterUtility(new ResoucesUtility());
            this.RegisterUtility(new MultilingualUtility());
            this.RegisterUtility(new SdkUtility());
        }

        /// <summary>
        /// 根据框架设置来添加模块
        /// </summary>
        protected void LoadFrameSettingUtility()
        {
            FrameManager frameManager = FrameManager.Instance;
            if (frameManager.WillShowCheatWindow)
            {
                this.RegisterUtility(frameManager.gameObject.AddComponent<CheatMonoUtility>());;
            }
            
            if (frameManager.WillShowLogWindow)
            {
                this.RegisterUtility(frameManager.gameObject.AddComponent<LogMonoUtility>());;
            }
            
            this.RegisterUtility(frameManager.gameObject.AddComponent<CoroutineMonoUtility>());;
            
            this.RegisterUtility(frameManager.gameObject.AddComponent<GUIUtility>());;
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
