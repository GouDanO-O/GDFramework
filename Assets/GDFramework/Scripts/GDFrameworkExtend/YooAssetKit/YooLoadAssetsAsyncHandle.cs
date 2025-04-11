using System;
using System.Collections;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.ActionKit;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.PoolKit;
using YooAsset;

namespace GDFrameworkExtend.YooAssetKit
{
    public class YooLoadAssetsAsyncHandle : TemporalityDataPoolable,ICanGetUtility
    {
        private Dictionary<string, AssetHandle> _handleDict = new Dictionary<string, AssetHandle>();
        
        public override void OnRecycled()
        {
            
        }

        public override void Recycle2Cache()
        {
            SafeObjectPool<YooLoadAssetsAsyncHandle>.Instance.Recycle(this);
        }
        
        public override void DeInitData()
        {
            
        }
        
        /// <summary>
        /// 同步获取资源
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="assetName"></param>
        /// <param name="onFinish"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsset(string assetName, Action<object> onFinish,string packageName="DefaultPackage")
        {
            var package = YooAssets.GetPackage(packageName);
            
            LoadAsset(package, assetName, (handle) =>
            {
                onFinish?.Invoke(handle);
            });
        }
        
        /// <summary>
        /// 异步获取资源
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="assetName"></param>
        /// <param name="onFinish"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAssetAsync(string assetName, Action<object> onFinish,string packageName="DefaultPackage")
        {
            var package = YooAssets.GetPackage(packageName);
            
            this.GetUtility<CoroutineMonoUtility>().
                StartCoroutine(LoadAssetAsync(package, assetName, (handle) =>
                {
                    onFinish?.Invoke(handle);
                }));
        }
        
        
        void LoadAsset(ResourcePackage resourcePackage, string assetName, Action<object> onFinish)
        {
            AssetHandle handle = resourcePackage.LoadAssetAsync(assetName);
            if (handle.Status == EOperationStatus.Succeed)
            {
                _handleDict.Add(assetName, handle);
                onFinish.Invoke(handle.AssetObject);
            }
            else
            {
                onFinish.Invoke(null);
            }
        }
        
        IEnumerator LoadAssetAsync(ResourcePackage resourcePackage, string assetName, Action<object> onFinish)
        {
            AssetHandle handle = resourcePackage.LoadAssetAsync(assetName);
            yield return handle;
            if (handle.Status == EOperationStatus.Succeed)
            {
                _handleDict.Add(assetName, handle);
                onFinish.Invoke(handle.AssetObject); 
            }
            else
            {
                onFinish.Invoke(null);
            }
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseObj(UnityEngine.Object obj)
        {
            if (obj == null) 
                return;
            
            // 查找所有包含该对象的资源句柄
            foreach (var pair in _handleDict)
            {
                if (pair.Value.AssetObject == obj)
                {
                    pair.Value.Release();
                    _handleDict.Remove(pair.Key);
                    break;
                }
            }
        }
        
        public static YooLoadAssetsAsyncHandle Allocate()
        {
            return SafeObjectPool<YooLoadAssetsAsyncHandle>.Instance.Allocate();
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    }
}