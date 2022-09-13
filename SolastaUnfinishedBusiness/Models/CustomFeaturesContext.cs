using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

public static class CustomFeaturesContext
{
    internal static void RecursiveGrantCustomFeatures(RulesetCharacterHero hero, string tag,
        [NotNull] List<FeatureDefinition> features, bool handleCustomCode = true)
    {
        foreach (var grantedFeature in features)
        {
            if (handleCustomCode && grantedFeature is IFeatureDefinitionCustomCode customFeature)
            {
                customFeature.ApplyFeature(hero, tag);
            }

            if (grantedFeature is FeatureDefinitionFeatureSet
                {
                    Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
                } set)
            {
                RecursiveGrantCustomFeatures(hero, tag, set.FeatureSet, handleCustomCode);
            }

            if (grantedFeature is not FeatureDefinitionProficiency featureDefinitionProficiency)
            {
                continue;
            }

            if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
            {
                continue;
            }

            featureDefinitionProficiency.Proficiencies
                .ForEach(prof =>
                    hero.TrainedFightingStyles
                        .Add(DatabaseRepository.GetDatabase<FightingStyleDefinition>()
                            .GetElement(prof)));
        }
    }

    internal static void RecursiveRemoveCustomFeatures([NotNull] RulesetCharacterHero hero, string tag,
        List<FeatureDefinition> features, bool handleCustomCode = true)
    {
        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        // this happens during character creation
        if (selectedClass == null)
        {
            return;
        }

        foreach (var grantedFeature in features)
        {
            if (handleCustomCode && grantedFeature is IFeatureDefinitionCustomCode customFeature)
            {
                customFeature.RemoveFeature(hero, tag);
            }

            if (grantedFeature is FeatureDefinitionFeatureSet
                {
                    Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
                } set)
            {
                RecursiveRemoveCustomFeatures(hero, tag, set.FeatureSet, handleCustomCode);
            }

            if (grantedFeature is not FeatureDefinitionProficiency featureDefinitionProficiency)
            {
                continue;
            }

            if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
            {
                continue;
            }

            featureDefinitionProficiency.Proficiencies
                .ForEach(prof =>
                    hero.TrainedFightingStyles
                        .Remove(DatabaseRepository.GetDatabase<FightingStyleDefinition>()
                            .GetElement(prof)));
        }

        hero.UpdateFeatureModifiers(tag);
    }

    private static void RemoveFeatureDefinitionPointPool(
        RulesetCharacterHero hero,
        [CanBeNull] RulesetSpellRepertoire heroRepertoire,
        string tag,
        [NotNull] FeatureDefinitionPointPool featureDefinitionPointPool)
    {
        var poolAmount = featureDefinitionPointPool.PoolAmount;

        switch (featureDefinitionPointPool.PoolType)
        {
            case HeroDefinitions.PointsPoolType.AbilityScore:
                // this is handled when attributes are refreshed
                break;

            case HeroDefinitions.PointsPoolType.Cantrip:
                heroRepertoire?.KnownCantrips.RemoveRange(heroRepertoire.KnownCantrips.Count - poolAmount,
                    poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Spell:
                heroRepertoire?.KnownSpells.RemoveRange(heroRepertoire.KnownSpells.Count - poolAmount, poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Expertise:
                hero.TrainedExpertises.RemoveRange(hero.TrainedExpertises.Count - poolAmount, poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Feat:
                for (var i = 0; i < poolAmount; i++)
                {
                    var feature = hero.TrainedFeats.Last();

                    RecursiveRemoveCustomFeatures(hero, tag, feature.Features);
                    hero.TrainedFeats.RemoveAt(hero.TrainedFeats.Count - 1);
                }

                break;

            case HeroDefinitions.PointsPoolType.Language:
                hero.TrainedLanguages.RemoveRange(hero.TrainedLanguages.Count - poolAmount, poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Metamagic:
                hero.TrainedMetamagicOptions.RemoveRange(hero.TrainedMetamagicOptions.Count - poolAmount,
                    poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Skill:
                hero.TrainedSkills.RemoveRange(hero.TrainedSkills.Count - poolAmount, poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.Tool:
                hero.TrainedToolTypes.RemoveRange(hero.TrainedToolTypes.Count - poolAmount, poolAmount);
                break;

            case HeroDefinitions.PointsPoolType.SpellUnlearn:
            case HeroDefinitions.PointsPoolType.Irrelevant:
                break;
        }
    }

    internal static void RemoveFeatures(
        [NotNull] RulesetCharacterHero hero,
        CharacterClassDefinition characterClassDefinition,
        string tag,
        List<FeatureDefinition> featuresToRemove)
    {
        var classLevel = hero.ClassesAndLevels[characterClassDefinition];
        var heroRepertoire =
            hero.SpellRepertoires.FirstOrDefault(x =>
                LevelUpContext.IsRepertoireFromSelectedClassSubclass(hero, x));
        var buildingData = hero.GetHeroBuildingData();
        var spellTag = tag;

        foreach (var featureDefinition in featuresToRemove)
        {
            switch (featureDefinition)
            {
                case FeatureDefinitionCastSpell when heroRepertoire != null:
                    hero.SpellRepertoires.Remove(heroRepertoire);

                    break;

                case FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells
                    when heroRepertoire != null:
                {
                    var spellsToRemove = featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups
                        .FirstOrDefault(x => x.ClassLevel == classLevel)?.SpellsList.Count ?? 0;

                    while (spellsToRemove-- > 0)
                    {
                        heroRepertoire.AutoPreparedSpells.RemoveAt(heroRepertoire.AutoPreparedSpells.Count - 1);
                    }

                    break;
                }
                case FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips when heroRepertoire != null:
                {
                    heroRepertoire.KnownCantrips.RemoveAll(featureDefinitionBonusCantrips.BonusCantrips.Contains);

                    if (buildingData == null)
                    {
                        continue;
                    }

                    if (buildingData.BonusCantrips.ContainsKey(spellTag))
                    {
                        buildingData.BonusCantrips[spellTag]
                            .RemoveAll(featureDefinitionBonusCantrips.BonusCantrips.Contains);
                    }

                    break;
                }
                case FeatureDefinitionFightingStyleChoice:
                    hero.TrainedFightingStyles.RemoveAt(hero.TrainedFightingStyles.Count - 1);

                    break;

                case FeatureDefinitionSubclassChoice:
                    hero.ClassesAndSubclasses.Remove(characterClassDefinition);

                    break;

                case FeatureDefinitionPointPool featureDefinitionPointPool:
                    RemoveFeatureDefinitionPointPool(hero, heroRepertoire, tag, featureDefinitionPointPool);

                    break;

                case FeatureDefinitionFeatureSet
                {
                    Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union
                } featureDefinitionFeatureSet:
                    RemoveFeatures(hero, characterClassDefinition, tag, featureDefinitionFeatureSet.FeatureSet);

                    break;
            }
        }
    }

    internal static void RechargeLinkedPowers(
        [NotNull] RulesetCharacter character,
        RuleDefinitions.RestType restType)
    {
        var pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();

        foreach (var usablePower in character.UsablePowers)
        {
            if (usablePower.PowerDefinition is not IPowerSharedPool pool)
            {
                continue;
            }

            var pointPoolPower = pool.GetUsagePoolPower();

            // Only add to recharge here if it (recharges on a short rest and this is a short or long rest) or
            // it recharges on a long rest and this is a long rest
            if (!pointPoolPowerDefinitions.Contains(pointPoolPower)
                && ((pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.ShortRest &&
                     restType is RuleDefinitions.RestType.ShortRest or RuleDefinitions.RestType.LongRest) ||
                    (pointPoolPower.RechargeRate == RuleDefinitions.RechargeRate.LongRest &&
                     restType == RuleDefinitions.RestType.LongRest)))
            {
                pointPoolPowerDefinitions.Add(pointPoolPower);
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
            if (usablePower.PowerDefinition is not IPowerSharedPool pool)
            {
                continue;
            }

            var pointPoolPower = pool.GetUsagePoolPower();

            if (pointPoolPower != poolPower.PowerDefinition)
            {
                continue;
            }

            usablePower.maxUses = totalUses / usablePower.PowerDefinition.CostPerUse;
            usablePower.remainingUses = remainingUses / usablePower.PowerDefinition.CostPerUse;
        }
    }

    [CanBeNull]
    public static RulesetUsablePower GetPoolPower([NotNull] RulesetUsablePower power, RulesetCharacter character)
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
        var totalPoolSize = poolPower.MaxUses;

        foreach (var modifierPower in character.UsablePowers)
        {
            if (modifierPower.PowerDefinition is IPowerPoolModifier modifier &&
                modifier.GetUsagePoolPower() == poolPower.PowerDefinition)
            {
                totalPoolSize += modifierPower.MaxUses;
            }
        }

        return totalPoolSize;
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

    internal static void UpdateUsageForPowerPool(this RulesetCharacter character,
        [NotNull] RulesetUsablePower modifiedPower,
        int poolUsage)
    {
        if (modifiedPower.PowerDefinition is not IPowerSharedPool sharedPoolPower)
        {
            return;
        }

        var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

        foreach (var poolPower in character.UsablePowers)
        {
            if (poolPower.PowerDefinition != pointPoolPower)
            {
                continue;
            }

            var maxUses = GetMaxUsesForPool(poolPower, character);
            var remainingUses = Mathf.Clamp(poolPower.RemainingUses - poolUsage, 0, maxUses);

            poolPower.remainingUses = remainingUses;
            AssignUsesToSharedPowersForPool(character, poolPower, remainingUses, maxUses);

            // refresh character control panel after power pool usage is updated
            // needed for custom point pools on portrait to update properly in some cases
            GameUiContext.GameHud.RefreshCharacterControlPanel();
            return;
        }
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
        if (power.CostPerUse == 0)
        {
            return int.MaxValue;
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

    private static int GetRemainingPowerPoolUses(
        [NotNull] this RulesetCharacter character,
        [NotNull] IPowerSharedPool sharedPoolPower)
    {
        var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

        return (from poolPower in character.UsablePowers
            where poolPower.PowerDefinition == pointPoolPower
            select poolPower.RemainingUses).FirstOrDefault();
    }

    public static EffectDescription ModifySpellEffect(EffectDescription original, [NotNull] RulesetEffectSpell spell)
    {
        return ModifySpellEffect(original, spell.SpellDefinition, spell.Caster);
    }

    public static EffectDescription ModifySpellEffect([NotNull] SpellDefinition spell, RulesetCharacter caster)
    {
        return ModifySpellEffect(spell.EffectDescription, spell, caster);
    }

    private static EffectDescription ModifySpellEffect(
        EffectDescription original,
        SpellDefinition spell,
        [CanBeNull] RulesetCharacter caster)
    {
        //TODO: find a way to cache result, so it works faster - this method is called several times per spell cast
        var result = original;

        var baseDefinition = spell.GetFirstSubFeatureOfType<ICustomMagicEffectBasedOnCaster>();
        if (baseDefinition != null && caster != null)
        {
            result = baseDefinition.GetCustomEffect(caster) ?? original;
        }

        var modifiers = caster.GetFeaturesByType<IModifySpellEffect>();

        if (!modifiers.Empty())
        {
            result = modifiers.Aggregate(result.Copy(), (current, f) => f.ModifyEffect(spell, current, caster));
        }

        return result;
    }

    /**Modifies spell description for GUI purposes. Uses only modifiers based on ICustomMagicEffectBasedOnCaster*/
    public static EffectDescription ModifySpellEffectGui(EffectDescription original, [NotNull] GuiSpellDefinition spell)
    {
        var result = original;
        var caster = Global.InspectedHero
                     ?? Global.ActiveLevelUpHero
                     ?? Global.ActivePlayerCharacter?.RulesetCharacter;

        var baseDefinition = spell.SpellDefinition.GetFirstSubFeatureOfType<ICustomMagicEffectBasedOnCaster>();

        if (baseDefinition != null && caster != null)
        {
            result = baseDefinition.GetCustomEffect(caster) ?? original;
        }

        return result;
    }

    // [NotNull]
    // public static EffectDescription AddEffectForms(EffectDescription baseEffect,
    //     [NotNull] params EffectForm[] effectForms)
    // {
    //     var newEffect = baseEffect.Copy();
    //
    //     newEffect.EffectForms.AddRange(effectForms);
    //
    //     return newEffect;
    // }

    // public static bool GetValidationErrors(
    //     [NotNull] IEnumerable<IFeatureDefinitionWithPrerequisites.Validate> validators,
    //     [NotNull] out List<string> errors)
    // {
    //     errors = validators
    //         .Select(v => v())
    //         .Where(v => v != null)
    //         .ToList();
    //
    //     return errors.Empty();
    // }
}
