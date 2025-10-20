using System;
using Core.Game.Chunk.World.Conditions.Interface;

namespace Core.Game.Chunk.World.Conditions
{
    [Serializable]
    public abstract class WorldCondition  : IWorldCondition
    {
        public abstract bool CheckCondition();
    }
}