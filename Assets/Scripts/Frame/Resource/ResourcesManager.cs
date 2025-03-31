using System;
using System.Collections.Generic;
using Frame.Models;
using Frame.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

namespace Frame.Game.Resource
{
    public enum EResourcesLoaderType
    {
        Launch,
    }
    
    public class ResourcesManager : AbstractSystem
    {
        Resouces_Utility _loader;
        
        private Dictionary<EResourcesLoaderType,BaseResourcesLoader> _resourcesLoaderDict=new Dictionary<EResourcesLoaderType,BaseResourcesLoader>();
        
        private EResourcesLoaderType _currentLoaderType;

        protected override void OnInit()
        {
            _loader = this.GetUtility<Resouces_Utility>();
            _loader.InitLoader();
            
            _resourcesLoaderDict.Add(EResourcesLoaderType.Launch,new LaunchResourcesLoader());
        }
        
        /// <summary>
        /// 开始初始化的资源加载 
        /// </summary>
        /// <param name="callback"></param>
        public void StartLaunchLoad(UnityAction callback)
        {
            _currentLoaderType=EResourcesLoaderType.Launch;
            if (_resourcesLoaderDict.ContainsKey(EResourcesLoaderType.Launch))
            {
                _resourcesLoaderDict[EResourcesLoaderType.Launch].InitLoader(_loader,callback);
            }
        }

        /// <summary>
        /// 检查当前资源段是否加载完成
        /// </summary>
        /// <returns></returns>
        public bool CheckIsLoadComplete()
        {
            bool isComplete = false;
            if (_resourcesLoaderDict.ContainsKey(_currentLoaderType))
            {
                isComplete = _resourcesLoaderDict[_currentLoaderType].IsLoadComplete();
            }
            
            return isComplete;
        }
    }
}

