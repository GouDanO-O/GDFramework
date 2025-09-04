// Assets/Scripts/Game/World/Runtime/SaveService.cs
using System;
using System.Collections.Generic;
using System.IO;
using Game.World.Tools;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class WorldSave
    {
        public int saveVersion = 1;
        public string gameVersion = "1.0.0";

        public string worldId; // dtoId
        public TimeData time = new();
        public PlayerData player = new();

        // 以区块 dtoId 为键，记录“上次所在房间”以支持 willCache...
        public Dictionary<string, string> lastRoomInAreaBlock = new();

        [Serializable] public class TimeData { public int year, month, day, hour, minute; }
        [Serializable] public class PlayerData { public string curAreaBlockId; public string curRoomId; }
    }

    public sealed class SaveService
    {
        readonly string _root;

        public SaveService()
        {
            _root = Path.Combine(Application.persistentDataPath, "Saves");
        }

        public string GetSavePath(string profileId, string worldInstanceId)
        {
            var dir = Path.Combine(_root, profileId);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{worldInstanceId}.json");
        }

        public void Save(string profileId, string worldInstanceId, WorldSave save)
        {
            var path = GetSavePath(profileId, worldInstanceId);
            File.WriteAllText(path, JsonConvert.SerializeObject(save, Formatting.Indented));
        }

        public WorldSave Load(string profileId, string worldInstanceId)
        {
            var path = GetSavePath(profileId, worldInstanceId);
            if (!File.Exists(path)) return null;
            return JsonConvert.DeserializeObject<WorldSave>(File.ReadAllText(path));
        }

        // 基于 World/AreaBlock Def 生成一个“首存档”
        public WorldSave CreateFirstSave(JsonPersistentRepo repo, string worldId)
        {
            var w = repo.Get<WorldDef>(worldId);
            if (w == null) throw new Exception($"找不到世界定义: {worldId}");

            var save = new WorldSave
            {
                worldId = w.id,
                time = new WorldSave.TimeData
                {
                    year = w.initialWorldYearTime,
                    month = w.initialWorldMonthTime,
                    day = w.initialWorldDayTime,
                    hour = w.initialWorldHourTime,
                    minute = w.initialWorldMinutesTime
                },
                player = new WorldSave.PlayerData
                {
                    curAreaBlockId = string.IsNullOrEmpty(w.initialPlayerLocateAreaBlockId)
                        ? (w.areaBlockIds.Count > 0 ? w.areaBlockIds[0] : null)
                        : w.initialPlayerLocateAreaBlockId,
                    curRoomId = null // 稍后根据 AreaBlockDef 决定
                }
            };

            // 选择初始房间
            var ab = repo.Get<AreaBlockDef>(save.player.curAreaBlockId);
            if (ab != null)
            {
                save.player.curRoomId = string.IsNullOrEmpty(ab.initialRoomId)
                    ? (ab.roomIds.Count > 0 ? ab.roomIds[0] : null)
                    : ab.initialRoomId;

                // 预置上次房间
                save.lastRoomInAreaBlock[ab.id] = save.player.curRoomId;
            }

            return save;
        }

        // 切换区块时根据策略选择进入房间
        public string ResolveEnterRoom(JsonPersistentRepo repo, WorldSave save, string targetAreaBlockId)
        {
            var ab = repo.Get<AreaBlockDef>(targetAreaBlockId);
            if (ab == null) return null;

            // 是否缓存历史房间
            if (ab.willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock)
            {
                if (save.lastRoomInAreaBlock.TryGetValue(targetAreaBlockId, out var cached) && !string.IsNullOrEmpty(cached))
                    return cached;
            }

            // 否则初始房间或列表首
            return string.IsNullOrEmpty(ab.initialRoomId)
                ? (ab.roomIds.Count > 0 ? ab.roomIds[0] : null)
                : ab.initialRoomId;
        }

        // 玩家离开区块时记录“上次房间”
        public void RecordLastRoom(WorldSave save, string areaBlockId, string roomId)
        {
            if (string.IsNullOrEmpty(areaBlockId) || string.IsNullOrEmpty(roomId)) return;
            save.lastRoomInAreaBlock[areaBlockId] = roomId;
        }
    }
}


// // 例如在游戏入口的初始化流程中：
// using UnityEngine;
//
// namespace Game.World
// {
//     public class Bootstrap : MonoBehaviour
//     {
//         IPersistentRepo _repo;
//         SaveService _save;
//         WorldSave _cur;
//
//         void Awake()
//         {
//             // 1) 路径（你可替换为 YooAsset RawFile 解包后的路径）
//             string baseRoot = System.IO.Path.Combine(Application.streamingAssetsPath, "BaseContent");
//             string modsRoot = System.IO.Path.Combine(Application.persistentDataPath, "Mods");
//
//             _repo = new JsonPersistentRepo(baseRoot, modsRoot);
//             _save = new SaveService();
//
//             // 2) 找一个世界开始（示例取第一个）
//             var enumerator = (_repo as JsonPersistentRepo).AllWorlds().GetEnumerator();
//             if (!enumerator.MoveNext()) { Debug.LogError("没有可用世界定义"); return; }
//             string worldId = enumerator.Current.id;
//
//             // 3) 如果没有存档 → 生成首存档
//             _cur = _save.Load("defaultProfile", "world_1");
//             if (_cur == null)
//             {
//                 _cur = _save.CreateFirstSave(_repo as JsonPersistentRepo, worldId);
//                 _save.Save("defaultProfile", "world_1", _cur);
//             }
//
//             Debug.Log($"世界: {_cur.worldId}  区块: {_cur.player.curAreaBlockId}  房间: {_cur.player.curRoomId}");
//         }
//     }
// }

