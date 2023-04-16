using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBuilders;

internal static class MetamagicBuilders
{
    private const string MetamagicAltruistic = "MetamagicAltruisticSpell";

    internal static MetamagicOptionDefinition BuildMetamagicAltruisticSpell()
    {
        var validator = new MetamagicApplicationValidator(IsMetamagicAltruisticSpellValid);

        var altruisticAlly = MetamagicOptionDefinitionBuilder
            .Create($"{MetamagicAltruistic}Ally")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCost()
            .SetCustomSubFeatures(new MetamagicAltruisticAlly(), validator)
            .AddToDB();

        var altruisticSelf = MetamagicOptionDefinitionBuilder
            .Create($"{MetamagicAltruistic}Self")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCost(sorceryPointsCost: 3)
            .SetCustomSubFeatures(new MetamagicAltruisticSelf(), validator)
            .AddToDB();

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicAltruistic)
            .SetGuiPresentation(Category.Feature)
            .SetCost(MetamagicCostMethod.SpellLevel)
            .SetCustomSubFeatures(new ReplaceMetamagicOption(altruisticAlly, altruisticSelf))
            .AddToDB();
    }

    private static void IsMetamagicAltruisticSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;

        if (effect.rangeType == RangeType.Self && effect.targetType == TargetType.Self)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellRangeMustBeSelf";

        result = false;
    }

    private sealed class MetamagicAltruisticAlly : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter character)
        {
            effect.rangeType = RangeType.Distance;
            effect.rangeParameter = 6;
            effect.targetType = TargetType.IndividualsUnique;
            effect.targetParameter = 1;
            effect.targetExcludeCaster = true;

            return effect;
        }
    }

    private sealed class MetamagicAltruisticSelf : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter character)
        {
            effect.rangeType = RangeType.Distance;
            effect.rangeParameter = 6;
            effect.targetType = TargetType.IndividualsUnique;
            effect.targetParameter = 1;
            effect.inviteOptionalAlly = true;

            return effect;
        }
    }
}
