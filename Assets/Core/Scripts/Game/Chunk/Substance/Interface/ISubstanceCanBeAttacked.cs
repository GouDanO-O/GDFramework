namespace Core.Game.Chunk.Substance.Interface
{
    /// <summary>
    /// 物品能受到攻击
    /// 能受到攻击,就一定会有生命值
    /// </summary>
    public interface ISubstanceCanBeAttacked : ISubstanceHealthy
    {
        
    }
}