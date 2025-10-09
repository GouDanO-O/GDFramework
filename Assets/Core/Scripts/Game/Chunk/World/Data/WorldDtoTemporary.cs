using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.World.Data
{
    [Serializable]
    public class WorldDtoTemporary : ChunkDtoTemporary
    {
        /// <summary>
        /// 当前的年数
        /// </summary>
        [LabelText("当前的年数")]
        public int curWorldYearTime;
        
        /// <summary>
        /// 当前的天数
        /// </summary>
        [LabelText("当前的天数")]
        public int curWorldDayTime;

        /// <summary>
        /// 当前的小时数
        /// </summary>
        [LabelText("当前的小时数")]
        public int curWorldHourTime;

        /// <summary>
        /// 当前的分钟数
        /// </summary>
        [LabelText("当前的分钟数")]
        public int curWorldMinutesTime;
        
        [LabelText("当前玩家是否所处这个世界")]
        public bool playerIsLocateThisWorld;

        /// <summary>
        /// 当前玩家所处的区块ID
        /// </summary>
        [LabelText("当前玩家所处的区块ID")]
        public string curPlayerLocateAreaBlockId;
    }
}