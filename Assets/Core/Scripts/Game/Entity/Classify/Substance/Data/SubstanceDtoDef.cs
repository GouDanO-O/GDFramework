using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Substance.Interface;

namespace Core.Game.Chunk.Substance.Classify.Substance.Data
{
    public class SubstanceDtoDef : EntityDtoDef,IEntityHealthDtoDef
    {
        public int InitialHealth { get; set; }
        
        public int InitialMaxHealth { get; set; }
    }
}