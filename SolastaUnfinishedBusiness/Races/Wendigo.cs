using System.Collections.Generic;
using JetBrains.Annotations;
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
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
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
            .SetCustomSubFeatures(
                new CustomAdditionalDamageSuckerPunch(
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create("AdditionalDamageSuckerPunch")
                        .SetGuiPresentationNoContent(true)
                        .SetDamageDice(DieType.D6, 2)
                        .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                        .SetNotificationTag("SuckerPunch")
                        .AddToDB()
                ))
            .AddToDB();


        var featureWendigoNaturalLunger = FeatureDefinitionBuilder
            .Create("FeatureWendigoNaturalLunger")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyWeaponAttackModeWendigoNaturalLunger())
            .AddToDB();
        var racePresentation = Tiefling.RacePresentation.DeepCopy();
        racePresentation.preferedSkinColors = new RangedInt(28, 47);

        var raceWendigo = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Wendigo, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(racePresentation)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(80)
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

    private class CustomAdditionalDamageSuckerPunch : CustomAdditionalDamage
    {
        public CustomAdditionalDamageSuckerPunch(IAdditionalDamageProvider provider) : base(provider)
        {
        }

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

            return battleManager.Battle.CurrentRound == 1 &&
                   battleManager.Battle.InitiativeSortedContenders.IndexOf(attacker)
                   < battleManager.Battle.InitiativeSortedContenders.IndexOf(defender);
        }
    }

    private class ModifyWeaponAttackModeWendigoNaturalLunger :
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
            }
            else
            {
                attackMode.reach = true;
                attackMode.reachRange = 2;
            }
        }
    }
}
