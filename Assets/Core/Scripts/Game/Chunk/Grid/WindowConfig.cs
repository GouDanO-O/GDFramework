using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 窗户配置
    /// </summary>
    [Serializable]
    public class WindowConfig
    {
        [LabelText("位置")]
        [PropertyTooltip("窗户的底部中心位置")]
        public Vector3Int Position;
        
        [LabelText("朝向")]
        public WallSide Side;
        
        [LabelText("宽度(格子数)")]
        [MinValue(1)]
        public int Width = 2;
        
        [LabelText("高度(格子数)")]
        [MinValue(1)]
        public int Height = 1;
        
        [LabelText("窗户类型")]
        public string WindowType = "标准窗";

        [HideInInspector]
        public string Description => $"{WindowType} ({Side}) at {Position}";
    }
}