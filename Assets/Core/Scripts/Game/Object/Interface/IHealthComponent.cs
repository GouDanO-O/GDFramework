namespace Core.World.Object.Interface
{
    public interface IHealthComponent : IComponent
    {
        float CurrentHealth { get; set; }
        
        float MaxHealth { get; }
        
        bool IsAlive { get; }
        
        void TakeDamage(float damage);
        
        void Heal(float amount);

        void Death();
    }
}