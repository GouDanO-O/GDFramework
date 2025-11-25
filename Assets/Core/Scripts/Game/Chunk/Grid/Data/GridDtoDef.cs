using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Grid.Data
{
    /// <summary>
    /// 网格配置定义
    /// </summary>
    [Serializable]
    public class GridDtoDef : ChunkDtoDef
    {
        [Title("基础配置")]
        [BoxGroup("基础")]
        
        [LabelText("网格尺寸")]
        [MinValue(1)]
        [PropertySpace(SpaceBefore = 5)]
        public Vector3Int GridSize = new Vector3Int(10, 3, 10);
        
        [LabelText("单元格大小(米)")]
        [MinValue(0.1f)]
        [PropertyTooltip("每个格子的实际尺寸,推荐1.0米")]
        public float CellSize = 1.0f;
        
        [LabelText("网格原点偏移")]
        [PropertyTooltip("相对于根节点的偏移")]
        public Vector3 GridOrigin = Vector3.zero;

        [Title("空间设置")]
        [BoxGroup("空间")]
        
        [LabelText("空间类型")]
        [EnumToggleButtons]
        public SpaceType SpaceType = SpaceType.General;
        
        [LabelText("空间主题")]
        public string SpaceTheme = "现代简约";

        [Title("结构生成")]
        [BoxGroup("结构")]
        
        [LabelText("自动生成地板")]
        [ToggleLeft]
        public bool AutoGenerateFloor = true;
        
        [LabelText("地板高度")]
        [ShowIf("AutoGenerateFloor")]
        [MinValue(0)]
        public int FloorLevel = 0;
        
        [PropertySpace]
        
        [LabelText("自动生成天花板")]
        [ToggleLeft]
        public bool AutoGenerateCeiling = true;
        
        [LabelText("天花板高度")]
        [ShowIf("AutoGenerateCeiling")]
        [MinValue(1)]
        public int CeilingLevel = 2;
        
        [PropertySpace]
        
        [LabelText("自动生成墙壁")]
        [ToggleLeft]
        public bool AutoGenerateWalls = true;
        
        [LabelText("墙壁厚度(格子数)")]
        [ShowIf("AutoGenerateWalls")]
        [MinValue(1)]
        [MaxValue(5)]
        public int WallThickness = 1;
        
        [LabelText("墙壁生成范围")]
        [ShowIf("AutoGenerateWalls")]
        [EnumToggleButtons]
        public WallGenerationType WallGeneration = WallGenerationType.AllSides;

        [Title("门窗配置")]
        [BoxGroup("门窗")]
        
        [LabelText("门的位置")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Description")]
        public List<DoorConfig> Doors = new List<DoorConfig>();
        
        [LabelText("窗户的位置")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Description")]
        public List<WindowConfig> Windows = new List<WindowConfig>();

        [Title("高级设置")]
        [BoxGroup("高级")]
        
        [LabelText("预留区域")]
        [PropertyTooltip("不可放置物体的特殊区域")]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<ReservedArea> ReservedAreas = new List<ReservedArea>();
        
        [LabelText("启用寻路")]
        [ToggleLeft]
        public bool EnablePathfinding = true;
        
        [LabelText("启用可视化调试")]
        [ToggleLeft]
        public bool EnableDebugVisualization = false;

        public override string GetTypePrefix()
        {
            return "GRID";
        }

        public override bool Validate(out string error)
        {
            if (!base.Validate(out error))
                return false;

            // 检查网格尺寸
            if (GridSize.x <= 0 || GridSize.y <= 0 || GridSize.z <= 0)
            {
                error = "网格尺寸必须大于0";
                return false;
            }

            if (GridSize.x > 100 || GridSize.y > 100 || GridSize.z > 100)
            {
                error = "网格尺寸不能超过100(性能考虑)";
                return false;
            }

            // 检查单元格大小
            if (CellSize <= 0)
            {
                error = "单元格大小必须大于0";
                return false;
            }

            // 检查地板和天花板
            if (AutoGenerateFloor && (FloorLevel < 0 || FloorLevel >= GridSize.y))
            {
                error = $"地板高度必须在0到{GridSize.y - 1}之间";
                return false;
            }

            if (AutoGenerateCeiling)
            {
                if (CeilingLevel <= FloorLevel)
                {
                    error = "天花板高度必须大于地板高度";
                    return false;
                }
                
                if (CeilingLevel >= GridSize.y)
                {
                    error = $"天花板高度必须小于{GridSize.y}";
                    return false;
                }
            }

            // 检查墙壁
            if (AutoGenerateWalls && WallThickness <= 0)
            {
                error = "墙壁厚度必须大于0";
                return false;
            }

            // 验证门的配置
            foreach (var door in Doors)
            {
                if (!ValidateDoorConfig(door, out error))
                    return false;
            }

            // 验证窗户的配置
            foreach (var window in Windows)
            {
                if (!ValidateWindowConfig(window, out error))
                    return false;
            }

            error = string.Empty;
            return true;
        }

        private bool ValidateDoorConfig(DoorConfig door, out string error)
        {
            if (!IsPositionInBounds(door.Position))
            {
                error = $"门的位置超出网格范围: {door.Position}";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private bool ValidateWindowConfig(WindowConfig window, out string error)
        {
            if (!IsPositionInBounds(window.Position))
            {
                error = $"窗户的位置超出网格范围: {window.Position}";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private bool IsPositionInBounds(Vector3Int pos)
        {
            return pos.x >= 0 && pos.x < GridSize.x &&
                   pos.y >= 0 && pos.y < GridSize.y &&
                   pos.z >= 0 && pos.z < GridSize.z;
        }

        /// <summary>
        /// 获取实际世界尺寸
        /// </summary>
        public Vector3 GetWorldSize()
        {
            return new Vector3(
                GridSize.x * CellSize,
                GridSize.y * CellSize,
                GridSize.z * CellSize
            );
        }

        /// <summary>
        /// 获取可用空间高度
        /// </summary>
        public int GetUsableHeight()
        {
            if (!AutoGenerateCeiling)
                return GridSize.y - FloorLevel;
            
            return CeilingLevel - FloorLevel;
        }
    }
}