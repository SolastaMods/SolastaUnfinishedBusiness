using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceWendigoBuilder
{
    private const string Name = "Wendigo";

    internal static CharacterRaceDefinition RaceWendigo { get; } = BuildWendigo();

    [NotNull]
    private static CharacterRaceDefinition BuildWendigo()
    {
        var attributeModifierWendigoStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}StrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var attributeModifierWendigoDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        var equipmentAffinityWendigoPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(EquipmentAffinityFeatHauler, $"EquipmentAffinity{Name}PowerfulBuild")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyWendigoLanguages = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Languages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Goblin")
            .AddToDB();

        var proficiencyWendigoStalker = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Stalker")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Stealth)
            .AddToDB();

        var additionalDamageWendigoSuckerPunch = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{Name}SuckerPunch")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new CustomAdditionalDamageSuckerPunch(
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create("AdditionalDamageSuckerPunch")
                        .SetGuiPresentationNoContent(true)
                        .SetNotificationTag("SuckerPunch")
                        .SetDamageDice(DieType.D6, 2)
                        .AddToDB()
                ))
            .AddToDB();

        var featureWendigoNaturalLunger = FeatureDefinitionBuilder
            .Create("FeatureWendigoNaturalLunger")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new IncreaseWeaponReach(1, ValidatorsWeapon.IsMelee, Lunger.Name))
            .AddToDB();

        var racePresentation = Tiefling.RacePresentation.DeepCopy();

        racePresentation.preferedSkinColors = new RangedInt(28, 47);

        var raceWendigo = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Wendigo, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(racePresentation)
            .SetBaseHeight(96)
            .SetBaseWeight(260)
            .SetMinimalAge(20)
            .SetMaximalAge(200)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                attributeModifierWendigoStrengthAbilityScoreIncrease,
                attributeModifierWendigoDexterityAbilityScoreIncrease,
                FeatureDefinitionMoveModes.MoveModeMove6,
                equipmentAffinityWendigoPowerfulBuild,
                proficiencyWendigoStalker,
                additionalDamageWendigoSuckerPunch,
                featureWendigoNaturalLunger,
                proficiencyWendigoLanguages)
            .AddToDB();

        RacesContext.RaceScaleMap[raceWendigo] = 7.8f / 6.4f;

        return raceWendigo;
    }

    private class CustomAdditionalDamageSuckerPunch(IAdditionalDamageProvider provider)
        : CustomAdditionalDamage(provider)
    {
        internal override bool IsValid(GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            return battleManager.Battle.ActiveContender == attacker &&
                   battleManager.Battle.CurrentRound == 1 &&
                   battleManager.Battle.InitiativeSortedContenders.IndexOf(attacker)
                   < battleManager.Battle.InitiativeSortedContenders.IndexOf(defender);
        }
    }
}
