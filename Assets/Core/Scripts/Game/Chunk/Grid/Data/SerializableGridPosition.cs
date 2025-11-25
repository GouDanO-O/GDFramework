using System;

namespace Core.Game.Chunk.Grid
{
    /// <summary>
    /// 可序列化的网格坐标
    /// </summary>
    [Serializable]
    public struct SerializableGridPosition
    {
        public int X;
        public int Y;
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

        public GridPosition ToRoomGridPosition()
        {
            return new GridPosition(X, Y, Z);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}