using System;
using Newtonsoft.Json;

namespace Core.Game.Grid
{
    /// <summary>
    /// 可序列化的网格坐标
    /// </summary>
    [Serializable]
    public struct SerializableGridPosition : IEquatable<SerializableGridPosition>
    {
        [JsonProperty]
        public int X;
        
        [JsonProperty]
        public int Y;
        
        [JsonProperty]
        public int Z;

        public SerializableGridPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public SerializableGridPosition(GridPosition pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }

        public GridPosition ToGridPosition()
        {
            return new GridPosition(X, Y, Z);
        }

        public bool Equals(SerializableGridPosition other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableGridPosition other && Equals(other);
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
            return $"({X},{Y},{Z})";
        }

        public static bool operator ==(SerializableGridPosition a, SerializableGridPosition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SerializableGridPosition a, SerializableGridPosition b)
        {
            return !a.Equals(b);
        }
    }
}