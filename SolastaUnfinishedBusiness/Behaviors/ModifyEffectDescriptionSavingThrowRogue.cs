using System;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors;

public sealed class ModifyEffectDescriptionSavingThrowRogue(
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    FeatureDefinitionPower baseDefinition) : IModifyEffectDescription
{
    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        return definition == baseDefinition;
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var proficiencyBonus = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
        var strength = character.TryGetAttributeValue(AttributeDefinitions.Strength);
        var dexterity = character.TryGetAttributeValue(AttributeDefinitions.Dexterity);
        var strDC = ComputeAbilityScoreBasedDC(strength, proficiencyBonus);
        var dexDC = ComputeAbilityScoreBasedDC(dexterity, proficiencyBonus);
        var saveDC = Math.Max(strDC, dexDC);

        if (rulesetEffect is RulesetEffectPower rulesetEffectPower)
        {
            rulesetEffectPower.usablePower.saveDC = saveDC;
        }

        return effectDescription;
    }
}
