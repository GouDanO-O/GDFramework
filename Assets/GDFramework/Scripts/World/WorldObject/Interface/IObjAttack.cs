using GDFrameworkCore;

namespace GDFramework.World.Object
{
    public interface IObjAttack
    {
        BindableProperty<float> AttackDamage  { get; set; }
        
        BindableProperty<float> AttackDistance { get; set; }
        
        BindableProperty<float> AttackFrequence { get; set; }
        
        void DoAttack();
        
        void BeAttacked();
    }
}