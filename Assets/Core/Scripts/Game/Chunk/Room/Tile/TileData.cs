using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room
{
    /// <summary>
    /// 瓦片数据
    /// </summary>
    [Serializable]
    public class TileData
    {
        [LabelText("瓦片类型")]
        public ETileType Type;

        [LabelText("瓦片位置")]
        public Vector2Int Position;

        [LabelText("是否可行走")]
        public bool Walkable;

        [LabelText("是否户外")]
        public bool IsOutdoor;

        [LabelText("瓦片旋转(0,90,180,270)")]
        public int Rotation;

        [LabelText("自定义数据")]
        public Dictionary<string, string> CustomData;

        public TileData()
        {
            Type = ETileType.Empty;
            Position = Vector2Int.zero;
            Walkable = true;
            IsOutdoor = false;
            Rotation = 0;
            CustomData = new Dictionary<string, string>();
        }

        public TileData(Vector2Int pos, ETileType type)
        {
            Position = pos;
            Type = type;
            Walkable = type != ETileType.Wall;
            IsOutdoor = type >= ETileType.Grass;
            Rotation = 0;
            CustomData = new Dictionary<string, string>();
        }
    }
}