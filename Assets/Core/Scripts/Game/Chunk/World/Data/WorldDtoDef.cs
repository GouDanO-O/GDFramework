using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World.Data
{
    [Serializable,JsonObject]
    public class WorldDtoDef : ChunkDtoDef
    {
        [LabelText("初始年数")]
        public int InitialWorldYearTime;
        
        [LabelText("初始月数")]
        public int InitialWorldMonthTime;
        
        [LabelText("初始天数")]
        public int InitialWorldDayTime;
        
        [LabelText("初始小时数")]
        public int InitialWorldHourTime;
        
        [LabelText("初始分钟数")]
        public int InitialWorldMinutesTime;
        
        [LabelText("初始玩家所处的区块ID"),ReadOnly]
        [InfoBox("无特殊事件的情况下,玩家会处于的第一个区块的ID")]
        public string InitialPlayerLocateRegionId;

        [LabelText("当在世界中生成的坐标")]
        public Vector2 InitialSpawnedPosition;
        
        [LabelText("第一次进入世界展示的区块"),ReadOnly]
        public List<string> InitialShowingRegionIdList;
        
        [LabelText("世界拥有的所有区块ID"), ReadOnly]
        public List<string> RegionIdList = new List<string>();
        
        public override string GetTypePrefix()
        {
            return "World";
        }
        
        public override bool Validate(out string error)
        {
            if (!base.Validate(out error)) return false;

            if (string.IsNullOrEmpty(InitialPlayerLocateRegionId))
            {
                error = "必须设置初始玩家区块";
                return false;
            }

            if (!RegionIdList.Contains(InitialPlayerLocateRegionId))
            {
                error = "初始玩家区块ID不在区块列表中";
                return false;
            }

            return true;
        }

        public void UpdatePosition(Vector2 position)
        {
            
        }
    }
}