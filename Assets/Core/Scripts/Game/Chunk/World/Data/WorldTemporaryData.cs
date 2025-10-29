using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.World.Data
{
    [Serializable]
    public class WorldTemporaryData : ChunkContainerTemporaryData
    {
        /// <summary>
        /// 当前的年数
        /// </summary>
        [LabelText("当前的年数")]
        public int CurWorldYearTime;
        
        /// <summary>
        /// 当前的天数
        /// </summary>
        [LabelText("当前的天数")]
        public int CurWorldDayTime;

        /// <summary>
        /// 当前的小时数
        /// </summary>
        [LabelText("当前的小时数")]
        public int CurWorldHourTime;

        /// <summary>
        /// 当前的分钟数
        /// </summary>
        [LabelText("当前的分钟数")]
        public int CurWorldMinutesTime;
        
        [LabelText("是否被激活")]
        public bool IsActive;

        /// <summary>
        /// 当前玩家所处的区块ID
        /// </summary>
        [LabelText("当前玩家所处的区块ID")]
        public string CurrentRegionInstanceId;
        
        
        public List<string> RegionInstanceIds = new List<string>();
        
        public WorldTemporaryData() : base() { }
        public WorldTemporaryData(string defId) : base(defId) { }
    }
}