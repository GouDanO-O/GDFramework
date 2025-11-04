using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine.TextCore.Text;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataModel : ChunkDataModel
    {
        /// <summary>
        /// 当前游戏固定宇宙数据配置列表
        /// 包括本体和创意工坊
        /// </summary>
        private List<UniverseDtoDef> _universeDtoDefList = new List<UniverseDtoDef>();
        
        /// <summary>
        /// 所有宇宙数据字典
        /// </summary>
        private Dictionary<string,UniverseData> _universeDataDict = new Dictionary<string,UniverseData>();
        
        private UniverseData _currentUniverseData;

        public override void InitializeDataModel()
        {

        }
        
        /// <summary>
        /// 获取当前宇宙配置
        /// </summary>
        /// <returns></returns>
        public UniverseData GetCurrentUniverseData()
        {
            if (_currentUniverseData == null)
                CreateUniverse();
            return _currentUniverseData;
        }

        /// <summary>
        /// 获取所有的宇宙配置
        /// 玩家可以根据不同的宇宙来选择
        /// </summary>
        /// <returns></returns>
        public List<UniverseData> GetAllUniverses()
        {
            return null;
        }

        /// <summary>
        /// 创建宇宙
        /// 仅第一次进入游戏
        /// </summary>
        /// <returns></returns>
        public void CreateUniverse()
        {
            
        }

        /// <summary>
        /// 添加数据中的固定配置
        /// </summary>
        /// <param name="dtoDef"></param>
        public void AddDtoDef(UniverseDtoDef dtoDef)
        {
            if (_universeDtoDefList.Contains(dtoDef))
            {
                return;
            }
            AddData(dtoDef);
            _universeDtoDefList.Add(dtoDef);
        }

        /// <summary>
        /// 将固定数据转换成
        /// </summary>
        /// <param name="dtoDef"></param>
        private void AddData(UniverseDtoDef dtoDef)
        {
            
        }
    }
}