using System;
using GDFrameworkExtend.Data;

namespace Game.World
{
    [Serializable]
    public class AreaBlockDataTemporary : TemporalityData
    {
        public bool isUnlocked;
        
        public bool isExplored;
    }
}