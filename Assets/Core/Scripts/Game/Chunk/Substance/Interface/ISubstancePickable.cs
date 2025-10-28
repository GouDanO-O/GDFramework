namespace Core.Game.Chunk.Substance.Interface
{
    /// <summary>
    /// 物体能够被拾取进背包
    /// 如果能够被拾取进背包,就代表物体既能放置又能移除
    /// </summary>
    public interface ISubstancePickable : ISubstancePlaceable, ISubstanceRemovable
    {
        
    }
}