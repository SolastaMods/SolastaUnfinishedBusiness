using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Spells;

internal static class AceHighSpells
{
    internal static void Register()
    {
        RegisterSpell(PactMarkBuilder.CreateAndAddToDB(), 0, DatabaseHelper.SpellListDefinitions.SpellListWarlock);
    }

    private sealed class PactMarkBuilder : SpellDefinitionBuilder
    {
        private const string PactMarkName = "PactMark";

        private PactMarkBuilder(string name) : base(DatabaseHelper.SpellDefinitions.HuntersMark, name,
            CENamespaceGuid)
        {
            var markedByPactEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = ConditionPactMarkMarkedByPactBuilder.MarkedByPactCondition
                },
                createdByCharacter = true
            };

            var pactMarkEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = ConditionPactMarkPactMarkBuilder.PactMarkCondition, applyToSelf = true
                },
                createdByCharacter = true
            };

            var effectDescription = Definition.EffectDescription;

            effectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            effectDescription.SetRangeParameter(24);
            effectDescription.SetTargetParameter(1);
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(markedByPactEffectForm);
            effectDescription.EffectForms.Add(pactMarkEffectForm);

            Definition.spellLevel = 1;
            Definition.somaticComponent = true;
            Definition.verboseComponent = true;
            Definition.schoolOfMagic = RuleDefinitions.SchoolEnchantement;
            Definition.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Definition.castingTime = RuleDefinitions.ActivationTime.BonusAction;
            Definition.effectDescription = effectDescription;
        }

        public static SpellDefinition CreateAndAddToDB()
        {
            return new PactMarkBuilder(PactMarkName)
                .SetGuiPresentation(Category.Spell)
                .AddToDB();
        }
    }

    private sealed class ConditionPactMarkPactMarkBuilder : ConditionDefinitionBuilder
    {
        private const string ConditionPactMarkPactMarkName = "ConditionPactMarkPactMark";

        internal static readonly ConditionDefinition PactMarkCondition =
            CreateAndAddToDB(ConditionPactMarkPactMarkName);

        private ConditionPactMarkPactMarkBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, CENamespaceGuid)
        {
            Definition.Features.SetRange(AdditionalDamagePactMarkBuilder.AdditionalDamagePactMark);
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new ConditionPactMarkPactMarkBuilder(name)
                .SetGuiPresentation(Category.Condition)
                .AddToDB();
        }
    }

    private sealed class ConditionPactMarkMarkedByPactBuilder : ConditionDefinitionBuilder
    {
        private const string ConditionPactMarkMarkedByPactName = "ConditionPactMarkMarkedByPact";

        internal static readonly ConditionDefinition MarkedByPactCondition =
            CreateAndAddToDB(ConditionPactMarkMarkedByPactName);

        private ConditionPactMarkMarkedByPactBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, CENamespaceGuid)
        {
            // empty
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new ConditionPactMarkMarkedByPactBuilder(name)
                .SetGuiPresentation(Category.Condition)
                .AddToDB();
        }
    }

    private sealed class AdditionalDamagePactMarkBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        private const string AdditionalDamagePactMarkBuilderName = "AdditionalDamagePactMark";

        internal static readonly FeatureDefinitionAdditionalDamage AdditionalDamagePactMark =
            CreateAndAddToDB(AdditionalDamagePactMarkBuilderName);

        private AdditionalDamagePactMarkBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, CENamespaceGuid)
        {
            Definition.attackModeOnly = false;
            Definition.requiredTargetCondition = ConditionPactMarkMarkedByPactBuilder.MarkedByPactCondition;
            Definition.notificationTag = "PactMarked";
        }

        private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name)
        {
            return new AdditionalDamagePactMarkBuilder(name)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }
    }
}
