using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine.Events;

namespace GDFramework.Resource
{
    public struct SResourcesLoaderNode
    {
        public string dataName;
        
        public Action<object> loaderCallback;
    }
    
    /// <summary>
    /// 基础的资源加载器
    /// 不同过程中要加载的资源不同,可以选择性的进行加载和释放
    /// </summary>
    public abstract class BaseResourcesLoader : ICanGetModel
    {
        protected UnityAction OnLoadComplete;

        protected ResoucesUtility ResLoader;

        protected int LoadedCount { get; private set; }

        protected int MaxLoadCount => WillLoadResourcesList.Count;

        /// <summary>
        /// 注意添加资源要按照顺序,不然加载的资源会乱序
        /// 资源加载按照添加进list中的顺序去遍历
        /// </summary>
        protected List<SResourcesLoaderNode> WillLoadResourcesList = new List<SResourcesLoaderNode>();

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        /// <summary>
        /// 初始化加载器
        /// </summary>
        /// <param name="resLoader"></param>
        public void InitLoader(ResoucesUtility resLoader)
        {
            ResLoader = resLoader;
            AddLoadingResource();
            StartLoading();
        }

        /// <summary>
        /// 初始化加载器
        /// </summary>
        /// <param name="resLoader"></param>
        public void InitLoader(ResoucesUtility resLoader, UnityAction callBack)
        {
            ResLoader = resLoader;
            OnLoadComplete = callBack;
            AddLoadingResource();
            StartLoading();
        }

        /// <summary>
        /// 添加待加载资源
        /// </summary>
        protected abstract void AddLoadingResource();

        /// <summary>
        /// 加载资源数据
        /// </summary>
        private void StartLoading()
        {
            int curLoopCount = 0;
            if (MaxLoadCount == 0)
            {
                OnLoadComplete?.Invoke();
                LogMonoUtility.AddLog("全部加载完成");
            }
            else
            {
                do
                {
                    LoadingResources(curLoopCount);
                } while (curLoopCount>=MaxLoadCount);
            }
        }

        /// <summary>
        /// 加载资源
        /// 注意---->要按照添加资源的顺序去加载
        /// 否则会乱序
        /// </summary>
        private void LoadingResources(int curLoopCount)
        {
            SResourcesLoaderNode curNode = WillLoadResourcesList[curLoopCount];
            ResLoader.LoadObjAsync(curNode.dataName,curNode.loaderCallback);
        }

        /// <summary>
        /// 每加载一个就进行检测
        /// </summary>
        protected void LoadingCheck()
        {
            LogMonoUtility.AddLog("加载数据成功");
            LoadedCount++;
            if (LoadedCount == MaxLoadCount)
            {
                OnLoadComplete?.Invoke();
                LogMonoUtility.AddLog("全部加载完成");
            }
        }

        /// <summary>
        /// 数据项是否加载完毕
        /// </summary>
        /// <returns></returns>
        public bool IsLoadComplete()
        {
            return LoadedCount < MaxLoadCount;
        }
    }
}