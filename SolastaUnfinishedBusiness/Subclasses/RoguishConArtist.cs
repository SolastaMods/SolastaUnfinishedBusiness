using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishConArtist : AbstractSubclass
{
    private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal RoguishConArtist()
    {
        // Make Con Artist subclass
        var abilityAffinity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityConArtist", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, RuleDefinitions.DieType.D8, 0,
                (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                (AttributeDefinitions.Charisma, SkillDefinitions.Performance))
            .AddToDB();

        var spellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellConArtist", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .AddRestrictedSchools(SchoolConjuration, SchoolTransmutation, SchoolEnchantment, SchoolIllusion)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetReplacedSpells(SharedSpellsContext.OneThirdCasterReplacedSpells)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetKnownSpells(4, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

        var feintBuilder = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 12,
                RuleDefinitions.TargetType.Individuals, 1, 0)
            .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetSavingThrowData(
                true, false, AttributeDefinitions.Wisdom, true,
                RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Charisma,
                15);

        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionConArtistFeint")
            .SetGuiPresentation(Category.Feature,
                ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference)
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

        var feint = FeatureDefinitionPowerBuilder
            .Create("PowerConArtistFeint", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .Configure(
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Charisma, feintBuilder.Build(), false /* unique instance */)
            .AddToDB();

        var proficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyConArtistMentalSavingThrows", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        // add subclass to db and add subclass to rogue class
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishConArtist", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(abilityAffinity, 3)
            .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
            .AddFeatureAtLevel(feint, 9)
            .AddFeatureAtLevel(DcIncreaseAffinity, 13)
            .AddFeatureAtLevel(proficiency, 17).AddToDB();
    }

    private static FeatureDefinitionMagicAffinity DcIncreaseAffinity => _dcIncreaseAffinity ??=
        FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityConArtistDC", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(GetSpellDcPresentation().Build())
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None,
                Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc,
                RuleDefinitions.SpellParamsModifierType.FlatValue, false, false, false)
            .AddToDB();

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    [NotNull]
    private static GuiPresentationBuilder GetSpellDcPresentation()
    {
        return new GuiPresentationBuilder(
            "Feature/&MagicAffinityConArtistDCTitle",
            Gui.Format("Feature/&MagicAffinityConArtistDCDescription",
                Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc.ToString()));
    }

    internal static void UpdateSpellDcBoost()
    {
        if (!DcIncreaseAffinity)
        {
            return;
        }

        DcIncreaseAffinity.saveDCModifier = Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc;
        DcIncreaseAffinity.guiPresentation = GetSpellDcPresentation().Build();
    }
}
