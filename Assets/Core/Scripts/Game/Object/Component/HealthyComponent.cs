using Core.World.Object.Interface;

namespace Core.World.Object.Component
{
    public class HealthyComponent : IHealthComponent
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }

        public float CurrentHealth { get; set; }
        
        public float MaxHealth { get; }
        
        public bool IsAlive { get; }
        
        public void TakeDamage(float damage)
        {
            
        }

        public void Heal(float amount)
        {
           
        }

        public void Death()
        {
            
        }
    }
}