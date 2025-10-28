using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Room.Data
{
    public class RoomDataModel : ChunkDataModel
    {
        private RoomDataManager _roomDataManager;
        
        protected override IChunkDataManager CreateDataManager()
        {
            _roomDataManager = new RoomDataManager();
            return _roomDataManager;
        }

        protected override void InitializeDataManager()
        {
            // 注册类型工厂
            _roomDataManager.RegisterTypeFactory<RoomData>(() => new RoomData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            
        }
        
        /// <summary>
        /// 创建新房间实例
        /// </summary>
        public RoomData CreateUniverse(string defId)
        {
            return _roomDataManager.CreateInstance<RoomData>(defId);
        }
        
        /// <summary>
        /// 加载房间实例
        /// </summary>
        public RoomData LoadUniverse(string instanceId)
        {
            return _roomDataManager.LoadInstance<RoomData>(instanceId);
        }
    }
}