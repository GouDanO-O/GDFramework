using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Procedure;
using GDFramework.Procedure;
using GDFrameworkExtend.FSM;
using GDFramework.Utility;
using GDFrameworkCore;
using YooAsset;

namespace GDFrameworkExtend.YooAssetKit
{
    public class YooAssetManager : AbstractSystem
    {
        public string PackageName = "DefaultPackage";

        public string PackageVersion ="1.0.0";
        
        public ResourceDownloaderOperation ResourceDownloaderOperation{get;private set;}

        private EPlayMode PlayMode => FrameManager.Instance.PlayMode;

        private ResourcePackage package;

        private IRemoteServices remoteServices;
        
        private string _defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        
        private string _fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        
        public PatchOperation PatchOperation;
        
        protected override void OnInit()
        {
            
        }

        public void InitYooAssetKit(Action callback)
        {
            this.GetUtility<CoroutineMonoUtility>().StartCoroutine(InitYooAsset(callback));
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        private IEnumerator InitYooAsset(Action callBack)
        {
            YooAssets.Initialize();
            
            // 开始补丁更新流程
            PatchOperation = new PatchOperation(PackageName, PlayMode);
            YooAssets.StartOperation(PatchOperation);
            yield return PatchOperation;
            
            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(PackageName);
            YooAssets.SetDefaultPackage(gamePackage);
            callBack.Invoke();
        }
        

        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="packageVersion"></param>
        public void UpdatePackageVersion(string packageVersion)
        {
            PackageVersion= packageVersion;
        }

        /// <summary>
        /// 更新下载器
        /// </summary>
        /// <param name="resourceDownloaderOperation"></param>
        public void UpdateDownloader(ResourceDownloaderOperation resourceDownloaderOperation)
        {
            ResourceDownloaderOperation = resourceDownloaderOperation;
        }

        /// <summary>
        /// 下载错误
        /// </summary>
        /// <param name="downloadError"></param>
        public void DownloadError(DownloadErrorData  downloadError)
        {
            
        }

        /// <summary>
        /// 更新下载进度
        /// </summary>
        /// <param name="downloadUpdate"></param>
        public void DownloadUpdate(DownloadUpdateData downloadUpdate)
        {
            
        }
        
    }
}

