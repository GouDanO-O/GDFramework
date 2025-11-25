using System.Collections.Generic;
using System.Linq;
using Core.Game.Grid.Data;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 布局策略
    /// </summary>
    public enum LayoutStrategy
    {
        RowByRow,    // 逐行排列
        AlongWalls,  // 沿墙排列
        Grid,        // 网格排列
        Center       // 中心排列
    }

    /// <summary>
    /// 网格工具类
    /// 提供各种辅助功能
    /// </summary>
    public static class GridUtils
    {
        #region 位置查找

        /// <summary>
        /// 查找最近的可用位置(BFS搜索)
        /// </summary>
        public static GridPosition FindNearestAvailablePosition(
            Grid3D grid, 
            GridPosition target, 
            Vector3Int objectSize,
            int maxSearchRadius = 10)
        {
            if (grid == null)
                return GridPosition.Zero;

            // BFS搜索最近的可用位置
            var queue = new Queue<GridPosition>();
            var visited = new HashSet<GridPosition>();
            
            queue.Enqueue(target);
            visited.Add(target);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                // 检查当前位置是否可用
                if (grid.IsAreaPlaceable(current, objectSize))
                {
                    return current;
                }

                // 搜索相邻位置
                foreach (var neighbor in current.GetNeighbors())
                {
                    // 检查搜索半径
                    if (neighbor.ManhattanDistance(target) > maxSearchRadius)
                        continue;

                    if (visited.Contains(neighbor))
                        continue;

                    if (!grid.IsInBounds(neighbor))
                        continue;

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return GridPosition.Zero; // 未找到
        }

        /// <summary>
        /// 查找沿墙的可用位置
        /// </summary>
        public static List<GridPosition> FindWallPositions(
            Grid3D grid,
            Vector3Int objectSize,
            int floorLevel)
        {
            if (grid == null)
                return new List<GridPosition>();

            var positions = new List<GridPosition>();
            var size = grid.Size;

            // 确保不会越界
            int maxX = Mathf.Max(0, size.x - objectSize.x);
            int maxZ = Mathf.Max(0, size.z - objectSize.z);

            // 沿着四面墙查找
            for (int x = 0; x < maxX; x++)
            {
                // 前墙
                var frontPos = new GridPosition(x, floorLevel, 1);
                if (grid.IsAreaPlaceable(frontPos, objectSize))
                    positions.Add(frontPos);

                // 后墙
                var backPos = new GridPosition(x, floorLevel, maxZ - 1);
                if (grid.IsAreaPlaceable(backPos, objectSize))
                    positions.Add(backPos);
            }

            for (int z = 0; z < maxZ; z++)
            {
                // 左墙
                var leftPos = new GridPosition(1, floorLevel, z);
                if (grid.IsAreaPlaceable(leftPos, objectSize))
                    positions.Add(leftPos);

                // 右墙
                var rightPos = new GridPosition(maxX - 1, floorLevel, z);
                if (grid.IsAreaPlaceable(rightPos, objectSize))
                    positions.Add(rightPos);
            }

            return positions;
        }

        /// <summary>
        /// 查找中心区域的位置
        /// </summary>
        public static List<GridPosition> FindCenterPositions(
            Grid3D grid,
            Vector3Int objectSize,
            int floorLevel,
            float centerRatio = 0.5f)
        {
            if (grid == null)
                return new List<GridPosition>();

            var positions = new List<GridPosition>();
            var size = grid.Size;

            int marginX = (int)(size.x * (1 - centerRatio) / 2);
            int marginZ = (int)(size.z * (1 - centerRatio) / 2);

            int startX = Mathf.Max(0, marginX);
            int endX = Mathf.Max(0, size.x - marginX - objectSize.x);
            int startZ = Mathf.Max(0, marginZ);
            int endZ = Mathf.Max(0, size.z - marginZ - objectSize.z);

            for (int x = startX; x < endX; x++)
            {
                for (int z = startZ; z < endZ; z++)
                {
                    var pos = new GridPosition(x, floorLevel, z);
                    if (grid.IsAreaPlaceable(pos, objectSize))
                    {
                        positions.Add(pos);
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// 随机获取可用位置
        /// </summary>
        public static GridPosition GetRandomAvailablePosition(
            Grid3D grid,
            Vector3Int objectSize,
            int floorLevel)
        {
            if (grid == null)
                return GridPosition.Zero;

            var availablePositions = new List<GridPosition>();
            var size = grid.Size;

            for (int x = 0; x <= size.x - objectSize.x; x++)
            {
                for (int z = 0; z <= size.z - objectSize.z; z++)
                {
                    var pos = new GridPosition(x, floorLevel, z);
                    if (grid.IsAreaPlaceable(pos, objectSize))
                    {
                        availablePositions.Add(pos);
                    }
                }
            }

            if (availablePositions.Count == 0)
                return GridPosition.Zero;

            return availablePositions[Random.Range(0, availablePositions.Count)];
        }

        #endregion

        #region 布局算法

        /// <summary>
        /// 自动布局物体列表
        /// </summary>
        public static Dictionary<string, GridPosition> AutoLayout(
            Grid3D grid,
            List<(string id, Vector3Int size)> objects,
            int floorLevel,
            LayoutStrategy strategy = LayoutStrategy.RowByRow)
        {
            if (grid == null || objects == null)
                return new Dictionary<string, GridPosition>();

            var placements = new Dictionary<string, GridPosition>();
            
            switch (strategy)
            {
                case LayoutStrategy.RowByRow:
                    placements = LayoutRowByRow(grid, objects, floorLevel);
                    break;
                    
                case LayoutStrategy.AlongWalls:
                    placements = LayoutAlongWalls(grid, objects, floorLevel);
                    break;
                    
                case LayoutStrategy.Grid:
                    placements = LayoutGrid(grid, objects, floorLevel);
                    break;

                case LayoutStrategy.Center:
                    placements = LayoutCenter(grid, objects, floorLevel);
                    break;
            }

            return placements;
        }

        private static Dictionary<string, GridPosition> LayoutRowByRow(
            Grid3D grid,
            List<(string id, Vector3Int size)> objects,
            int floorLevel)
        {
            var placements = new Dictionary<string, GridPosition>();
            int currentX = 1;
            int currentZ = 1;
            int rowHeight = 0;

            foreach (var obj in objects)
            {
                // 检查是否需要换行
                if (currentX + obj.size.x >= grid.Size.x - 1)
                {
                    currentX = 1;
                    currentZ += rowHeight + 1;
                    rowHeight = 0;
                }

                // 检查是否超出边界
                if (currentZ + obj.size.z >= grid.Size.z - 1)
                    break;

                var pos = new GridPosition(currentX, floorLevel, currentZ);
                if (grid.IsAreaPlaceable(pos, obj.size))
                {
                    placements[obj.id] = pos;
                    currentX += obj.size.x + 1;
                    rowHeight = Mathf.Max(rowHeight, obj.size.z);
                }
            }

            return placements;
        }

        private static Dictionary<string, GridPosition> LayoutAlongWalls(
            Grid3D grid,
            List<(string id, Vector3Int size)> objects,
            int floorLevel)
        {
            var placements = new Dictionary<string, GridPosition>();
            
            foreach (var obj in objects)
            {
                var wallPositions = FindWallPositions(grid, obj.size, floorLevel);
                
                if (wallPositions.Count > 0)
                {
                    placements[obj.id] = wallPositions[0];
                }
            }

            return placements;
        }

        private static Dictionary<string, GridPosition> LayoutGrid(
            Grid3D grid,
            List<(string id, Vector3Int size)> objects,
            int floorLevel)
        {
            var placements = new Dictionary<string, GridPosition>();
            int spacing = 2;
            int index = 0;

            int cols = Mathf.Max(1, (grid.Size.x - 2) / spacing);
            
            foreach (var obj in objects)
            {
                int row = index / cols;
                int col = index % cols;

                var pos = new GridPosition(
                    1 + col * spacing,
                    floorLevel,
                    1 + row * spacing
                );

                if (grid.IsInBounds(pos) && grid.IsAreaPlaceable(pos, obj.size))
                {
                    placements[obj.id] = pos;
                }

                index++;
            }

            return placements;
        }

        private static Dictionary<string, GridPosition> LayoutCenter(
            Grid3D grid,
            List<(string id, Vector3Int size)> objects,
            int floorLevel)
        {
            var placements = new Dictionary<string, GridPosition>();

            foreach (var obj in objects)
            {
                var centerPositions = FindCenterPositions(grid, obj.size, floorLevel, 0.6f);
                
                if (centerPositions.Count > 0)
                {
                    placements[obj.id] = centerPositions[0];
                }
            }

            return placements;
        }

        #endregion

        #region 可视化辅助

        /// <summary>
        /// 绘制网格调试信息
        /// </summary>
        public static void DrawGridGizmos(Grid3D grid, Color floorColor, Color wallColor, Color occupiedColor)
        {
            if (grid == null) return;

            foreach (var cell in grid.GetAllCells())
            {
                Color color = Color.white;
                
                switch (cell.CellType)
                {
                    case GridCellType.Floor:
                        color = floorColor;
                        break;
                    case GridCellType.Wall:
                    case GridCellType.Ceiling:
                        color = wallColor;
                        break;
                    case GridCellType.Object:
                        color = occupiedColor;
                        break;
                    case GridCellType.Door:
                        color = Color.green;
                        break;
                    case GridCellType.Window:
                        color = Color.cyan;
                        break;
                    case GridCellType.Reserved:
                        color = Color.yellow;
                        break;
                }

                Gizmos.color = color;
                Vector3 size = Vector3.one * grid.CellSize * 0.9f;
                Gizmos.DrawCube(cell.WorldPosition, size);
            }
        }

        /// <summary>
        /// 生成网格热力图数据
        /// </summary>
        public static float[,] GenerateOccupancyHeatmap(Grid3D grid, int level)
        {
            if (grid == null)
                return new float[0, 0];

            var heatmap = new float[grid.Size.x, grid.Size.z];

            for (int x = 0; x < grid.Size.x; x++)
            {
                for (int z = 0; z < grid.Size.z; z++)
                {
                    var pos = new GridPosition(x, level, z);
                    var cell = grid.GetCell(pos);
                    
                    if (cell == null)
                    {
                        heatmap[x, z] = 0;
                    }
                    else if (cell.IsOccupied)
                    {
                        heatmap[x, z] = 1.0f;
                    }
                    else if (cell.IsWalkable)
                    {
                        heatmap[x, z] = 0.3f;
                    }
                    else
                    {
                        heatmap[x, z] = 0;
                    }
                }
            }

            return heatmap;
        }

        #endregion

        #region 验证工具

        /// <summary>
        /// 验证网格配置
        /// </summary>
        public static List<string> ValidateGridConfiguration(GridDtoDef def)
        {
            var errors = new List<string>();

            if (def == null)
            {
                errors.Add("配置为空");
                return errors;
            }

            if (!def.Validate(out string error))
            {
                errors.Add(error);
            }

            // 检查门窗位置
            foreach (var door in def.Doors)
            {
                if (!IsPositionValid(door.Position, def.GridSize))
                {
                    errors.Add($"门位置无效: {door.Position}");
                }
            }

            foreach (var window in def.Windows)
            {
                if (!IsPositionValid(window.Position, def.GridSize))
                {
                    errors.Add($"窗户位置无效: {window.Position}");
                }
            }

            return errors;
        }

        private static bool IsPositionValid(Vector3Int pos, Vector3Int size)
        {
            return pos.x >= 0 && pos.x < size.x &&
                   pos.y >= 0 && pos.y < size.y &&
                   pos.z >= 0 && pos.z < size.z;
        }

        #endregion

        #region 坐标转换工具

        /// <summary>
        /// 批量转换世界坐标到网格坐标
        /// </summary>
        public static List<GridPosition> WorldPositionsToGrid(Grid3D grid, List<Vector3> worldPositions)
        {
            if (grid == null || worldPositions == null)
                return new List<GridPosition>();

            var gridPositions = new List<GridPosition>();
            foreach (var worldPos in worldPositions)
            {
                gridPositions.Add(grid.WorldToGrid(worldPos));
            }
            return gridPositions;
        }

        /// <summary>
        /// 批量转换网格坐标到世界坐标
        /// </summary>
        public static List<Vector3> GridPositionsToWorld(Grid3D grid, List<GridPosition> gridPositions)
        {
            if (grid == null || gridPositions == null)
                return new List<Vector3>();

            var worldPositions = new List<Vector3>();
            foreach (var gridPos in gridPositions)
            {
                worldPositions.Add(grid.GridToWorld(gridPos));
            }
            return worldPositions;
        }

        #endregion
    }
}