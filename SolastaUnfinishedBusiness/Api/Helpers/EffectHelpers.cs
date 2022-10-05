using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class EffectHelpers
{
    /**DC and magic attack bonus will be calculated based on the stats of the user, not from device itself*/
    public const int BASED_ON_USER = -1;
    /**DC and magic attack bonus will be calculated based on the stats of character who summoned item, not from device itself*/
    public const int BASED_ON_ITEM_SUMMONER = -2;
    
    internal static int CalculateSaveDc(RulesetCharacter character, EffectDescription effectDescription,
        string className, int def = 10)
    {
        //TODO: implement missing computation methods (like Ki and Breath Weapon)
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
        }

        return def;
    }

    internal static RulesetCharacter GetCharacterByEffectGuid(ulong guid)
    {
        if (guid == 0) { return null; }

        if (RulesetEntity.TryGetEntity<RulesetEffect>(guid, out var effect))
        {
            if (effect is RulesetEffectSpell spell)
            {
                return spell.Caster;
            }
            else if (effect is RulesetEffectPower power)
            {
                return power.User;
            }
        }

        return null;
    }
}
