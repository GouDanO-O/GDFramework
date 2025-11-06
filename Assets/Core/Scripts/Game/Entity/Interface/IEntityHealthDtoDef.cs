using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Substance.Interface
{
    public interface IEntityHealthDtoDef
    {
        [LabelText("初始生命值")]
        int InitialHealth { get; set; }
        
        [LabelText("初始最大生命值")]
        int InitialMaxHealth { get; set; }
    }
}