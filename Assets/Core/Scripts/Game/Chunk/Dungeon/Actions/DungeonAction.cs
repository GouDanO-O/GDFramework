using System;
using Core.Game.Chunk.Dungeon.Actions.Interface;

namespace Core.Game.Chunk.Dungeon.Actions
{
    [Serializable]
    public abstract class DungeonAction : IDungeonAction
    {
        public string name;
        
        public string description;
        
        public abstract void Execute();
    }
}