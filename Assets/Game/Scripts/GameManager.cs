using Game.Procedure;
using GDFrameworkCore;
using UnityEngine;
using YooAsset;

namespace Game
{
    /// <summary>
    /// 框架和游戏流程分离
    /// </summary>
    public class GameManager : FrameManager
    {
        protected override void InitProcedure()
        {
            base.InitProcedure();
            _procedureManager.RegisterFsmNode(new LaunchProcedure());
            _procedureManager.RegisterFsmNode(new MainMenuProcedure());
        }
    }
}