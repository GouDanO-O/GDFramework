namespace GDFramework_Core.GAS.RunningTime.Ability
{
    public enum EAbilityActivateResult
    {
        Success,
        FailHasActivated,
        FailTagRequirement,
        FailCost,
        FailCooldown,
        FailOtherReason
    }
}