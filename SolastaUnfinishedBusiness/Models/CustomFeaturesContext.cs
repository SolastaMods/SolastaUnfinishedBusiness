//TODO: refactor this into LevelUpContext and remaining into CustomBehaviors

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

internal static class CustomFeaturesContext
{
    private static readonly Dictionary<ulong, Dictionary<string, EffectDescription>> SpellEffectCache = new();

    internal static void RecursiveGrantCustomFeatures(
        RulesetCharacterHero hero,
        string tag,
        [NotNull] List<FeatureDefinition> features)
    {
        foreach (var grantedFeature in features)
        {
            foreach (var customCode in grantedFeature.GetAllSubFeaturesOfType<IFeatureDefinitionCustomCode>())
            {
                customCode.ApplyFeature(hero, tag);
            }

            switch (grantedFeature)
            {
                case FeatureDefinitionFeatureSet
                {
                    Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
                } featureDefinitionFeatureSet:
                    RecursiveGrantCustomFeatures(hero, tag, featureDefinitionFeatureSet.FeatureSet);
                    break;

                case FeatureDefinitionProficiency
                {
                    ProficiencyType: RuleDefinitions.ProficiencyType.FightingStyle
                } featureDefinitionProficiency:
                    featureDefinitionProficiency.Proficiencies
                        .ForEach(prof =>
                            hero.TrainedFightingStyles
                                .Add(DatabaseRepository.GetDatabase<FightingStyleDefinition>()
                                    .GetElement(prof)));
                    break;
            }
        }
    }

    // internal static void RecursiveRemoveCustomFeatures([NotNull] RulesetCharacterHero hero, string tag,
    //     List<FeatureDefinition> features, bool handleCustomCode = true)
    // {
    //     var selectedClass = LevelUpContext.GetSelectedClass(hero);
    //
    //     // this happens during character creation
    //     if (selectedClass == null)
    //     {
    //         return;
    //     }
    //
    //     foreach (var grantedFeature in features)
    //     {
    //         if (handleCustomCode && grantedFeature is IFeatureDefinitionCustomCode customFeature)
    //         {
    //             customFeature.RemoveFeature(hero, tag);
    //         }
    //
    //         if (grantedFeature is FeatureDefinitionFeatureSet
    //             {
    //                 Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
    //             } set)
    //         {
    //             RecursiveRemoveCustomFeatures(hero, tag, set.FeatureSet, handleCustomCode);
    //         }
    //
    //         if (grantedFeature is not FeatureDefinitionProficiency featureDefinitionProficiency)
    //         {
    //             continue;
    //         }
    //
    //         if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
    //         {
    //             continue;
    //         }
    //
    //         featureDefinitionProficiency.Proficiencies
    //             .ForEach(prof =>
    //                 hero.TrainedFightingStyles
    //                     .Remove(DatabaseRepository.GetDatabase<FightingStyleDefinition>()
    //                         .GetElement(prof)));
    //     }
    //
    //     hero.UpdateFeatureModifiers(tag);
    // }

    // private static void RemoveFeatureDefinitionPointPool(
    //     RulesetCharacterHero hero,
    //     [CanBeNull] RulesetSpellRepertoire heroRepertoire,
    //     string tag,
    //     [NotNull] FeatureDefinitionPointPool featureDefinitionPointPool)
    // {
    //     var poolAmount = featureDefinitionPointPool.PoolAmount;
    //
    //     switch (featureDefinitionPointPool.PoolType)
    //     {
    //         case HeroDefinitions.PointsPoolType.AbilityScore:
    //             // this is handled when attributes are refreshed
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Cantrip:
    //             heroRepertoire?.KnownCantrips.RemoveRange(heroRepertoire.KnownCantrips.Count - poolAmount,
    //                 poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Spell:
    //             heroRepertoire?.KnownSpells.RemoveRange(heroRepertoire.KnownSpells.Count - poolAmount, poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Expertise:
    //             hero.TrainedExpertises.RemoveRange(hero.TrainedExpertises.Count - poolAmount, poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Feat:
    //             for (var i = 0; i < poolAmount; i++)
    //             {
    //                 var feature = hero.TrainedFeats.Last();
    //
    //                 RecursiveRemoveCustomFeatures(hero, tag, feature.Features);
    //                 hero.TrainedFeats.RemoveAt(hero.TrainedFeats.Count - 1);
    //             }
    //
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Language:
    //             hero.TrainedLanguages.RemoveRange(hero.TrainedLanguages.Count - poolAmount, poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Metamagic:
    //             hero.TrainedMetamagicOptions.RemoveRange(hero.TrainedMetamagicOptions.Count - poolAmount,
    //                 poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Skill:
    //             hero.TrainedSkills.RemoveRange(hero.TrainedSkills.Count - poolAmount, poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.Tool:
    //             hero.TrainedToolTypes.RemoveRange(hero.TrainedToolTypes.Count - poolAmount, poolAmount);
    //             break;
    //
    //         case HeroDefinitions.PointsPoolType.SpellUnlearn:
    //         case HeroDefinitions.PointsPoolType.Irrelevant:
    //             break;
    //     }
    // }

    // internal static void RemoveFeatures(
    //     [NotNull] RulesetCharacterHero hero,
    //     CharacterClassDefinition characterClassDefinition,
    //     string tag,
    //     List<FeatureDefinition> featuresToRemove)
    // {
    //     var classLevel = hero.ClassesAndLevels[characterClassDefinition];
    //     var heroRepertoire =
    //         hero.SpellRepertoires.FirstOrDefault(x =>
    //             LevelUpContext.IsRepertoireFromSelectedClassSubclass(hero, x));
    //     var buildingData = hero.GetHeroBuildingData();
    //     var spellTag = tag;
    //
    //     foreach (var featureDefinition in featuresToRemove)
    //     {
    //         switch (featureDefinition)
    //         {
    //             case FeatureDefinitionCastSpell when heroRepertoire != null:
    //                 hero.SpellRepertoires.Remove(heroRepertoire);
    //
    //                 break;
    //
    //             case FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells
    //                 when heroRepertoire != null:
    //             {
    //                 var spellsToRemove = featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups
    //                     .FirstOrDefault(x => x.ClassLevel == classLevel)?.SpellsList.Count ?? 0;
    //
    //                 while (spellsToRemove-- > 0)
    //                 {
    //                     heroRepertoire.AutoPreparedSpells.RemoveAt(heroRepertoire.AutoPreparedSpells.Count - 1);
    //                 }
    //
    //                 break;
    //             }
    //             case FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips when heroRepertoire != null:
    //             {
    //                 heroRepertoire.KnownCantrips.RemoveAll(featureDefinitionBonusCantrips.BonusCantrips.Contains);
    //
    //                 if (buildingData == null)
    //                 {
    //                     continue;
    //                 }
    //
    //                 if (buildingData.BonusCantrips.ContainsKey(spellTag))
    //                 {
    //                     buildingData.BonusCantrips[spellTag]
    //                         .RemoveAll(featureDefinitionBonusCantrips.BonusCantrips.Contains);
    //                 }
    //
    //                 break;
    //             }
    //             case FeatureDefinitionFightingStyleChoice:
    //                 hero.TrainedFightingStyles.RemoveAt(hero.TrainedFightingStyles.Count - 1);
    //
    //                 break;
    //
    //             case FeatureDefinitionSubclassChoice:
    //                 hero.ClassesAndSubclasses.Remove(characterClassDefinition);
    //
    //                 break;
    //
    //             case FeatureDefinitionPointPool featureDefinitionPointPool:
    //                 RemoveFeatureDefinitionPointPool(hero, heroRepertoire, tag, featureDefinitionPointPool);
    //
    //                 break;
    //
    //             case FeatureDefinitionFeatureSet
    //             {
    //                 Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
    //             } featureDefinitionFeatureSet:
    //                 RemoveFeatures(hero, characterClassDefinition, tag, featureDefinitionFeatureSet.FeatureSet);
    //
    //                 break;
    //         }
    //     }
    // }

    internal static void RechargeLinkedPowers(
        [NotNull] RulesetCharacter character,
        RuleDefinitions.RestType restType)
    {
        var pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();

        foreach (var usablePower in character.UsablePowers)
        {
            FeatureDefinitionPower rechargedPower;

            if (usablePower.PowerDefinition is IPowerSharedPool pool)
            {
                rechargedPower = pool.GetUsagePoolPower();
            }
            else if (usablePower.PowerDefinition.HasSubFeatureOfType<HasModifiedUses>())
            {
                rechargedPower = usablePower.PowerDefinition;
            }
            else
            {
                continue;
            }

            // Only add to recharge here if it (recharges on a short rest and this is a short or long rest) or
            // it recharges on a long rest and this is a long rest
            if (!pointPoolPowerDefinitions.Contains(rechargedPower)
                && (rechargedPower.RechargeRate == RuleDefinitions.RechargeRate.ShortRest
                    || (rechargedPower.RechargeRate == RuleDefinitions.RechargeRate.LongRest
                        && restType == RuleDefinitions.RestType.LongRest)))
            {
                pointPoolPowerDefinitions.Add(rechargedPower);
            }
        }

        // Find the UsablePower of the point pool powers.
        foreach (var poolPower in character.UsablePowers)
        {
            if (!pointPoolPowerDefinitions.Contains(poolPower.PowerDefinition))
            {
                continue;
            }

            var poolSize = GetMaxUsesForPool(poolPower, character);

            poolPower.remainingUses = poolSize;

            AssignUsesToSharedPowersForPool(character, poolPower, poolSize, poolSize);
        }
    }

    private static void AssignUsesToSharedPowersForPool(
        [NotNull] RulesetCharacter character,
        RulesetUsablePower poolPower,
        int remainingUses,
        int totalUses)
    {
        // Find powers that rely on this pool
        foreach (var usablePower in character.UsablePowers)
        {
            var power = usablePower.PowerDefinition;

            if (power is not IPowerSharedPool pool)
            {
                continue;
            }

            var pointPoolPower = pool.GetUsagePoolPower();

            if (pointPoolPower != poolPower.PowerDefinition)
            {
                continue;
            }

            if (power.CostPerUse == 0)
            {
                //Shared pool powers should have some cost, otherwise why make them shared?
                Main.Error($"Shared pool power '{power.Name}' has zero use cost!");
                usablePower.maxUses = totalUses;
                usablePower.remainingUses = remainingUses;
            }
            else
            {
                usablePower.maxUses = totalUses / power.CostPerUse;
                usablePower.remainingUses = remainingUses / power.CostPerUse;
            }
        }
    }

    [CanBeNull]
    internal static RulesetUsablePower GetPoolPower([NotNull] RulesetUsablePower power, RulesetCharacter character)
    {
        if (power.PowerDefinition is not IPowerSharedPool pool)
        {
            return null;
        }

        var poolPower = pool.GetUsagePoolPower();

        return character.UsablePowers.FirstOrDefault(usablePower => usablePower.PowerDefinition == poolPower);
    }

    internal static int GetMaxUsesForPool([NotNull] RulesetUsablePower poolPower, [NotNull] RulesetCharacter character)
    {
        return poolPower.MaxUses + character.GetSubFeaturesByType<IPowerUseModifier>()
            .Where(m => m.PowerPool == poolPower.PowerDefinition)
            .Sum(m => m.PoolChangeAmount(character));
    }

    internal static int GetMaxUsesForPool(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power)
    {
        if (power is IPowerSharedPool poolPower)
        {
            power = poolPower.GetUsagePoolPower();
        }

        var usablePower = character.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);

        return usablePower == null ? 0 : GetMaxUsesForPool(usablePower, character);
    }

    // internal static void UpdateUsageForPower([NotNull] this RulesetCharacter character, FeatureDefinitionPower power,
    //     int poolUsage)
    // {
    //     foreach (var poolPower in character.UsablePowers)
    //     {
    //         if (poolPower.PowerDefinition != power)
    //         {
    //             continue;
    //         }
    //
    //         var maxUses = GetMaxUsesForPool(poolPower, character);
    //         var remainingUses = Mathf.Clamp(poolPower.RemainingUses - poolUsage, 0, maxUses);
    //
    //         poolPower.remainingUses = remainingUses;
    //         AssignUsesToSharedPowersForPool(character, poolPower, remainingUses, maxUses);
    //
    //         return;
    //     }
    // }

    internal static void UpdateUsageForPower(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power,
        int poolUsage)
    {
        if (power is IPowerSharedPool poolPower)
        {
            power = poolPower.GetUsagePoolPower();
        }

        var usablePower = character.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);

        if (usablePower != null)
        {
            UpdateUsageForPowerPool(character, poolUsage, usablePower);
        }
    }

    internal static void UpdateUsageForPowerPool(this RulesetCharacter character,
        [NotNull] RulesetUsablePower modifiedPower,
        int poolUsage)
    {
        if (modifiedPower.PowerDefinition is not IPowerSharedPool sharedPoolPower)
        {
            return;
        }

        var pointPoolPower = sharedPoolPower.GetUsagePoolPower();
        var usablePower = character.UsablePowers.FirstOrDefault(u => u.PowerDefinition == pointPoolPower);

        if (usablePower != null)
        {
            UpdateUsageForPowerPool(character, poolUsage, usablePower);
        }
    }

    private static void UpdateUsageForPowerPool(
        RulesetCharacter character,
        int poolUsage,
        RulesetUsablePower usablePower)
    {
        var maxUses = GetMaxUsesForPool(usablePower, character);
        var remainingUses = Mathf.Clamp(usablePower.RemainingUses - poolUsage, 0, maxUses);

        usablePower.remainingUses = remainingUses;
        AssignUsesToSharedPowersForPool(character, usablePower, remainingUses, maxUses);

        // refresh character control panel after power pool usage is updated
        // needed for custom point pools on portrait to update properly in some cases
        GameUiContext.GameHud.RefreshCharacterControlPanel();
    }

    // internal static int GetRemainingPowerUses(this RulesetCharacter character, [NotNull] RulesetUsablePower usablePower)
    // {
    //     if (usablePower.PowerDefinition is not IPowerSharedPool sharedPoolPower)
    //     {
    //         return usablePower.PowerDefinition.CostPerUse == 0
    //             ? int.MaxValue
    //             : usablePower.RemainingUses / usablePower.PowerDefinition.CostPerUse;
    //     }
    //
    //     return GetRemainingPowerPoolUses(character, sharedPoolPower);
    // }

    internal static int GetRemainingPowerUses(this RulesetCharacter character, [NotNull] FeatureDefinitionPower power)
    {
        if (power.CostPerUse == 0 || power.RechargeRate == RuleDefinitions.RechargeRate.AtWill)
        {
            return int.MaxValue;
        }

        if (power.RechargeRate == RuleDefinitions.RechargeRate.KiPoints)
        {
            return character.TryGetAttributeValue(AttributeDefinitions.KiPoints) - character.UsedKiPoints;
        }

        if (power is IPowerSharedPool poolPower)
        {
            return GetRemainingPowerPoolUses(character, poolPower) / power.CostPerUse;
        }

        var usablePower = character.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);

        if (usablePower == null)
        {
            return 0;
        }

        return usablePower.RemainingUses / power.CostPerUse;
    }

    internal static int GetRemainingPowerCharges(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power)
    {
        if (power is IPowerSharedPool poolPower)
        {
            return GetRemainingPowerPoolUses(character, poolPower);
        }

        var usablePower = character.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);

        return usablePower?.RemainingUses ?? 0;
    }

    private static int GetRemainingPowerPoolUses(
        this RulesetCharacter character,
        [NotNull] IPowerSharedPool sharedPoolPower)
    {
        var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

        return (from poolPower in character.UsablePowers
            where poolPower.PowerDefinition == pointPoolPower
            select poolPower.RemainingUses).FirstOrDefault();
    }

    internal static EffectDescription ModifySpellEffect(EffectDescription original, [NotNull] RulesetEffectSpell spell)
    {
        return ModifyMagicEffect(original, spell.SpellDefinition, spell.Caster);
    }

    internal static EffectDescription ModifySpellEffect([NotNull] SpellDefinition spell, RulesetCharacter caster)
    {
        return ModifyMagicEffect(spell.EffectDescription, spell, caster);
    }

    internal static EffectDescription ModifyPowerEffect(EffectDescription original, [NotNull] RulesetEffectPower power)
    {
        return ModifyMagicEffect(original, power.PowerDefinition, power.User);
    }

    private static string Key(BaseDefinition definition)
    {
        return $"{definition.GetType()}:{definition.Name}";
    }

    private static EffectDescription GetCachedEffect(RulesetCharacter caster, BaseDefinition definition)
    {
        if (!SpellEffectCache.TryGetValue(caster.Guid, out var effects))
        {
            return null;
        }

        return !effects.TryGetValue(Key(definition), out var effect) ? null : effect;
    }

    private static void CacheEffect(RulesetCharacter caster, BaseDefinition definition, EffectDescription effect)
    {
        Dictionary<string, EffectDescription> effects;

        if (!SpellEffectCache.ContainsKey(caster.Guid))
        {
            effects = new Dictionary<string, EffectDescription>();
            SpellEffectCache.Add(caster.Guid, effects);
        }
        else
        {
            effects = SpellEffectCache[caster.Guid];
        }

        effects.AddOrReplace(Key(definition), effect);
    }

    internal static void ClearSpellEffectCache(RulesetCharacter caster)
    {
        SpellEffectCache.Remove(caster.Guid);
    }

    private static EffectDescription ModifyMagicEffect(
        EffectDescription original,
        BaseDefinition definition,
        [CanBeNull] RulesetCharacter caster)
    {
        var result = original;

        if (caster == null)
        {
            return result;
        }

        var cached = GetCachedEffect(caster, definition);

        if (cached != null)
        {
            return cached;
        }

        var baseDefinition = definition.GetFirstSubFeatureOfType<ICustomMagicEffectBasedOnCaster>();

        if (baseDefinition != null)
        {
            result = baseDefinition.GetCustomEffect(caster) ?? original;
        }

        var modifiers = caster.GetSubFeaturesByType<IModifyMagicEffect>();

        modifiers.AddRange(definition.GetAllSubFeaturesOfType<IModifyMagicEffect>());

        if (!modifiers.Empty())
        {
            result = modifiers.Aggregate(EffectDescriptionBuilder.Create(result).Build(),
                (current, f) => f.ModifyEffect(definition, current, caster));
        }

        CacheEffect(caster, definition, result);

        return result;
    }

    /**Modifies spell/power description for GUI purposes.*/
    internal static EffectDescription ModifyMagicEffectGui(EffectDescription original,
        [NotNull] BaseDefinition definition)
    {
        return ModifyMagicEffect(original, definition, Global.CurrentCharacter);
    }

    // [NotNull]
    // internal static EffectDescription SetEffectFormss(EffectDescription baseEffect,
    //     [NotNull] params EffectForm[] effectForms)
    // {
    //     var newEffect = baseEffect.Copy();
    //
    //     newEffect.EffectForms.AddRange(effectForms);
    //
    //     return newEffect;
    // }

    internal static bool ValidatePrerequisites(
        [NotNull] RulesetCharacter character,
        [NotNull] BaseDefinition feature,
        [NotNull] IEnumerable<IDefinitionWithPrerequisites.Validate> validators,
        [NotNull] out List<string> prerequisites)
    {
        var result = true;
        prerequisites = new List<string>();

        foreach (var validator in validators)
        {
            if (!validator(character, feature, out var line))
            {
                result = false;
            }

            if (line != null)
            {
                prerequisites.Add(line);
            }
        }

        return result;
    }
}
