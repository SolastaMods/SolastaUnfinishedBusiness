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
            .SetCustomSubFeatures(new ProvideMetamagicBehaviorMetamagicAltruisticSpell())
            .AddToDB();
    }

    private sealed class ProvideMetamagicBehaviorMetamagicAltruisticSpell : IProvideMetamagicBehavior
    {
        public string MetamagicOptionName()
        {
            return MetamagicAltruistic;
        }

        public bool IsMetamagicOptionAvailable(
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

        public void MetamagicSelected(
            RulesetCharacter caster,
            RulesetEffectSpell rulesetEffectSpell,
            MetamagicOptionDefinition metamagicOption)
        {
            var effect = rulesetEffectSpell.EffectDescription;

            if (effect.rangeType != RangeType.Self || effect.targetType != TargetType.Self)
            {
                return;
            }

            effect.rangeType = RangeType.Distance;
            effect.rangeParameter = 6;
            effect.targetType = TargetType.IndividualsUnique;
            effect.targetParameter = 2;
        }
    }
}
