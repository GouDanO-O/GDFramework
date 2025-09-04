using System;
using System.Collections.Generic;

namespace Game.World
{
    [Serializable]
    public class WorldDef
    {
        public int schemaVersion = 1;
        public string id;           // dtoId
        public string stableUid;    // 资源GUID
        public string configId;

        // 初始时间
        public int initialWorldYearTime;
        public int initialWorldMonthTime;
        public int initialWorldDayTime;
        public int initialWorldHourTime;
        public int initialWorldMinutesTime;

        // 初始落点
        public string initialPlayerLocateAreaBlockId; // dtoId

        // 组成
        public List<string> areaBlockIds = new();
    }

    [Serializable]
    public class AreaBlockDef
    {
        public int schemaVersion = 1;
        public string id;         // dtoId
        public string stableUid;
        public string configId;

        public string initialRoomId;   // dtoId
        public bool willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock;

        public List<string> roomIds = new();
    }

    [Serializable]
    public class RoomDef
    {
        public int schemaVersion = 1;
        public string id;         // dtoId
        public string stableUid;
        public string configId;

        public List<string> nodeIds = new();
    }

    [Serializable]
    public class NodeDef
    {
        public int schemaVersion = 1;
        public string id;         // dtoId
        public string stableUid;
        public string configId;
    }

    // 基础/Mod 的 manifest
    [Serializable]
    public class ContentManifest
    {
        public string modId = "Base";    // BaseContent 用“Base”
        public string gameVersion = "1.0.0";
        public int schemaVersion = 1;
        public Includes include = new();

        [Serializable]
        public class Includes
        {
            public List<string> worlds = new();
            public List<string> areaBlocks = new();
            public List<string> rooms = new();
            public List<string> nodes = new();
        }
    }
}