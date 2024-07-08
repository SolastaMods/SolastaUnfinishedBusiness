using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class EffectHelpers
{
    /**DC and magic attack bonus will be calculated based on the stats of the user, not from device itself*/
    public const int BasedOnUser = -1;

    /**DC and magic attack bonus will be calculated based on the stats of character who summoned item, not from device itself*/
    public const int BasedOnItemSummoner = -2;

    // use this to start a custom visual effect during combat
    internal static void StartVisualEffect(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        IMagicEffect magicEffect,
        EffectType effectType = EffectType.Impact)
    {
        var prefab = effectType switch
        {
            EffectType.Caster => magicEffect.EffectDescription.EffectParticleParameters.CasterParticle,
            EffectType.QuickCaster => magicEffect.EffectDescription.EffectParticleParameters.CasterQuickSpellParticle,
            EffectType.Condition => magicEffect.EffectDescription.EffectParticleParameters.ConditionParticle,
            EffectType.Effect => magicEffect.EffectDescription.EffectParticleParameters.EffectParticle,
            EffectType.Impact => magicEffect.EffectDescription.EffectParticleParameters.ImpactParticle,
            EffectType.Zone => magicEffect.EffectDescription.EffectParticleParameters.ZoneParticle,
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };

        if (!prefab)
        {
            return;
        }

        var sentParameters = new ParticleSentParameters(attacker, defender, magicEffect.Name);

        WorldLocationPoolManager
            .GetElement(prefab, true)
            .GetComponent<ParticleSetup>()
            .Setup(sentParameters);
    }

    internal static void StartVisualEffect(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        EffectParticleParameters effectParticleParameters,
        EffectType effectType = EffectType.Impact)
    {
        var prefab = effectType switch
        {
            EffectType.Caster => effectParticleParameters.CasterParticle,
            EffectType.QuickCaster => effectParticleParameters.CasterQuickSpellParticle,
            EffectType.Condition => effectParticleParameters.ConditionParticle,
            EffectType.Effect => effectParticleParameters.EffectParticle,
            EffectType.Impact => effectParticleParameters.ImpactParticle,
            EffectType.Zone => effectParticleParameters.ZoneParticle,
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };

        if (!prefab)
        {
            return;
        }

        var sentParameters = new ParticleSentParameters(attacker, defender, "test");

        WorldLocationPoolManager
            .GetElement(prefab, true)
            .GetComponent<ParticleSetup>()
            .Setup(sentParameters);
    }

    internal static int CalculateSaveDc(RulesetCharacter character, EffectDescription effectDescription,
        CharacterClassDefinition classDefinition, int def = 10)
    {
        switch (effectDescription.DifficultyClassComputation)
        {
            case RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature:
            {
                var rulesetSpellRepertoire = character.GetClassSpellRepertoire(classDefinition);

                if (rulesetSpellRepertoire != null)
                {
                    return rulesetSpellRepertoire.SaveDC;
                }

                break;
            }
            case RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency:
                var attributeValue = character.TryGetAttributeValue(effectDescription.SavingThrowDifficultyAbility);
                var proficiencyBonus = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                return RuleDefinitions.ComputeAbilityScoreBasedDC(attributeValue, proficiencyBonus);

            case RuleDefinitions.EffectDifficultyClassComputation.FixedValue:
                return effectDescription.FixedSavingThrowDifficultyClass;
            //TODO: implement missing computation methods (like Ki and Breath Weapon)
            case RuleDefinitions.EffectDifficultyClassComputation.Ki:
                break;
            case RuleDefinitions.EffectDifficultyClassComputation.BreathWeapon:
                break;
            case RuleDefinitions.EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency:
                break;
            // ReSharper disable once RedundantEmptySwitchSection
            default:
                break;
        }

        return def;
    }

    /// <summary>
    /// Utility to replace a saving throw score with a different one.
    /// If useNewBonuses = false, then only the ability modifier will be replaced.
    /// If useNewBonuses = true, then the replacement will include all bonuses of the replacement (e.g. proficiency).
    /// For example, if useNewBonuses = true, and you are replacing a WIS save with INT and you are proficient in INT, you will roll as though it were an INT save.
    /// 
    /// Note that this functions differently from Vanilla.
    /// Vanilla Solasta has the Mana Painter Sorcerer, and its Mana Absorption feature will override the source saving throw be Charisma entirely, if the Charisma mod is higher.
    /// This has interesting consequences. For example, if you are hit with Fireball...
    ///    1) you get the Charisma proficiency bonus included for being a sorcerer, and
    ///    2) you benefit from features that give you bonuses to Charisma (e.g. Gnome advantage for INT/WIS/CHA)
    /// </summary>
    internal static bool ReplaceSavingThrowSourceIfHigher(RulesetCharacter defender, string replacementAbilityScoreName, bool useNewBonuses, ref string abilityScoreName, ref int saveBonus, List<RuleDefinitions.TrendInfo> savingThrowModifierTrends)
    {
        if (useNewBonuses)
        {
            List<RuleDefinitions.TrendInfo> oldTrends = new List<RuleDefinitions.TrendInfo>();
            List<RuleDefinitions.TrendInfo> newTrends = new List<RuleDefinitions.TrendInfo>();
            int origMod = defender.ComputeBaseSavingThrowBonus(abilityScoreName, oldTrends);
            int replacementMod = defender.ComputeBaseSavingThrowBonus(replacementAbilityScoreName, newTrends); // Expected to return proficiency bonuses included.
            if (replacementMod < origMod)
            {
                return false;
            }

            // Other bonuses (e.g. from items) might have entries in the existing Trends list.
            // Remove only the items related to the old ability, and replace them with the new bonuses.
            // This code assumes that trends actually include the original ability score.
            foreach (RuleDefinitions.TrendInfo info in oldTrends)
            {
                for (int i = 0; i < savingThrowModifierTrends.Count; ++i)
                {
                    RuleDefinitions.TrendInfo t = savingThrowModifierTrends[i];
                    if (info.sourceName == t.sourceName && info.value == t.value && info.sourceType == t.sourceType)
                    {
                        savingThrowModifierTrends.RemoveAt(i);
                        break;
                    }
                }
            }

            savingThrowModifierTrends.InsertRange(0, newTrends);

            // Don't directly set the save bonus, since we don't know what else could be accumulated in there. Use the delta instead.
            saveBonus += replacementMod - origMod;
            abilityScoreName = replacementAbilityScoreName;
            return true;
        }
        else
        {
            int origMod = AttributeDefinitions.ComputeAbilityScoreModifier(defender.TryGetAttributeValue(abilityScoreName));
            int replacementMod = AttributeDefinitions.ComputeAbilityScoreModifier(defender.TryGetAttributeValue(replacementAbilityScoreName));
            if (replacementMod < origMod)
            {
                return false;
            }

            // Search for the original ability and replace it.
            for (int i = 0; i < savingThrowModifierTrends.Count; ++i)
            {
                RuleDefinitions.TrendInfo info = savingThrowModifierTrends[i];

                if (info.sourceType == RuleDefinitions.FeatureSourceType.AbilityScore && info.sourceName == abilityScoreName)
                {
                    saveBonus += replacementMod - info.value;
                    abilityScoreName = replacementAbilityScoreName;

                    info.value = replacementMod;
                    info.sourceName = replacementAbilityScoreName;

                    return true; // Assumes only one ability score source
                }
            }

            // Unexpected for the ability score to not be in the trends.
            return false;
        }
    }

    internal static RulesetCharacter GetSummoner(RulesetCharacter summon)
    {
        return summon.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
            RuleDefinitions.ConditionConjuredCreature, out var activeCondition)
            ? GetCharacterByGuid(activeCondition.SourceGuid)
            : null;
    }

    internal static RulesetCharacter GetCharacterByGuid(ulong guid)
    {
        if (guid == 0)
        {
            return null;
        }

        if (!RulesetEntity.TryGetEntity<RulesetEntity>(guid, out var entity))
        {
            return null;
        }

        return entity as RulesetCharacter;
    }

    internal static RulesetEffect GetEffectByGuid(ulong guid)
    {
        if (guid == 0)
        {
            return null;
        }

        return !RulesetEntity.TryGetEntity<RulesetEffect>(guid, out var entity) ? null : entity;
    }

    internal static RulesetItem GetItemByGuid(ulong guid)
    {
        if (guid == 0)
        {
            return null;
        }

        return !RulesetEntity.TryGetEntity<RulesetItem>(guid, out var item) ? null : item;
    }

    internal static List<RulesetCharacter> GetSummonedCreatures(RulesetEffect effect)
    {
        var summons = new List<RulesetCharacter>();

        if (effect == null)
        {
            return summons;
        }

        foreach (var conditionGuid in effect.trackedConditionGuids)
        {
            if (!RulesetEntity.TryGetEntity<RulesetCondition>(conditionGuid, out var condition)
                || condition.Name != RuleDefinitions.ConditionConjuredCreature)
            {
                continue;
            }

            if (RulesetEntity.TryGetEntity<RulesetCharacter>(condition.TargetGuid, out var creature)
                && creature != null)
            {
                summons.TryAdd(creature);
            }
        }

        return summons;
    }

    internal static RulesetCharacter GetCharacterByEffectGuid(ulong guid)
    {
        return GetEffectByGuid(guid) switch
        {
            RulesetEffectSpell spell => spell.Caster,
            RulesetEffectPower power => power.User,
            _ => null
        };
    }

    internal static List<RulesetEffect> GetAllEffectsBySourceGuid(ulong guid)
    {
        return ServiceRepository.GetService<IRulesetEntityService>().RulesetEntities.Values
            .OfType<RulesetEffect>()
            .Where(e => e.SourceGuid == guid)
            .ToList();
    }

    internal static List<RulesetCondition> GetAllConditionsBySourceGuid(ulong guid)
    {
        return ServiceRepository.GetService<IRulesetEntityService>().RulesetEntities.Values
            .OfType<RulesetCondition>()
            .Where(e => e.SourceGuid == guid)
            .ToList();
    }

    internal static void DoTerminate(this RulesetEffect effect, RulesetCharacter source = null)
    {
        source ??= GetCharacterByGuid(effect.SourceGuid);

        if (source != null)
        {
            switch (effect)
            {
                case RulesetEffectPower power:
                    source.TerminatePower(power);
                    return;
                case RulesetEffectSpell spell:
                    source.TerminateSpell(spell);
                    return;
            }
        }

        effect.Terminate(true);
    }

    internal static (RulesetCharacter, BaseDefinition) GetCharacterAndSourceDefinitionByEffectGuid(ulong guid)
    {
        if (guid == 0)
        {
            return (null, null);
        }

        if (!RulesetEntity.TryGetEntity<RulesetEffect>(guid, out var effect))
        {
            return (null, null);
        }

        return effect switch
        {
            RulesetEffectSpell spell => (spell.Caster, spell.SourceDefinition),
            RulesetEffectPower power => (power.User, power.PowerDefinition),
            _ => (null, null)
        };
    }

    internal static void SetGuid(this RulesetEffect effect, ulong guid)
    {
        switch (effect)
        {
            case RulesetEffectPower power:
                power.userId = guid;
                break;
            case RulesetEffectSpell spell:
                spell.casterId = guid;
                break;
        }
    }

    internal enum EffectType
    {
        Caster,
        QuickCaster,
        Condition,
        Effect,
        Impact,
        Zone
    }
}
