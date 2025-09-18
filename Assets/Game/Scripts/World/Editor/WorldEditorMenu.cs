using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Game.World.Editor
{
    public static class WorldEditorMenu
    {
        [MenuItem("Game/World/Sync & Validate Selected WorldDto")]
        public static void SyncAndValidateSelected()
        {
            var obj = Selection.activeObject as WorldDto;
            if (obj == null)
            {
                Debug.LogError("请选择一个 WorldDto 资产");
                return;
            }
            obj.SyncIdsAndIndexes();
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            Debug.Log($"[World] 已同步并校验: {obj.name}");
        }

        [MenuItem("Game/World/Export Selected WorldDto To StreamingAssets/BaseContent")] 
        public static void ExportSelected()
        {
            var obj = Selection.activeObject as WorldDto;
            if (obj == null)
            {
                Debug.LogError("请选择一个 WorldDto 资产");
                return;
            }

            // 确保同步与校验
            obj.SyncIdsAndIndexes();

            // 目标根目录
            string root = Path.Combine(Application.streamingAssetsPath, "BaseContent");
            Directory.CreateDirectory(root);

            var manifest = new ContentManifest
            {
                modId = "Base",
                gameVersion = Application.version,
                schemaVersion = 1,
                include = new ContentManifest.Includes()
            };

            // 导出 World
            var w = new WorldDef
            {
                id = obj.dtoId,
                stableUid = obj.stableUid,
                configId = obj.configId,
                initialWorldYearTime = obj.initialWorldYearTime,
                initialWorldMonthTime = obj.initialWorldMonthTime,
                initialWorldDayTime = obj.initialWorldDayTime,
                initialWorldHourTime = obj.initialWorldHourTime,
                initialWorldMinutesTime = obj.initialWorldMinutesTime,
                initialPlayerLocateAreaBlockId = obj.initialPlayerLocateAreaBlockId,
                areaBlockIds = new List<string>(obj.areaBlockIds)
            };
            WriteJson(root, w.id + ".json", w);
            manifest.include.worlds.Add(w.id + ".json");

            // 导出 AreaBlock / Room / Node
            foreach (var abDto in obj.areaBlockDatas)
            {
                if (abDto == null) continue;
                var ab = new AreaBlockDef
                {
                    id = abDto.dtoId,
                    stableUid = abDto.stableUid,
                    configId = abDto.configId,
                    parentId = obj.dtoId,
                    initialRoomId = abDto.initialRoomId,
                    willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock = abDto.willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock,
                    roomIds = new List<string>(abDto.roomIds)
                };
                WriteJson(root, ab.id + ".json", ab);
                manifest.include.areaBlocks.Add(ab.id + ".json");

                foreach (var roomDto in abDto.roomDatas)
                {
                    if (roomDto == null) continue;
                    var room = new RoomDef
                    {
                        id = roomDto.dtoId,
                        stableUid = roomDto.stableUid,
                        configId = roomDto.configId,
                        parentId = abDto.dtoId,
                        nodeIds = new List<string>(roomDto.nodeIds)
                    };
                    WriteJson(root, room.id + ".json", room);
                    manifest.include.rooms.Add(room.id + ".json");

                    foreach (var nodeDto in roomDto.nodeDatas)
                    {
                        if (nodeDto == null) continue;
                        var node = new NodeDef
                        {
                            id = nodeDto.dtoId,
                            stableUid = nodeDto.stableUid,
                            configId = nodeDto.configId,
                            parentId = roomDto.dtoId
                        };
                        WriteJson(root, node.id + ".json", node);
                        manifest.include.nodes.Add(node.id + ".json");
                    }
                }
            }

            // manifest.json
            WriteJson(root, "manifest.json", manifest, true);

            AssetDatabase.Refresh();
            Debug.Log($"[World] 导出完成: {root}");
        }

        static void WriteJson(string root, string relName, object obj, bool indented = false)
        {
            string path = Path.Combine(root, relName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var json = JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
            File.WriteAllText(path, json);
        }
    }
}


