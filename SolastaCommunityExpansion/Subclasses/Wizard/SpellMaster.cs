using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class SpellMaster : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("9f322734-1498-4f65-ace5-e6072b1d99be");
    private readonly CharacterSubclassDefinition Subclass;

    internal SpellMaster()
    {
        var prepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterPrepared", SubclassNamespace)
            .SetGuiPresentation("TraditionSpellMasterPrepared", Category.Subclass)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.ProficiencyBonus)
            .AddToDB();

        var extraPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterExtraPrepared", SubclassNamespace)
            .SetGuiPresentation("TraditionSpellMasterExtraPrepared", Category.Subclass)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.SpellcastingAbilityBonus)
            .AddToDB();

        var extraKnown = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterKnowledge", SubclassNamespace)
            .SetGuiPresentation("MagicAffinitySpellMasterBonusScribing", Category.Subclass)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 1, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var knowledgeAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterScriber", SubclassNamespace)
            .SetGuiPresentation("MagicAffinitySpellMasterScribing", Category.Subclass)
            .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, RuleDefinitions.AdvantageType.Advantage,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var bonusCantrips = FeatureDefinitionPointPoolBuilder
            .Create("TraditionSpellMasterBonusCantrips", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass)
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
            .OnlyUniqueChoices()
            .AddToDB();

        var spellResistance = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("TraditionSpellMasterSpellResistance", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass)
            .SetAffinities(
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Charisma
            )
            .AddToDB();

        var effectParticleParameters = new EffectParticleParameters();

        effectParticleParameters.Copy(PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);

        var bonusRecoveryEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 0, 0, 0, 0)
            .SetCreatedByCharacter()
            .AddEffectForm(EffectFormBuilder.Create().SetSpellForm(9).Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(effectParticleParameters)
            .Build();

        BonusRecovery = FeatureDefinitionPowerBuilder.Create("PowerSpellMasterBonusRecovery", SubclassNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Rest, 1,
                RuleDefinitions.RechargeRate.LongRest, false, false, AttributeDefinitions.Intelligence,
                bonusRecoveryEffectDescription, false)
            .AddToDB();

        UpdateBonusRecovery();

        // Make Spell Master subclass
        var spellMaster = CharacterSubclassDefinitionBuilder
            .Create("SpellMaster", SubclassNamespace)
            .SetGuiPresentation("TraditionSpellMaster", Category.Subclass,
                DomainInsight.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(prepared, 2)
            .AddFeatureAtLevel(extraKnown, 2)
            .AddFeatureAtLevel(BonusRecovery, 2)
            .AddFeatureAtLevel(knowledgeAffinity, 6)
            .AddFeatureAtLevel(bonusCantrips, 6)
            .AddFeatureAtLevel(extraPrepared, 10)
            .AddFeatureAtLevel(spellResistance, 14)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("ArcaneDepth", SubclassNamespace)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", BonusRecovery.Name)
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Subclass,
                PowerWizardArcaneRecovery.GuiPresentation.SpriteReference)
            .AddToDB();

        Subclass = spellMaster;
    }

    private static FeatureDefinitionPower BonusRecovery { get; set; }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    internal static void UpdateBonusRecovery()
    {
        if (Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster)
        {
            BonusRecovery.guiPresentation = GuiPresentationBuilder.Build("MagicAffinitySpellMasterRecoveryUnlimited",
                Category.Subclass, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            BonusRecovery.costPerUse = 0;
            BonusRecovery.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        }
        else
        {
            BonusRecovery.guiPresentation = GuiPresentationBuilder.Build("MagicAffinitySpellMasterRecovery",
                Category.Subclass, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            BonusRecovery.costPerUse = 1;
            BonusRecovery.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        }
    }
}
