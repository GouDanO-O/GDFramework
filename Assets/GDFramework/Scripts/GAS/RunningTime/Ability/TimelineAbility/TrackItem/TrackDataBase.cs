using System;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class TrackDataBase
    {
        public string trackName;

        public virtual void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
        }

        public virtual void DefaultInit()
        {
        }
    }
}