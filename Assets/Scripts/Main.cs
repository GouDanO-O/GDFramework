using GDFramework_Core.Models;
using GDFramework_Core.Multilingual;
using GDFramework_Core.Resource;
using GDFramework_Core.Scene;
using GDFramework_Core.SDK;
using GDFramework_Core.Utility;
using GDFramework_Core.View;
using GDFramework_General.Models.Resource;
using QFramework;

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
        this.RegisterModel(new LaunchResourcesDataModel());
        this.RegisterModel(new MultilingualDataModel());
        this.RegisterModel(new CheatDataModel());
    }
    
    /// <summary>
    /// 注册Utility
    /// </summary>
    protected void Regiest_Utility()
    {
        this.RegisterUtility(new ResoucesUtility());
        this.RegisterUtility(new MultilingualUtility());
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