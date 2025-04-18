using GDFrameworkCore;

namespace GDFramework.World.Object
{
    public abstract class ObjAttack : IObjAttack
    {
        public BindableProperty<float> AttackDamage { get; set; }
        public BindableProperty<float> AttackDistance { get; set; }
        public BindableProperty<float> AttackFrequence { get; set; }
        public void DoAttack()
        {
            throw new System.NotImplementedException();
        }

        public void BeAttacked()
        {
            throw new System.NotImplementedException();
        }
    }
}