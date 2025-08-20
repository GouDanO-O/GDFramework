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
    [Serializable, JsonObject]
    public class AreaBlockData : ConfigData
    {
        [LabelText("地图区块固定数据")] public AreaBlockDataPersistent areaBlockDataPersistent;

        [LabelText("地图区块对局数据"), ReadOnly] public AreaBlockDataTemporary areaBlockDataTemporary;

        public override void SaveConfigData(string worldRootDir, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "area_default";

            areaBlockDataPersistent ??= new AreaBlockDataPersistent();
            areaBlockDataPersistent.roomIds ??= new List<string>();
            areaBlockDataPersistent.roomDatas ??= new List<RoomData>();
            areaBlockDataPersistent.roomIds.Clear();

            // 1) 房间父目录
            string areaDir = Path.Combine(worldRootDir, configId);
            Directory.CreateDirectory(areaDir);

            // 2) 逐房间保存（房间 JSON 写在 areaDir；节点由 Room 处理）
            foreach (var room in areaBlockDataPersistent.roomDatas.Where(r => r != null))
            {
                string rid = string.IsNullOrEmpty(room.configId) ? "room_auto" : room.configId;
                if (areaBlockDataPersistent.roomIds.Contains(rid))
                    LogMonoUtility.AddErrorLog($"重复的房间ID: {rid}");
                else
                    areaBlockDataPersistent.roomIds.Add(rid);

                room.SaveConfigData(areaDir, settings ?? JsonSettings.Make());
            }

            // 3) 区块自身 JSON：写在 worldRootDir（与 world 同级目录下的 worldId 子目录里）
            base.SaveConfigData(worldRootDir, settings ?? JsonSettings.Make());
        }
    }
}