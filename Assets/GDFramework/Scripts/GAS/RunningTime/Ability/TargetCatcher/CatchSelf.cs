using System.Collections.Generic;
using GDFramework.GAS.RunningTime.Component;

namespace GDFramework.GAS.RunningTime.Ability.TargetCatcher
{
    public sealed class CatchSelf : TargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget,
            List<AbilitySystemComponent> results)
        {
            results.Add(Owner);
        }
    }
}