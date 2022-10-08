using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishConArtist : AbstractSubclass
{
    internal const string Name = "RoguishConArtist";

    internal RoguishConArtist()
    {
        var abilityCheckAffinityConArtist = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityConArtist")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.Advantage, DieType.D8, 0,
                (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                (AttributeDefinitions.Charisma, SkillDefinitions.Performance))
            .AddToDB();

        var castSpellConArtist = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellConArtist")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .AddRestrictedSchools(
                SchoolEnchantment,
                SchoolOfMagicDefinitions.SchoolConjuration,
                SchoolOfMagicDefinitions.SchoolTransmutation,
                SchoolOfMagicDefinitions.SchoolIllusion)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        var conditionConArtistFeint = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionConArtistFeint")
            .SetGuiPresentation(Category.Feature,
                ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .SetAdditionalDamageData(DieType.D8, 3, ConditionDefinition.DamageQuantity.Dice, true)
            .AddToDB();

        var powerConArtistFeint = FeatureDefinitionPowerBuilder
            .Create("PowerConArtistFeint")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                0,
                UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                0,
                RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Charisma,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Enemy, RangeType.Distance, 12,
                        TargetType.Individuals, 1, 0)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfTurn)
                    .SetSavingThrowData(
                        true, false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Charisma,
                        15)
                    .AddEffectForm(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(
                                conditionConArtistFeint, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var magicAffinityConArtistDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityConArtistDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 3)
            .AddToDB();

        var proficiencyConArtistMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyConArtistMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                abilityCheckAffinityConArtist,
                castSpellConArtist)
            .AddFeaturesAtLevel(9,
                powerConArtistFeint)
            .AddFeaturesAtLevel(13,
                magicAffinityConArtistDc)
            .AddFeaturesAtLevel(17,
                proficiencyConArtistMentalSavingThrows)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
}
