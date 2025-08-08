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

        [LabelText("世界描述")]
        public string worldDes;

        [LabelText("初始区块ID(玩家第一次进入世界所处的区块ID)")]
        public string initialAreaBlockId;
        
        [LabelText("区块数据列表")]
        public List<AreaBlockData> areaBlockDatas = new List<AreaBlockData>();
        
        public override void SaveConfigData()
        {
            
        }


    }
}