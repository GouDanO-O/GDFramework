using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    /// <summary>
    /// 房间
    /// 房间里面存有所持有的所有格子块
    /// 包括格子块上的放置的物体
    /// </summary>
    public class RoomData : ChunkData
    {
        public new RoomDtoDef DtoDef => base.DtoDef as RoomDtoDef;
        public new RoomTemporaryData TemporaryData => base.TemporaryData as RoomTemporaryData;

        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            return new RoomTemporaryData(DefId);
        }

        protected override Type GetTemporaryDataType()
        {
            return typeof(RoomTemporaryData);
        }

        /// <summary>
        /// 获取指定位置的瓦片
        /// </summary>
        public TileData GetTile(Vector2Int pos)
        {
            string key = RoomTemporaryData.GetTileKey(pos);
            return TemporaryData.TileMap.TryGetValue(key, out var tile) ? tile : null;
        }

        /// <summary>
        /// 设置瓦片
        /// </summary>
        public void SetTile(Vector2Int pos, TileData tile)
        {
            string key = RoomTemporaryData.GetTileKey(pos);
            TemporaryData.TileMap[key] = tile;
            TemporaryData.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 删除瓦片
        /// </summary>
        public void RemoveTile(Vector2Int pos)
        {
            string key = RoomTemporaryData.GetTileKey(pos);
            TemporaryData.TileMap.Remove(key);
            TemporaryData.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 检查位置是否可行走
        /// </summary>
        public bool IsWalkable(Vector2Int pos)
        {
            var tile = GetTile(pos);
            if (tile == null || !tile.Walkable)
                return false;

            // 检查是否有阻挡的物体
            foreach (var obj in TemporaryData.PlacedObjects)
            {
                if (!obj.BlocksMovement) continue;
                
                if (pos.x >= obj.Position.x && pos.x < obj.Position.x + obj.Size.x &&
                    pos.y >= obj.Position.y && pos.y < obj.Position.y + obj.Size.y)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 放置物体
        /// </summary>
        public bool PlaceObject(PlaceableObjectData obj)
        {
            // 检查位置是否有效
            if (!CanPlaceObject(obj.Position, obj.Size))
                return false;

            TemporaryData.PlacedObjects.Add(obj);
            TemporaryData.LastModifyTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        public bool RemoveObject(string objectId)
        {
            var obj = TemporaryData.PlacedObjects.Find(o => o.ObjectId == objectId);
            if (obj != null)
            {
                TemporaryData.PlacedObjects.Remove(obj);
                TemporaryData.LastModifyTime = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否可以放置物体
        /// </summary>
        private bool CanPlaceObject(Vector2Int pos, Vector2Int size)
        {
            // 检查边界
            if (pos.x < 0 || pos.y < 0 || 
                pos.x + size.x > DtoDef.Width || 
                pos.y + size.y > DtoDef.Height)
                return false;

            // 检查是否与其他物体重叠
            foreach (var existing in TemporaryData.PlacedObjects)
            {
                if (IsOverlapping(pos, size, existing.Position, existing.Size))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 检查两个矩形是否重叠
        /// </summary>
        private bool IsOverlapping(Vector2Int pos1, Vector2Int size1, Vector2Int pos2, Vector2Int size2)
        {
            return !(pos1.x + size1.x <= pos2.x || pos2.x + size2.x <= pos1.x ||
                     pos1.y + size1.y <= pos2.y || pos2.y + size2.y <= pos1.y);
        }
    }
}