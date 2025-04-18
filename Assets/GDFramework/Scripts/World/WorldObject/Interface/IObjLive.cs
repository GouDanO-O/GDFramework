using GDFrameworkCore;

namespace GDFramework.World.Object
{
    public interface IObjLive
    {
        BindableProperty<float> MaxHealth { get; set; }
        
        BindableProperty<float> CurrentHealth { get; set; }
        
        BindableProperty<float> MaxArmor { get; set; }
        
        BindableProperty<float> CurrentArmor { get; set; }

        void BeHarmed(float damage);

        void BeCuring(float curingValue);

        void Death();
    }
}