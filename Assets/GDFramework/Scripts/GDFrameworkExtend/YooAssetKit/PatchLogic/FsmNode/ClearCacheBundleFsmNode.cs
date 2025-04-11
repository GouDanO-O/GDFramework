using GDFramework.Utility;
using YooAsset;

namespace GDFrameworkExtend.YooAssetKit
{
    internal class ClearCacheBundleFsmNode : PatchFsmNode
    {
        public override void OnEnter()
        {
            base.OnEnter();
            LogMonoUtility.AddLog("清理未使用的缓存文件");
            var package = YooAssets.GetPackage(PackageName);
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            operation.Completed += Operation_Completed;
        }
        
        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            FsmManager.ChangeFsmNode(typeof(StartGameFsmNode));
        }
    }
}