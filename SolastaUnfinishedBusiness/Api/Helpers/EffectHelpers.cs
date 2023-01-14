using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class EffectHelpers
{
    /**DC and magic attack bonus will be calculated based on the stats of the user, not from device itself*/
    public const int BasedOnUser = -1;

    /**DC and magic attack bonus will be calculated based on the stats of character who summoned item, not from device itself*/
    public const int BasedOnItemSummoner = -2;

    internal static int CalculateSaveDc(RulesetCharacter character, EffectDescription effectDescription,
        string className, int def = 10)
    {
        switch (effectDescription.DifficultyClassComputation)
        {
            case RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature:
            {
                var rulesetSpellRepertoire = character.GetClassSpellRepertoire(className);

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
#if false
            //TODO: implement missing computation methods (like Ki and Breath Weapon)
            case RuleDefinitions.EffectDifficultyClassComputation.Ki:
                break;
            case RuleDefinitions.EffectDifficultyClassComputation.BreathWeapon:
                break;
            case RuleDefinitions.EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency:
                break;
#endif
        }

        return def;
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
}
