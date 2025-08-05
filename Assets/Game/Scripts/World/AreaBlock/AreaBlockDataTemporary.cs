using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class AreaBlockDataTemporary : TemporalityData
    {
        [LabelText("是否已经解锁")]
        public bool isUnlocked;

        [ShowIf("isUnlocked"),LabelText("当前探索的进度")] 
        public float curExploreProgress;
    }
}