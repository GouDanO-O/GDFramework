using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Procedure.Resource;
using Core.Game.Storage;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;

namespace Core.Game.Chunk.Region.Data
{
    public class RegionDataModel : ChunkDataModel,ICanGetSystem
    {
        /// <summary>
        /// 当前游戏固定数据配置列表
        /// </summary>
        private List<RegionDtoDef> _dtoDefList = new List<RegionDtoDef>();
        
        /// <summary>
        /// DefId -> DtoDef 的快速查找字典
        /// </summary>
        private Dictionary<string, RegionDtoDef> _defIdToDefDict = new Dictionary<string, RegionDtoDef>();
        
        /// <summary>
        /// 所有运行时数据字典 (InstanceId -> Data)
        /// </summary>
        private Dictionary<string, RegionData> _dataDict = new Dictionary<string, RegionData>();
        
        public override void InitializeDataModel()
        {
            
        }
        
        /// <summary>
        /// 添加数据中的固定配置
        /// </summary>
        /// <param name="dtoDef"></param>
        public void AddDtoDef(RegionDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                LogKit.Error("无法添加空的 RegionDtoDef");
                return;
            }

            if (_defIdToDefDict.ContainsKey(dtoDef.DefId))
            {
                LogKit.Error($"RegionDtoDef 已存在,跳过: {dtoDef.DefId}");
                return;
            }

            _dtoDefList.Add(dtoDef);
            _defIdToDefDict[dtoDef.DefId] = dtoDef;
            
            LogKit.Error($"添加配置: {dtoDef.DefName} (DefId: {dtoDef.DefId}");
            TryLoadExistingInstancesForDef(dtoDef);
        }

        public RegionDtoDef GetDefById(string defId)
        {
            return _defIdToDefDict.TryGetValue(defId, out var def) ? def : null; 
        }
        
        /// <summary>
        /// 尝试为配置加载已存在的临时数据实例
        /// </summary>
        private void TryLoadExistingInstancesForDef(RegionDtoDef def)
        {
            var storageSystem = this.GetSystem<StorageSystem>();
            if (storageSystem == null)
            {
                LogKit.Error("StorageSystem 未初始化,跳过临时数据加载");
                return;
            }
        }
        
    }
}