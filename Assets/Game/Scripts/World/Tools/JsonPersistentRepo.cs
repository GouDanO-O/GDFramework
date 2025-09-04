// Assets/Scripts/Game/World/Runtime/JsonPersistentRepo.cs
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Game.World.Tools
{
    public interface IPersistentRepo
    {
        T Get<T>(string dtoId) where T : class;
        IEnumerable<WorldDef> AllWorlds();
        IEnumerable<AreaBlockDef> AllAreaBlocks();
        IEnumerable<RoomDef> AllRooms();
        IEnumerable<NodeDef> AllNodes();
    }

    public class JsonPersistentRepo : IPersistentRepo
    {
        readonly Dictionary<string, JToken> _index = new();
        readonly JsonSerializer _ser;

        public JsonPersistentRepo(string baseRoot, string modsRoot)
        {
            _ser = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            };

            // 1) Base：如果有 manifest.json 用 manifest；否则全量扫描
            var baseManifest = Path.Combine(baseRoot, "manifest.json");
            if (File.Exists(baseManifest))
                LoadViaManifest(baseRoot, baseManifest);
            else
                ScanAllJson(baseRoot);

            // 2) Mods：每个 mod 目录需要包含 manifest.json
            if (Directory.Exists(modsRoot))
            {
                foreach (var modDir in Directory.GetDirectories(modsRoot))
                {
                    var mf = Path.Combine(modDir, "manifest.json");
                    if (File.Exists(mf))
                        LoadViaManifest(modDir, mf); // 覆盖同 id
                }
            }

            Debug.Log($"[Repo] 索引完成，条目数：{_index.Count}");
        }

        void LoadViaManifest(string root, string manifestPath)
        {
            var mf = Read<ContentManifest>(manifestPath);
            if (mf?.include == null) return;

            void LoadList(List<string> rels)
            {
                foreach (var rel in rels)
                {
                    var full = SafeJoin(root, rel);
                    IndexOne(full);
                }
            }

            LoadList(mf.include.worlds);
            LoadList(mf.include.areaBlocks);
            LoadList(mf.include.rooms);
            LoadList(mf.include.nodes);
        }

        void ScanAllJson(string root)
        {
            if (!Directory.Exists(root)) return;
            foreach (var file in Directory.GetFiles(root, "*.json", SearchOption.AllDirectories))
                IndexOne(file);
        }

        void IndexOne(string path)
        {
            if (!File.Exists(path)) return;
            try
            {
                var token = JToken.Parse(File.ReadAllText(path));
                var id = token.Value<string>("id");
                if (!string.IsNullOrEmpty(id))
                    _index[id] = token; // 后加载覆盖前加载
            }
            catch (Exception e)
            {
                Debug.LogError($"[Repo] 解析失败: {path}\n{e}");
            }
        }

        T Read<T>(string path) where T : class
        {
            using var sr = new StreamReader(path);
            using var jr = new JsonTextReader(sr);
            return _ser.Deserialize<T>(jr);
        }

        string SafeJoin(string root, string rel)
        {
            var full = Path.GetFullPath(Path.Combine(root, rel));
            var rootFull = Path.GetFullPath(root);
            if (!full.StartsWith(rootFull)) throw new Exception("越权路径访问被拒绝");
            return full;
        }

        public T Get<T>(string dtoId) where T : class
        {
            if (_index.TryGetValue(dtoId, out var t))
                return t.ToObject<T>();
            return null;
        }

        public IEnumerable<WorldDef> AllWorlds()    => FilterAll<WorldDef>();
        public IEnumerable<AreaBlockDef> AllAreaBlocks() => FilterAll<AreaBlockDef>();
        public IEnumerable<RoomDef> AllRooms()      => FilterAll<RoomDef>();
        public IEnumerable<NodeDef> AllNodes()      => FilterAll<NodeDef>();

        IEnumerable<T> FilterAll<T>() where T : class
        {
            foreach (var kv in _index)
            {
                var obj = kv.Value.ToObject<T>();
                if (obj != null) yield return obj;
            }
        }
    }
}
