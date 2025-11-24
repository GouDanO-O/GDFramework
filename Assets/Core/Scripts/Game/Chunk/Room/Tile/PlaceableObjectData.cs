using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room
{
    /// <summary>
    /// 可放置物体数据
    /// </summary>
    [Serializable]
    public class PlaceableObjectData
    {
        [LabelText("物体ID")]
        public string ObjectId;

        [LabelText("物体类型")]
        public EPlaceableObjectType ObjectType;

        [LabelText("占据的瓦片位置")]
        public Vector2Int Position;

        [LabelText("占据尺寸(瓦片数)")]
        public Vector2Int Size;

        [LabelText("旋转角度")]
        public int Rotation;

        [LabelText("预制体路径")]
        public string PrefabPath;

        [LabelText("是否阻挡移动")]
        public bool BlocksMovement;

        [LabelText("自定义属性")]
        public Dictionary<string, string> Properties;

        public PlaceableObjectData()
        {
            ObjectId = Guid.NewGuid().ToString("N").Substring(0, 8);
            Position = Vector2Int.zero;
            Size = Vector2Int.one;
            Rotation = 0;
            BlocksMovement = true;
            Properties = new Dictionary<string, string>();
        }
    }
}