using GDFramework.Utility;

namespace GDFramework.YooAssetKit
{
    internal class DownloadPackageOverFsmNode : PatchFsmNode
    {
        public override void OnEnter()
        {
            base.OnEnter();
            LogMonoUtility.AddLog("资源文件下载完毕");
            
        }
    }
}