using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class ConArtist : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("fdf8dc11-5006-489e-951c-92a8d72ca4c0");
        private readonly CharacterSubclassDefinition Subclass;

        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity => _dcIncreaseAffinity ??= FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityRoguishConArtistDC", SubclassNamespace)
            .SetGuiPresentation(GetSpellDCPresentation().Build())
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.FlatValue, Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc, false, false, false)
            .AddToDB();

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal ConArtist()
        {
            // Make Con Artist subclass
            FeatureDefinitionAbilityCheckAffinity abilityAffinity = FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("AbilityAffinityRogueConArtist", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .BuildAndSetAffinityGroups(
                    RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, RuleDefinitions.DieType.D8, 0,
                    (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Performance))
                .AddToDB();

            FeatureDefinitionCastSpellBuilder spellCasting = FeatureDefinitionCastSpellBuilder
                .Create("CastSpellConArtist", SubclassNamespace)
                .SetGuiPresentation("RoguishConArtistSpellcasting", Category.Subclass)
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
                .SetSpellCastingAbility(AttributeDefinitions.Charisma)
                .SetSpellList(SpellListDefinitions.SpellListWizard)
                .AddRestrictedSchools(SchoolConjuration, SchoolTransmutation, SchoolEnchantment, SchoolIllusion)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
                .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
                .SetKnownSpells(4, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
                .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

            EffectDescriptionBuilder feintBuilder = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 12, 
                    RuleDefinitions.TargetType.Individuals, 1, 0, ActionDefinitions.ItemSelectionType.None)
                .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetSavingThrowData(
                    true, false, AttributeDefinitions.Wisdom, true, 
                    RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Charisma,
                    15, false);

            var condition = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionTrueStrike, "RogueConArtistFeintCondition", SubclassNamespace)
                .SetGuiPresentation("RoguishConArtistFeintCondition", Category.Subclass, ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference)
                .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacked)
                .SetAdditionalDamageData(RuleDefinitions.DieType.D8, 3, ConditionDefinition.DamageQuantity.Dice, true)
                .AddToDB();

            feintBuilder.AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .CreatedByCharacter()
                    .SetConditionForm(
                        condition, ConditionForm.ConditionOperation.Add, false, false)
                    .Build());

            FeatureDefinitionPower feint = FeatureDefinitionPowerBuilder
                .Create("RoguishConArtistFeint", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .Configure(
                    0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
                    false, false, AttributeDefinitions.Charisma, feintBuilder.Build(), false /* unique instance */)
                .AddToDB();

            FeatureDefinitionProficiency proficiency = FeatureDefinitionProficiencyBuilder
                .Create("RoguishConArtistMentalSavingThrows", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom)
                .AddToDB();

            // add subclass to db and add subclass to rogue class
            Subclass = CharacterSubclassDefinitionBuilder
                .Create("RoguishConArtist", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(abilityAffinity, 3)
                .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
                .AddFeatureAtLevel(feint, 9)
                .AddFeatureAtLevel(DcIncreaseAffinity, 13)
                .AddFeatureAtLevel(proficiency, 17).AddToDB();
        }

        private static GuiPresentationBuilder GetSpellDCPresentation()
        {
            return new GuiPresentationBuilder("Subclass/&MagicAffinityRoguishConArtistDCTitle", "Subclass/&MagicAffinityRoguishConArtistDC" + Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc + "Description");
        }

        public static void UpdateSpellDCBoost()
        {
            if (DcIncreaseAffinity)
            {
                DcIncreaseAffinity.SetSaveDCModifier(Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc);
                DcIncreaseAffinity.SetGuiPresentation(GetSpellDCPresentation().Build());
            }
        }
    }
}
