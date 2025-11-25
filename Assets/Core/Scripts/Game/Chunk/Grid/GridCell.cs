using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Chunk.Grid
{
    public class GridCell
    {
        public Vector3Int Position;
        public EGridCellType CellType;           // 地板/墙壁/空气
        public bool IsWalkable;
        public List<GameObject> Objects;

        public GridCell(GridPosition pos, EGridCellType type, float cellSize)
        {
            
        }
    }
}