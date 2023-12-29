using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceTieflingBuilder
{
    internal static CharacterRaceDefinition RaceTiefling { get; } = BuildTiefling();

    private static bool IsFlightValid(RulesetCharacter character)
    {
        return !character.IsWearingMediumArmor() && !character.IsWearingHeavyArmor();
    }

    [NotNull]
    private static CharacterRaceDefinition BuildTiefling()
    {
        var contentPack = ServiceRepository.GetService<IGamingPlatformService>()
            .IsContentPackAvailable(GamingPlatformDefinitions.ContentPack.PalaceOfIce)
            ? CeContentPackContext.CeContentPack
            : GamingPlatformDefinitions.ContentPack.PalaceOfIce;

        #region subraces

        //
        // Devil's Tongue
        //

        var attributeModifierTieflingIntelligenceAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingIntelligenceAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 1)
            .AddToDB();

        var castSpellTieflingDevilTongue = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingDevilTongue")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create("SpellListTieflingDevilTongue")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                    .SetSpellsAtLevel(1, SpellDefinitions.CharmPerson)
                    .SetSpellsAtLevel(2, SpellDefinitions.CalmEmotions)
                    .FinalizeSpells(true, 2)
                    .AddToDB())
            .AddToDB();

        var raceTieflingDevilTongue = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingDevilTongue")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingIntelligenceAbilityScoreIncrease,
                castSpellTieflingDevilTongue)
            .AddToDB();

        raceTieflingDevilTongue.contentPack = contentPack;

        //
        // Feral
        //

        var attributeModifierTieflingDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingDexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        // Hellfire

        var castSpellTieflingFeral = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingFeral")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create("SpellListTieflingFeral")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ProduceFlame)
                    .SetSpellsAtLevel(1, SpellDefinitions.BurningHands)
                    .FinalizeSpells(true, 1)
                    .AddToDB())
            .AddToDB();

        // Demonic Wings

        var sprite = Sprites.GetSprite("PowerDragonWings", Resources.PowerDragonWings, 256, 128);

        var conditionTieflingFeralWings = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, "ConditionTieflingFeralWings")
            .AddToDB();

        conditionTieflingFeralWings.AddCustomSubFeatures(new CheckTieflingFeralFlying(conditionTieflingFeralWings));

        var powerDemonicWingsSprout = FeatureDefinitionPowerBuilder
            .Create("PowerTieflingFeralDemonicWingsSprout")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionTieflingFeralWings))
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(IsFlightValid,
                ValidatorsCharacter.HasNoneOfConditions(ConditionFlyingAdaptive, conditionTieflingFeralWings.Name)))
            .AddToDB();

        var powerDemonicWingsDismiss = FeatureDefinitionPowerBuilder
            .Create("PowerTieflingFeralDemonicWingsDismiss")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTieflingFeralWings,
                            ConditionForm.ConditionOperation.Remove),
                        //Leaving this for compatibility
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlyingAdaptive,
                            ConditionForm.ConditionOperation.Remove))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionFlyingAdaptive,
                    conditionTieflingFeralWings.Name)))
            .AddToDB();

        var featureSetDemonicWings = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTieflingFeralDemonicWings")
            .SetGuiPresentation("PowerTieflingFeralDemonicWingsSprout", Category.Feature)
            .AddFeatureSet(powerDemonicWingsSprout, powerDemonicWingsDismiss)
            .AddToDB();

        var raceTieflingFeral = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingFeral")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingDexterityAbilityScoreIncrease,
                castSpellTieflingFeral,
                featureSetDemonicWings)
            .AddToDB();

        raceTieflingDevilTongue.contentPack = contentPack;

        //
        // Mephistopheles
        //

        var castSpellTieflingMephistopheles = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingMephistopheles")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create("SpellListTieflingMephistopheles")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.FireBolt)
                    .SetSpellsAtLevel(1, SpellDefinitions.BurningHands)
                    .SetSpellsAtLevel(2, SpellDefinitions.FlameBlade)
                    .FinalizeSpells(true, 2)
                    .AddToDB())
            .AddToDB();

        var raceTieflingMephistopheles = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingMephistopheles")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingDexterityAbilityScoreIncrease,
                castSpellTieflingMephistopheles)
            .AddToDB();

        raceTieflingMephistopheles.contentPack = contentPack;

        //
        // Zariel
        //

        var attributeModifierTieflingStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTieflingStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var castSpellTieflingZariel = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, "CastSpellTieflingZariel")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create("SpellListTieflingZariel")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellsContext.SunlightBlade)
                    .SetSpellsAtLevel(1, SpellsContext.SearingSmite)
                    .SetSpellsAtLevel(2, SpellDefinitions.BrandingSmite)
                    .FinalizeSpells(true, 2)
                    .AddToDB())
            .AddToDB();

        var raceTieflingZariel = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTieflingZariel")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
                attributeModifierTieflingStrengthAbilityScoreIncrease,
                castSpellTieflingZariel)
            .AddToDB();

        raceTieflingZariel.contentPack = contentPack;

        #endregion

        castSpellTieflingFeral.slotsPerLevels.Clear();

        for (var level = 1; level <= 20; level++)
        {
            castSpellTieflingFeral.slotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = [level >= 3 ? 1 : 0]
            });
        }

        var raceTiefling = CharacterRaceDefinitionBuilder
            .Create(Tiefling, "RaceTiefling")
            .SetOrUpdateGuiPresentation("Tiefling", Category.Race)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                FeatureSetTieflingHellishResistance,
                AttributeModifierTieflingAbilityScoreIncreaseCha,
                ProficiencyTieflingStaticLanguages)
            .AddToDB();

        raceTiefling.contentPack = contentPack;
        raceTiefling.subRaces.SetRange(
            raceTieflingDevilTongue,
            raceTieflingFeral,
            raceTieflingMephistopheles,
            raceTieflingZariel);

        return raceTiefling;
    }

    private sealed class CheckTieflingFeralFlying : IOnItemEquipped
    {
        private readonly ConditionDefinition _condition;

        public CheckTieflingFeralFlying(ConditionDefinition condition)
        {
            _condition = condition;
        }

        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (IsFlightValid(hero))
            {
                return;
            }

            var rulesetCondition = hero.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _condition);

            if (rulesetCondition != null)
            {
                hero.RemoveCondition(rulesetCondition);
            }
        }
    }
}
