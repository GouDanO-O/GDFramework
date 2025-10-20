using System;
using Core.Game.Chunk.Region.Conditions.Interface;

namespace Core.Game.Chunk.Region.Conditions
{
    [Serializable]
    public abstract class RegionCondition : IRegionCondition
    {
        public abstract bool CheckCondition();
    }
}