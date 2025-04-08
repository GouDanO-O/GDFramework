﻿using System;
using System.Collections.Generic;
using GDFramework.GAS.RunningTime.Ability;
using GDFramework.GAS.RunningTime.Effects;
using GDFramework.GAS.RunningTime.Tags;

namespace GDFramework.GAS.RunningTime.Component
{
    public interface IAbilitySystemComponent
    {
        void SetPreset(AbilitySystemComponentPreset ascPreset);

        void Init(GameplayTag[] baseTag, Type[] attributeTypes, AbilityAsset[] baseAbilities, int level);

        void SetLevel(int level);

        bool HasTag(GameplayTag tag);

        bool HasAllTags(GameplayTagSet tags);

        bool HasAnyTags(GameplayTagSet tags);

        void AddFixedTag(GameplayTag tags);

        void AddFixedTags(GameplayTagSet tags);

        void RemoveFixedTag(GameplayTag gameplayTag);

        void RemoveFixedTags(GameplayTagSet tags);

        GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target);

        GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect);

        void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec);

        void RemoveGameplayEffect(GameplayEffectSpec spec);

        void Tick();

        Dictionary<string, float> DataSnapshotDict();

        AbilitySpec GrantAbility(AbstractAbility ability);

        void RemoveAbility(string abilityName);

        float? GetAttributeCurrentValue(string setName, string attributeShortName);

        float? GetAttributeBaseValue(string setName, string attributeShortName);

        bool TryActivateAbility(string abilityName, params object[] args);

        void TryEndAbility(string abilityName);

        CooldownTimer CheckCooldownFromTags(GameplayTagSet tags);

        T AttrSet<T>() where T : AttributeSet.AttributeSet;

        void ClearGameplayEffect();
    }
}