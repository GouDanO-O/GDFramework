using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Chunk.Grid
{
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
                LogKit.Error("GridDef 为空,无法初始化网格");
                return;
            }

            // 创建3D网格
            Grid = new Grid3D(
                GridDef.GetGridSize(),
                GridDef.CellSize,
                Vector3.zero
            );

            // 生成基础结构
            GenerateBasicStructure();

            // 恢复临时数据中的占用信息
            RestoreOccupations();

            LogKit.Log($"房间网格初始化完成: {GridDef.DefId}, 尺寸: {GridDef.GetGridSize()}, 单元格大小: {GridDef.CellSize}m");
        }

        /// <summary>
        /// 生成基础结构(地板、墙壁、天花板)
        /// </summary>
        private void GenerateBasicStructure()
        {
            if (Grid == null) return;

            var size = GridDef.GetGridSize();
            
            // 生成地板
            GenerateFloor(GridDef.FloorLevel);

            // 生成天花板
            if (GridDef.EnableCeiling)
            {
                GenerateCeiling(GridDef.CeilingLevel);
            }

            // 生成墙壁
            if (GridDef.AutoGenerateWalls)
            {
                GenerateWalls(GridDef.WallThickness);
            }
        }

        /// <summary>
        /// 生成地板
        /// </summary>
        private void GenerateFloor(int floorLevel)
        {
            var size = GridDef.GetGridSize();
            
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var pos = new GridPosition(x, floorLevel, z);
                    Grid.SetCellType(pos, EGridCellType.Floor);
                }
            }
            
            LogKit.Log($"地板生成完成,高度: {floorLevel}");
        }

        /// <summary>
        /// 生成天花板
        /// </summary>
        private void GenerateCeiling(int ceilingLevel)
        {
            var size = GridDef.GetGridSize();
            
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var pos = new GridPosition(x, ceilingLevel, z);
                    Grid.SetCellType(pos, EGridCellType.Wall);
                }
            }
            
            LogKit.Log($"天花板生成完成,高度: {ceilingLevel}");
        }

        /// <summary>
        /// 生成墙壁
        /// </summary>
        private void GenerateWalls(int thickness)
        {
            var size = GridDef.GetGridSize();
            int minY = GridDef.FloorLevel;
            int maxY = GridDef.EnableCeiling ? GridDef.CeilingLevel : size.y;

            // 前后墙
            for (int x = 0; x < size.x; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int t = 0; t < thickness; t++)
                    {
                        Grid.SetCellType(new GridPosition(x, y, t), EGridCellType.Wall);
                        Grid.SetCellType(new GridPosition(x, y, size.z - 1 - t), EGridCellType.Wall);
                    }
                }
            }

            // 左右墙
            for (int z = 0; z < size.z; z++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int t = 0; t < thickness; t++)
                    {
                        Grid.SetCellType(new GridPosition(t, y, z), EGridCellType.Wall);
                        Grid.SetCellType(new GridPosition(size.x - 1 - t, y, z), EGridCellType.Wall);
                    }
                }
            }
            
            LogKit.Log($"墙壁生成完成,厚度: {thickness}");
        }

        /// <summary>
        /// 恢复占用信息
        /// </summary>
        private void RestoreOccupations()
        {
            if (TemporaryData?.OccupiedCells == null) return;

            foreach (var occupation in TemporaryData.OccupiedCells)
            {
                var positions = occupation.Positions.Select(p => p.ToRoomGridPosition());
                Grid.TryOccupyCells(positions, occupation.ObjectId);
            }
            
            LogKit.Log($"恢复占用信息: {TemporaryData.OccupiedCells.Count} 个物体");
        }

        /// <summary>
        /// 放置物体
        /// </summary>
        public bool PlaceObject(string objectId, GridPosition position, Vector3Int objectSize)
        {
            if (Grid == null) return false;

            // 检查是否可以放置
            if (!Grid.IsAreaPlaceable(position, objectSize))
            {
                LogKit.Warning($"无法放置物体 {objectId},位置不可用: {position}");
                return false;
            }

            // 获取占用的所有格子
            var occupiedPositions = new List<GridPosition>();
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    for (int z = 0; z < objectSize.z; z++)
                    {
                        occupiedPositions.Add(new GridPosition(
                            position.X + x,
                            position.Y + y,
                            position.Z + z
                        ));
                    }
                }
            }

            // 占用格子
            if (!Grid.TryOccupyCells(occupiedPositions, objectId))
            {
                return false;
            }

            // 保存到临时数据
            var serializablePositions = occupiedPositions.Select(p => new SerializableGridPosition(p)).ToList();
            TemporaryData.AddOccupation(objectId, serializablePositions);
            SaveTemporaryData();

            LogKit.Log($"物体放置成功: {objectId} 在 {position}, 大小: {objectSize}");
            return true;
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        public bool RemoveObject(string objectId)
        {
            if (Grid == null) return false;

            Grid.ReleaseCellsByObject(objectId);
            TemporaryData.RemoveOccupation(objectId);
            SaveTemporaryData();

            LogKit.Log($"物体移除成功: {objectId}");
            return true;
        }

        /// <summary>
        /// 检查位置是否可用
        /// </summary>
        public bool IsPositionAvailable(GridPosition position, Vector3Int size)
        {
            return Grid?.IsAreaPlaceable(position, size) ?? false;
        }

        /// <summary>
        /// 获取可放置的位置列表
        /// </summary>
        public List<GridPosition> GetAvailablePositions(Vector3Int objectSize)
        {
            if (Grid == null) return new List<GridPosition>();

            var availablePositions = new List<GridPosition>();
            var size = GridDef.GetGridSize();

            for (int x = 0; x <= size.x - objectSize.x; x++)
            {
                for (int y = 0; y <= size.y - objectSize.y; y++)
                {
                    for (int z = 0; z <= size.z - objectSize.z; z++)
                    {
                        var pos = new GridPosition(x, y, z);
                        if (Grid.IsAreaPlaceable(pos, objectSize))
                        {
                            availablePositions.Add(pos);
                        }
                    }
                }
            }

            return availablePositions;
        }

        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            return new GridTemporaryData(DefId);
        }

        protected override Type GetTemporaryDataType()
        {
            return typeof(GridTemporaryData);
        }
    }
}