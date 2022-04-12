using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Models.SpellsContext;

namespace SolastaCommunityExpansion.Spells
{
    internal static class AceHighSpells
    {
        private static SpellDefinition _pactMarkSpell;
        internal static SpellDefinition PactMarkSpell => _pactMarkSpell ??= PactMarkSpellBuilder.CreateAndAddToDB();

        internal static void Register()
        {
            RegisterSpell(PactMarkSpell, 1, WarlockSpellList);
        }

        internal class PactMarkSpellBuilder : SpellDefinitionBuilder
        {
            private const string PactMarkSpellName = "AHPactMarkSpell";

            protected PactMarkSpellBuilder(string name) : base(DatabaseHelper.SpellDefinitions.HuntersMark, name, DefinitionBuilder.CENamespaceGuid)
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

            public static SpellDefinition CreateAndAddToDB()
            {
                return new PactMarkSpellBuilder(PactMarkSpellName).AddToDB();
            }
        }

        internal class PactMarkPactMarkConditionBuilder : ConditionDefinitionBuilder
        {
            private const string PactMarkPactMarkConditionName = "AHPactMarkPactMarkCondition";

            protected PactMarkPactMarkConditionBuilder(string name) : base(DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, DefinitionBuilder.CENamespaceGuid)
            {
                Definition.GuiPresentation.Title = "Spell/&AHPactMarkPactMarkConditionTitle";
                Definition.GuiPresentation.Description = "Spell/&AHPactMarkPactMarkConditionDescription";
                Definition.Features.Clear();
                Definition.Features.Add(PactMarkAdditionalDamageBuilder.PactMarkAdditionalDamage);
            }

            public static ConditionDefinition CreateAndAddToDB(string name)
            {
                return new PactMarkPactMarkConditionBuilder(name).AddToDB();
            }

            public static readonly ConditionDefinition PactMarkCondition = CreateAndAddToDB(PactMarkPactMarkConditionName);
        }

        internal class PactMarkMarkedByPactConditionBuilder : ConditionDefinitionBuilder
        {
            private const string PactMarkMarkedByPactConditionName = "AHPactMarkMarkedByPactCondition";

            protected PactMarkMarkedByPactConditionBuilder(string name) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, DefinitionBuilder.CENamespaceGuid)
            {
                Definition.GuiPresentation.Title = "Spell/&AHPactMarkMarkedByPactConditionTitle";
                Definition.GuiPresentation.Description = "Spell/&AHPactMarkMarkedByPactConditionDescription";
            }

            public static ConditionDefinition CreateAndAddToDB(string name)
            {
                return new PactMarkMarkedByPactConditionBuilder(name).AddToDB();
            }

            public static readonly ConditionDefinition MarkedByPactCondition = CreateAndAddToDB(PactMarkMarkedByPactConditionName);
        }

        internal class PactMarkAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder
        {
            private const string PactMarkAdditionalDamageBuilderName = "AHPactMarkAdditionalDamage";

            protected PactMarkAdditionalDamageBuilder(string name) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, DefinitionBuilder.CENamespaceGuid)
            {
                Definition.GuiPresentation.Title = "Spell/&AHPactMarkAdditionalDamageTitle";
                Definition.GuiPresentation.Description = "Spell/&AHPactMarkAdditionalDamageDescription";
                Definition.SetAttackModeOnly(false);
                Definition.SetRequiredTargetCondition(PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition);
                Definition.SetNotificationTag("PactMarked");
            }

            public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name)
            {
                return new PactMarkAdditionalDamageBuilder(name).AddToDB();
            }

            public static readonly FeatureDefinitionAdditionalDamage PactMarkAdditionalDamage = CreateAndAddToDB(PactMarkAdditionalDamageBuilderName);
        }

        //internal class HellishRebukeSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        //{
        //    private const string HellishRebukeSpellName = "AHHellishRebukeSpell";
        //    private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(Settings.GUID, HellishRebukeSpellName).ToString();

        //    protected HellishRebukeSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.SacredFlame, name, guid)
        //    {
        //        Definition.GuiPresentation.Title = "Feat/&AHHellishRebukeSpellTitle";
        //        Definition.GuiPresentation.Description = "Feat/&AHHellishRebukeSpellDescription";
        //        Definition.SetSpellLevel(1);
        //        Definition.SetSomaticComponent(true);
        //        Definition.SetVerboseComponent(true);
        //        Definition.SetCastingTime(RuleDefinitions.ActivationTime.Reaction);

        //        //D10 damage
        //        var damageForm = new DamageForm
        //        {
        //            DiceNumber = 2,
        //            DamageType = "DamageFire",
        //            DieType = RuleDefinitions.DieType.D10
        //        };

        //        var damageEffectForm = new EffectForm
        //        {
        //            HasSavingThrow = true,
        //            FormType = EffectForm.EffectFormType.Damage,
        //            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage,
        //            SaveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn,
        //            DamageForm = damageForm
        //        };


        //        //Additional die per spell level
        //        var advancement = new EffectAdvancement();
        //        advancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);
        //        advancement.SetAdditionalDicePerIncrement(1);

        //        var effectDescription = Definition.EffectDescription;
        //        effectDescription.SetRangeParameter(12);
        //        effectDescription.EffectForms.Clear();
        //        effectDescription.SetTargetParameter(1);
        //        damageEffectForm.HasSavingThrow = true;
        //        effectDescription.SavingThrowAbility = "Dexterity";
        //        effectDescription.SetEffectAdvancement(advancement);
        //        effectDescription.EffectForms.Add(damageEffectForm);

        //        Definition.SetEffectDescription(effectDescription);
        //    }

        //    public static SpellDefinition CreateAndAddToDB(string name, string guid)
        //    {
        //        return new HellishRebukeSpellBuilder(name, guid).AddToDB();
        //    }

        //    public static SpellDefinition HellishRebukeSpell = CreateAndAddToDB(HellishRebukeSpellName, HellishRebukeSpellNameGuid);
        //}
    }
}
