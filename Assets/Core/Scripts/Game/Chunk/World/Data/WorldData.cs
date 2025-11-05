using System;
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
    public class WorldData : ChunkContainerData
    {
        public WorldDtoDef WorldDef => DtoDef as WorldDtoDef;
        public WorldTemporaryData WorldTempData => TemporaryData as WorldTemporaryData;

        public void AddRegion(string regionInstanceId) => AddChild(regionInstanceId);
        public void RemoveRegion(string regionInstanceId) => RemoveChild(regionInstanceId);
        public void SetActiveRegion(string regionInstanceId) => SetActiveChild(regionInstanceId);

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