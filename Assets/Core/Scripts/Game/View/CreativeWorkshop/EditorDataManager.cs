using System.Collections.Generic;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
    public class EditorDataManager : AbstractSystem
    {
        private ChangeTrackerSystem _changeTrackerSystem;
        
        private UniverseDataModel _universeDataModel;
        
        private WorldDataModel _worldDataModel;
        
        private RegionDataModel _regionDataModel;
        
        private DungeonDataModel _dungeonDataModel;

        private RoomDataModel _roomDataModel;
        
        /// <summary>
        /// 当前聚焦的宇宙
        /// </summary>
        private UniverseDtoDef _currentFocusUniverse;

        /// <summary>
        /// 当前宇宙中聚焦的世界数据
        /// </summary>
        private WorldDtoDef _currentFocusWorld;
        
        /// <summary>
        /// 当前世界中聚焦的区域数据
        /// </summary>
        private RegionDtoDef _currentFocusRegion;
        
        /// <summary>
        /// 当前区域中聚焦的副本数据
        /// </summary>
        private DungeonDtoDef _currentFocusDungeon;
        
        /// <summary>
        /// 当前副本中聚焦的房间数据
        /// </summary>
        private RoomDtoDef _currentFocusRoom;
        
        /// <summary>
        /// 当前宇宙中的所有世界数据
        /// </summary>
        private Dictionary<string, WorldDtoDef> _trackingWorlds = new Dictionary<string, WorldDtoDef>();
        
        /// <summary>
        /// 当前世界中所有的区域
        /// </summary>
        private Dictionary<string,RegionDtoDef> _trackingRegions = new Dictionary<string, RegionDtoDef>();
        
        /// <summary>
        /// 当前区域中所有的副本
        /// </summary>
        private Dictionary<string,DungeonDtoDef> _trackingDungeons = new Dictionary<string, DungeonDtoDef>();
        
        /// <summary>
        /// 当前副本中所有的房间
        /// </summary>
        private Dictionary<string,RoomDtoDef> _trackingRooms = new Dictionary<string, RoomDtoDef>();

        private UI_Editor_UniversePanel _universePanel;
        
        private UI_Editor_WorldPanel _worldPanel;
        
        
        protected override void OnInit()
        {
            _changeTrackerSystem = this.GetSystem<ChangeTrackerSystem>();
            _universeDataModel = this.GetModel<UniverseDataModel>();
            _worldDataModel = this.GetModel<WorldDataModel>();
            _regionDataModel = this.GetModel<RegionDataModel>();
            _dungeonDataModel = this.GetModel<DungeonDataModel>();
            _roomDataModel = this.GetModel<RoomDataModel>();
        }
        

        public void ClearEditorData()
        {
            StopAllTrackedSnapshots();
        }

        #region Universe

        /// <summary>
        /// 添加新宇宙配置
        /// </summary>
        public UniverseDtoDef AddNewUniverseDtoDef()
        {
            UniverseDtoDef newUniverseDtoDef = new UniverseDtoDef
            {
                DefName = "新宇宙",
                DefDescription = "这是一个新的宇宙",
                InitialPlayerLocateWorldId = "",
                InitialShowingWorldIdList = new List<string>(),
                WorldIdList = new List<string>()
            };
            
            newUniverseDtoDef.GenerateDefId();
            _universeDataModel.AddDtoDef(newUniverseDtoDef);
            LogKit.Log($"<color=green>✓ 创建新宇宙: {newUniverseDtoDef.DefName} ({newUniverseDtoDef.DefId})</color>");
            return newUniverseDtoDef;

        }
        
        /// <summary>
        /// 设置当前焦点宇宙
        /// </summary>
        /// <param name="focusUniverse"></param>
        public void UpdateFocusUniverse(UniverseDtoDef focusUniverse)
        {
            if (focusUniverse == null)
                return;
            _currentFocusUniverse = focusUniverse;
            StartTrackingUniverse(focusUniverse);
        }

        public List<UniverseDtoDef> GetAllUniverseDefs()
        {
            return _universeDataModel.GetAllUniverseDefs();
        }
        
        public UniverseDtoDef GetFocusedUniverse()
        {
            return _currentFocusUniverse;
        }


        #endregion

        #region World

        /// <summary>
        /// 添加新世界到当前焦点宇宙
        /// </summary>
        public WorldDtoDef AddNewWorldToFocusUniverse()
        {
            WorldDtoDef newWorld = new WorldDtoDef()
            {
                DefName = "新世界",
                DefDescription = "这是一个新的世界",
                InitialPlayerLocateRegionId = "",
                InitialShowingRegionIdList = new List<string>(),
                RegionIdList = new List<string>()
            };

            newWorld.GenerateDefId();
            //如果当前宇宙中,没有其他世界,则设置第一个创建的世界为初始世界
            if (_currentFocusUniverse.WorldIdList.Count == 0)
            {
                _currentFocusUniverse.InitialShowingWorldIdList.Add(newWorld.DefId);
                _currentFocusUniverse.InitialPlayerLocateWorldId = newWorld.DefId;
            }
            
            _currentFocusUniverse.WorldIdList.Add(newWorld.DefId);
            _worldDataModel.AddDtoDef(newWorld);
            StartTrackingWorld(newWorld);
            
            return newWorld;
        }

        /// <summary>
        /// 更新当前焦点世界
        /// </summary>
        /// <param name="world"></param>
        public void UpdateFocusedWorld(WorldDtoDef world)
        {
            _currentFocusWorld = world;
        }

        /// <summary>
        /// 获取当前宇宙中聚焦的世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetFocusedWorld()
        {
            return _currentFocusWorld;
        }

        #endregion

        #region Region

        /// <summary>
        /// 从当前焦点世界中添加区域
        /// </summary>
        public RegionDtoDef AddNewRegionToFocusWorld()
        {
            RegionDtoDef newRegion = new RegionDtoDef()
            {
                DefName = "新区域",
                DefDescription = "这是一个新的区域",
                InitialPlayerLocateDungeonId = "",
                InitialShowingDungeonIdList = new List<string>(),
                DungeonIdList = new List<string>()
            };

            newRegion.GenerateDefId();
            //如果当前世界中,没有其他区域,则设置第一个创建的区域为初始区域
            if (_currentFocusWorld.RegionIdList.Count == 0)
            {
                _currentFocusWorld.InitialShowingRegionIdList.Add(newRegion.DefId);
                _currentFocusWorld.InitialPlayerLocateRegionId = newRegion.DefId;
            }
            
            _currentFocusWorld.RegionIdList.Add(newRegion.DefId);
            _regionDataModel.AddDtoDef(newRegion);
            StartTrackingRegion(newRegion);
            
            return newRegion;
        }

        /// <summary>
        /// 更新焦点区域
        /// </summary>
        /// <param name="region"></param>
        public void UpdateFocusRegion(RegionDtoDef region)
        {
            _currentFocusRegion = region;
            StartTrackingRegion(region);
        }
        
        public RegionDtoDef GetFocusedRegion()
        {
            return _currentFocusRegion;
        }

        #endregion

        #region Dungeon

        public DungeonDtoDef AddNewDungeonToFocusRegion()
        {
            DungeonDtoDef newDungeon = new DungeonDtoDef()
            {
                DefName = "新副本",
                DefDescription = "这是一个新的副本",
                InitialPlayerLocateRoomId = "",
                InitialShowingRoomIdList = new List<string>(),
                RoomIdList = new List<string>()
            };
            
            newDungeon.GenerateDefId();
            //如果当前世界中,没有其他区域,则设置第一个创建的区域为初始区域
            if (_currentFocusRegion.DungeonIdList.Count == 0)
            {
                _currentFocusRegion.InitialShowingDungeonIdList.Add(newDungeon.DefId);
                _currentFocusRegion.InitialPlayerLocateDungeonId = newDungeon.DefId;
            }
            
            _currentFocusRegion.DungeonIdList.Add(newDungeon.DefId);
            _dungeonDataModel.AddDtoDef(newDungeon);
            StartTrackingDungeon(newDungeon);
            
            return newDungeon;
        }

        /// <summary>
        /// 更新焦点副本
        /// </summary>
        /// <param name="dungeon"></param>
        public void UpdateDungeon(DungeonDtoDef dungeon)
        {
            _currentFocusDungeon = dungeon;
            StartTrackingDungeon(dungeon);
        }

        public DungeonDtoDef GetFocusedDungeon()
        {
            return _currentFocusDungeon;
        }

        #endregion

        #region Room
        
        /// <summary>
        /// 添加房间到当前聚焦副本中
        /// </summary>
        /// <returns></returns>
        public RoomDtoDef AddNewRoomToFocusDungeon()
        {
            RoomDtoDef newRoom = new RoomDtoDef()
            {
                DefName = "新房间",
                DefDescription = "这是一个新房间",
            };
            
            newRoom.GenerateDefId();
            //如果当前副本中,没有其他房间,则设置第一个创建的区域为初始房间
            if (_currentFocusDungeon.RoomIdList.Count == 0)
            {
                _currentFocusDungeon.InitialShowingRoomIdList.Add(newRoom.DefId);
                _currentFocusDungeon.InitialPlayerLocateRoomId = newRoom.DefId;
            }
            
            _currentFocusDungeon.RoomIdList.Add(newRoom.DefId);
            _roomDataModel.AddDtoDef(newRoom);
            StartTrackingRoom(newRoom);
            
            return newRoom;
        }

        /// <summary>
        /// 更新焦点房间
        /// </summary>
        /// <param name="room"></param>
        public void UpdateFocusRoom(RoomDtoDef room)
        {
            _currentFocusRoom = room;
            StartTrackingRoom(room);
        }

        public RoomDtoDef GetFocusedRoom()
        {
            return _currentFocusRoom;
        }

        #endregion
        
        #region 追踪管理

        /// <summary>
        /// 获取当前追踪日志
        /// </summary>
        /// <returns></returns>
        public string GetChangeSummary()
        {
            return _changeTrackerSystem.GetChangeSummary();
        }
        
        /// <summary>
        /// 检查当前是否有未保存的修改
        /// 必须先保存当前修改,才能进入下一步
        /// </summary>
        public bool HasAnyChangeDidNotSave()
        {
            // 更新所有追踪对象的状态
            CheckAllTrackedChanges();
            
            bool hasChanges = _changeTrackerSystem.HasAnyChanges();
            
            if (hasChanges)
            {
                _changeTrackerSystem.LogChanges();
            }
            
            return hasChanges;
        }
        
        /// <summary>
        /// 检查所有追踪对象的变化
        /// </summary>
        private void CheckAllTrackedChanges()
        {
            if(_currentFocusUniverse == null)
                return;

            SyncUIDataToObjects();
            
            _changeTrackerSystem.HasChanges(_currentFocusUniverse,_currentFocusUniverse.DefId);
            
            foreach (var kvp in _trackingWorlds)
            {
                _changeTrackerSystem.HasChanges(kvp.Value, kvp.Key);
            }
            
            foreach (var kvp in _trackingRegions)
            {
                _changeTrackerSystem.HasChanges(kvp.Value, kvp.Key);
            }
            
            foreach (var kvp in _trackingDungeons)
            {
                _changeTrackerSystem.HasChanges(kvp.Value, kvp.Key);
            }
            
            foreach (var kvp in _trackingRooms)
            {
                _changeTrackerSystem.HasChanges(kvp.Value, kvp.Key);
            }
        }
        
        /// <summary>
        /// 从UI同步数据到对象
        /// </summary>
        private void SyncUIDataToObjects()
        {
            SyncUniverseUIDataToObjects();
            SyncWorldUIDataToObjects();
            SyncRegionDataToObjects();
            SyncDungeonUIDataToObjects();
            SyncRoomUIDataToObjects();
        }

        private void SyncUniverseUIDataToObjects()
        {
            if (_universePanel != null)
            {
                _currentFocusUniverse.DefName = _universePanel.GetCurUniverseName();
                _currentFocusUniverse.DefDescription = _universePanel.GetCurUniverseDes();
            }
        }

        private void SyncWorldUIDataToObjects()
        {
            if (_worldPanel != null)
            {

            }
        }

        private void SyncRegionDataToObjects()
        {
            
        }

        private void SyncDungeonUIDataToObjects()
        {
            
        }
        
        private void SyncRoomUIDataToObjects()
        {
            
        }
        
        /// <summary>
        /// 开始追踪宇宙
        /// </summary>
        private void StartTrackingUniverse(UniverseDtoDef universeDef)
        {
            _changeTrackerSystem.StartTracking(universeDef, universeDef.DefId);
            LogKit.Log($"开始追踪宇宙: {universeDef.DefName} ({universeDef.DefId})");
        }

        /// <summary>
        /// 开始追踪世界
        /// </summary>
        public void StartTrackingWorld(WorldDtoDef worldDef)
        {
            if (worldDef == null)
                return;
                
            _trackingWorlds[worldDef.DefId] = worldDef;
            _changeTrackerSystem.StartTracking(worldDef, worldDef.DefId);
            
            LogKit.Log($"开始追踪世界: {worldDef.DefName} ({worldDef.DefId})");
        }

        /// <summary>
        /// 开始追踪区块
        /// </summary>
        /// <param name="regionDef"></param>
        public void StartTrackingRegion(RegionDtoDef regionDef)
        {
            if (regionDef == null)
                return;
                
            _trackingRegions[regionDef.DefId] = regionDef;
            _changeTrackerSystem.StartTracking(regionDef, regionDef.DefId);
            
            LogKit.Log($"开始追踪区域: {regionDef.DefName} ({regionDef.DefId})");
        }

        /// <summary>
        /// 开始追踪副本
        /// </summary>
        /// <param name="dungeonDef"></param>
        public void StartTrackingDungeon(DungeonDtoDef dungeonDef)
        {
            if (dungeonDef == null)
                return;
                
            _trackingDungeons[dungeonDef.DefId] = dungeonDef;
            _changeTrackerSystem.StartTracking(dungeonDef, dungeonDef.DefId);
            
            LogKit.Log($"开始追踪副本: {dungeonDef.DefName} ({dungeonDef.DefId})");
        }

        /// <summary>
        /// 开始追踪房间
        /// </summary>
        /// <param name="roomDef"></param>
        public void StartTrackingRoom(RoomDtoDef roomDef)
        {
            if (roomDef == null)
                return;
                
            _trackingRooms[roomDef.DefId] = roomDef;
            _changeTrackerSystem.StartTracking(roomDef, roomDef.DefId);
            
            LogKit.Log($"开始追踪房间: {roomDef.DefName} ({roomDef.DefId})");
        }
        
        /// <summary>
        /// 更新当前的快照
        /// </summary>
        public void UpdateAllTrackedSnapshots()
        {
            UpdateUniverseTrackedSnapshots();
            UpdateWorldTrackedSnapshots();
            UpdateRegionTrackedSnapshots();
            UpdateDungeonTrackedSnapshots();
            UpdateRoomTrackedSnapshots();
            LogKit.Log("<color=green>已更新所有数据快照</color>");
        }

        public void UpdateUniverseTrackedSnapshots()
        {
            if (_currentFocusUniverse != null)
            {
                _currentFocusUniverse.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(_currentFocusUniverse,_currentFocusUniverse.DefId);
                
                LogKit.Log("<color=green>已更新当前宇宙数据快照</color>");
            }
        }

        public void UpdateWorldTrackedSnapshots()
        {
            foreach (var worldDtoDef in _trackingWorlds)
            {
                worldDtoDef.Value.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(worldDtoDef.Value, worldDtoDef.Key);
            }
            
            LogKit.Log("<color=green>已更新当前世界数据快照</color>");
        }

        public void UpdateRegionTrackedSnapshots()
        {
            foreach (var regionDtoDef in _trackingRegions)
            {
                regionDtoDef.Value.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(regionDtoDef.Value, regionDtoDef.Key);
            }
            
            LogKit.Log("<color=green>已更新当前区域数据快照</color>");
        }

        public void UpdateDungeonTrackedSnapshots()
        {
            foreach (var dungeonDtoDef in _trackingDungeons)
            {
                dungeonDtoDef.Value.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(dungeonDtoDef.Value, dungeonDtoDef.Key);
            }
            
            LogKit.Log("<color=green>已更新当前副本数据快照</color>");
        }

        public void UpdateRoomTrackedSnapshots()
        {
            foreach (var roomDtoDef in _trackingRooms)
            {
                roomDtoDef.Value.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(roomDtoDef.Value, roomDtoDef.Key);
            }
            
            LogKit.Log("<color=green>已更新当前房间数据快照</color>");
        }
        
        /// <summary>
        /// 停止所有的追踪快照
        /// </summary>
        public void StopAllTrackedSnapshots()
        {
            StopUniverseTrackedSnapshots();
            StopWorldTrackedSnapshots();
            StopRegionTrackedSnapshots();
            StopDungeonTrackedSnapshots();
            StopRoomTrackedSnapshots();
            LogKit.Log("<color=green>停止追踪全部快照</color>");
        }

        public void StopUniverseTrackedSnapshots()
        {
            if (_currentFocusUniverse != null)
            {
                _changeTrackerSystem.StopTracking(_currentFocusUniverse.DefId);
                _currentFocusUniverse = null;
                LogKit.Log("<color=green>停止追踪宇宙快照</color>");
            }
        }

        public void StopWorldTrackedSnapshots()
        {
            foreach (var worldDtoDef in _trackingWorlds)
            {
                _changeTrackerSystem.StopTracking(worldDtoDef.Key);
            }
            
            _trackingWorlds.Clear();
            LogKit.Log("<color=green>停止追踪世界快照</color>");
        }

        public void StopRegionTrackedSnapshots()
        {
            foreach (var regionDtoDef in _trackingRegions)
            {
                _changeTrackerSystem.StopTracking(regionDtoDef.Key);
            }
            
            _trackingRegions.Clear();
            LogKit.Log("<color=green>停止追踪区域快照</color>");
        }

        public void StopDungeonTrackedSnapshots()
        {
            foreach (var dungeonDtoDef in _trackingDungeons)
            {
                _changeTrackerSystem.StopTracking(dungeonDtoDef.Key);
            }
            
            _trackingDungeons.Clear();
            LogKit.Log("<color=green>停止追踪副本快照</color>");
        }

        public void StopRoomTrackedSnapshots()
        {
            foreach (var roomDtoDef in _trackingRooms)
            {
                _changeTrackerSystem.StopTracking(roomDtoDef.Key);
            }
            
            _trackingRooms.Clear();
            LogKit.Log("<color=green>停止追踪房间快照</color>");
        }

        #endregion
    }
}