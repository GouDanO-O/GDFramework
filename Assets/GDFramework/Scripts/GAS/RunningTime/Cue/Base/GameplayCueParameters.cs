using GDFramework.GAS.RunningTime.Ability;
using GDFramework.GAS.RunningTime.Effects;

namespace GDFramework.GAS.RunningTime.Cue.Base
{
    public struct GameplayCueParameters
    {
        public GameplayEffectSpec sourceGameplayEffectSpec;

        public AbilitySpec sourceAbilitySpec;

        public object[] customArguments;
        // AggregatedSourceTags
        // AggregatedTargetTags
        // EffectContext
        // Magnitude
    }
}