using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Room.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Dungeon.Data
{
    public class DungeonData : ChunkContainerData
    {
        public DungeonDtoDef DungeonDef => DtoDef as DungeonDtoDef;
        public DungeonTemporaryData DungeonTempData => TemporaryData as DungeonTemporaryData;
        

        public void AddRoom(string roomInstanceId) => AddChild(roomInstanceId);
        public void RemoveRoom(string roomInstanceId) => RemoveChild(roomInstanceId);
        public void SetActiveRoom(string roomInstanceId) => SetActiveChild(roomInstanceId);
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