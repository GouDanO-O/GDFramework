using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [LabelText("当前房间的数据")]
    public class RoomData : PersistentData
    {
        [LabelText("房间里面持有的根节点")]
        public List<ActionNode> ActionNodeList;
    }
    
    public class Room
    {
        public RoomData RoomData;


    }
}