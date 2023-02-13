using System;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Metamagic;

internal static class MetamagicBuilders
{
    private const string MetamagicAltruistic = "MetamagicAltruisticSpell";

    internal static MetamagicOptionDefinition BuildMetamagicAltruisticSpell()
    {
        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicAltruistic)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .SetCustomSubFeatures(new MetamagicApplicationValidatorMetamagicAltruisticSpell())
            .AddToDB();
    }

    private sealed class MetamagicApplicationValidatorMetamagicAltruisticSpell : IMetamagicApplicationValidator,
        IModifyMagicEffect
    {
        public bool IsMetamagicOptionValid(
            RulesetCharacter caster,
            RulesetEffectSpell rulesetEffectSpell,
            MetamagicOptionDefinition metamagicOption,
            ref string failure)
        {
            var effect = rulesetEffectSpell.EffectDescription;

            if (effect.rangeType != RangeType.Self || effect.targetType != TargetType.Self)
            {
                failure = "Failure/&FailureFlagSpellRangeMustBeSelf";

                return false;
            }

            failure = String.Empty;

            return true;
        }

        public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
            RulesetCharacter character)
        {
            effect.rangeType = RangeType.Distance;
            effect.rangeParameter = 6;
            effect.targetType = TargetType.IndividualsUnique;
            effect.targetParameter = 1;

            return effect;
        }
    }
}
