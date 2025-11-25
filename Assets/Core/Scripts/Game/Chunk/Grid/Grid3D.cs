using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 网格统计信息
    /// </summary>
    [Serializable]
    public struct GridStatistics
    {
        public int TotalCells;
        public int EmptyCells;
        public int FloorCells;
        public int WallCells;
        public int OccupiedCells;
        public int WalkableCells;
        public int PlaceableCells;
        public int OccupiedObjects;

        public override string ToString()
        {
            return $"Grid Stats - Total:{TotalCells}, Floor:{FloorCells}, Wall:{WallCells}, " +
                   $"Occupied:{OccupiedCells}/{OccupiedObjects} objects, " +
                   $"Walkable:{WalkableCells}, Placeable:{PlaceableCells}";
        }
    }

    /// <summary>
    /// 3D网格系统核心
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
        /// 总单元格数量
        /// </summary>
        public int TotalCellCount => Size.x * Size.y * Size.z;

        /// <summary>
        /// 所有单元格 [x, y, z]
        /// </summary>
        private GridCell[,,] _cells;

        /// <summary>
        /// 单元格字典(用于快速查找)
        /// </summary>
        private Dictionary<GridPosition, GridCell> _cellDict;

        /// <summary>
        /// 占用对象映射表 (objectId -> List<GridPosition>)
        /// </summary>
        private Dictionary<string, List<GridPosition>> _occupationMap;

        public Grid3D(Vector3Int size, float cellSize, Vector3 origin)
        {
            if (size.x <= 0 || size.y <= 0 || size.z <= 0)
                throw new ArgumentException("Grid size must be positive");
            
            if (cellSize <= 0)
                throw new ArgumentException("Cell size must be positive");

            Size = size;
            CellSize = cellSize;
            Origin = origin;
            
            _cells = new GridCell[size.x, size.y, size.z];
            _cellDict = new Dictionary<GridPosition, GridCell>();
            _occupationMap = new Dictionary<string, List<GridPosition>>();
            
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
                        var cell = new GridCell(pos, GridCellType.Empty, CellSize);
                        _cells[x, y, z] = cell;
                        _cellDict[pos] = cell;
                    }
                }
            }
        }

        #region 基础查询

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
        /// 获取所有单元格
        /// </summary>
        public IEnumerable<GridCell> GetAllCells()
        {
            return _cellDict.Values;
        }

        #endregion

        #region 单元格类型操作

        /// <summary>
        /// 设置单元格类型
        /// </summary>
        public bool SetCellType(GridPosition pos, GridCellType type)
        {
            var cell = GetCell(pos);
            if (cell == null || cell.IsLocked)
                return false;
            
            cell.SetCellType(type);
            return true;
        }

        /// <summary>
        /// 批量设置单元格类型
        /// </summary>
        public void SetCellTypeRange(IEnumerable<GridPosition> positions, GridCellType type)
        {
            foreach (var pos in positions)
            {
                SetCellType(pos, type);
            }
        }

        /// <summary>
        /// 设置区域的单元格类型
        /// </summary>
        public void SetCellTypeArea(GridPosition startPos, Vector3Int areaSize, GridCellType type)
        {
            var positions = GetPositionsInArea(startPos, areaSize);
            SetCellTypeRange(positions, type);
        }

        #endregion

        #region 占用管理

        /// <summary>
        /// 占用单个单元格
        /// </summary>
        public bool TryOccupyCell(GridPosition pos, string objectId)
        {
            var cell = GetCell(pos);
            if (cell == null || !cell.TryOccupy(objectId))
                return false;

            // 记录占用关系
            if (!_occupationMap.ContainsKey(objectId))
            {
                _occupationMap[objectId] = new List<GridPosition>();
            }
            _occupationMap[objectId].Add(pos);

            return true;
        }

        /// <summary>
        /// 批量占用单元格(用于多格物体)
        /// </summary>
        public bool TryOccupyCells(IEnumerable<GridPosition> positions, string objectId)
        {
            var posList = positions.ToList();
            
            // 先检查所有格子是否可占用
            foreach (var pos in posList)
            {
                var cell = GetCell(pos);
                if (cell == null || !cell.IsPlaceable)
                    return false;
            }

            // 全部可占用,执行占用
            foreach (var pos in posList)
            {
                TryOccupyCell(pos, objectId);
            }

            return true;
        }

        /// <summary>
        /// 占用区域
        /// </summary>
        public bool TryOccupyArea(GridPosition startPos, Vector3Int areaSize, string objectId)
        {
            var positions = GetPositionsInArea(startPos, areaSize);
            return TryOccupyCells(positions, objectId);
        }

        /// <summary>
        /// 释放单个单元格
        /// </summary>
        public void ReleaseCell(GridPosition pos)
        {
            var cell = GetCell(pos);
            if (cell == null || !cell.IsOccupied)
                return;

            string objectId = cell.OccupyingObjectId;
            cell.Release();

            // 从占用映射中移除
            if (_occupationMap.ContainsKey(objectId))
            {
                _occupationMap[objectId].Remove(pos);
                if (_occupationMap[objectId].Count == 0)
                {
                    _occupationMap.Remove(objectId);
                }
            }
        }

        /// <summary>
        /// 释放所有被指定物体占用的单元格
        /// </summary>
        public void ReleaseCellsByObject(string objectId)
        {
            if (!_occupationMap.ContainsKey(objectId))
                return;

            var positions = new List<GridPosition>(_occupationMap[objectId]);
            foreach (var pos in positions)
            {
                ReleaseCell(pos);
            }
        }

        /// <summary>
        /// 获取物体占用的所有位置
        /// </summary>
        public List<GridPosition> GetObjectOccupiedPositions(string objectId)
        {
            if (_occupationMap.TryGetValue(objectId, out var positions))
            {
                return new List<GridPosition>(positions);
            }
            return new List<GridPosition>();
        }

        /// <summary>
        /// 检查物体是否存在
        /// </summary>
        public bool HasObject(string objectId)
        {
            return _occupationMap.ContainsKey(objectId);
        }

        #endregion

        #region 区域检查

        /// <summary>
        /// 检查位置是否可通行
        /// </summary>
        public bool IsWalkable(GridPosition pos)
        {
            var cell = GetCell(pos);
            return cell != null && cell.IsWalkable;
        }

        /// <summary>
        /// 检查位置是否可放置
        /// </summary>
        public bool IsPlaceable(GridPosition pos)
        {
            var cell = GetCell(pos);
            return cell != null && cell.IsPlaceable;
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
                        
                        if (!IsPlaceable(checkPos))
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检查区域是否可通行
        /// </summary>
        public bool IsAreaWalkable(GridPosition startPos, Vector3Int areaSize)
        {
            var positions = GetPositionsInArea(startPos, areaSize);
            return positions.All(pos => IsWalkable(pos));
        }

        #endregion

        #region 区域查询

        /// <summary>
        /// 获取区域内的所有位置
        /// </summary>
        public List<GridPosition> GetPositionsInArea(GridPosition startPos, Vector3Int areaSize)
        {
            var positions = new List<GridPosition>();
            
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
                        
                        if (IsInBounds(pos))
                            positions.Add(pos);
                    }
                }
            }
            
            return positions;
        }

        /// <summary>
        /// 获取区域内的所有单元格
        /// </summary>
        public List<GridCell> GetCellsInArea(GridPosition startPos, Vector3Int areaSize)
        {
            var positions = GetPositionsInArea(startPos, areaSize);
            return positions.Select(pos => GetCell(pos)).Where(cell => cell != null).ToList();
        }

        /// <summary>
        /// 获取所有特定类型的单元格
        /// </summary>
        public List<GridCell> GetCellsByType(GridCellType type)
        {
            return _cellDict.Values.Where(cell => cell.CellType == type).ToList();
        }

        /// <summary>
        /// 获取所有可通行的单元格
        /// </summary>
        public List<GridCell> GetWalkableCells()
        {
            return _cellDict.Values.Where(cell => cell.IsWalkable).ToList();
        }

        /// <summary>
        /// 获取所有可放置的单元格
        /// </summary>
        public List<GridCell> GetPlaceableCells()
        {
            return _cellDict.Values.Where(cell => cell.IsPlaceable).ToList();
        }

        /// <summary>
        /// 获取指定层的所有单元格
        /// </summary>
        public List<GridCell> GetCellsAtLevel(int y)
        {
            if (y < 0 || y >= Size.y)
                return new List<GridCell>();

            var cells = new List<GridCell>();
            for (int x = 0; x < Size.x; x++)
            {
                for (int z = 0; z < Size.z; z++)
                {
                    cells.Add(_cells[x, y, z]);
                }
            }
            return cells;
        }

        #endregion

        #region 坐标转换

        /// <summary>
        /// 世界坐标转网格坐标
        /// </summary>
        public GridPosition WorldToGrid(Vector3 worldPos)
        {
            var localPos = worldPos - Origin;
            return GridPosition.FromWorldPosition(localPos, CellSize);
        }

        /// <summary>
        /// 网格坐标转世界坐标(中心点)
        /// </summary>
        public Vector3 GridToWorld(GridPosition gridPos)
        {
            return Origin + gridPos.ToWorldPosition(CellSize);
        }

        /// <summary>
        /// 网格坐标转世界坐标(角点)
        /// </summary>
        public Vector3 GridToWorldCorner(GridPosition gridPos)
        {
            return Origin + gridPos.ToWorldPositionCorner(CellSize);
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

        /// <summary>
        /// 获取网格边界
        /// </summary>
        public Bounds GetBounds()
        {
            Vector3 size = new Vector3(Size.x * CellSize, Size.y * CellSize, Size.z * CellSize);
            Vector3 center = Origin + size * 0.5f;
            return new Bounds(center, size);
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 清空网格(重置所有单元格)
        /// </summary>
        public void Clear()
        {
            foreach (var cell in _cellDict.Values)
            {
                if (!cell.IsLocked)
                {
                    cell.Reset();
                }
            }
            _occupationMap.Clear();
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public GridStatistics GetStatistics()
        {
            return new GridStatistics
            {
                TotalCells = TotalCellCount,
                EmptyCells = GetCellsByType(GridCellType.Empty).Count,
                FloorCells = GetCellsByType(GridCellType.Floor).Count,
                WallCells = GetCellsByType(GridCellType.Wall).Count,
                OccupiedCells = GetCellsByType(GridCellType.Object).Count,
                WalkableCells = GetWalkableCells().Count,
                PlaceableCells = GetPlaceableCells().Count,
                OccupiedObjects = _occupationMap.Count
            };
        }

        #endregion
    }
}