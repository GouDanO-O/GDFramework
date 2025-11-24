using System;
using System.Collections.Generic;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 房间编辑器系统
    /// </summary>
    public class RoomEditorSystem : AbstractSystem
    {
        private RoomData _currentRoom;
        private bool _isEditing;

        // 编辑状态
        private EditorMode _editorMode = EditorMode.Tile;
        private ETileType _selectedTileType = ETileType.Floor;
        private PlaceableObjectData _selectedObjectTemplate;

        // 预览
        private Vector2Int _hoveredTilePos;

        protected override void OnInit()
        {
        }

        #region 编辑器控制

        /// <summary>
        /// 开始编辑房间
        /// </summary>
        public void StartEditRoom(RoomData roomData)
        {
            _currentRoom = roomData;
            _isEditing = true;
            LogKit.Log($"开始编辑房间: {roomData.DefId}");
        }

        /// <summary>
        /// 停止编辑
        /// </summary>
        public void StopEditRoom()
        {
            if (_currentRoom != null)
            {
                _currentRoom.SaveTemporaryData();
                LogKit.Log($"保存并停止编辑房间: {_currentRoom.DefId}");
            }

            _currentRoom = null;
            _isEditing = false;
        }

        /// <summary>
        /// 切换编辑模式
        /// </summary>
        public void SetEditorMode(EditorMode mode)
        {
            _editorMode = mode;
            LogKit.Log($"切换编辑模式: {mode}");
        }

        /// <summary>
        /// 选择瓦片类型
        /// </summary>
        public void SelectTileType(ETileType tileType)
        {
            _selectedTileType = tileType;
            _editorMode = EditorMode.Tile;
        }

        /// <summary>
        /// 选择物体模板
        /// </summary>
        public void SelectObjectTemplate(PlaceableObjectData template)
        {
            _selectedObjectTemplate = template;
            _editorMode = EditorMode.Object;
        }

        #endregion

        #region 瓦片编辑

        /// <summary>
        /// 放置瓦片
        /// </summary>
        public bool PlaceTile(Vector2Int position, ETileType tileType)
        {
            if (!_isEditing || _currentRoom == null)
            {
                LogKit.Warning("未处于编辑状态");
                return false;
            }

            if (!IsPositionValid(position))
            {
                LogKit.Warning($"位置超出边界: {position}");
                return false;
            }

            var tile = new TileData(position, tileType);
            _currentRoom.SetTile(position, tile);

            LogKit.Log($"放置瓦片: {position}, 类型: {tileType}");
            return true;
        }

        /// <summary>
        /// 删除瓦片
        /// </summary>
        public bool RemoveTile(Vector2Int position)
        {
            if (!_isEditing || _currentRoom == null)
                return false;

            _currentRoom.RemoveTile(position);
            LogKit.Log($"删除瓦片: {position}");
            return true;
        }

        /// <summary>
        /// 批量填充瓦片
        /// </summary>
        public void FillArea(Vector2Int start, Vector2Int end, ETileType tileType)
        {
            if (!_isEditing || _currentRoom == null)
                return;

            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    PlaceTile(new Vector2Int(x, y), tileType);
                }
            }

            LogKit.Log($"填充区域: {start} 到 {end}, 类型: {tileType}");
        }

        /// <summary>
        /// 清空所有瓦片
        /// </summary>
        public void ClearAllTiles()
        {
            if (!_isEditing || _currentRoom == null)
                return;

            _currentRoom.TemporaryData.TileMap.Clear();
            LogKit.Log("清空所有瓦片");
        }

        #endregion

        #region 物体编辑

        /// <summary>
        /// 放置物体
        /// </summary>
        public bool PlaceObject(Vector2Int position, PlaceableObjectData objectData)
        {
            if (!_isEditing || _currentRoom == null)
                return false;

            objectData.Position = position;

            if (_currentRoom.PlaceObject(objectData))
            {
                LogKit.Log($"放置物体: {objectData.ObjectId} 在 {position}");
                return true;
            }

            LogKit.Warning($"无法放置物体在 {position}");
            return false;
        }

        /// <summary>
        /// 放置选中的物体模板
        /// </summary>
        public bool PlaceSelectedObject(Vector2Int position)
        {
            if (_selectedObjectTemplate == null)
            {
                LogKit.Warning("未选择物体模板");
                return false;
            }

            // 创建新实例
            var newObj = CloneObjectData(_selectedObjectTemplate);
            return PlaceObject(position, newObj);
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        public bool RemoveObjectAt(Vector2Int position)
        {
            if (!_isEditing || _currentRoom == null)
                return false;

            var obj = GetObjectAt(position);
            if (obj != null)
            {
                return _currentRoom.RemoveObject(obj.ObjectId);
            }

            return false;
        }

        /// <summary>
        /// 获取指定位置的物体
        /// </summary>
        public PlaceableObjectData GetObjectAt(Vector2Int position)
        {
            if (_currentRoom == null)
                return null;

            foreach (var obj in _currentRoom.TemporaryData.PlacedObjects)
            {
                if (position.x >= obj.Position.x && position.x < obj.Position.x + obj.Size.x &&
                    position.y >= obj.Position.y && position.y < obj.Position.y + obj.Size.y)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 移动物体
        /// </summary>
        public bool MoveObject(string objectId, Vector2Int newPosition)
        {
            if (!_isEditing || _currentRoom == null)
                return false;

            var obj = _currentRoom.TemporaryData.PlacedObjects.Find(o => o.ObjectId == objectId);
            if (obj == null)
                return false;

            // 暂时移除以检查新位置
            _currentRoom.RemoveObject(objectId);

            obj.Position = newPosition;

            // 尝试放置到新位置
            if (_currentRoom.PlaceObject(obj))
            {
                LogKit.Log($"移动物体 {objectId} 到 {newPosition}");
                return true;
            }
            else
            {
                // 放置失败,恢复原位置
                LogKit.Warning($"无法移动物体到 {newPosition}");
                return false;
            }
        }

        #endregion

        #region 辅助功能

        /// <summary>
        /// 处理鼠标点击
        /// </summary>
        public void HandleMouseClick(Vector2Int tilePos, bool isRightClick = false)
        {
            if (!_isEditing || _currentRoom == null)
                return;

            if (isRightClick)
            {
                // 右键删除
                if (_editorMode == EditorMode.Tile)
                {
                    RemoveTile(tilePos);
                }
                else if (_editorMode == EditorMode.Object)
                {
                    RemoveObjectAt(tilePos);
                }
            }
            else
            {
                // 左键放置
                if (_editorMode == EditorMode.Tile)
                {
                    PlaceTile(tilePos, _selectedTileType);
                }
                else if (_editorMode == EditorMode.Object)
                {
                    PlaceSelectedObject(tilePos);
                }
            }
        }

        /// <summary>
        /// 更新悬停位置
        /// </summary>
        public void UpdateHoveredPosition(Vector2Int position)
        {
            _hoveredTilePos = position;
        }

        /// <summary>
        /// 检查位置是否有效
        /// </summary>
        private bool IsPositionValid(Vector2Int position)
        {
            if (_currentRoom == null) return false;

            return position.x >= 0 && position.x < _currentRoom.DtoDef.Width &&
                   position.y >= 0 && position.y < _currentRoom.DtoDef.Height;
        }

        /// <summary>
        /// 克隆物体数据
        /// </summary>
        private PlaceableObjectData CloneObjectData(PlaceableObjectData source)
        {
            return new PlaceableObjectData
            {
                ObjectId = Guid.NewGuid().ToString("N").Substring(0, 8),
                ObjectType = source.ObjectType,
                Size = source.Size,
                Rotation = source.Rotation,
                PrefabPath = source.PrefabPath,
                BlocksMovement = source.BlocksMovement,
                Properties = new Dictionary<string, string>(source.Properties)
            };
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        public void AutoSave()
        {
            if (_currentRoom != null && _isEditing)
            {
                _currentRoom.SaveTemporaryData();
                LogKit.Log("自动保存房间数据");
            }
        }

        #endregion

        #region 属性访问

        public bool IsEditing => _isEditing;
        public RoomData CurrentRoom => _currentRoom;
        public EditorMode CurrentMode => _editorMode;
        public ETileType SelectedTileType => _selectedTileType;
        public Vector2Int HoveredPosition => _hoveredTilePos;

        #endregion
    }

    /// <summary>
    /// 编辑器模式
    /// </summary>
    public enum EditorMode
    {
        Tile, // 瓦片编辑
        Object, // 物体编辑
        Erase // 擦除模式
    }
}