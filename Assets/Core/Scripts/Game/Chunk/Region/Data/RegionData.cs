using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
namespace Core.Game.Chunk.Region.Data
{
    public class RegionData : ChunkContainerData
    {
        public RegionDtoDef RegionDef => DtoDef as RegionDtoDef;
        public RegionTemporaryData RegionTempData => TemporaryData as RegionTemporaryData;

        public void AddDungeon(string dungeonInstanceId) => AddChild(dungeonInstanceId);
        public void RemoveDungeon(string dungeonInstanceId) => RemoveChild(dungeonInstanceId);
        public void SetActiveDungeon(string dungeonInstanceId) => SetActiveChild(dungeonInstanceId);

        public void Load()
        {
            SaveTemporaryData();
        }

        public void Unload()
        {
            SaveTemporaryData();
        }


        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            throw new NotImplementedException();
        }

        protected override Type GetTemporaryDataType()
        {
            throw new NotImplementedException();
        }
    }
}