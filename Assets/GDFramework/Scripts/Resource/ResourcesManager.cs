using System;
using System.Collections.Generic;
using GDFramework_Core.Models;
using GDFramework_Core.Utility;
using GDFramework_General.Resource;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

namespace GDFramework_Core.Resource
{
    public class ResourcesManager : AbstractSystem
    {
        private ResoucesUtility _loader;

        private Dictionary<EResourcesLoaderType, BaseResourcesLoader> _resourcesLoaderDict = new();

        private EResourcesLoaderType _currentLoaderType;

        protected override void OnInit()
        {
            _loader = this.GetUtility<ResoucesUtility>();
            _loader.InitLoader();
        }

        /// <summary>
        /// 开始加载模块的资源
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="callback"></param>
        public void StartLoadingResources(EResourcesLoaderType loaderType, BaseResourcesLoader resourcesLoader,
            UnityAction callback)
        {
            if (!_resourcesLoaderDict.ContainsKey(loaderType)) _resourcesLoaderDict.Add(loaderType, resourcesLoader);

            _resourcesLoaderDict[loaderType].InitLoader(_loader, callback);
        }

        /// <summary>
        /// 检查当前资源段是否加载完成
        /// </summary>
        /// <returns></returns>
        public bool CheckIsLoadComplete()
        {
            var isComplete = false;
            if (_resourcesLoaderDict.ContainsKey(_currentLoaderType))
                isComplete = _resourcesLoaderDict[_currentLoaderType].IsLoadComplete();

            return isComplete;
        }
    }
}