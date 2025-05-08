using System.Collections.Generic;
using GDFramework.World.Object;
using GDFrameworkCore;

namespace GDFramework.World.Map
{
    /// <summary>
    /// 世界地图数据
    /// </summary>
    public class WorldMapModel : AbstractModel
    {
        private Biology _hostPlayer;

        /// <summary>
        /// key =>地图区块ID
        /// value =>地图区块数据
        /// </summary>
        private Dictionary<int, WorldMapAreaQuad> _worldMapAreaQuadDict = new Dictionary<int, WorldMapAreaQuad>();
        
        protected override void OnInit()
        {
            
        }
    }
}