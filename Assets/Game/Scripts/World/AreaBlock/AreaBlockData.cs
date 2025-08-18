using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class AreaBlockData
    {
        [LabelText("地图区块固定数据")]
        public AreaBlockDataPersistent areaBlockDataPersistent;
        
        [LabelText("地图区块对局数据"),ReadOnly]
        public AreaBlockDataTemporary areaBlockDataTemporary;
        
        private Dictionary<string,RoomData> _roomDataDict = new Dictionary<string, RoomData>();
        
        public RoomData GetCurrentRoomData(string roomId)
        {
            if (areaBlockDataPersistent == null)
            {
                LogMonoUtility.AddErrorLog("世界固定数据为空");
                return null;
            }

            if (areaBlockDataPersistent.roomDatas.Count == 0)
            {
                LogMonoUtility.AddErrorLog("区块中的房间数据为空");
                return null;
            }

            if (_roomDataDict.Count == 0)
            {
                LogMonoUtility.AddErrorLog("区块里面的房间字典为空");
                return null;
            }
            
            if (_roomDataDict.ContainsKey(roomId))
            {
                return _roomDataDict[roomId];
            }

            LogMonoUtility.AddErrorLog("区块字典未包含该ID");
            return null;
        }
    }
}