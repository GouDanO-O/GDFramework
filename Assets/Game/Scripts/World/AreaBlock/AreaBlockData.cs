using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable,JsonObject]
    public struct AreaBlockDto
    {
        [LabelText("地图区块固定数据")] 
        public AreaBlockDataPersistent areaBlockDataPersistent;

        [LabelText("地图区块对局数据"), ReadOnly] 
        public AreaBlockDataTemporary areaBlockDataTemporary;
    }
    
    [Serializable, JsonObject]
    public class AreaBlockData : ConfigData
    {
        public AreaBlockDto areaBlockDto;

        public override void SaveConfigData(string worldRootDir, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "area_default";

            areaBlockDto.areaBlockDataPersistent ??= new AreaBlockDataPersistent();
            areaBlockDto.areaBlockDataPersistent.roomIds ??= new List<string>();
            areaBlockDto.areaBlockDataPersistent.roomDatas ??= new List<RoomData>();
            areaBlockDto.areaBlockDataPersistent.roomIds.Clear();

            // 1) 房间父目录
            string areaDir = Path.Combine(worldRootDir, configId);
            Directory.CreateDirectory(areaDir);

            // 2) 逐房间保存（房间 JSON 写在 areaDir；节点由 Room 处理）
            foreach (var room in areaBlockDto.areaBlockDataPersistent.roomDatas.Where(r => r != null))
            {
                string rid = string.IsNullOrEmpty(room.configId) ? "room_auto" : room.configId;
                if (areaBlockDto.areaBlockDataPersistent.roomIds.Contains(rid))
                    LogMonoUtility.AddErrorLog($"重复的房间ID: {rid}");
                else
                    areaBlockDto.areaBlockDataPersistent.roomIds.Add(rid);

                room.SaveConfigData(areaDir, settings ?? JsonSettings.Make());
            }

            // 3) 区块自身 JSON：写在 worldRootDir（与 world 同级目录下的 worldId 子目录里）
            base.SaveConfigData(worldRootDir, settings ?? JsonSettings.Make());
        }
    }
}