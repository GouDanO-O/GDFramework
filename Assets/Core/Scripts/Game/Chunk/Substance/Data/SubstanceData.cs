using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Substance.Data
{
    /// <summary>
    /// 世界中,一切可以进行互动的物体
    /// 人,也是一个物体,只不过他的行为会比物体会更复杂
    /// </summary>
    public class SubstanceData : ChunkData
    {
        [LabelText("物体固定数据")]
        public SubstanceDto SubstanceDto;

        [LabelText("物体临时数据")]
        public SubstanceDtoTemporary SubstanceDtoTemporary;
    }
}