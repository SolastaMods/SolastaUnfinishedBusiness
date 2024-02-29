using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardSpellMaster : AbstractSubclass
{
    private const string Name = "SpellMaster";

    internal const string PowerSpellMasterBonusRecoveryName = $"Power{Name}BonusRecovery";

    public WizardSpellMaster()
    {
        // LEVEL 02

        var magicAffinitySpellMasterKnowledge = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Knowledge")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(
                1f, 1f, 1, AdvantageType.None, PreparedSpellsModifier.None)
            .AddToDB();

        var magicAffinitySpellMasterPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Prepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, AdvantageType.None,
                PreparedSpellsModifier.ProficiencyBonus)
            .AddToDB();

        var powerBonusRecovery = FeatureDefinitionPowerBuilder
            .Create(PowerSpellMasterBonusRecoveryName)
            .SetGuiPresentation($"MagicAffinity{Name}Recovery", Category.Feature, PowerWizardArcaneRecovery)
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

        // LEVEL 06

        var magicAffinitySpellMasterScriber = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Scriber")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, AdvantageType.Advantage,
                PreparedSpellsModifier.None)
            .AddToDB();

        var pointPoolSpellMasterBonusCantrips = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}BonusCantrips")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
            .OnlyUniqueChoices()
            .AddToDB();

        // LEVEL 10

        var magicAffinitySpellMasterExtraPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExtraPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(
                1f, 1f, 0, AdvantageType.None, PreparedSpellsModifier.SpellcastingAbilityBonus)
            .AddToDB();

        // LEVEL 14

        var savingThrowAffinitySpellMasterSpellResistance = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}SpellResistance")
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
            .Create($"RestActivity{Name}ArcaneDepth")
            .SetGuiPresentation($"MagicAffinity{Name}Recovery", Category.Feature, PowerWizardArcaneRecovery)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                PowerSpellMasterBonusRecoveryName)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Wizard{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.WizardSpellMaster, 256))
            .AddFeaturesAtLevel(2,
                magicAffinitySpellMasterKnowledge,
                magicAffinitySpellMasterPrepared,
                powerBonusRecovery)
            .AddFeaturesAtLevel(6,
                magicAffinitySpellMasterScriber,
                pointPoolSpellMasterBonusCantrips)
            .AddFeaturesAtLevel(10,
                magicAffinitySpellMasterExtraPrepared)
            .AddFeaturesAtLevel(14,
                savingThrowAffinitySpellMasterSpellResistance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
