using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Models.SpellsContext;

namespace SolastaCommunityExpansion.Spells;

internal static class AceHighSpells
{
    private static SpellDefinition _pactMarkSpell;
    private static SpellDefinition _hellishRebuke;
    internal static SpellDefinition PactMarkSpell => _pactMarkSpell ??= PactMarkSpellBuilder.CreateAndAddToDB();
    internal static SpellDefinition HellishRebukeSpell => _hellishRebuke ??= BuildHellishRebuke();

    internal static void Register()
    {
        RegisterSpell(PactMarkSpell, 1, WarlockSpellList);
        RegisterSpell(HellishRebukeSpell, 1, WarlockSpellList);
    }

    private static SpellDefinition BuildHellishRebuke()
    {
        return SpellDefinitionBuilder
            .Create("AHHellishRebuke", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("HellishRebuke", Resources.HellishRebuke,
                    128, 128))
            .SetSpellLevel(1)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetCustomSubFeatures(CustomReactionsContext.AlwaysReactToDamaged)
            .SetCastingTime(RuleDefinitions.ActivationTime.Reaction)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ScorchingRay)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Individuals
                )
                .SetSavingThrowData(
                    true,
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Charisma
                )
                .SetEffectAdvancement(
                    RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                    additionalDicePerIncrement: 1
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .SetDamageForm(
                        false,
                        RuleDefinitions.DieType.D10,
                        RuleDefinitions.DamageTypeFire,
                        0,
                        RuleDefinitions.DieType.D10,
                        2
                    )
                    .Build()
                )
                .Build()
            )
            .AddToDB();
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
