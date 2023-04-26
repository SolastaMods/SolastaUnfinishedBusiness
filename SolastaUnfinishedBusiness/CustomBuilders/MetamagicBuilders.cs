using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBuilders;

internal static class MetamagicBuilders
{
    private const string MetamagicAltruistic = "MetamagicAltruisticSpell";
    private const string MetamagicFocused = "MetamagicFocusedSpell";
    private const string MetamagicPowerful = "MetamagicPowerfulSpell";
    private const string MetamagicWidened = "MetamagicWidenedSpell";

    #region Metamagic Altruistic

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

    #endregion

    #region Metamagic Focused

    internal static MetamagicOptionDefinition BuildMetamagicFocusedSpell()
    {
        var validator = new MetamagicApplicationValidator(IsMetamagicFocusedSpellValid);

        var magicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagiAffinity{MetamagicFocused}")
            .SetGuiPresentation(MetamagicFocused, Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{MetamagicFocused}")
            .SetGuiPresentation(MetamagicFocused, Category.Feature,
                DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance)
            .SetPossessive()
            .AddFeatures(magicAffinity)
            .AddToDB();

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicFocused)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .SetCustomSubFeatures(new ModifyMagicEffectMetamagicFocused(condition), validator)
            .AddToDB();
    }

    private static void IsMetamagicFocusedSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var spell = rulesetEffectSpell.SpellDefinition;

        if (spell.RequiresConcentration)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustRequireConcentration";

        result = false;
    }

    private sealed class ModifyMagicEffectMetamagicFocused : IModifyMagicEffect
    {
        private readonly ConditionDefinition _conditionFocused;

        public ModifyMagicEffectMetamagicFocused(ConditionDefinition conditionFocused)
        {
            _conditionFocused = conditionFocused;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition, EffectDescription effect, RulesetCharacter character)
        {
            effect.EffectForms.Add(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(_conditionFocused, ConditionForm.ConditionOperation.Add, true)
                    .Build());

            return effect;
        }
    }

    #endregion

    #region Metamagic Powerful

    internal static MetamagicOptionDefinition BuildMetamagicPowerfulSpell()
    {
        var validator = new MetamagicApplicationValidator(IsMetamagicPowerfulSpellValid);

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicPowerful)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .SetCustomSubFeatures(new ModifyMagicEffectMetamagicPowerful(), validator)
            .AddToDB();
    }

    private static void IsMetamagicPowerfulSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;

        if (effect.EffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage))
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustHaveDamageForm";

        result = false;
    }

    private sealed class ModifyMagicEffectMetamagicPowerful : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition, EffectDescription effect, RulesetCharacter character)
        {
            foreach (var effectForm in effect.EffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                effectForm.DamageForm.diceNumber += 1;
            }

            return effect;
        }
    }

    #endregion

    #region Metamagic Widened

    internal static MetamagicOptionDefinition BuildMetamagicWidenedSpell()
    {
        var validator = new MetamagicApplicationValidator(IsMetamagicWidenedSpellValid);

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicWidened)
            .SetGuiPresentation(Category.Feature)
            .SetCost(MetamagicCostMethod.FixedValue, 2)
            .SetCustomSubFeatures(new ModifyMagicEffectMetamagicWidened(), validator)
            .AddToDB();
    }

    private static void IsMetamagicWidenedSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;

        if (effect.targetType is not TargetType.Cone or TargetType.Cube or TargetType.Cylinder or TargetType.Sphere)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustBeOfTargetArea";

        result = false;
    }

    private sealed class ModifyMagicEffectMetamagicWidened : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition, EffectDescription effect, RulesetCharacter character)
        {
            effect.targetParameter += 1;

            return effect;
        }
    }

    #endregion
}
