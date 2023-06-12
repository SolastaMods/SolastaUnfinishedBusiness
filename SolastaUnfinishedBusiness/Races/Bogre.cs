using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;


namespace SolastaUnfinishedBusiness.Races;

internal static class RaceBogreBuilder
{
    private const string Name = "Bogre";

    internal static CharacterRaceDefinition RaceBogre { get; } = BuildBogre();

    [NotNull]
    private static CharacterRaceDefinition BuildBogre()
    {
        var attributeModifierBogreStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}StrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var attributeModifierBogreDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        var equipmentAffinityBogrePowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(EquipmentAffinityFeatHauler, $"EquipmentAffinity{Name}PowerfulBuild")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyBogreLanguages = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Languages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Goblin")
            .AddToDB();

        var proficiencyBogreStalker = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Stalker")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Stealth)
            .AddToDB();
        var additionalDamageBogreSuckerPunch = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}SuckerPunch")
            .SetGuiPresentation(Category.Feature)
            .SetDamageDice(DieType.D6, 2)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetHasLowerInitiativeOnFirstRound)
            .SetNotificationTag("SuckerPunch")
            .AddToDB();

        var featureBogreNaturalLunger = FeatureDefinitionBuilder
                .Create("FeatureBogreNaturalLunger")
                .SetGuiPresentation(Category.Feature)
                .SetCustomSubFeatures(new ModifyWeaponAttackModeBogreNaturalLunger())
                .AddToDB();
        var racePresentation = Tiefling.RacePresentation.DeepCopy();
        racePresentation.preferedSkinColors = new RangedInt(40, 47);
        racePresentation.preferedHairColors = new RangedInt(35, 41);
        
        var raceBogre = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Bogre, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(racePresentation)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(80)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                attributeModifierBogreStrengthAbilityScoreIncrease,
                attributeModifierBogreDexterityAbilityScoreIncrease,
                FeatureDefinitionMoveModes.MoveModeMove6,
                equipmentAffinityBogrePowerfulBuild,
                proficiencyBogreStalker,
                additionalDamageBogreSuckerPunch,
                featureBogreNaturalLunger,
                proficiencyBogreLanguages)
            .AddToDB();


        RacesContext.RaceScaleMap[raceBogre] = 7.8f / 6.4f;
        return raceBogre;
    }

    private class ModifyWeaponAttackModeBogreNaturalLunger :
        IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            var itemDefinition = attackMode?.SourceDefinition as ItemDefinition;

            if (attackMode == null ||
                !ValidatorsWeapon.IsMelee(itemDefinition))
            {
                return;
            }
            if (attackMode.reach)
            {
                attackMode.reachRange = 3;
            } else
            {
                attackMode.reach = true;
                attackMode.reachRange = 2;
            }
        }
    }
}
