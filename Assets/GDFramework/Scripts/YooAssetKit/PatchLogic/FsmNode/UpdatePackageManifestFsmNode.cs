using System.Collections;
using GDFramework.Utility;
using UnityEngine;
using YooAsset;

namespace GDFramework.YooAssetKit
{
    internal class UpdatePackageManifestFsmNode : PatchFsmNode
    {
        public override void OnEnter()
        {
            base.OnEnter();
            LogMonoUtility.AddLog("更新资源清单");
            StartCoroutine(UpdateManifest());
        }

        private IEnumerator UpdateManifest()
        {
            var package = YooAssets.GetPackage(PackageName);
            var operation = package.UpdatePackageManifestAsync(PackageVersion);
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                LogMonoUtility.AddErrorLog(operation.Error);
                FsmManager.ChangeFsmNode(typeof(UpdatePackageManifestFsmNode));
                yield break;
            }
            else
            {
                FsmManager.ChangeFsmNode(typeof(CreateDownloaderFsmNode));
            }
        }
    }
}