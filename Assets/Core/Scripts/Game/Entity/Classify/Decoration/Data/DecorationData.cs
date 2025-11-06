using Core.Game.Chunk.Substance.Data;

namespace Core.Game.Chunk.Substance.Classify.Decoration.Data
{
    /// <summary>
    /// 装饰物--数据
    /// </summary>
    public class DecorationData : EntityData
    {
        public DecorationDtoDef DecorationDef => DtoDef as DecorationDtoDef;
        
        public DecorationTemporaryData DecorationTempData => TemporaryData as DecorationTemporaryData;
    }
}