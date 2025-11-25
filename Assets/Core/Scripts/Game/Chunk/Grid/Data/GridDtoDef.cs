using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Grid
{
    public class GridDtoDef : ChunkDtoDef
    {
        [Title("网格配置")]
        [LabelText("网格尺寸(X-宽度)")]
        [MinValue(1)]
        public int GridSizeX = 10;

        [LabelText("网格尺寸(Y-高度)")]
        [MinValue(1)]
        public int GridSizeY = 3;

        [LabelText("网格尺寸(Z-深度)")]
        [MinValue(1)]
        public int GridSizeZ = 10;

        [LabelText("单元格大小(米)")]
        [MinValue(0.1f)]
        public float CellSize = 1.0f;

        [LabelText("是否启用天花板")]
        public bool EnableCeiling = true;

        [LabelText("默认地板高度")]
        public int FloorLevel = 0;

        [LabelText("默认天花板高度")]
        [ShowIf("EnableCeiling")]
        public int CeilingLevel = 2;

        [Title("边界设置")]
        [LabelText("自动生成墙壁")]
        public bool AutoGenerateWalls = true;

        [LabelText("墙壁厚度(格子数)")]
        [ShowIf("AutoGenerateWalls")]
        [MinValue(1)]
        public int WallThickness = 1;

        public Vector3Int GetGridSize()
        {
            return new Vector3Int(GridSizeX, GridSizeY, GridSizeZ);
        }

        public override string GetTypePrefix()
        {
            return "ROOMGRID";
        }

        public override bool Validate(out string error)
        {
            if (!base.Validate(out error))
                return false;

            if (GridSizeX <= 0 || GridSizeY <= 0 || GridSizeZ <= 0)
            {
                error = "网格尺寸必须大于0";
                return false;
            }

            if (CellSize <= 0)
            {
                error = "单元格大小必须大于0";
                return false;
            }

            if (FloorLevel < 0 || FloorLevel >= GridSizeY)
            {
                error = $"地板高度必须在0到{GridSizeY - 1}之间";
                return false;
            }

            if (EnableCeiling && (CeilingLevel <= FloorLevel || CeilingLevel >= GridSizeY))
            {
                error = $"天花板高度必须在地板之上且小于{GridSizeY}";
                return false;
            }

            return true;
        }
    }
}