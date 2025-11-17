using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;

namespace Core.Game.View
{
    public class EditorDataManager : AbstractSystem
    {
        private ChangeTrackerSystem _changeTrackerSystem;
        
        private UniverseDataModel _universeDataModel;
        
        private WorldDataModel _worldDataModel;
        
        /// <summary>
        /// 当前聚焦的宇宙
        /// </summary>
        private UniverseDtoDef _currentFocusUniverse;

        /// <summary>
        /// 当前宇宙中聚焦的世界数据
        /// </summary>
        private WorldDtoDef _currentFocusUniverseWorld;
        
        /// <summary>
        /// 当前宇宙中的所有世界数据
        /// </summary>
        private Dictionary<string, WorldDtoDef> _trackingWorlds = new Dictionary<string, WorldDtoDef>();
        
        protected override void OnInit()
        {
            _changeTrackerSystem = this.GetSystem<ChangeTrackerSystem>();
            _universeDataModel = this.GetModel<UniverseDataModel>();
            _worldDataModel = this.GetModel<WorldDataModel>();
        }
        

        public void ClearEditorData()
        {
            StopTrackedSnapshots();
        }

        #region Universe

        /// <summary>
        /// 添加新宇宙配置
        /// </summary>
        public void AddNewUniverseDtoDef()
        {
            var newUniverseDtoDef = new UniverseDtoDef
            {
                DefName = "新宇宙",
                DefDescription = "这是一个新的宇宙",
                InitialPlayerLocateWorldId = "",
                InitialShowingWorldIdList = new List<string>(),
                WorldIdList = new List<string>()
            };
            
            newUniverseDtoDef.GenerateDefId();
            // if (GetAllUniverseDefs().Count == 0)
            // {
            //     SetFocusUniverse(newUniverseDtoDef);
            // }
            _universeDataModel.AddDtoDef(newUniverseDtoDef);
            LogKit.Log($"<color=green>✓ 创建新宇宙: {newUniverseDtoDef.DefName} ({newUniverseDtoDef.DefId})</color>");
        }
        
        /// <summary>
        /// 设置当前焦点宇宙
        /// </summary>
        /// <param name="focusUniverse"></param>
        public void SetFocusUniverse(UniverseDtoDef focusUniverse)
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
            var newWorld = new WorldDtoDef()
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
        /// 获取当前宇宙中聚焦的世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetFocusedWorld()
        {
            return _currentFocusUniverseWorld;
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
            // 先从UI同步数据到对象
            SyncUIDataToObjects();
            
            _changeTrackerSystem.HasChanges(_currentFocusUniverse,_currentFocusUniverse.DefId);
            
            // 检查当前宇宙中的世界的变化
            foreach (var kvp in _trackingWorlds)
            {
                _changeTrackerSystem.HasChanges(kvp.Value, kvp.Key);
            }
        }
        
        /// <summary>
        /// 从UI同步数据到对象
        /// </summary>
        private void SyncUIDataToObjects()
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

        public void StartTrackingRegion(RegionDtoDef regionDef)
        {
            
        }
        
        /// <summary>
        /// 更新当前的快照
        /// </summary>
        public void UpdateTrackedSnapshots()
        {
            if (_currentFocusUniverse != null)
            {
                _currentFocusUniverse.SaveThisDef();
                _changeTrackerSystem.UpdateSnapshot(_currentFocusUniverse,_currentFocusUniverse.DefId);
                foreach (var worldDtoDef in _trackingWorlds)
                {
                    worldDtoDef.Value.SaveThisDef();
                    _changeTrackerSystem.UpdateSnapshot(worldDtoDef.Value, worldDtoDef.Key);
                }
                
                LogKit.Log("<color=green>已更新所有数据快照</color>");
            }
        }
        
        /// <summary>
        /// 停止追踪快照
        /// </summary>
        public void StopTrackedSnapshots()
        {
            if (_currentFocusUniverse != null)
            {
                _changeTrackerSystem.StopTracking(_currentFocusUniverse.DefId);
                _currentFocusUniverse = null;
                foreach (var worldDtoDef in _trackingWorlds)
                {
                    _changeTrackerSystem.StopTracking(worldDtoDef.Key);
                }
                _trackingWorlds.Clear();
                LogKit.Log("<color=green>停止追踪并更新快照</color>");
            }
        }

        #endregion
    }
}