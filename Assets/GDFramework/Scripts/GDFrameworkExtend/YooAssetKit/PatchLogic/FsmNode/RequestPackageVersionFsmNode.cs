using System.Collections;
using GDFramework.Utility;
using UnityEngine;
using YooAsset;

namespace GDFrameworkExtend.YooAssetKit
{
    internal class RequestPackageVersionFsmNode : PatchFsmNode
    {
        public override void OnEnter()
        {
            base.OnEnter();
            LogMonoUtility.AddLog("请求资源版本");
            StartCoroutine(UpdatePackageVersion());
        }
        
        private IEnumerator UpdatePackageVersion()
        {
            var package = YooAssets.GetPackage(PackageName);
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                LogMonoUtility.AddErrorLog(operation.Error);
            }
            else
            {
                LogMonoUtility.AddLog($"请求资源版本: {operation.PackageVersion}");
                YooAssetManager.UpdatePackageVersion(operation.PackageVersion);
                FsmManager.ChangeFsmNode(typeof(UpdatePackageManifestFsmNode));
            }
        }
    }
}