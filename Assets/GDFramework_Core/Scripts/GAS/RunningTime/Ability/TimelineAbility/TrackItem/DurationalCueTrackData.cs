using System;
using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Cue.Base;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class DurationalCueTrackData:TrackDataBase
    {
        public List<DurationalCueClipEvent> clipEvents = new List<DurationalCueClipEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.DurationalCues.Add(this);
        }
    }
    
    [Serializable]
    public class DurationalCueClipEvent : ClipEventBase
    {
        public GameplayCueDurational cue;
    }
}