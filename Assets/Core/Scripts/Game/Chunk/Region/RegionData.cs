using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Region
{
    public class RegionData : ChunkData
    {
        public string UniqueId { get; set; }     
        
        [LabelText("当前区块的固定数据")]
        private RegionDto regionDto;

        [LabelText("当前区块的临时数据")]
        private RegionDtoTemporary regionDtoTemporary;
        
        [LabelText("当前区块持有的房间数据")]
        private Dictionary<string,RoomData> curHoldingRoomDtoDict = new Dictionary<string, RoomData>();
    }
}