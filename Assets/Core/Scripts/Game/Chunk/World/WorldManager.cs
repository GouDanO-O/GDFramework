using UnityEngine;
using System.IO;
using Core.Game.Chunk.Region;
using GDFrameworkCore;
using Core.Game.Chunk.World.Data;
namespace Core.Game.Chunk.World
{
    public class WorldManager : ChunkManager
    {
        private RegionManager _regionManager;
        
        protected override string ComponentControllerPath
        {
            get
            {
                return GDFramework.FrameData.DefaultPackage.Prefabs.UniverseControllerAssetGroup.UniverseController;
            }
        }

        protected override void OnInit()
        {

        }

        protected override void InitManager()
        {
            base.InitManager();
        }

        protected override void InitChunkData()
        {
            base.InitChunkData();
        }

        protected override void InitComponent()
        {
            base.InitComponent();
        }
        
        protected override void SpawnComponentController()
        {
            
        }

        #region 世界管理

        /// <summary>
        /// 设置登录进来的焦点世界里面的区块
        /// </summary>
        public void SetFocusRegion()
        {
            this._regionManager = this.GetSystem<RegionManager>();
            this._regionManager.SetFocusRoom();
        }

        #endregion
        

    }
}