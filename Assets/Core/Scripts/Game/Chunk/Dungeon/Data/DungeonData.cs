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
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new DungeonTemporaryData(defId);
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<DungeonTemporaryData>(instanceId);
        }

        public void AddRoom(string roomInstanceId) => AddChild(roomInstanceId);
        public void RemoveRoom(string roomInstanceId) => RemoveChild(roomInstanceId);
        public List<string> GetAllRoomIds() => GetAllChildIds();
        public void SetActiveRoom(string roomInstanceId) => SetActiveChild(roomInstanceId);
        public string GetActiveRoomId() => GetActiveChildId();

        public void Complete()
        {
            SaveTemporaryData();
        }
    }
}