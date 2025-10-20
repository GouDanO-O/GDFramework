using System;
using Core.Game.Chunk.World.Actions.Interface;

namespace Core.Game.Chunk.World.Actions
{
    [Serializable]
    public abstract class WorldAction : IWorldAction
    {
        public abstract void Execute();
    }
}