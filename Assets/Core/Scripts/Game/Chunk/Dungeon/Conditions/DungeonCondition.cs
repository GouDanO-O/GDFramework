using System;
using Core.Game.Chunk.Dungeon.Conditions.Interface;

namespace Core.Game.Chunk.Dungeon.Conditions
{
    [Serializable]
    public abstract class DungeonCondition : IDungeonCondition
    {
        public abstract bool CheckCondition();
    }
}