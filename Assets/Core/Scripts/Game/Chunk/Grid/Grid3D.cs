using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Chunk.Grid
{
    /// <summary>
    /// 3D房间网格系统
    /// </summary>
    public class Grid3D
    {
        /// <summary>
        /// 网格尺寸
        /// </summary>
        public Vector3Int Size { get; private set; }

        /// <summary>
        /// 单元格大小(米)
        /// </summary>
        public float CellSize { get; private set; }

        /// <summary>
        /// 网格原点(世界坐标)
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// 所有单元格 [x, y, z]
        /// </summary>
        private GridCell[,,] _cells;

        /// <summary>
        /// 单元格字典(快速查找)
        /// </summary>
        private Dictionary<GridPosition, GridCell> _cellDict;

        public Grid3D(Vector3Int size, float cellSize, Vector3 origin)
        {
            Size = size;
            CellSize = cellSize;
            Origin = origin;
            
            _cells = new GridCell[size.x, size.y, size.z];
            _cellDict = new Dictionary<GridPosition, GridCell>();
            
            InitializeGrid();
        }

        /// <summary>
        /// 初始化网格
        /// </summary>
        private void InitializeGrid()
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    for (int z = 0; z < Size.z; z++)
                    {
                        var pos = new GridPosition(x, y, z);
                        var cell = new GridCell(pos, EGridCellType.Empty, CellSize);
                        _cells[x, y, z] = cell;
                        _cellDict[pos] = cell;
                    }
                }
            }
        }

        /// <summary>
        /// 坐标是否在网格范围内
        /// </summary>
        public bool IsInBounds(GridPosition pos)
        {
            return pos.X >= 0 && pos.X < Size.x &&
                   pos.Y >= 0 && pos.Y < Size.y &&
                   pos.Z >= 0 && pos.Z < Size.z;
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        public GridCell GetCell(GridPosition pos)
        {
            if (!IsInBounds(pos))
                return null;
            
            return _cells[pos.X, pos.Y, pos.Z];
        }

        /// <summary>
        /// 获取单元格(世界坐标)
        /// </summary>
        public GridCell GetCellFromWorld(Vector3 worldPos)
        {
            var localPos = worldPos - Origin;
            var gridPos = GridPosition.FromWorldPosition(localPos, CellSize);
            return GetCell(gridPos);
        }

        /// <summary>
        /// 设置单元格类型
        /// </summary>
        public bool SetCellType(GridPosition pos, EGridCellType type)
        {
            var cell = GetCell(pos);
            if (cell == null)
                return false;
            
            cell.CellType = type;
            return true;
        }

        /// <summary>
        /// 批量设置单元格类型
        /// </summary>
        public void SetCellTypeRange(IEnumerable<GridPosition> positions, EGridCellType type)
        {
            foreach (var pos in positions)
            {
                SetCellType(pos, type);
            }
        }

        /// <summary>
        /// 占用单元格
        /// </summary>
        public bool TryOccupyCell(GridPosition pos, string objectId)
        {
            var cell = GetCell(pos);
            return cell != null && cell.TryOccupy(objectId);
        }

        /// <summary>
        /// 批量占用单元格(用于多格物体)
        /// </summary>
        public bool TryOccupyCells(IEnumerable<GridPosition> positions, string objectId)
        {
            // 先检查所有格子是否可占用
            var cellList = new List<GridCell>();
            foreach (var pos in positions)
            {
                var cell = GetCell(pos);
                if (cell == null || !cell.IsPlaceable)
                    return false;
                cellList.Add(cell);
            }

            // 全部可占用,执行占用
            foreach (var cell in cellList)
            {
                cell.TryOccupy(objectId);
            }

            return true;
        }

        /// <summary>
        /// 释放单元格
        /// </summary>
        public void ReleaseCell(GridPosition pos)
        {
            var cell = GetCell(pos);
            cell?.Release();
        }

        /// <summary>
        /// 释放所有被指定物体占用的单元格
        /// </summary>
        public void ReleaseCellsByObject(string objectId)
        {
            foreach (var cell in _cellDict.Values)
            {
                if (cell.IsOccupiedBy(objectId))
                {
                    cell.Release();
                }
            }
        }

        /// <summary>
        /// 检查区域是否可放置
        /// </summary>
        public bool IsAreaPlaceable(GridPosition startPos, Vector3Int areaSize)
        {
            for (int x = 0; x < areaSize.x; x++)
            {
                for (int y = 0; y < areaSize.y; y++)
                {
                    for (int z = 0; z < areaSize.z; z++)
                    {
                        var checkPos = new GridPosition(
                            startPos.X + x, 
                            startPos.Y + y, 
                            startPos.Z + z
                        );
                        
                        var cell = GetCell(checkPos);
                        if (cell == null || !cell.IsPlaceable)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取区域内的所有单元格
        /// </summary>
        public List<GridCell> GetCellsInArea(GridPosition startPos, Vector3Int areaSize)
        {
            var cells = new List<GridCell>();
            
            for (int x = 0; x < areaSize.x; x++)
            {
                for (int y = 0; y < areaSize.y; y++)
                {
                    for (int z = 0; z < areaSize.z; z++)
                    {
                        var pos = new GridPosition(
                            startPos.X + x,
                            startPos.Y + y,
                            startPos.Z + z
                        );
                        
                        var cell = GetCell(pos);
                        if (cell != null)
                            cells.Add(cell);
                    }
                }
            }
            
            return cells;
        }

        /// <summary>
        /// 获取所有特定类型的单元格
        /// </summary>
        public List<GridCell> GetCellsByType(EGridCellType type)
        {
            var result = new List<GridCell>();
            foreach (var cell in _cellDict.Values)
            {
                if (cell.CellType == type)
                    result.Add(cell);
            }
            return result;
        }

        /// <summary>
        /// 获取所有可通行的单元格
        /// </summary>
        public List<GridCell> GetWalkableCells()
        {
            var result = new List<GridCell>();
            foreach (var cell in _cellDict.Values)
            {
                if (cell.IsWalkable)
                    result.Add(cell);
            }
            return result;
        }

        /// <summary>
        /// 清空网格(重置所有单元格)
        /// </summary>
        public void Clear()
        {
            foreach (var cell in _cellDict.Values)
            {
                cell.CellType = EGridCellType.Empty;
                cell.Release();
            }
        }

        /// <summary>
        /// 世界坐标转网格坐标
        /// </summary>
        public GridPosition WorldToGrid(Vector3 worldPos)
        {
            var localPos = worldPos - Origin;
            return GridPosition.FromWorldPosition(localPos, CellSize);
        }

        /// <summary>
        /// 网格坐标转世界坐标
        /// </summary>
        public Vector3 GridToWorld(GridPosition gridPos)
        {
            return Origin + gridPos.ToWorldPosition(CellSize);
        }

        /// <summary>
        /// 获取网格中心世界坐标
        /// </summary>
        public Vector3 GetGridCenter()
        {
            return Origin + new Vector3(
                Size.x * CellSize * 0.5f,
                Size.y * CellSize * 0.5f,
                Size.z * CellSize * 0.5f
            );
        }
    }
}