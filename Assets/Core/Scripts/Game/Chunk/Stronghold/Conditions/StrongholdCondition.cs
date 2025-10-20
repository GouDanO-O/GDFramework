using System;
using Core.Game.Chunk.Stronghold.Conditions.Interface;

namespace Core.Game.Chunk.Stronghold.Conditions
{
    [Serializable]
    public abstract class StrongholdCondition : IStrongholdCondition
    {
        public abstract bool CheckCondition();
    }
}