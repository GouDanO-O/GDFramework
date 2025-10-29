using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Room.Data
{
    public class RoomDataModel : ChunkDataModel
    {
        private ChunkDataManager _roomDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _roomDataManager = new ChunkDataManager();
            return _roomDataManager;
        }

        protected override void InitializeDataManager()
        {
            _roomDataManager.RegisterTypeFactory<RoomData>(() => new RoomData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载Room配置
        }

        public RoomData CreateRoom(string defId) => _roomDataManager.CreateInstance<RoomData>(defId);
        public RoomData LoadRoom(string instanceId) => _roomDataManager.LoadInstance<RoomData>(instanceId);
        public RoomData GetRoom(string instanceId) => _roomDataManager.GetInstance(instanceId) as RoomData;
    }
}