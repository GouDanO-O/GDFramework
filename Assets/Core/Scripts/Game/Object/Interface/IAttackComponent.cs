namespace Core.World.Object.Interface
{
    public interface IAttackComponent : IComponent
    {
        float AttackPower { get; }
        
        float AttackRange { get; }
        
        float AttackCooldown { get; }
        
        bool CanAttack(IWorldObject target);
        
        void Attack(IWorldObject target);
    }
}