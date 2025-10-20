using System;
using Core.Game.Chunk.Stronghold.Actions.Interface;

namespace Core.Game.Chunk.Stronghold.Actions
{
    [Serializable]
    public abstract class StrongholdAction : IStrongholdAction
    {
        public string name;
        
        public string description;
        
        public abstract void Execute();
    }
}