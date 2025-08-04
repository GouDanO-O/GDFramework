using System;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class WorldData
    {
        [LabelText("世界固定数据")]
        public WorldDataPersistent worldDataPersistent;
        
        [LabelText("世界对局数据")]
        public WorldDataTemporary worldDataTemporary;
        
    }
}