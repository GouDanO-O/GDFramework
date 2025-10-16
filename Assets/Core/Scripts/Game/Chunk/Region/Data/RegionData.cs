using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Stronghold.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Region.Data
{
    public class RegionData : ChunkData
    {
        [LabelText("当前区块的固定数据")]
        private RegionDto _regionDto;

        [LabelText("当前区块的临时数据")]
        private RegionDtoTemporary _regionDtoTemporary;
        
        [LabelText("当前区块持有的副本数据")]
        private Dictionary<string,StrongholdData> _curHoldingStrongholdDtoDict = new Dictionary<string, StrongholdData>();
    }
}