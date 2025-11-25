using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Grid
{
    /// <summary>
    /// 网格单元格类型
    /// </summary>
    public enum GridCellType
    {
        Empty = 0,      // 空(未初始化)
        Floor = 1,      // 地板(可行走)
        Wall = 2,       // 墙壁(不可通行)
        Ceiling = 3,    // 天花板
        Object = 4,     // 被物体占用
        Door = 5,       // 门
        Window = 6,     // 窗户
        Reserved = 7    // 预留(特殊用途)
    }

    /// <summary>
    /// 网格单元格标记
    /// </summary>
    [Flags]
    public enum GridCellFlags
    {
        None = 0,
        Walkable = 1 << 0,      // 可行走
        Placeable = 1 << 1,     // 可放置物体
        Transparent = 1 << 2,   // 透明(光线可穿过)
        Interactive = 1 << 3,   // 可交互
        Climbable = 1 << 4,     // 可攀爬
        Locked = 1 << 5         // 锁定(不可修改)
    }

    /// <summary>
    /// 网格单元格
    /// </summary>
    [Serializable]
    public class GridCell
    {
        /// <summary>
        /// 网格坐标
        /// </summary>
        public GridPosition Position { get; private set; }

        /// <summary>
        /// 单元格类型
        /// </summary>
        public GridCellType CellType { get; private set; }

        /// <summary>
        /// 单元格标记
        /// </summary>
        public GridCellFlags Flags { get; private set; }

        /// <summary>
        /// 世界坐标(中心点)
        /// </summary>
        public Vector3 WorldPosition { get; private set; }

        /// <summary>
        /// 移动代价(用于寻路,默认1.0)
        /// </summary>
        public float MovementCost { get; set; }

        /// <summary>
        /// 占用该格子的物体ID
        /// </summary>
        public string OccupyingObjectId { get; private set; }

        /// <summary>
        /// 占用时间
        /// </summary>
        public DateTime? OccupiedTime { get; private set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        private Dictionary<string, object> _customData;

        /// <summary>
        /// 是否可通行
        /// </summary>
        public bool IsWalkable => HasFlag(GridCellFlags.Walkable) && string.IsNullOrEmpty(OccupyingObjectId);

        /// <summary>
        /// 是否可放置物体
        /// </summary>
        public bool IsPlaceable => HasFlag(GridCellFlags.Placeable) && string.IsNullOrEmpty(OccupyingObjectId);

        /// <summary>
        /// 是否被占用
        /// </summary>
        public bool IsOccupied => !string.IsNullOrEmpty(OccupyingObjectId);

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLocked => HasFlag(GridCellFlags.Locked);

        public GridCell(GridPosition position, GridCellType cellType, float cellSize = 1f)
        {
            Position = position;
            CellType = cellType;
            WorldPosition = position.ToWorldPosition(cellSize);
            MovementCost = 1f;
            _customData = new Dictionary<string, object>();
            
            // 根据类型设置默认标记
            SetDefaultFlags(cellType);
        }

        /// <summary>
        /// 设置默认标记
        /// </summary>
        private void SetDefaultFlags(GridCellType type)
        {
            switch (type)
            {
                case GridCellType.Floor:
                    Flags = GridCellFlags.Walkable | GridCellFlags.Placeable;
                    break;
                case GridCellType.Wall:
                    Flags = GridCellFlags.None;
                    MovementCost = float.MaxValue;
                    break;
                case GridCellType.Ceiling:
                    Flags = GridCellFlags.None;
                    break;
                case GridCellType.Door:
                    Flags = GridCellFlags.Walkable | GridCellFlags.Interactive | GridCellFlags.Transparent;
                    break;
                case GridCellType.Window:
                    Flags = GridCellFlags.Transparent;
                    break;
                case GridCellType.Empty:
                    Flags = GridCellFlags.Walkable | GridCellFlags.Transparent;
                    break;
                default:
                    Flags = GridCellFlags.None;
                    break;
            }
        }

        /// <summary>
        /// 设置单元格类型
        /// </summary>
        public void SetCellType(GridCellType type)
        {
            if (IsLocked) return;
            
            CellType = type;
            SetDefaultFlags(type);
        }

        /// <summary>
        /// 添加标记
        /// </summary>
        public void AddFlag(GridCellFlags flag)
        {
            Flags |= flag;
        }

        /// <summary>
        /// 移除标记
        /// </summary>
        public void RemoveFlag(GridCellFlags flag)
        {
            Flags &= ~flag;
        }

        /// <summary>
        /// 检查是否有指定标记
        /// </summary>
        public bool HasFlag(GridCellFlags flag)
        {
            return (Flags & flag) == flag;
        }

        /// <summary>
        /// 设置标记
        /// </summary>
        public void SetFlags(GridCellFlags flags)
        {
            Flags = flags;
        }

        /// <summary>
        /// 占用格子
        /// </summary>
        public bool TryOccupy(string objectId)
        {
            if (!IsPlaceable || IsLocked)
                return false;

            OccupyingObjectId = objectId;
            OccupiedTime = DateTime.Now;
            CellType = GridCellType.Object;
            RemoveFlag(GridCellFlags.Walkable | GridCellFlags.Placeable);
            
            return true;
        }

        /// <summary>
        /// 释放占用
        /// </summary>
        public void Release()
        {
            if (IsLocked) return;
            
            OccupyingObjectId = null;
            OccupiedTime = null;
            
            if (CellType == GridCellType.Object)
            {
                CellType = GridCellType.Floor;
                SetDefaultFlags(GridCellType.Floor);
            }
        }

        /// <summary>
        /// 是否被特定物体占用
        /// </summary>
        public bool IsOccupiedBy(string objectId)
        {
            return OccupyingObjectId == objectId;
        }

        /// <summary>
        /// 锁定格子
        /// </summary>
        public void Lock()
        {
            AddFlag(GridCellFlags.Locked);
        }

        /// <summary>
        /// 解锁格子
        /// </summary>
        public void Unlock()
        {
            RemoveFlag(GridCellFlags.Locked);
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        public void SetCustomData(string key, object value)
        {
            _customData[key] = value;
        }

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        public T GetCustomData<T>(string key, T defaultValue = default)
        {
            if (_customData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// 是否有自定义数据
        /// </summary>
        public bool HasCustomData(string key)
        {
            return _customData.ContainsKey(key);
        }

        /// <summary>
        /// 清除自定义数据
        /// </summary>
        public void ClearCustomData()
        {
            _customData.Clear();
        }

        /// <summary>
        /// 重置格子到初始状态
        /// </summary>
        public void Reset()
        {
            if (IsLocked) return;
            
            Release();
            CellType = GridCellType.Empty;
            SetDefaultFlags(GridCellType.Empty);
            MovementCost = 1f;
            _customData.Clear();
        }

        public override string ToString()
        {
            return $"Cell[{Position}] Type:{CellType} Flags:{Flags} Cost:{MovementCost:F1} Occupied:{OccupyingObjectId ?? "None"}";
        }
    }
}