using System;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 网格统计信息
    /// </summary>
    [Serializable]
    public struct RoomGridStatistics
    {
        public int TotalTiles;
        public int WalkableTiles;
        public int PlaceableTiles;
        public int OccupiedTiles;
        public int PlacedObjects;

        public override string ToString()
        {
            return $"Tiles:{TotalTiles} Walkable:{WalkableTiles} Placeable:{PlaceableTiles} " +
                   $"Occupied:{OccupiedTiles} Objects:{PlacedObjects}";
        }
    }
}