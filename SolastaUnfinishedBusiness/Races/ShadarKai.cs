using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceShadarKaiBuilder
{
    internal static CharacterRaceDefinition SubraceShadarKai { get; } = BuildShadarKai();

    [NotNull]
    private static CharacterRaceDefinition BuildShadarKai()
    {
        var shadarKaiSpriteReference = Sprites.GetSprite("ShadarKai", Resources.ShadarKai, 1024, 512);

        var pointPoolAbilityScore = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolShadarKaiAbilityScoreIncrease")
            .SetGuiPresentation("Feature/&AbilityScoreIncreaseTitle", "Feature/&AttributeIncreaseAny1Description")
            .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 1)
            .AddToDB();

        var shadarKaiRacePresentation = Elf.RacePresentation.DeepCopy();

        var conditionTeleport = ConditionDefinitionBuilder
            .Create("ConditionShadarKaiTeleport")
            .SetGuiPresentation("PowerShadarKaiTeleport", Category.Feature,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .AddFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .SetConditionParticleReference(ConditionDefinitions.ConditionHopeless)
            .AddToDB();

        conditionTeleport.GuiPresentation.description = Gui.NoLocalization;

        var effectFormTeleport =
            EffectFormBuilder.ConditionForm(conditionTeleport, ConditionForm.ConditionOperation.Add, true, true);

        var powerTeleport = FeatureDefinitionPowerBuilder
            .Create("PowerShadarKaiTeleport")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.DimensionDoor)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .UseQuickAnimations()
                    .SetParticleEffectParameters(SpellDefinitions.DimensionDoor)
                    .Build())
            .AddToDB();

        powerTeleport.AddCustomSubFeatures(new MagicEffectFinishedByMeTeleport(powerTeleport, effectFormTeleport));

        var necroticResistance = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityShadarKai")
            .SetGuiPresentation(Category.Feature)
            .SetDamageType(DamageTypeNecrotic)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
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
                powerTeleport,
                necroticResistance)
            .AddToDB();

        raceShadarKai.subRaces.Clear();
        Elf.SubRaces.Add(raceShadarKai);

        return raceShadarKai;
    }

    private sealed class MagicEffectFinishedByMeTeleport(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerTeleport,
        EffectForm effectFormTeleport) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerTeleport;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel) >= 3)
            {
                effectDescription.EffectForms.Add(effectFormTeleport);
            }

            return effectDescription;
        }
    }
}
