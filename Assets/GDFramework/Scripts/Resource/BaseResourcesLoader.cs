using System;
using System.Collections.Generic;
using GDFramework_Core.Models;
using GDFramework_Core.Utility;
using QFramework;
using UnityEngine.Events;

namespace GDFramework_Core.Resource
{
    /**
     * 基础的资源加载器
     * 不同过程中要加载的资源不同,可以选择性的进行加载和释放
     */
    public abstract class BaseResourcesLoader : ICanGetModel
    {
        public UnityAction OnLoadComplete;

        protected ResoucesUtility _loader;

        protected int LoadedCount { get; private set; }

        protected int MaxLoadCount => WillLoadResourcesList.Count;

        /// <summary>
        /// 注意添加资源要按照顺序,不然加载的资源会乱序
        /// 资源加载按照添加进list中的顺序去遍历
        /// </summary>
        protected List<string> WillLoadResourcesList = new();

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
            _loader = resLoader;
            StartLoading();
        }

        /// <summary>
        /// 初始化加载器
        /// </summary>
        /// <param name="resLoader"></param>
        public void InitLoader(ResoucesUtility resLoader, UnityAction callBack)
        {
            _loader = resLoader;
            OnLoadComplete = callBack;
            StartLoading();
        }

        /// <summary>
        /// 加载资源数据
        /// </summary>
        protected virtual void StartLoading()
        {
            LoadingResources();
        }

        protected abstract void LoadingResources();

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