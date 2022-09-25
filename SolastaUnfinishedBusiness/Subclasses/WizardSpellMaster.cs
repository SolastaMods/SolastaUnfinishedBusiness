using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardSpellMaster : AbstractSubclass
{
    internal const string PowerSpellMasterBonusRecoveryName = "PowerSpellMasterBonusRecovery";

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal WizardSpellMaster()
    {
        var prepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.ProficiencyBonus)
            .AddToDB();

        var extraPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterExtraPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.SpellcastingAbilityBonus)
            .AddToDB();

        var extraKnown = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterKnowledge")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 1, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var knowledgeAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterScriber")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, RuleDefinitions.AdvantageType.Advantage,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var bonusCantrips = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolSpellMasterBonusCantrips")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
            .OnlyUniqueChoices()
            .AddToDB();

        var spellResistance = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinitySpellMasterSpellResistance")
            .SetGuiPresentation(Category.Feature)
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

        BonusRecovery = FeatureDefinitionPowerBuilder
            .Create(PowerSpellMasterBonusRecoveryName)
            .SetGuiPresentationNoContent(true)
            .Configure(1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Rest,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                bonusRecoveryEffectDescription)
            .AddToDB();

        UpdateBonusRecovery();

        // Make Spell Master subclass
        var spellMaster = CharacterSubclassDefinitionBuilder
            .Create("WizardSpellMaster")
            .SetGuiPresentation(Category.Subclass,
                DomainInsight.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(prepared, 2)
            .AddFeatureAtLevel(extraKnown, 2)
            .AddFeatureAtLevel(BonusRecovery, 2)
            .AddFeatureAtLevel(knowledgeAffinity, 6)
            .AddFeatureAtLevel(bonusCantrips, 6)
            .AddFeatureAtLevel(extraPrepared, 10)
            .AddFeatureAtLevel(spellResistance, 14)
            .AddToDB();

        _ = RestActivityDefinitionBuilder
            .Create("SpellMasterArcaneDepth")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                BonusRecovery.Name)
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Feature,
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
                Category.Feature, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            BonusRecovery.costPerUse = 0;
            BonusRecovery.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        }
        else
        {
            BonusRecovery.guiPresentation = GuiPresentationBuilder.Build("MagicAffinitySpellMasterRecovery",
                Category.Feature, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            BonusRecovery.costPerUse = 1;
            BonusRecovery.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        }
    }
}
