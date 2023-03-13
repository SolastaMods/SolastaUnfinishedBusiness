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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;

namespace SolastaUnfinishedBusiness.Subclasses;

/*

- Acrobatic Performer

Starting at 3rd level, you become proficient with quarterstaff and gain the ability to use it as finesse weapon.

Your reach with quarterstaff increases to 10 ft. The damage die changes from a d8 to a d10 when wielded with two hands.

When you take the Attack action with a quarterstaff, you can use your bonus action to make a melee attack with the opposite end of the weapon. This attack uses the same ability modifier as the primary attack and deals 1d4 bludgeoning damage.

You add half your proficiency bonus to your armor class while wielding a quarterstaff, and you are wearing no armor or light armor without shield.

You also gain proficiency in the Acrobat skill if you don't already have it, or expertise if you do.


- Swift as the Wind

Starting at 9th level, you can move along walls, climbing no longer costs you extra movement, you can jump an additional 10 feet and you have reduced falling damage.

Additionally, opportunity attacks are made with disadvantage against you. You must be wielding a quarterstaff for this feature to work.


- Fluid Motions

Starting at 13th level, you are unaffected by difficult terrain and you have advantage on prone and shove attempts against you. You must be wielding a quarterstaff for this feature to work.


- Heroic Uncanny Dodge

Starting at 17th level, if an attack roll successfully hits you, you may use your reaction to force the attack to miss instead. You may use this ability a number of times per long rest equal to your Dexterity modifier.

*/

internal sealed class RoguishAcrobat : AbstractSubclass
{
    private const string Name = "RoguishAcrobat";

    private static readonly IsWeaponValidHandler IsQuarterstaff =
        ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.QuarterstaffType);

    internal RoguishAcrobat()
    {
        // LEVEL 03 - Acrobatic Performer

        var attributeModifierAcrobaticPerformer = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}AcrobaticPerformer")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithQuarterstaffWithoutShield)
            .AddToDB();

        var featureAcrobaticPerformer = FeatureDefinitionBuilder
            .Create($"Feature{Name}AcrobaticPerformer")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new AddQuarterstaffFollowupAttack(),
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, IsQuarterstaff),
                new ModifyAttackModeForWeaponTypeQuarterstaff(IsQuarterstaff),
                new UpgradeWeaponDice((_, _) => (1, DieType.D6, DieType.D10), IsQuarterstaff))
            .AddToDB();

        var proficiencyAcrobaticPerformer = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}AcrobaticPerformer")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
            .AddToDB();

        var featureSetAcrobaticPerformer = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AcrobaticPerformer")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                attributeModifierAcrobaticPerformer,
                featureAcrobaticPerformer,
                proficiencyAcrobaticPerformer)
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
            .SetClimbing(true, true, true)
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
            .SetImmunities(difficultTerrainImmunity: true)
            .SetCustomSubFeatures(ValidatorsCharacter.HasQuarterstaff)
            .AddToDB();

        var savingThrowAffinityFluidMotions = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm, $"SavingThrowAffinity{Name}FluidMotions")
            .SetCustomSubFeatures(ValidatorsCharacter.HasQuarterstaff)
            .AddToDB();

        var featureSetFluidMotions = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}FluidMotions")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(movementAffinityFluidMotions, savingThrowAffinityFluidMotions)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("RoguishAcrobat", Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, featureSetAcrobaticPerformer)
            .AddFeaturesAtLevel(9, featureSetSwiftWind)
            .AddFeaturesAtLevel(13, featureSetFluidMotions)
            .AddFeaturesAtLevel(17)
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
