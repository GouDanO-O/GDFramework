using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Grid.Data;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 网格管理器
    /// 管理多个网格的创建、查询和生命周期
    /// </summary>
    public class GridManager : AbstractSystem
    {
        /// <summary>
        /// 所有网格数据 (DefId -> GridData)
        /// </summary>
        private Dictionary<string, GridData> _grids;

        /// <summary>
        /// 当前激活的网格ID
        /// </summary>
        public string CurrentGridId { get; private set; }

        /// <summary>
        /// 当前激活的网格
        /// </summary>
        public GridData CurrentGrid => GetGrid(CurrentGridId);

        /// <summary>
        /// 网格数量
        /// </summary>
        public int GridCount => _grids.Count;

        protected override void OnInit()
        {
            _grids = new Dictionary<string, GridData>();
            LogKit.Log("[GridManager] 初始化完成");
        }

        #region 网格管理

        /// <summary>
        /// 创建新网格
        /// </summary>
        public GridData CreateGrid(GridDtoDef def)
        {
            if (def == null)
            {
                LogKit.Error("[GridManager] GridDtoDef 为空");
                return null;
            }

            if (string.IsNullOrEmpty(def.DefId))
            {
                LogKit.Error("[GridManager] DefId 为空");
                return null;
            }

            // 检查是否已存在
            if (_grids.ContainsKey(def.DefId))
            {
                LogKit.Warning($"[GridManager] 网格已存在: {def.DefId}");
                return _grids[def.DefId];
            }

            // 创建网格数据
            var gridData = new GridData();
            gridData.InitChunkData(def);

            // 添加到管理器
            _grids[def.DefId] = gridData;

            LogKit.Log($"[GridManager] 创建网格成功: {def.DefId} ({def.SpaceType})");
            return gridData;
        }

        /// <summary>
        /// 加载已有网格
        /// </summary>
        public GridData LoadGrid(string defId)
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Error("[GridManager] DefId 为空");
                return null;
            }

            // 如果已加载,直接返回
            if (_grids.ContainsKey(defId))
            {
                return _grids[defId];
            }

            // TODO: 从存储系统加载配置
            // var storageSystem = this.GetSystem<StorageSystem>();
            // var def = storageSystem.LoadDef<GridDtoDef>(defId);
            // if (def == null)
            // {
            //     LogKit.Error($"[GridManager] 加载配置失败: {defId}");
            //     return null;
            // }
            // return CreateGrid(def);
            
            LogKit.Error($"[GridManager] 网格不存在: {defId}");
            return null;
        }

        /// <summary>
        /// 获取网格
        /// </summary>
        public GridData GetGrid(string defId)
        {
            if (string.IsNullOrEmpty(defId))
                return null;

            return _grids.TryGetValue(defId, out var grid) ? grid : null;
        }

        /// <summary>
        /// 检查网格是否存在
        /// </summary>
        public bool HasGrid(string defId)
        {
            return !string.IsNullOrEmpty(defId) && _grids.ContainsKey(defId);
        }

        /// <summary>
        /// 移除网格
        /// </summary>
        public bool RemoveGrid(string defId)
        {
            if (!_grids.ContainsKey(defId))
            {
                LogKit.Warning($"[GridManager] 网格不存在: {defId}");
                return false;
            }

            var grid = _grids[defId];
            
            // 如果是当前网格,先切换
            if (CurrentGridId == defId)
            {
                CurrentGridId = null;
            }

            // 清理网格
            grid.ClearAllObjects();
            _grids.Remove(defId);

            LogKit.Log($"[GridManager] 移除网格: {defId}");
            return true;
        }

        /// <summary>
        /// 切换当前网格
        /// </summary>
        public bool SwitchToGrid(string defId)
        {
            if (!_grids.ContainsKey(defId))
            {
                LogKit.Error($"[GridManager] 网格不存在,无法切换: {defId}");
                return false;
            }

            CurrentGridId = defId;
            LogKit.Log($"[GridManager] 切换到网格: {defId}");
            return true;
        }

        /// <summary>
        /// 获取所有网格
        /// </summary>
        public List<GridData> GetAllGrids()
        {
            return _grids.Values.ToList();
        }

        /// <summary>
        /// 获取所有网格ID
        /// </summary>
        public List<string> GetAllGridIds()
        {
            return _grids.Keys.ToList();
        }

        #endregion

        #region 物体管理(快捷方法)

        /// <summary>
        /// 在当前网格放置物体
        /// </summary>
        public bool PlaceObjectInCurrentGrid(string objectId, GridPosition position, Vector3Int size)
        {
            if (CurrentGrid == null)
            {
                LogKit.Error("[GridManager] 当前没有激活的网格");
                return false;
            }

            return CurrentGrid.PlaceObject(objectId, position, size);
        }

        /// <summary>
        /// 在当前网格移除物体
        /// </summary>
        public bool RemoveObjectFromCurrentGrid(string objectId)
        {
            if (CurrentGrid == null)
            {
                LogKit.Error("[GridManager] 当前没有激活的网格");
                return false;
            }

            return CurrentGrid.RemoveObject(objectId);
        }

        /// <summary>
        /// 在指定网格放置物体
        /// </summary>
        public bool PlaceObjectInGrid(string gridId, string objectId, GridPosition position, Vector3Int size)
        {
            var grid = GetGrid(gridId);
            if (grid == null)
            {
                LogKit.Error($"[GridManager] 网格不存在: {gridId}");
                return false;
            }

            return grid.PlaceObject(objectId, position, size);
        }

        /// <summary>
        /// 在指定网格移除物体
        /// </summary>
        public bool RemoveObjectFromGrid(string gridId, string objectId)
        {
            var grid = GetGrid(gridId);
            if (grid == null)
            {
                LogKit.Error($"[GridManager] 网格不存在: {gridId}");
                return false;
            }

            return grid.RemoveObject(objectId);
        }

        /// <summary>
        /// 在当前网格移动物体
        /// </summary>
        public bool MoveObjectInCurrentGrid(string objectId, GridPosition newPosition)
        {
            if (CurrentGrid == null)
            {
                LogKit.Error("[GridManager] 当前没有激活的网格");
                return false;
            }

            return CurrentGrid.MoveObject(objectId, newPosition);
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 获取当前网格的可用位置
        /// </summary>
        public List<GridPosition> GetAvailablePositionsInCurrentGrid(Vector3Int objectSize, int maxResults = 100)
        {
            if (CurrentGrid == null)
            {
                LogKit.Warning("[GridManager] 当前没有激活的网格");
                return new List<GridPosition>();
            }

            return CurrentGrid.GetAvailablePositions(objectSize, maxResults);
        }

        /// <summary>
        /// 查找物体所在的网格
        /// </summary>
        public string FindGridByObject(string objectId)
        {
            foreach (var kvp in _grids)
            {
                if (kvp.Value.TemporaryData.HasObject(objectId))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取物体所在的网格数据
        /// </summary>
        public GridData FindGridDataByObject(string objectId)
        {
            foreach (var kvp in _grids)
            {
                if (kvp.Value.TemporaryData.HasObject(objectId))
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有网格的统计信息
        /// </summary>
        public Dictionary<string, GridStatistics> GetAllGridStatistics()
        {
            var stats = new Dictionary<string, GridStatistics>();
            foreach (var kvp in _grids)
            {
                stats[kvp.Key] = kvp.Value.GetGridStatistics();
            }
            return stats;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 清空所有网格
        /// </summary>
        public void ClearAllGrids()
        {
            var gridIds = _grids.Keys.ToList();
            foreach (var gridId in gridIds)
            {
                RemoveGrid(gridId);
            }

            CurrentGridId = null;
            LogKit.Log("[GridManager] 已清空所有网格");
        }

        /// <summary>
        /// 保存所有网格数据
        /// </summary>
        public void SaveAllGrids()
        {
            int count = 0;
            foreach (var grid in _grids.Values)
            {
                grid.SaveTemporaryData();
                count++;
            }
            LogKit.Log($"[GridManager] 已保存所有网格数据: {count} 个");
        }

        /// <summary>
        /// 清空所有网格中的物体
        /// </summary>
        public void ClearAllObjectsInAllGrids()
        {
            foreach (var grid in _grids.Values)
            {
                grid.ClearAllObjects();
            }
            LogKit.Log("[GridManager] 已清空所有网格中的物体");
        }

        #endregion

        #region 调试和日志

        /// <summary>
        /// 打印管理器状态
        /// </summary>
        public void PrintStatus()
        {
            LogKit.Log("=== GridManager Status ===");
            LogKit.Log($"Total Grids: {_grids.Count}");
            LogKit.Log($"Current Grid: {CurrentGridId ?? "None"}");
            
            foreach (var kvp in _grids)
            {
                var grid = kvp.Value;
                var stats = grid.GetGridStatistics();
                LogKit.Log($"Grid [{kvp.Key}]: {grid.GridDef.SpaceType}, {stats}");
            }
        }

        /// <summary>
        /// 获取状态摘要
        /// </summary>
        public string GetStatusSummary()
        {
            if (_grids.Count == 0)
                return "Grids: 0";

            int totalObjects = _grids.Values.Sum(g => g.TemporaryData.PlacedObjectCount);
            int totalCells = _grids.Values.Sum(g => g.GetGridStatistics().TotalCells);
            int occupiedCells = _grids.Values.Sum(g => g.TemporaryData.TotalOccupiedCells);

            float occupiedPercent = totalCells > 0 ? (float)occupiedCells / totalCells * 100 : 0;

            return $"Grids: {_grids.Count}, Objects: {totalObjects}, " +
                   $"Cells: {occupiedCells}/{totalCells} ({occupiedPercent:F1}%)";
        }

        /// <summary>
        /// 获取指定网格的详细信息
        /// </summary>
        public string GetGridDetailInfo(string defId)
        {
            var grid = GetGrid(defId);
            if (grid == null)
                return $"Grid [{defId}] not found";

            var stats = grid.GetGridStatistics();
            var def = grid.GridDef;

            return $"Grid [{defId}]\n" +
                   $"  Type: {def.SpaceType}\n" +
                   $"  Size: {def.GridSize}\n" +
                   $"  CellSize: {def.CellSize}m\n" +
                   $"  Objects: {grid.TemporaryData.PlacedObjectCount}\n" +
                   $"  Stats: {stats}";
        }

        #endregion
    }
}