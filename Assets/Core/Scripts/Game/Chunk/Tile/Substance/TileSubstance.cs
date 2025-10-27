using System;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile.Substance
{
    [Serializable]
    public class TileSubstance
    {
        [LabelText("堆叠数量")]
        public int stackCount;

        [LabelText("物体ID")]
        public string tileSubstanceId;
    }
}