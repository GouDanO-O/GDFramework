using System;
using Core.Game.Chunk.Region.Actions.Interface;

namespace Core.Game.Chunk.Region.Actions
{
    [Serializable]
    public abstract class RegionAction : IRegionAction
    {
        public abstract void Execute();
    }
}