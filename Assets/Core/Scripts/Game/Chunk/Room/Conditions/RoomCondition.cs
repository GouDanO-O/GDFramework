using Core.Game.Chunk.Room.Conditions.Interface;

namespace Core.Game.Chunk.Room.Conditions
{
    /// <summary>
    /// 进入房间所需要的条件
    /// </summary>
    public abstract class RoomCondition : IRoomCondition
    {
        public abstract bool CheckCondition();
    }
}