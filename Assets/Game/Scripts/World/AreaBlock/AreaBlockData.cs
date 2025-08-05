using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class AreaBlockData
    {
        [LabelText("地图区块固定数据")]
        public AreaBlockDataPersistent areaBlockDataPersistent;
        
        [LabelText("地图区块对局数据"),ReadOnly]
        public AreaBlockDataTemporary areaBlockDataTemporary;
        
        
    }
}