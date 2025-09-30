using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.World
{
    [Serializable]
    public class WorldDtoDef : ChunkDtoDef
    {
        [LabelText("初始年数")]
        public int initialWorldYearTime;
        
        [LabelText("初始月数")]
        public int initialWorldMonthTime;
        
        [LabelText("初始天数")]
        public int initialWorldDayTime;
        
        [LabelText("初始小时数")]
        public int initialWorldHourTime;
        
        [LabelText("初始分钟数")]
        public int initialWorldMinutesTime;
        
        [LabelText("初始玩家所处的区块ID(即无特殊事件的情况下,玩家会处于的第一个区块的ID)")]
        public string initialPlayerLocateRegionId;

        [LabelText("用于区块数据列表")]
        public List<RegionDto> regionDatas = new List<RegionDto>();

        [LabelText("当前世界拥有的区块ID"), ReadOnly]
        public List<string> regionIds = new List<string>();
    }
}