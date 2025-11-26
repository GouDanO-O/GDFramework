using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块坐标
    /// 使用 X-Z 平面作为地面，Y 作为高度
    /// </summary>
    [Serializable]
    public struct TilePosition : IEquatable<TilePosition>
    {
        [JsonProperty]
        public int X;

        [JsonProperty]
        public int Z;

        public TilePosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static TilePosition Zero => new TilePosition(0, 0);

        /// <summary>
        /// 转换为世界坐标（地块中心点）
        /// </summary>
        /// <param name="tileSize">地块大小（米）</param>
        /// <param name="height">高度（Y轴）</param>
        public Vector3 ToWorldPosition(float tileSize = 1f, float height = 0f)
        {
            return new Vector3(
                (X + 0.5f) * tileSize,
                height,
                (Z + 0.5f) * tileSize
            );
        }

        /// <summary>
        /// 从世界坐标转换为地块坐标
        /// </summary>
        public static TilePosition FromWorldPosition(Vector3 worldPos, float tileSize = 1f)
        {
            return new TilePosition(
                Mathf.FloorToInt(worldPos.x / tileSize),
                Mathf.FloorToInt(worldPos.z / tileSize)
            );
        }

        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        public int ManhattanDistance(TilePosition other)
        {
            return Mathf.Abs(X - other.X) + Mathf.Abs(Z - other.Z);
        }

        /// <summary>
        /// 获取四方向邻居
        /// </summary>
        public TilePosition[] GetNeighbors()
        {
            return new[]
            {
                new TilePosition(X + 1, Z), // 右
                new TilePosition(X - 1, Z), // 左
                new TilePosition(X, Z + 1), // 前
                new TilePosition(X, Z - 1) // 后
            };
        }

        /// <summary>
        /// 获取八方向邻居（包括对角线）
        /// </summary>
        public TilePosition[] GetAllNeighbors()
        {
            return new[]
            {
                new TilePosition(X + 1, Z),
                new TilePosition(X - 1, Z),
                new TilePosition(X, Z + 1),
                new TilePosition(X, Z - 1),
                new TilePosition(X + 1, Z + 1),
                new TilePosition(X + 1, Z - 1),
                new TilePosition(X - 1, Z + 1),
                new TilePosition(X - 1, Z - 1)
            };
        }

        /// <summary>
        /// 生成用于字典的Key
        /// </summary>
        public string ToKey()
        {
            return $"{X}_{Z}";
        }

        public static TilePosition FromKey(string key)
        {
            var parts = key.Split('_');
            return new TilePosition(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public bool Equals(TilePosition other)
        {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Z;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Z})";
        }

        public static bool operator ==(TilePosition a, TilePosition b) => a.Equals(b);
        public static bool operator !=(TilePosition a, TilePosition b) => !a.Equals(b);
        public static TilePosition operator +(TilePosition a, TilePosition b) => new TilePosition(a.X + b.X, a.Z + b.Z);
        public static TilePosition operator -(TilePosition a, TilePosition b) => new TilePosition(a.X - b.X, a.Z - b.Z);
    }
}