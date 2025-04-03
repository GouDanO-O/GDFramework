using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Component;

namespace GDFramework_Core.GAS.RunningTime.Ability.TargetCatcher
{
    public sealed class CatchTarget : TargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget,
            List<AbilitySystemComponent> results)
        {
            results.Add(mainTarget);
        }
    }
}