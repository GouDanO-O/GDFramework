using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class WorldDataPersistent : ConfigData
    {
        [LabelText("世界名称")]
        public string worldName;

        [LabelText("世界ID")]
        public string worldId;
        
        [LabelText("区块数据列表")]
        public List<AreaBlockData> areaBlockDatas = new List<AreaBlockData>();
        
        public override void SaveConfigData()
        {
            
        }
    }
}