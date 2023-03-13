using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishAcrobat : AbstractSubclass
{
    private const string Name = "RoguishAcrobat";

    private static readonly IsWeaponValidHandler IsQuarterstaff =
        ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.QuarterstaffType);

    internal RoguishAcrobat()
    {
        // LEVEL 03

        // Acrobatic Connoisseur
        var proficiencyAcrobaticConnoisseur = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Connoisseur")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
            .AddToDB();

        // Acrobatic Defender
        var attributeModifierAcrobaticDefender = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}Defender")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithQuarterstaffWithoutShield)
            .AddToDB();

        // Acrobatic Warrior
        var featureAcrobaticWarrior = FeatureDefinitionBuilder
            .Create($"Feature{Name}Warrior")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new AddQuarterstaffFollowupAttack(),
                new AddTagToWeapon(
                    TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, IsQuarterstaff),
                new ModifyAttackModeForWeaponTypeQuarterstaff(IsQuarterstaff),
                new UpgradeWeaponDice((_, _) => (1, DieType.D6, DieType.D10), IsQuarterstaff))
            .AddToDB();

        // LEVEL 09 - Swift as the Wind

        var combatAffinitySwiftWind = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetAttackOfOpportunityOnMeAdvantage(AdvantageType.Disadvantage)
            .SetCustomSubFeatures(ValidatorsCharacter.HasQuarterstaff)
            .AddToDB();

        var movementAffinitySwiftWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}SwiftWind")
            .SetGuiPresentationNoContent(true)
            .SetAdditionalFallThreshold(3)
            .SetClimbing(true, true)
            .SetEnhancedJump(2)
            .SetCustomSubFeatures(ValidatorsCharacter.HasQuarterstaff)
            .AddToDB();

        var featureSetSwiftWind = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SwiftWind")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(combatAffinitySwiftWind, movementAffinitySwiftWind)
            .AddToDB();

        // LEVEL 13 - Fluid Motions

        var movementAffinityFluidMotions = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}FluidMotions")
            .SetGuiPresentationNoContent(true)
            .SetClimbing(true, true, true)
            .SetImmunities(difficultTerrainImmunity: true)
            .SetCustomSubFeatures(ValidatorsCharacter.HasQuarterstaff)
            .AddToDB();

        // no need to have quarterstaff check here. by design although description should state that
        var featureSetFluidMotions = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetDomainLawUnyieldingEnforcer, $"FeatureSet{Name}FluidMotions")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureSetFluidMotions.FeatureSet.Add(movementAffinityFluidMotions);

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("RoguishAcrobat", Resources.RoguishAcrobat, 256))
            .AddFeaturesAtLevel(3,
                proficiencyAcrobaticConnoisseur,
                attributeModifierAcrobaticDefender,
                featureAcrobaticWarrior)
            .AddFeaturesAtLevel(9,
                featureSetSwiftWind)
            .AddFeaturesAtLevel(13,
                featureSetFluidMotions)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAttackModeForWeaponTypeQuarterstaff : IModifyAttackModeForWeapon
    {
        private readonly IsWeaponValidHandler _isWeaponValid;

        public ModifyAttackModeForWeaponTypeQuarterstaff(IsWeaponValidHandler isWeaponValid)
        {
            _isWeaponValid = isWeaponValid;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!_isWeaponValid(attackMode, null, character))
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }
    }
}
