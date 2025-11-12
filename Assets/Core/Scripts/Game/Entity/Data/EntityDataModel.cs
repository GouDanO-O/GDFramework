using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;

namespace Core.Game.Chunk.Substance.Data
{
    public class EntityDataModel : AbstractModel
    {
        /// <summary>
        /// 当前游戏固定数据配置列表
        /// </summary>
        private List<EntityDtoDef> _dtoDefList = new List<EntityDtoDef>();
        
        /// <summary>
        /// DefId -> DtoDef 的快速查找字典
        /// </summary>
        private Dictionary<string, EntityDtoDef> _defIdToDefDict = new Dictionary<string, EntityDtoDef>();
        
        
        protected override void OnInit()
        {
            
        }
        
        /// <summary>
        /// 添加数据中的固定配置
        /// </summary>
        /// <param name="dtoDef"></param>
        public void AddDtoDef(EntityDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                LogKit.Error("无法添加空的 EntityDtoDef");
                return;
            }

            if (_defIdToDefDict.ContainsKey(dtoDef.DefId))
            {
                LogKit.Error($"EntityDtoDef 已存在,跳过: {dtoDef.DefId}");
                return;
            }

            _dtoDefList.Add(dtoDef);
            _defIdToDefDict[dtoDef.DefId] = dtoDef;
            
            LogKit.Error($"添加配置: {dtoDef.DefName} (DefId: {dtoDef.DefId}");
        }
    }
}