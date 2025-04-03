namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.AbilityTask
{
    public abstract class AbilityTaskBase
    {
        protected AbilitySpec _spec;
        public AbilitySpec Spec => _spec;

        public virtual void Init(AbilitySpec spec)
        {
            _spec = spec;
        }
    }
}