using System;
using System.Collections.Generic;
using System.Reflection;
using GDFramework.GAS.General;
using GDFramework.GAS.RunningTime.Ability.TimelineAbility.TrackItem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GDFramework.GAS.RunningTime.Ability.TimelineAbility
{
    public abstract class TimelineAbilityAssetBase : AbilityAsset
    {
        [TitleGroup("Data")]
        [HorizontalGroup("Data/H1", 1 / 3f)]
        [TabGroup("Data/H1/V1", "Timeline", SdfIconType.ClockHistory, TextColor = "#00FF00")]
        [Button("查看/编辑能力时间轴", ButtonSizes.Large, Icon = SdfIconType.Hammer)]
        [PropertyOrder(-1)]
        private void EditAbilityTimeline()
        {
            try
            {
                var assembly = Assembly.Load("com.exhard.exgas.editor");
                var type = assembly.GetType("GAS.Editor.AbilityTimelineEditorWindow");
                var methodInfo = type.GetMethod("ShowWindow", BindingFlags.Public | BindingFlags.Static);
                methodInfo!.Invoke(null, new object[] { this });
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"调用\"GAS.Editor.AbilityTimelineEditorWindow\"类的静态方法ShowWindow(TimelineAbilityAsset asset)失败, 代码可能被重构了: {e}");
            }
        }

        [TabGroup("Data/H1/V1", "Timeline")] [LabelText(GasTextDefine.ABILITY_MANUAL_ENDABILITY)] [LabelWidth(100)]
        public bool manualEndAbility;

        [HideInInspector] public int FrameCount; // 能力结束时间

        [HideInInspector] public List<DurationalCueTrackData> DurationalCues = new();

        [HideInInspector] public List<InstantCueTrackData> InstantCues = new();

        [HideInInspector] public List<ReleaseGameplayEffectTrackData> ReleaseGameplayEffect = new();

        [HideInInspector] public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new();

        [HideInInspector] public List<TaskMarkEventTrackData> InstantTasks = new();

        [HideInInspector] public List<TaskClipEventTrackData> OngoingTasks = new();

#if UNITY_EDITOR
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public abstract class TimelineAbilityAssetT<T> : TimelineAbilityAssetBase where T : class
    {
        public sealed override Type AbilityType()
        {
            return typeof(T);
        }
    }

    /// <summary>
    /// 这是一个最朴素的TimelineAbilityAsset实现, 如果要实现更复杂的TimelineAbilityAsset, 请用TimelineAbilityAssetBase或TimelineAbilityAssetT为基类
    /// </summary>
    public sealed class TimelineAbilityAsset : TimelineAbilityAssetT<TimelineAbility>
    {
    }
}