using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World
{
    [CreateAssetMenu(fileName = "WorldDto", menuName = "Core/WorldDto")]
    public class WorldDto : ChunkDto
    {
        /// <summary>
        /// 初始年数
        /// </summary>
        [LabelText("初始年数")]
        public int initialWorldYearTime;

        /// <summary>
        /// 初始月数
        /// </summary>
        [LabelText("初始月数")]
        public int initialWorldMonthTime;

        /// <summary>
        /// 初始天数
        /// </summary>
        [LabelText("初始天数")]
        public int initialWorldDayTime;

        /// <summary>
        /// 初始小时数
        /// </summary>
        [LabelText("初始小时数")]
        public int initialWorldHourTime;

        /// <summary>
        /// 初始分钟数
        /// </summary>
        [LabelText("初始分钟数")]
        public int initialWorldMinutesTime;

        /// <summary>
        /// 当前玩家所处的区块ID
        /// </summary>
        [LabelText("初始玩家所处的区块ID(即无特殊事件的情况下,玩家会处于的第一个区块的ID)")]
        public string initialPlayerLocateRegionId;

        [LabelText("区块数据列表")]
        public List<RegionData> regionDatas = new List<RegionData>();

        [LabelText("当前世界拥有的区块ID"), ReadOnly]
        public List<string> regionIds = new List<string>();
    }
}