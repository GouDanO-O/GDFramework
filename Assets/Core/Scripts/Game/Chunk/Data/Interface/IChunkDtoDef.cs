using System.Collections.Generic;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 固定数据定义接口(配置)
    /// </summary>
    public interface IChunkDtoDef
    {
        string DefId { get; }
        string DefName { get; set; }
        string DefDescription { get; set; }

        void SaveThisDef();

        void DeleteThisDef();
        
        bool Validate(out string error);
    }
}