using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable,JsonObject]
    public class AreaBlockData : ConfigData
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
        
        public void SaveConfigData(string path)
        {
            areaBlockDataPersistent.roomIds.Clear();
            
            if (configId == "")
            {
                configId = "default";
            }
            string areaBlockPath = path+ "/"+configId;
            for (int i = 0; i < areaBlockDataPersistent.roomDatas.Count; i++)
            {
                RoomData roomDataPersistent = areaBlockDataPersistent.roomDatas[i];
                string curId = roomDataPersistent.configId;
                if (areaBlockDataPersistent.roomIds.Contains(curId))
                {
                    LogMonoUtility.AddErrorLog("重复的房间ID");
                }
                else
                {
                    roomDataPersistent.SaveConfigData(areaBlockPath);
                    areaBlockDataPersistent.roomIds.Add(curId);
                }
            }
            base.SaveConfigData(path);
        }
    }
}