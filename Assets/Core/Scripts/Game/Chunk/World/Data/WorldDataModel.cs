using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Procedure.Resource;

namespace Core.Game.Chunk.World.Data
{
    public class WorldDataModel : ChunkDataModel
    {
        public override void InitializeDataModel()
        {
            
        }
        
        /// <summary>
        /// 添加数据中的固定配置
        /// </summary>
        /// <param name="dtoDef"></param>
        public void AddDtoDef(LaunchResourcesLoader.HierarchyContext context,WorldDtoDef dtoDef)
        {

        }
    }
}