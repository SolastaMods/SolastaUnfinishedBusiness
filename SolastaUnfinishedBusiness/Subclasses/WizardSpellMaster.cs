using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardSpellMaster : AbstractSubclass
{
    internal const string PowerSpellMasterBonusRecoveryName = "PowerSpellMasterBonusRecovery";

    internal WizardSpellMaster()
    {
        // level 02

        var magicAffinitySpellMasterKnowledge = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterKnowledge")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 1, AdvantageType.None,
                PreparedSpellsModifier.None)
            .AddToDB();

        var magicAffinitySpellMasterPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, AdvantageType.None,
                PreparedSpellsModifier.ProficiencyBonus)
            .AddToDB();

        var magicAffinitySpellMasterRecovery = FeatureDefinitionPowerBuilder
            .Create(PowerSpellMasterBonusRecoveryName)
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Feature, PowerWizardArcaneRecovery)
            .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSpellForm(9)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .SetParticleEffectParameters(PowerWizardArcaneRecovery)
                    .Build())
            .AddToDB();

        // level 06

        var magicAffinitySpellMasterScriber = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterScriber")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, AdvantageType.Advantage,
                PreparedSpellsModifier.None)
            .AddToDB();

        var pointPoolSpellMasterBonusCantrips = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolSpellMasterBonusCantrips")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
            .OnlyUniqueChoices()
            .AddToDB();

        // level 10

        var magicAffinitySpellMasterExtraPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterExtraPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, AdvantageType.None,
                PreparedSpellsModifier.SpellcastingAbilityBonus)
            .AddToDB();

        // level 14

        var savingThrowAffinitySpellMasterSpellResistance = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinitySpellMasterSpellResistance")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(
                CharacterSavingThrowAffinity.Advantage, true,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Charisma)
            .AddToDB();

        _ = RestActivityDefinitionBuilder
            .Create("RestActivitySpellMasterArcaneDepth")
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Feature, PowerWizardArcaneRecovery)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                magicAffinitySpellMasterRecovery.Name)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardSpellMaster")
            .SetGuiPresentation(Category.Subclass, DomainInsight)
            .AddFeaturesAtLevel(2,
                magicAffinitySpellMasterKnowledge,
                magicAffinitySpellMasterPrepared,
                magicAffinitySpellMasterRecovery)
            .AddFeaturesAtLevel(6,
                magicAffinitySpellMasterScriber,
                pointPoolSpellMasterBonusCantrips)
            .AddFeaturesAtLevel(10,
                magicAffinitySpellMasterExtraPrepared)
            .AddFeaturesAtLevel(14,
                savingThrowAffinitySpellMasterSpellResistance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
