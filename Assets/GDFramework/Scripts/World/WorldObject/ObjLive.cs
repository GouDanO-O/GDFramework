using GDFrameworkCore;

namespace GDFramework.World.Object
{
    public abstract class ObjLive : IObjLive
    {
        public BindableProperty<float> MaxHealth { get; set; }
        
        public BindableProperty<float> CurrentHealth { get; set; }
        
        public BindableProperty<float> MaxArmor { get; set; }
        
        public BindableProperty<float> CurrentArmor { get; set; }
        
        public void BeHarmed(float damage)
        {
            
        }

        public void BeCuring(float curingValue)
        {
            
        }

        public void Death()
        {
            
        }
    }
}