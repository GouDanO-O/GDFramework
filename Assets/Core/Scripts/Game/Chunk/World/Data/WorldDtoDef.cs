using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World.Data
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
        
        [LabelText("初始玩家所处的区块ID"),ReadOnly]
        [InfoBox("无特殊事件的情况下,玩家会处于的第一个区块的ID")]
        public string initialPlayerLocateRegionId;

        [LabelText("当在世界中生成的坐标")]
        public Vector2 InitialSpawnedPosition;
        
        [LabelText("第一次进入世界展示的区块"),ReadOnly]
        public List<string> initialShowingRegionIdList;
        
        [LabelText("世界拥有的所有区块ID"), ReadOnly]
        public List<string> regionIdList = new List<string>();

        public WorldDtoDef() : base()
        {
            
        }
        
        protected override string GetTypePrefix()
        {
            return "World";
        }
        
        public override bool Validate(out string error)
        {
            if (!base.Validate(out error)) return false;

            if (string.IsNullOrEmpty(initialPlayerLocateRegionId))
            {
                error = "必须设置初始玩家区块";
                return false;
            }

            if (!regionIdList.Contains(initialPlayerLocateRegionId))
            {
                error = "初始玩家区块ID不在区块列表中";
                return false;
            }

            return true;
        }
    }
}