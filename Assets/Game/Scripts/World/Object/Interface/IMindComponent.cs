namespace Game.World.Object.Interface
{
    public enum EMindLevel
    {
        Simple = 1,    // 简单智能
        Medium = 2,    // 中等智能
        Higher = 3,    // 高等智能
    }
    
    public interface IMindComponent : IComponent
    {
        EMindLevel Level { get; }
        
        void MakeDecision();
    }
}