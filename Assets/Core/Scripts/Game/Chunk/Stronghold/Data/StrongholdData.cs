using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Stronghold.Data
{
    public class StrongholdData : ChunkData
    {
        [LabelText("副本数据")]
        private StrongholdDto strongholdDto;

        private StrongholdTemporaryData strongholdTemporaryData;
        
        [LabelText("当前副本持有的房间数据")]
        private Dictionary<string,RoomData> _curHoldingRoomDtoDict = new Dictionary<string, RoomData>();
    }
}