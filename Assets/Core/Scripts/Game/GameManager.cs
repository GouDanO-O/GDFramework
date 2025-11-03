using Core.Game.Chunk;
using Core.Game.Procedure;
using GDFrameworkCore;
using UnityEngine;
using YooAsset;

namespace Core.Game
{
    /// <summary>
    /// 框架和游戏流程分离
    /// </summary>
    public class GameManager : FrameManager
    {
        protected ChunkManager ChunkManager;
        
        protected override void InitProcedure()
        {
            base.InitProcedure();
            _procedureManager.RegisterFsmNode(new LaunchProcedure());
            _procedureManager.RegisterFsmNode(new MainMenuProcedure());
            _procedureManager.RegisterFsmNode(new GameSceneProcedure());
        }

        /// <summary>
        /// 世界初始化完成
        /// </summary>
        public void LoadGameSceneComplete()
        {
            Debug.Log("世界初始化完成");
        }

        public string GetCurGamingTime()
        {
            return "";
        }
    }
}