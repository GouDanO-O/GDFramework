using System.Collections.Generic;
using GDFrameworkExtend.Data;

namespace Game.World
{
    
    public class AreaBlockData : PersistentData
    {
        
    }
    
    /// <summary>
    /// 每个区块里面包含有多个房间
    /// 区块都必定会有入口,但是不一定会有出口
    /// 同时,也可能一个区块具有多个入口或者多个出口
    /// </summary>
    public class AreaBlock
    {
        public List<Room> AreaBlockRoomList;
    }
}