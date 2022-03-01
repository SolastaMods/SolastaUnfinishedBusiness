using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.AHSpells
{
    internal class PactMarkSpellBuilder : SpellDefinitionBuilder
    {
        private const string PactMarkSpellName = "AHPactMarkSpellBuilder";
        private static readonly string PactMarkSpellNameGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkSpellName).ToString();

        protected PactMarkSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.HuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkSpellTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkSpellDescription";
            Definition.SetSpellLevel(1);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);
            Definition.SetSchoolOfMagic("SchoolEnchantment");
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.BonusAction);

            var markedByPactEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition
                }
            };
            markedByPactEffectForm.SetCreatedByCharacter(true);

            var pactMarkEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = PactMarkPactMarkConditionBuilder.PactMarkCondition
                }
            };
            pactMarkEffectForm.ConditionForm.SetApplyToSelf(true);
            pactMarkEffectForm.SetCreatedByCharacter(true);

            var effectDescription = Definition.EffectDescription;
            effectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            effectDescription.SetRangeParameter(24);
            effectDescription.SetTargetParameter(1);
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(markedByPactEffectForm);
            effectDescription.EffectForms.Add(pactMarkEffectForm);

            Definition.SetEffectDescription(effectDescription);
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition PactMarkSpell = CreateAndAddToDB(PactMarkSpellName, PactMarkSpellNameGuid);
    }




    internal class PactMarkPactMarkConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PactMarkPactMarkConditionName = "AHPactMarkPactMarkCondition";
        private static readonly string PactMarkPactMarkConditionGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkPactMarkConditionName).ToString();

        protected PactMarkPactMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkPactMarkConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkPactMarkConditionDescription";
            Definition.Features.Clear();
            Definition.Features.Add(PactMarkAdditionalDamageBuilder.PactMarkAdditionalDamage);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkPactMarkConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition PactMarkCondition = CreateAndAddToDB(PactMarkPactMarkConditionName, PactMarkPactMarkConditionGuid);
    }



    internal class PactMarkMarkedByPactConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PactMarkMarkedByPactConditionName = "AHPactMarkMarkedByPactCondition";
        private static readonly string PactMarkMarkedByPactConditionGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkMarkedByPactConditionName).ToString();

        protected PactMarkMarkedByPactConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkMarkedByPactConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkMarkedByPactConditionDescription";
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkMarkedByPactConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition MarkedByPactCondition = CreateAndAddToDB(PactMarkMarkedByPactConditionName, PactMarkMarkedByPactConditionGuid);
    }


    internal class PactMarkAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        private const string PactMarkAdditionalDamageBuilderName = "AHPactMarkAdditionalDamage";
        private static readonly string PactMarkAdditionalDamageGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkAdditionalDamageBuilderName).ToString();

        protected PactMarkAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkAdditionalDamageDescription";
            Definition.SetAttackModeOnly(false);
            Definition.SetRequiredTargetCondition(PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition);
            Definition.SetNotificationTag("PactMarked");
        }

        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkAdditionalDamageBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAdditionalDamage PactMarkAdditionalDamage = CreateAndAddToDB(PactMarkAdditionalDamageBuilderName, PactMarkAdditionalDamageGuid);
    }
}
