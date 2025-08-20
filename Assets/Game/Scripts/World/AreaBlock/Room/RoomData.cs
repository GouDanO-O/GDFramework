using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable, JsonObject]
    public class RoomData : ConfigData
    {
        [LabelText("房间固定数据")] public RoomDataPersistent roomDataPersistent;

        [LabelText("房间对局数据"), ReadOnly] public RoomDataTemporary roomDataTemporary;

        public override void SaveConfigData(string areaDir, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "room_default";

            roomDataPersistent ??= new RoomDataPersistent();
            roomDataPersistent.nodeIds ??= new List<string>();
            roomDataPersistent.nodeDatas ??= new List<NodeData>();
            roomDataPersistent.nodeIds.Clear();

            // 1) 节点父目录
            string roomDir = Path.Combine(areaDir, configId);
            Directory.CreateDirectory(roomDir);

            // 2) 逐节点保存（节点 JSON 写在 roomDir）
            foreach (var node in roomDataPersistent.nodeDatas.Where(n => n != null))
            {
                string nid = string.IsNullOrEmpty(node.configId) ? "node_auto" : node.configId;
                if (roomDataPersistent.nodeIds.Contains(nid))
                    LogMonoUtility.AddErrorLog($"重复的节点ID: {nid}");
                else
                    roomDataPersistent.nodeIds.Add(nid);

                node.SaveConfigData(roomDir, settings ?? JsonSettings.Make());
            }

            // 3) 房间自身 JSON：写在 areaDir
            base.SaveConfigData(areaDir, settings ?? JsonSettings.Make());
        }
    }
}