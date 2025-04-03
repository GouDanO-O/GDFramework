using System;
using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.AbilityTask.TaskData;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class TaskMarkEventTrackData : TrackDataBase
    {
        public List<TaskMarkEvent> markEvents = new List<TaskMarkEvent>();
        
        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.InstantTasks.Add(this);
        }
    }

    [Serializable]
    public class TaskMarkEvent:MarkEventBase
    {
        public List<InstantTaskData> InstantTasks = new List<InstantTaskData>();
    }
}