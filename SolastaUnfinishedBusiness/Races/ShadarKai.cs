using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceShadarKaiBuilder
{
    internal static CharacterRaceDefinition SubraceShadarKai { get; } = BuildShadarKai();

    [NotNull]
    private static CharacterRaceDefinition BuildShadarKai()
    {
        var shadarKaiSpriteReference = Sprites.GetSprite("ShadarKai", Resources.Darkelf, 1024, 512);

        var pointPoolAbilityScore = FeatureDefinitionPointPoolBuilder
            .Create($"PointPoolShadarKaiAbilityScore")
            .SetGuiPresentation("Feature/&AbilityScoreIncreaseTitle", "Feature/&AttributeIncreaseAny1Description")
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 1)
            .AddToDB();

        var shadarKaiRacePresentation = Elf.RacePresentation.DeepCopy();

        var powerTeleport = FeatureDefinitionPowerBuilder
            .Create("PowerShadarKaiTeleport")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.DimensionDoor)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();
        
        shadarKaiRacePresentation.femaleNameOptions = ElfHigh.RacePresentation.FemaleNameOptions;
        shadarKaiRacePresentation.maleNameOptions = ElfHigh.RacePresentation.MaleNameOptions;
        shadarKaiRacePresentation.surNameOptions = []; // names are added from names.txt resources
        shadarKaiRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        shadarKaiRacePresentation.preferedHairColors = new RangedInt(48, 53);

        var raceShadarKai = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceShadarKai")
            .SetGuiPresentation(Category.Race, shadarKaiSpriteReference)
            .SetRacePresentation(shadarKaiRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionFeatureSets.FeatureSetElfHighLanguages,
                FeatureDefinitionProficiencys.ProficiencyElfWeaponTraining,
                pointPoolAbilityScore,
                powerTeleport)
            .AddToDB();

        raceShadarKai.subRaces.Clear();
        Elf.SubRaces.Add(raceShadarKai);

        return raceShadarKai;
    }
}
