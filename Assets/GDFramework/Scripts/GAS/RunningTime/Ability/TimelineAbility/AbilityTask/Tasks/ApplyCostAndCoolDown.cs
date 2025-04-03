using System;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.AbilityTask.Tasks
{
    [Serializable]
    public class ApplyCostAndCoolDown : InstantAbilityTask
    {
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}