using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class ConArtist : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("fdf8dc11-5006-489e-951c-92a8d72ca4c0");
        private readonly CharacterSubclassDefinition Subclass;

        #region DcIncreaseAffinity
        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity => _dcIncreaseAffinity ??= FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityRoguishConArtistDC", SubclassNamespace)
            .SetGuiPresentation(GetSpellDCPresentation().Build())
            .SetCastingModifiers(0, Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc, false, false, false).AddToDB();
        #endregion

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
            CharacterSubclassDefinitionBuilder conArtist = CharacterSubclassDefinitionBuilder
                .Create("RoguishConArtist", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass, DomainInsight.GuiPresentation.SpriteReference);

            GuiPresentationBuilder conAffinity = new GuiPresentationBuilder(
                "Subclass/&AbilityAffinityRogueConArtistTitle",
                "Subclass/&AbilityAffinityRogueConArtistDescription");
            FeatureDefinitionAbilityCheckAffinity abilityAffinity = BuildAbilityAffinity(
                new List<(string, string)> {
                    (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Performance)
                },
                RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, "AbilityAffinityRogueConArtist", conAffinity.Build());
            conArtist.AddFeatureAtLevel(abilityAffinity, 3);

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

            conArtist.AddFeatureAtLevel(spellCasting.AddToDB(), 3);

            EffectDescriptionBuilder feintBuilder = new EffectDescriptionBuilder();
            feintBuilder.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals, 1, 0, ActionDefinitions.ItemSelectionType.None);
            feintBuilder.SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            feintBuilder.SetSavingThrowData(true, false, AttributeDefinitions.Wisdom, true, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Charisma,
                15, false, new List<SaveAffinityBySenseDescription>());
            GuiPresentationBuilder feintGuiCondition = new GuiPresentationBuilder(
                "Subclass/&RoguishConArtistFeintConditionTitle",
                "Subclass/&RoguishConArtistFeintConditionDescription");
            feintGuiCondition.SetSpriteReference(ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference);
            feintBuilder.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetConditionForm(new AdvantageBuilder("RogueConArtistFeintCondition",
                GuidHelper.Create(SubclassNamespace, "RogueConArtistFeintCondition").ToString(),
                ConditionDefinitions.ConditionTrueStrike, feintGuiCondition.Build()).AddToDB(), ConditionForm.ConditionOperation.Add,
                false, false, new List<ConditionDefinition>()).Build());
            //feintBuilder.AddEffectForm(new EffectFormBuilder().SetConditionForm(DatabaseHelper.ConditionDefinitions.))
            FeatureDefinitionPower feint = FeatureDefinitionPowerBuilder
                .Create("RoguishConArtistFeint", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .Configure(
                    0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
                    false, false, AttributeDefinitions.Charisma, feintBuilder.Build(), false /* unique instance */)
                .AddToDB();
            conArtist.AddFeatureAtLevel(feint, 9);
            conArtist.AddFeatureAtLevel(DcIncreaseAffinity, 13);

            FeatureDefinitionProficiency proficiency = FeatureDefinitionProficiencyBuilder
                .Create("RoguishConArtistMentalSavingThrows", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom)
                .AddToDB();

            conArtist.AddFeatureAtLevel(proficiency, 17);

            // add subclass to db and add subclass to rogue class
            Subclass = conArtist.AddToDB();
        }

        private sealed class AdvantageBuilder : ConditionDefinitionBuilder
        {
            public AdvantageBuilder(string name, string guid, ConditionDefinition original, GuiPresentation guiPresentation) : base(original, name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);
                Definition.SpecialInterruptions.SetRange(RuleDefinitions.ConditionInterruption.Attacked);
                Definition.SetAdditionalDamageWhenHit(true);
                Definition.SetAdditionalDamageDieType(RuleDefinitions.DieType.D8);
                Definition.SetAdditionalDamageDieNumber(3);
                Definition.SetAdditionalDamageQuantity(ConditionDefinition.DamageQuantity.Dice);
            }
        }

        private static GuiPresentationBuilder GetSpellDCPresentation()
        {
            return new GuiPresentationBuilder("Subclass/&MagicAffinityRoguishConArtistDCTitle", "Subclass/&MagicAffinityRoguishConArtistDC" + Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc + "Description");
        }

        private static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(IEnumerable<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs,
            RuleDefinitions.CharacterAbilityCheckAffinity affinityType, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionAbilityCheckAffinityBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .BuildAndSetAffinityGroups(affinityType, RuleDefinitions.DieType.D8, 0, abilityProficiencyPairs)
                .AddToDB();
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
