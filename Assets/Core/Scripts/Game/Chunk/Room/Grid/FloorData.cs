using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 楼层数据
    /// </summary>
    [Serializable]
    public class FloorData
    {
        [LabelText("楼层索引")]
        [JsonProperty]
        public int FloorIndex;

        [LabelText("楼层名称")]
        [JsonProperty]
        public string FloorName;

        [LabelText("地块数据")]
        [JsonProperty]
        public Dictionary<string, TileData> Tiles = new Dictionary<string, TileData>();

        [LabelText("放置的物品")]
        [JsonProperty]
        public Dictionary<string, PlacedObjectData> PlacedObjects = new Dictionary<string, PlacedObjectData>();

        public FloorData()
        {
            FloorIndex = 0;
            FloorName = "1F";
        }

        public FloorData(int index)
        {
            FloorIndex = index;
            FloorName = $"{index + 1}F";
        }
    }
}