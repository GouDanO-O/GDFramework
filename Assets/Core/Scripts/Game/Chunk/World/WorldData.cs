using System.Collections.Generic;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;
using UnityEngine.Analytics;

namespace Core.Game.Chunk.World
{
    public class WorldData : IAnalytic.IData
    {
        public string UniqueId { get; set; }        
        
        [LabelText("当前世界的固定数据")]
        private WorldDto worldDto;
        
        [LabelText("当前世界的临时数据")]
        private WorldDtoTemporary worldDtoTemporary;
        
        [LabelText("当前世界的区块数据")]
        private Dictionary<string, RegionData> curHoldingRegionDtoDict = new Dictionary<string, RegionData>();


    }
}