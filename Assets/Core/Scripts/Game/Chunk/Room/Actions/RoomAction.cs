using Core.Game.Action;
using Core.Game.Chunk.Room.Actions.Interface;

namespace Core.Game.Chunk.Room.Actions
{
    /// <summary>
    /// 在房间内会触发的事件
    /// 可以是进入房间时触发,也可以是离开房间时触发
    /// 还可以是在房间中获得的某个物品就会触发
    /// </summary>
    public abstract class RoomAction : IRoomAction
    {
        public abstract void Execute();
    }
}