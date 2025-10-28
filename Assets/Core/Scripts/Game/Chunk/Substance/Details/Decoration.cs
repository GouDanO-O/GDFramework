using Core.Game.Chunk.Substance.Interface;

namespace Core.Game.Chunk.Substance.Details
{
    /// <summary>
    /// 装饰物
    /// 一般无法被攻击所破坏
    /// 但是可以被工具移除
    /// </summary>
    public class Decoration : Substance,ISubstanceRemovable
    {
        
    }
}