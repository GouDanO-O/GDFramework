// Assets/Scripts/Game/World/Editor/ContentExporter.cs
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Game.World;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Game.World.Editor
{
    public static class ContentExporter
    {
        const string DefaultOutRoot = "Assets/StreamingAssets/BaseContent";

        [MenuItem("Tools/Content/Export Selected Worlds")]
        public static void ExportSelected()
        {
            var worlds = new List<WorldDto>();
            foreach (var obj in Selection.objects)
                if (obj is WorldDto w) worlds.Add(w);

            if (worlds.Count == 0)
            {
                Debug.LogWarning("请选择一个或多个 WorldDto 资产再导出。");
                return;
            }
            Export(worlds, DefaultOutRoot);
        }

        [MenuItem("Tools/Content/Export All Worlds")]
        public static void ExportAll()
        {
            var guids = AssetDatabase.FindAssets("t:Game.World.WorldDto");
            var worlds = new List<WorldDto>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var w = AssetDatabase.LoadAssetAtPath<WorldDto>(path);
                if (w) worlds.Add(w);
            }
            Export(worlds, DefaultOutRoot);
        }

        static void Export(List<WorldDto> worlds, string rootOut)
        {
            if (!Directory.Exists(rootOut)) Directory.CreateDirectory(rootOut);
            var pathWorlds = Path.Combine(rootOut, "worlds");
            var pathAreaBlocks = Path.Combine(rootOut, "areaBlocks");
            var pathRooms = Path.Combine(rootOut, "rooms");
            var pathNodes = Path.Combine(rootOut, "nodes");
            Directory.CreateDirectory(pathWorlds);
            Directory.CreateDirectory(pathAreaBlocks);
            Directory.CreateDirectory(pathRooms);
            Directory.CreateDirectory(pathNodes);

            var manifest = new ContentManifest { modId = "Base" };

            foreach (var wso in worlds)
            {
                // 先同步 dtoId / 列表
                wso.SyncIdsAndIndexes();

                // --- World ---
                var wdef = new WorldDef
                {
                    id = wso.dtoId,
                    stableUid = wso.stableUid,
                    configId = wso.configId,
                    initialWorldYearTime = wso.initialWorldYearTime,
                    initialWorldMonthTime = wso.initialWorldMonthTime,
                    initialWorldDayTime = wso.initialWorldDayTime,
                    initialWorldHourTime = wso.initialWorldHourTime,
                    initialWorldMinutesTime = wso.initialWorldMinutesTime,
                    initialPlayerLocateAreaBlockId = wso.initialPlayerLocateAreaBlockId,
                    areaBlockIds = new List<string>(wso.areaBlockIds)
                };
                var worldFile = Path.Combine(pathWorlds, $"{wso.configId}.json");
                WriteJson(worldFile, wdef);
                manifest.include.worlds.Add(Rel(worldFile, rootOut));

                // --- AreaBlocks ---
                foreach (var abso in wso.areaBlockDatas)
                {
                    if (!abso) continue;
                    var abdef = new AreaBlockDef
                    {
                        id = abso.dtoId,
                        stableUid = abso.stableUid,
                        configId = abso.configId,
                        initialRoomId = string.IsNullOrEmpty(abso.initialRoomId)
                            ? (abso.roomIds.Count > 0 ? abso.roomIds[0] : null)
                            : abso.initialRoomId,
                        willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock =
                            abso.willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock,
                        roomIds = new List<string>(abso.roomIds)
                    };
                    var abFile = Path.Combine(pathAreaBlocks, $"{abso.configId}.json");
                    WriteJson(abFile, abdef);
                    manifest.include.areaBlocks.Add(Rel(abFile, rootOut));

                    // --- Rooms ---
                    foreach (var rso in abso.roomDatas)
                    {
                        if (!rso) continue;
                        var rdef = new RoomDef
                        {
                            id = rso.dtoId,
                            stableUid = rso.stableUid,
                            configId = rso.configId,
                            nodeIds = new List<string>(rso.nodeIds)
                        };
                        var rFile = Path.Combine(pathRooms, $"{rso.configId}.json");
                        WriteJson(rFile, rdef);
                        manifest.include.rooms.Add(Rel(rFile, rootOut));

                        // --- Nodes（如果你已有 NodeDto，可在这里导出） ---
                        // 这里放个兜底：从 nodeIds 直接生成 NodeDef（字段少时够用）
                        foreach (var nid in rso.nodeIds)
                        {
                            // 导出一个最小 NodeDef（如果你有 NodeDto 资产，建议改成按 SO 实体导出）
                            var ndef = new NodeDef
                            {
                                id = nid,
                                stableUid = "", // 如果有 NodeDto SO，请填对应 GUID
                                configId = nid.Substring(nid.LastIndexOf('/') + 1)
                            };
                            var nFile = Path.Combine(pathNodes, $"{ndef.configId}.json");
                            if (!File.Exists(nFile)) // 避免重复写（不同房间可能引用同名节点）
                            {
                                WriteJson(nFile, ndef);
                                manifest.include.nodes.Add(Rel(nFile, rootOut));
                            }
                        }
                    }
                }
            }

            // manifest
            var manifestFile = Path.Combine(rootOut, "manifest.json");
            WriteJson(manifestFile, manifest);

            AssetDatabase.Refresh();
            Debug.Log($"[Exporter] 导出完成。Root: {rootOut}");
        }

        static void WriteJson(string fullPath, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(fullPath, json);
        }

        static string Rel(string full, string root)
        {
            full = full.Replace("\\", "/");
            root = root.Replace("\\", "/").TrimEnd('/');
            return full.StartsWith(root) ? full.Substring(root.Length + 1) : full;
        }
    }
}
#endif
