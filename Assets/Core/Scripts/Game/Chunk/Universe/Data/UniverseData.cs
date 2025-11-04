using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World.Data;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseData : ChunkContainerData
    {
        public UniverseDtoDef UniverseDef => DtoDef as UniverseDtoDef;
        public UniverseTemporaryData UniverseTempData => TemporaryData as UniverseTemporaryData;
        
        /// <summary>
        /// 当前宇宙中所拥有的所有世界
        /// </summary>
        private List<WorldData> _universeWorldDataList = new List<WorldData>();

        #region Init

        public override void SetTempData(string defId)
        {
           
        }

        #endregion

        #region World

        /// <summary>
        /// 获取宇宙中的所有世界ID
        /// </summary>
        /// <returns></returns>
        public List<WorldData> GetAllWorlds()
        {
            return _universeWorldDataList;
        }
        
        public void AddWorld(string worldInstanceId) => AddChild(worldInstanceId);
        public void RemoveWorld(string worldInstanceId) => RemoveChild(worldInstanceId);
        public void SetActiveWorld(string worldInstanceId) => SetActiveChild(worldInstanceId);
        #endregion
        

    }
}