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
            .SetCost(MetamagicCostMethod.SpellLevel)
            .SetCustomSubFeatures(new ProvideMetamagicBehaviorMetamagicAltruisticSpell())
            .AddToDB();
    }

    private sealed class ProvideMetamagicBehaviorMetamagicAltruisticSpell : IProvideMetamagicBehavior
    {
        public bool IsMetamagicOptionAvailable(
            RulesetEffectSpell rulesetEffectSpell,
            RulesetCharacter caster,
            MetamagicOptionDefinition metamagicOption,
            ref string failure,
            ref bool result)
        {
            if (metamagicOption.Name != MetamagicAltruistic)
            {
                return false;
            }

            var effect = rulesetEffectSpell.EffectDescription;

            if (effect.rangeType != RangeType.Self || effect.targetType != TargetType.Self)
            {
                failure = "Failure/&FailureFlagSpellRangeMustBeSelf";
                result = false;

                return true;
            }

            failure = String.Empty;
            result = true;

            return true;
        }

        public void MetamagicSelected(
            GameLocationCharacter caster,
            RulesetEffectSpell spellEffect,
            MetamagicOptionDefinition metamagicOption)
        {
            var effect = spellEffect.EffectDescription;

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
