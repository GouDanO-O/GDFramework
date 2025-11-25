using System;
using UnityEngine;

namespace Core.Game.Chunk.Grid
{
    /// <summary>
    /// 3D房间网格坐标
    /// </summary>
    [Serializable]
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int X;
        public int Y;
        public int Z;

        public GridPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static GridPosition Zero => new GridPosition(0, 0, 0);

        /// <summary>
        /// 转换为世界坐标
        /// </summary>
        public Vector3 ToWorldPosition(float cellSize = 1f)
        {
            return new Vector3(X * cellSize, Y * cellSize, Z * cellSize);
        }

        /// <summary>
        /// 从世界坐标转换
        /// </summary>
        public static GridPosition FromWorldPosition(Vector3 worldPos, float cellSize = 1f)
        {
            return new GridPosition(
                Mathf.FloorToInt(worldPos.x / cellSize),
                Mathf.FloorToInt(worldPos.y / cellSize),
                Mathf.FloorToInt(worldPos.z / cellSize)
            );
        }

        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        public int ManhattanDistance(GridPosition other)
        {
            return Mathf.Abs(X - other.X) + Mathf.Abs(Y - other.Y) + Mathf.Abs(Z - other.Z);
        }

        /// <summary>
        /// 欧几里得距离
        /// </summary>
        public float EuclideanDistance(GridPosition other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            int dz = Z - other.Z;
            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 获取相邻的6个方向位置
        /// </summary>
        public GridPosition[] GetNeighbors()
        {
            return new[]
            {
                new GridPosition(X + 1, Y, Z),  // 右
                new GridPosition(X - 1, Y, Z),  // 左
                new GridPosition(X, Y + 1, Z),  // 上
                new GridPosition(X, Y - 1, Z),  // 下
                new GridPosition(X, Y, Z + 1),  // 前
                new GridPosition(X, Y, Z - 1)   // 后
            };
        }

        /// <summary>
        /// 获取26个方向的邻居(包括对角线)
        /// </summary>
        public GridPosition[] GetAllNeighbors()
        {
            var neighbors = new GridPosition[26];
            int index = 0;
            
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0) continue;
                        neighbors[index++] = new GridPosition(X + dx, Y + dy, Z + dz);
                    }
                }
            }
            
            return neighbors;
        }

        public bool Equals(GridPosition other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + X;
                hash = hash * 31 + Y;
                hash = hash * 31 + Z;
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !a.Equals(b);
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }
}