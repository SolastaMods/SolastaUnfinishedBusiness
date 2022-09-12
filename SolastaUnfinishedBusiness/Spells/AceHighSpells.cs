using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
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
            Definition.GuiPresentation.Title = "Spell/&PactMarkTitle";
            Definition.GuiPresentation.Description = "Spell/&PactMarkDescription";
            Definition.spellLevel = 1;
            Definition.somaticComponent = true;
            Definition.verboseComponent = true;
            Definition.schoolOfMagic = "SchoolEnchantment";
            Definition.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Definition.castingTime = RuleDefinitions.ActivationTime.BonusAction;

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

            Definition.effectDescription = effectDescription;
        }

        public static SpellDefinition CreateAndAddToDB()
        {
            return new PactMarkBuilder(PactMarkName).AddToDB();
        }
    }

    private sealed class ConditionPactMarkPactMarkBuilder : ConditionDefinitionBuilder
    {
        private const string ConditionPactMarkPactMarkName = "ConditionPactMarkPactMark";

        public static readonly ConditionDefinition PactMarkCondition =
            CreateAndAddToDB(ConditionPactMarkPactMarkName);

        private ConditionPactMarkPactMarkBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&ConditionPactMarkPactMarkTitle";
            Definition.GuiPresentation.Description = "Spell/&ConditionPactMarkPactMarkDescription";
            Definition.Features.Clear();
            Definition.Features.Add(AdditionalDamagePactMarkBuilder.AdditionalDamagePactMark);
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new ConditionPactMarkPactMarkBuilder(name).AddToDB();
        }
    }

    private sealed class ConditionPactMarkMarkedByPactBuilder : ConditionDefinitionBuilder
    {
        private const string ConditionPactMarkMarkedByPactName = "ConditionPactMarkMarkedByPact";

        public static readonly ConditionDefinition MarkedByPactCondition =
            CreateAndAddToDB(ConditionPactMarkMarkedByPactName);

        private ConditionPactMarkMarkedByPactBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&ConditionPactMarkMarkedByPactTitle";
            Definition.GuiPresentation.Description = "Spell/&ConditionPactMarkMarkedByPactDescription";
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new ConditionPactMarkMarkedByPactBuilder(name).AddToDB();
        }
    }

    private sealed class AdditionalDamagePactMarkBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        private const string AdditionalDamagePactMarkBuilderName = "AdditionalDamagePactMark";

        public static readonly FeatureDefinitionAdditionalDamage AdditionalDamagePactMark =
            CreateAndAddToDB(AdditionalDamagePactMarkBuilderName);

        private AdditionalDamagePactMarkBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&AdditionalDamagePactMarkTitle";
            Definition.GuiPresentation.Description = "Spell/&AdditionalDamagePactMarkDescription";
            Definition.attackModeOnly = false;
            Definition.requiredTargetCondition = ConditionPactMarkMarkedByPactBuilder.MarkedByPactCondition;
            Definition.notificationTag = "PactMarked";
        }

        private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name)
        {
            return new AdditionalDamagePactMarkBuilder(name).AddToDB();
        }
    }
}
