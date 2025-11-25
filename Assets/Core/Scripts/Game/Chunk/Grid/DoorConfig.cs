using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 门配置
    /// </summary>
    [Serializable]
    public class DoorConfig
    {
        [LabelText("位置")]
        [PropertyTooltip("门的底部中心位置")]
        public Vector3Int Position;
        
        [LabelText("朝向")]
        public WallSide Side;
        
        [LabelText("宽度(格子数)")]
        [MinValue(1)]
        public int Width = 1;
        
        [LabelText("高度(格子数)")]
        [MinValue(1)]
        public int Height = 2;
        
        [LabelText("门类型")]
        public string DoorType = "标准门";

        [HideInInspector]
        public string Description => $"{DoorType} ({Side}) at {Position}";
    }
}