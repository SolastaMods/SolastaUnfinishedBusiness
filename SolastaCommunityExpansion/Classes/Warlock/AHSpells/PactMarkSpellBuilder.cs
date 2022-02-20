using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.AHSpells
{
    internal class PactMarkSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string PactMarkSpellName = "AHPactMarkSpellBuilder";
        private static readonly string PactMarkSpellNameGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkSpellName).ToString();

        [Obsolete]
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

        [Obsolete]
        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkSpellBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        public static SpellDefinition PactMarkSpell = CreateAndAddToDB(PactMarkSpellName, PactMarkSpellNameGuid);
    }




    internal class PactMarkPactMarkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string PactMarkPactMarkConditionName = "AHPactMarkPactMarkCondition";
        private static readonly string PactMarkPactMarkConditionGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkPactMarkConditionName).ToString();

        [Obsolete]
        protected PactMarkPactMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkPactMarkConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkPactMarkConditionDescription";
            Definition.Features.Clear();
            Definition.Features.Add(PactMarkAdditionalDamageBuilder.PactMarkAdditionalDamage);
        }

        [Obsolete]
        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkPactMarkConditionBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        public static ConditionDefinition PactMarkCondition = CreateAndAddToDB(PactMarkPactMarkConditionName, PactMarkPactMarkConditionGuid);
    }



    internal class PactMarkMarkedByPactConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string PactMarkMarkedByPactConditionName = "AHPactMarkMarkedByPactCondition";
        private static readonly string PactMarkMarkedByPactConditionGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkMarkedByPactConditionName).ToString();

        [Obsolete]
        protected PactMarkMarkedByPactConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkMarkedByPactConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkMarkedByPactConditionDescription";
        }

        [Obsolete]
        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkMarkedByPactConditionBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        public static ConditionDefinition MarkedByPactCondition = CreateAndAddToDB(PactMarkMarkedByPactConditionName, PactMarkMarkedByPactConditionGuid);
    }


    internal class PactMarkAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        private const string PactMarkAdditionalDamageBuilderName = "AHPactMarkAdditionalDamage";
        private static readonly string PactMarkAdditionalDamageGuid = GuidHelper.Create(new Guid(Settings.GUID), PactMarkAdditionalDamageBuilderName).ToString();

        [Obsolete]
        protected PactMarkAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&AHPactMarkAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Spell/&AHPactMarkAdditionalDamageDescription";
            Definition.SetAttackModeOnly(false);
            Definition.SetRequiredTargetCondition(PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition);
            Definition.SetNotificationTag("PactMarked");
        }

        [Obsolete]
        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkAdditionalDamageBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionAdditionalDamage PactMarkAdditionalDamage = CreateAndAddToDB(PactMarkAdditionalDamageBuilderName, PactMarkAdditionalDamageGuid);
    }
}
