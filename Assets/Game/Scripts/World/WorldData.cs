using System.Collections.Generic;
using Game.World.Interface;
using Sirenix.OdinInspector;

namespace Game.World
{
    public class WorldData : IData
    {
        public string UniqueId { get; set; }        
        
        [LabelText("当前世界的固定数据")]
        private WorldDto worldDto;
        
        [LabelText("当前世界的临时数据")]
        private WorldDtoTemporary worldDtoTemporary;
        
        [LabelText("当前世界的区块数据")]
        private Dictionary<string, AreaBlockData> curHoldingAreaBlockDtoDict = new Dictionary<string, AreaBlockData>();


    }
}