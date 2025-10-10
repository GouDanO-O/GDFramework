using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine.Analytics;

namespace Core.Game.Chunk.World.Data
{
    public class WorldData : ChunkData
    {
        [LabelText("当前世界的固定数据")]
        private WorldDto _worldDto;
        
        [LabelText("当前世界的临时数据")]
        private WorldDtoTemporary _worldDtoTemporary;
        
        [LabelText("当前世界的区块数据")]
        private Dictionary<string, RegionData> _curHoldingRegionDtoDict = new Dictionary<string, RegionData>();

        public override void InitDto(IChunkDto chunkDto)
        {
            if (chunkDto is WorldDto)
            {
                this._worldDto = (WorldDto)chunkDto;
            }
        }

        public override void InitTemporaryData(ITemporaryData temporaryData)
        {
            
        }

        public override void SaveTemporaryData()
        {
            
        }
    }
}