using System;
using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.AbilityTask;
using GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.AbilityTask.TaskData;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class TaskClipEventTrackData:TrackDataBase
    {
        public List<TaskClipEvent> clipEvents = new List<TaskClipEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.OngoingTasks.Add(this);
        }

        public override void DefaultInit()
        {
            base.DefaultInit();
            trackName = "Task Clips";
        }
    }
    
    [Serializable]
    public class TaskClipEvent : ClipEventBase
    {
        public OngoingTaskData ongoingTask;

        public OngoingAbilityTask Load()
        {
            return ongoingTask.Load() as OngoingAbilityTask;
        }
    }
}