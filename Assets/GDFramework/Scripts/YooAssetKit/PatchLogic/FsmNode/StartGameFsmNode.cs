using Game;
using GDFramework.Utility;

namespace GDFramework.YooAssetKit
{
    internal class StartGameFsmNode : PatchFsmNode
    {
        public override void OnEnter()
        {
            base.OnEnter();
            LogMonoUtility.AddLog("进入游戏");
            PatchOperation.SetFinish();
        }
    }
}