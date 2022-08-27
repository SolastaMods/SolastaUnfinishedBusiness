using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Models.SpellsContext;

namespace SolastaCommunityExpansion.Spells;

internal static class AceHighSpells
{
    internal static void Register()
    {
        RegisterSpell(PactMarkSpellBuilder.CreateAndAddToDB(), 0, DatabaseHelper.SpellListDefinitions.SpellListWarlock);
    }

    private sealed class PactMarkSpellBuilder : SpellDefinitionBuilder
    {
        private const string PactMarkSpellName = "AHPactMarkSpell";

        private PactMarkSpellBuilder(string name) : base(DatabaseHelper.SpellDefinitions.HuntersMark, name,
            CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkSpellTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkSpellDescription";
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
                    ConditionDefinition = PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition
                },
                createdByCharacter = true
            };

            var pactMarkEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = PactMarkPactMarkConditionBuilder.PactMarkCondition, applyToSelf = true
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
            return new PactMarkSpellBuilder(PactMarkSpellName).AddToDB();
        }
    }

    private sealed class PactMarkPactMarkConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PactMarkPactMarkConditionName = "AHPactMarkPactMarkCondition";

        public static readonly ConditionDefinition PactMarkCondition =
            CreateAndAddToDB(PactMarkPactMarkConditionName);

        private PactMarkPactMarkConditionBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkPactMarkConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkPactMarkConditionDescription";
            Definition.Features.Clear();
            Definition.Features.Add(PactMarkAdditionalDamageBuilder.PactMarkAdditionalDamage);
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new PactMarkPactMarkConditionBuilder(name).AddToDB();
        }
    }

    private sealed class PactMarkMarkedByPactConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PactMarkMarkedByPactConditionName = "AHPactMarkMarkedByPactCondition";

        public static readonly ConditionDefinition MarkedByPactCondition =
            CreateAndAddToDB(PactMarkMarkedByPactConditionName);

        private PactMarkMarkedByPactConditionBuilder(string name) : base(
            DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkMarkedByPactConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkMarkedByPactConditionDescription";
        }

        private static ConditionDefinition CreateAndAddToDB(string name)
        {
            return new PactMarkMarkedByPactConditionBuilder(name).AddToDB();
        }
    }

    private sealed class PactMarkAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        private const string PactMarkAdditionalDamageBuilderName = "AHPactMarkAdditionalDamage";

        public static readonly FeatureDefinitionAdditionalDamage PactMarkAdditionalDamage =
            CreateAndAddToDB(PactMarkAdditionalDamageBuilderName);

        private PactMarkAdditionalDamageBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkAdditionalDamageDescription";
            Definition.attackModeOnly = false;
            Definition.requiredTargetCondition = PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition;
            Definition.notificationTag = "PactMarked";
        }

        private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name)
        {
            return new PactMarkAdditionalDamageBuilder(name).AddToDB();
        }
    }
}
