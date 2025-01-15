using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Game
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
            this.RegisterSystem(new SceneLoader());
            this.RegisterSystem(new MultilingualManager());
            this.RegisterSystem(new ViewManager());
            this.RegisterSystem(new SdkManager());
        }
    
        /// <summary>
        /// 注册Model
        /// </summary>
        protected void Regiest_Model()
        {
            this.RegisterModel(new ResourcesData_Model());
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
