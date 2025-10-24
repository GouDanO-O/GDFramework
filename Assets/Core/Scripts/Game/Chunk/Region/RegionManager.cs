using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Components;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region
{
    /// <summary>
    /// 每个区块里面包含有多个房间
    /// 区块都必定会有入口,但是不一定会有出口
    /// 同时,也可能一个区块具有多个入口或者多个出口
    /// </summary>
    public class RegionManager : ChunkManager
    {
        private RoomManager _roomManager;
        
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

        #region 区块管理

        /// <summary>
        /// 设置登录进来的焦点区块里面的房间
        /// </summary>
        public void SetFocusRoom()
        {
            this._roomManager = this.GetSystem<RoomManager>();
            this._roomManager.SetFocusNodes();
        }

        #endregion
        

    }
}