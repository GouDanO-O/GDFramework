using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Grid.Data
{
    /// <summary>
    /// 网格运行时数据
    /// 整合配置、网格系统和临时数据
    /// </summary>
    public class GridData : ChunkData
    {
        /// <summary>
        /// 3D网格实例
        /// </summary>
        public Grid3D Grid { get; private set; }

        /// <summary>
        /// 网格配置
        /// </summary>
        public GridDtoDef GridDef => DtoDef as GridDtoDef;

        /// <summary>
        /// 临时数据
        /// </summary>
        public new GridTemporaryData TemporaryData => base.TemporaryData as GridTemporaryData;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        #region 初始化

        public override void InitChunkData(IChunkDtoDef def)
        {
            base.InitChunkData(def);
            InitializeGrid();
        }

        /// <summary>
        /// 初始化网格
        /// </summary>
        private void InitializeGrid()
        {
            if (GridDef == null)
            {
                LogKit.Error("[Grid] GridDef 为空,无法初始化网格");
                return;
            }

            try
            {
                // 创建3D网格
                Grid = new Grid3D(
                    GridDef.GridSize,
                    GridDef.CellSize,
                    GridDef.GridOrigin
                );

                // 生成基础结构
                GenerateBasicStructure();

                // 生成门窗
                GenerateDoorsAndWindows();

                // 设置预留区域
                SetupReservedAreas();

                // 恢复临时数据中的占用信息
                RestoreOccupations();

                // 更新统计信息
                UpdateStatistics();

                // 标记为已初始化
                IsInitialized = true;
                TemporaryData.MarkAsInitialized();

                LogKit.Log($"[Grid] 初始化完成: {GridDef.DefId}");
                LogKit.Log($"[Grid] 尺寸: {GridDef.GridSize}, 单元格: {GridDef.CellSize}m");
                LogKit.Log($"[Grid] 统计: {Grid.GetStatistics()}");
            }
            catch (Exception ex)
            {
                LogKit.Error($"[Grid] 初始化失败: {ex.Message}\n{ex.StackTrace}");
                IsInitialized = false;
            }
        }

        #endregion

        #region 结构生成

        /// <summary>
        /// 生成基础结构(地板、墙壁、天花板)
        /// </summary>
        private void GenerateBasicStructure()
        {
            if (Grid == null) return;

            // 生成地板
            if (GridDef.AutoGenerateFloor)
            {
                GenerateFloor(GridDef.FloorLevel);
            }

            // 生成天花板
            if (GridDef.AutoGenerateCeiling)
            {
                GenerateCeiling(GridDef.CeilingLevel);
            }

            // 生成墙壁
            if (GridDef.AutoGenerateWalls)
            {
                GenerateWalls(GridDef.WallThickness, GridDef.WallGeneration);
            }
        }

        /// <summary>
        /// 生成地板
        /// </summary>
        private void GenerateFloor(int floorLevel)
        {
            var size = GridDef.GridSize;
            int count = 0;
            
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var pos = new GridPosition(x, floorLevel, z);
                    if (Grid.SetCellType(pos, GridCellType.Floor))
                    {
                        count++;
                    }
                }
            }
            
            LogKit.Log($"[Grid] 地板生成完成: {count} 个格子, 高度Y={floorLevel}");
        }

        /// <summary>
        /// 生成天花板
        /// </summary>
        private void GenerateCeiling(int ceilingLevel)
        {
            var size = GridDef.GridSize;
            int count = 0;
            
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var pos = new GridPosition(x, ceilingLevel, z);
                    if (Grid.SetCellType(pos, GridCellType.Ceiling))
                    {
                        count++;
                    }
                }
            }
            
            LogKit.Log($"[Grid] 天花板生成完成: {count} 个格子, 高度Y={ceilingLevel}");
        }

        /// <summary>
        /// 生成墙壁
        /// </summary>
        private void GenerateWalls(int thickness, WallGenerationType genType)
        {
            var size = GridDef.GridSize;
            int minY = GridDef.FloorLevel;
            int maxY = GridDef.AutoGenerateCeiling ? GridDef.CeilingLevel : size.y;
            int count = 0;

            // 根据生成类型决定生成哪些墙
            bool generateFront = genType == WallGenerationType.AllSides || 
                                genType == WallGenerationType.FrontAndBack;
            bool generateBack = genType == WallGenerationType.AllSides || 
                               genType == WallGenerationType.FrontAndBack;
            bool generateLeft = genType == WallGenerationType.AllSides || 
                               genType == WallGenerationType.LeftAndRight;
            bool generateRight = genType == WallGenerationType.AllSides || 
                                genType == WallGenerationType.LeftAndRight;

            // 前墙 (+Z)
            if (generateFront)
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = minY + 1; y < maxY; y++) // 跳过地板层
                    {
                        for (int t = 0; t < thickness; t++)
                        {
                            var pos = new GridPosition(x, y, size.z - 1 - t);
                            if (Grid.SetCellType(pos, GridCellType.Wall))
                                count++;
                        }
                    }
                }
            }

            // 后墙 (-Z)
            if (generateBack)
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = minY + 1; y < maxY; y++)
                    {
                        for (int t = 0; t < thickness; t++)
                        {
                            var pos = new GridPosition(x, y, t);
                            if (Grid.SetCellType(pos, GridCellType.Wall))
                                count++;
                        }
                    }
                }
            }

            // 左墙 (-X)
            if (generateLeft)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = minY + 1; y < maxY; y++)
                    {
                        for (int t = 0; t < thickness; t++)
                        {
                            var pos = new GridPosition(t, y, z);
                            if (Grid.SetCellType(pos, GridCellType.Wall))
                                count++;
                        }
                    }
                }
            }

            // 右墙 (+X)
            if (generateRight)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = minY + 1; y < maxY; y++)
                    {
                        for (int t = 0; t < thickness; t++)
                        {
                            var pos = new GridPosition(size.x - 1 - t, y, z);
                            if (Grid.SetCellType(pos, GridCellType.Wall))
                                count++;
                        }
                    }
                }
            }
            
            LogKit.Log($"[Grid] 墙壁生成完成: {count} 个格子, 厚度={thickness}, 类型={genType}");
        }

        /// <summary>
        /// 生成门窗
        /// </summary>
        private void GenerateDoorsAndWindows()
        {
            // 生成门
            foreach (var doorConfig in GridDef.Doors)
            {
                GenerateDoor(doorConfig);
            }

            // 生成窗户
            foreach (var windowConfig in GridDef.Windows)
            {
                GenerateWindow(windowConfig);
            }
        }

        /// <summary>
        /// 生成单个门
        /// </summary>
        private void GenerateDoor(DoorConfig config)
        {
            var positions = GetDoorPositions(config);
            Grid.SetCellTypeRange(positions, GridCellType.Door);
            LogKit.Log($"[Grid] 生成门: {config.DoorType} at {config.Position}, 尺寸: {config.Width}x{config.Height}");
        }

        /// <summary>
        /// 生成单个窗户
        /// </summary>
        private void GenerateWindow(WindowConfig config)
        {
            var positions = GetWindowPositions(config);
            Grid.SetCellTypeRange(positions, GridCellType.Window);
            LogKit.Log($"[Grid] 生成窗户: {config.WindowType} at {config.Position}, 尺寸: {config.Width}x{config.Height}");
        }

        /// <summary>
        /// 获取门的位置列表
        /// </summary>
        private List<GridPosition> GetDoorPositions(DoorConfig config)
        {
            var positions = new List<GridPosition>();
            var basePos = new GridPosition(config.Position.x, config.Position.y, config.Position.z);

            // 根据朝向决定生成方向
            switch (config.Side)
            {
                case WallSide.Front:
                case WallSide.Back:
                    // 水平展开
                    for (int x = 0; x < config.Width; x++)
                    {
                        for (int y = 0; y < config.Height; y++)
                        {
                            positions.Add(new GridPosition(basePos.X + x, basePos.Y + y, basePos.Z));
                        }
                    }
                    break;
                    
                case WallSide.Left:
                case WallSide.Right:
                    // 深度展开
                    for (int z = 0; z < config.Width; z++)
                    {
                        for (int y = 0; y < config.Height; y++)
                        {
                            positions.Add(new GridPosition(basePos.X, basePos.Y + y, basePos.Z + z));
                        }
                    }
                    break;
            }

            return positions;
        }

        /// <summary>
        /// 获取窗户的位置列表
        /// </summary>
        private List<GridPosition> GetWindowPositions(WindowConfig config)
        {
            var positions = new List<GridPosition>();
            var basePos = new GridPosition(config.Position.x, config.Position.y, config.Position.z);

            switch (config.Side)
            {
                case WallSide.Front:
                case WallSide.Back:
                    for (int x = 0; x < config.Width; x++)
                    {
                        for (int y = 0; y < config.Height; y++)
                        {
                            positions.Add(new GridPosition(basePos.X + x, basePos.Y + y, basePos.Z));
                        }
                    }
                    break;
                    
                case WallSide.Left:
                case WallSide.Right:
                    for (int z = 0; z < config.Width; z++)
                    {
                        for (int y = 0; y < config.Height; y++)
                        {
                            positions.Add(new GridPosition(basePos.X, basePos.Y + y, basePos.Z + z));
                        }
                    }
                    break;
            }

            return positions;
        }

        /// <summary>
        /// 设置预留区域
        /// </summary>
        private void SetupReservedAreas()
        {
            foreach (var area in GridDef.ReservedAreas)
            {
                var startPos = new GridPosition(area.StartPosition.x, area.StartPosition.y, area.StartPosition.z);
                Grid.SetCellTypeArea(startPos, area.Size, GridCellType.Reserved);
                LogKit.Log($"[Grid] 设置预留区域: {area.Purpose} at {startPos}, 大小: {area.Size}");
            }
        }

        #endregion
        
// 这是GridData类的第2部分，接续第1部分

        #region 数据恢复

        /// <summary>
        /// 恢复占用信息
        /// </summary>
        private void RestoreOccupations()
        {
            if (TemporaryData?.OccupiedCells == null || TemporaryData.OccupiedCells.Count == 0)
            {
                LogKit.Log("[Grid] 无需恢复占用信息");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var occupation in TemporaryData.OccupiedCells)
            {
                var positions = occupation.Positions.Select(p => p.ToGridPosition()).ToList();
                
                if (Grid.TryOccupyCells(positions, occupation.ObjectId))
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                    LogKit.Warning($"[Grid] 恢复占用失败: {occupation.ObjectId}");
                }
            }
            
            LogKit.Log($"[Grid] 占用信息恢复完成: 成功{successCount}, 失败{failCount}");
        }

        #endregion

        #region 物体管理

        /// <summary>
        /// 放置物体
        /// </summary>
        public bool PlaceObject(string objectId, GridPosition position, Vector3Int objectSize)
        {
            if (!IsInitialized)
            {
                LogKit.Error("[Grid] 网格未初始化,无法放置物体");
                return false;
            }

            if (string.IsNullOrEmpty(objectId))
            {
                LogKit.Error("[Grid] objectId 不能为空");
                return false;
            }

            // 检查是否已存在
            if (TemporaryData.HasObject(objectId))
            {
                LogKit.Warning($"[Grid] 物体已存在,先移除: {objectId}");
                RemoveObject(objectId);
            }

            // 检查是否可以放置
            if (!Grid.IsAreaPlaceable(position, objectSize))
            {
                LogKit.Warning($"[Grid] 位置不可用: {position}, 大小: {objectSize}");
                return false;
            }

            // 获取占用的所有格子
            var occupiedPositions = Grid.GetPositionsInArea(position, objectSize);

            // 占用格子
            if (!Grid.TryOccupyCells(occupiedPositions, objectId))
            {
                LogKit.Error($"[Grid] 占用格子失败: {objectId}");
                return false;
            }

            // 保存到临时数据
            var serializablePositions = occupiedPositions.Select(p => new SerializableGridPosition(p)).ToList();
            TemporaryData.AddOccupation(objectId, serializablePositions);
            
            // 更新统计
            UpdateStatistics();
            
            // 保存
            SaveTemporaryData();

            LogKit.Log($"[Grid] 物体放置成功: {objectId} at {position}, 大小: {objectSize}, 占用格子: {occupiedPositions.Count}");
            return true;
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        public bool RemoveObject(string objectId)
        {
            if (!IsInitialized)
            {
                LogKit.Error("[Grid] 网格未初始化,无法移除物体");
                return false;
            }

            if (string.IsNullOrEmpty(objectId))
            {
                LogKit.Error("[Grid] objectId 不能为空");
                return false;
            }

            if (!TemporaryData.HasObject(objectId))
            {
                LogKit.Warning($"[Grid] 物体不存在: {objectId}");
                return false;
            }

            // 释放格子
            Grid.ReleaseCellsByObject(objectId);
            
            // 从临时数据移除
            TemporaryData.RemoveOccupation(objectId);
            
            // 更新统计
            UpdateStatistics();
            
            // 保存
            SaveTemporaryData();

            LogKit.Log($"[Grid] 物体移除成功: {objectId}");
            return true;
        }

        /// <summary>
        /// 移动物体
        /// </summary>
        public bool MoveObject(string objectId, GridPosition newPosition)
        {
            if (!TemporaryData.HasObject(objectId))
            {
                LogKit.Error($"[Grid] 物体不存在: {objectId}");
                return false;
            }

            // 获取物体当前占用的格子
            var currentPositions = TemporaryData.GetObjectPositions(objectId);
            if (currentPositions.Count == 0)
            {
                LogKit.Error($"[Grid] 物体没有占用格子: {objectId}");
                return false;
            }

            // 计算物体尺寸
            var positions = currentPositions.Select(p => p.ToGridPosition()).ToList();
            var min = new GridPosition(
                positions.Min(p => p.X),
                positions.Min(p => p.Y),
                positions.Min(p => p.Z)
            );
            var max = new GridPosition(
                positions.Max(p => p.X),
                positions.Max(p => p.Y),
                positions.Max(p => p.Z)
            );
            var size = new Vector3Int(
                max.X - min.X + 1,
                max.Y - min.Y + 1,
                max.Z - min.Z + 1
            );

            // 先移除
            RemoveObject(objectId);

            // 尝试放置到新位置
            if (PlaceObject(objectId, newPosition, size))
            {
                LogKit.Log($"[Grid] 物体移动成功: {objectId} -> {newPosition}");
                return true;
            }
            else
            {
                // 放置失败,恢复到原位置
                var originalPos = min;
                PlaceObject(objectId, originalPos, size);
                LogKit.Error($"[Grid] 物体移动失败,已恢复: {objectId}");
                return false;
            }
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 检查位置是否可用
        /// </summary>
        public bool IsPositionAvailable(GridPosition position, Vector3Int size)
        {
            return IsInitialized && Grid.IsAreaPlaceable(position, size);
        }

        /// <summary>
        /// 获取可放置的位置列表
        /// </summary>
        public List<GridPosition> GetAvailablePositions(Vector3Int objectSize, int maxResults = 100)
        {
            if (!IsInitialized || Grid == null)
                return new List<GridPosition>();

            var availablePositions = new List<GridPosition>();
            var size = GridDef.GridSize;

            // 只在地板层查找
            int floorY = GridDef.FloorLevel + 1; // 物体放在地板上方一层

            for (int x = 0; x <= size.x - objectSize.x; x++)
            {
                for (int z = 0; z <= size.z - objectSize.z; z++)
                {
                    var pos = new GridPosition(x, floorY, z);
                    if (Grid.IsAreaPlaceable(pos, objectSize))
                    {
                        availablePositions.Add(pos);
                        
                        if (availablePositions.Count >= maxResults)
                            return availablePositions;
                    }
                }
            }

            return availablePositions;
        }

        /// <summary>
        /// 获取物体信息
        /// </summary>
        public GridOccupationInfo GetObjectInfo(string objectId)
        {
            if (!TemporaryData.HasObject(objectId))
                return null;

            return TemporaryData.OccupiedCells.FirstOrDefault(o => o.ObjectId == objectId);
        }

        /// <summary>
        /// 获取所有物体
        /// </summary>
        public List<string> GetAllObjects()
        {
            return TemporaryData.GetAllObjectIds();
        }

        #endregion

        #region 统计和维护

        /// <summary>
        /// 更新统计信息
        /// </summary>
        private void UpdateStatistics()
        {
            if (Grid == null) return;

            var stats = Grid.GetStatistics();
            TemporaryData.UpdateStatistics(stats.TotalCells);
        }

        /// <summary>
        /// 获取网格统计信息
        /// </summary>
        public GridStatistics GetGridStatistics()
        {
            return Grid?.GetStatistics() ?? default;
        }

        /// <summary>
        /// 清空所有物体
        /// </summary>
        public void ClearAllObjects()
        {
            if (!IsInitialized) return;

            var objectIds = TemporaryData.GetAllObjectIds().ToList();
            foreach (var objectId in objectIds)
            {
                RemoveObject(objectId);
            }

            LogKit.Log("[Grid] 已清空所有物体");
        }

        #endregion

        #region ChunkData 实现

        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            return new GridTemporaryData(DefId);
        }

        protected override Type GetTemporaryDataType()
        {
            return typeof(GridTemporaryData);
        }
        #endregion
    }
}