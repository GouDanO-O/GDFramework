using UnityEngine;
using System.IO;
using Core.Game.Chunk.Region;
using GDFrameworkCore;
using Core.Game.Chunk.World.Data;
namespace Core.Game.Chunk.World
{
    public class WorldManager : AbstractSystem
    {
        private RegionManager _regionManager;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            InitWorldData();
            InitWorldComponent();
        }

        private void InitWorldData()
        {

        }

        private void InitWorldComponent()
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