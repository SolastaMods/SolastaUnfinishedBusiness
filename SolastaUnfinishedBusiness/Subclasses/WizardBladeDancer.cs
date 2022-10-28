using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardBladeDancer : AbstractSubclass
{
    internal WizardBladeDancer()
    {
        var proficiencyBladeDancerLightArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerLightArmor")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Armor, ArmorCategoryDefinitions.LightArmorCategory.Name)
            .AddToDB();

        var proficiencyBladeDancerMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerMartialWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon, WeaponCategoryDefinitions.MartialWeaponCategory.Name)
            .AddToDB();

        var replaceAttackWithCantripBladeDancer = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripBladeDancer")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        ConditionBladeDancerBladeDance = ConditionDefinitionBuilder
            .Create("ConditionBladeDancerBladeDance")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetDuration(DurationType.Minute, 1)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDancerBladeDance")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifierAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Intelligence)
                    .SetSituationalContext((SituationalContext)
                        ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityIslandHalflingAcrobatics,
                        "AbilityCheckAffinityBladeDancerBladeDanceAcrobatics")
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityBladeDancerBladeDanceConstitution")
                    .SetGuiPresentationNoContent(true)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.None,
                        DieType.D1,
                        4,
                        (AttributeDefinitions.Constitution, string.Empty))
                    .AddToDB())
            .SetConditionType(ConditionType.Beneficial)
            .SetTerminateWhenRemoved(true)
            .AddToDB();

        var effectBladeDance = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(DurationType.Minute, 1)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionBladeDancerBladeDance, ConditionForm.ConditionOperation.Add)
                    .Build())
            .Build();

        var powerBladeDancerBladeDance = FeatureDefinitionPowerBuilder
            .Create("PowerBladeDancerBladeDance")
            .SetGuiPresentation(
                "Feature/&FeatureBladeDanceTitle",
                "Condition/&ConditionBladeDancerBladeDanceDescription",
                FeatureDefinitionPowers.PowerClericDivineInterventionWizard)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(effectBladeDance)
            .SetUniqueInstance()
            .SetCustomSubFeatures(new ValidatorsPowerUse(IsBladeDanceValid))
            .AddToDB();

        ConditionBladeDancerDanceOfDefense = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerBladeDance, "ConditionBladeDancerDanceOfDefense")
            .SetGuiPresentation("ConditionBladeDancerBladeDance", Category.Condition, ConditionHeroism)
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create("ReduceDamageBladeDancerDanceOfDefense")
                    .SetGuiPresentation(Category.Feature)
                    .SetNotificationTag("DanceOfDefense")
                    .SetReducedDamage(-5)
                    .SetSourceType(FeatureSourceType.CharacterFeature)
                    .SetSourceName("ReduceDamageBladeDancerDanceOfDefense")
                    .AddToDB())
            .AddToDB();

        var powerBladeDancerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, "PowerBladeDancerDanceOfDefense")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(effectBladeDance)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionBladeDancerDanceOfDefense, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerBladeDancerBladeDance)
            .AddToDB();

        ConditionBladeDancerDanceOfVictory = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerDanceOfDefense, "ConditionBladeDancerDanceOfVictory")
            .SetGuiPresentation("ConditionBladeDancerBladeDance", Category.Condition, ConditionHeroism)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierBladeDancerDanceOfVictory")
                    .SetGuiPresentation(Category.Feature)
                    .SetDamageRollModifier(5)
                    .AddToDB())
            .AddToDB();

        var powerBladeDancerDanceOfVictory = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, "PowerBladeDancerDanceOfVictory")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(effectBladeDance)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionBladeDancerDanceOfVictory, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .Build())
            .SetOverriddenPower(powerBladeDancerDanceOfDefense)
            .AddToDB();

        //
        // use sets for better descriptions on level up
        //

        var featureSetBladeDancerBladeDance = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerBladeDance")
            .SetGuiPresentation("FeatureBladeDance", Category.Feature)
            .AddFeatureSet(powerBladeDancerBladeDance)
            .AddToDB();

        var featureSetBladeDancerDanceOfDefense = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerDanceOfDefense")
            .SetGuiPresentation("ReduceDamageBladeDancerDanceOfDefense", Category.Feature)
            .AddFeatureSet(powerBladeDancerDanceOfDefense)
            .AddToDB();

        var featureSetBladeDancerDanceOfVictory = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerDanceOfVictory")
            .SetGuiPresentation("AttackModifierBladeDancerDanceOfVictory", Category.Feature)
            .AddFeatureSet(powerBladeDancerDanceOfVictory)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardBladeDancer")
            .SetGuiPresentation(Category.Subclass, DomainMischief)
            .AddFeaturesAtLevel(2,
                proficiencyBladeDancerLightArmor,
                proficiencyBladeDancerMartialWeapon,
                featureSetBladeDancerBladeDance)
            .AddFeaturesAtLevel(6,
                replaceAttackWithCantripBladeDancer,
                FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack)
            .AddFeaturesAtLevel(10,
                featureSetBladeDancerDanceOfDefense)
            .AddFeaturesAtLevel(14,
                featureSetBladeDancerDanceOfVictory)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    private static ConditionDefinition ConditionBladeDancerBladeDance { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfDefense { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfVictory { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    private static bool IsBladeDanceValid(RulesetCharacter hero)
    {
        return !hero.IsWearingMediumArmor()
               && !hero.IsWearingHeavyArmor()
               && !hero.IsWearingShield()
               && !hero.IsWieldingTwoHandedWeapon();
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem rulesetItem)
    {
        if (IsBladeDanceValid(hero))
        {
            return;
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerBladeDance.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerBladeDance });
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerDanceOfDefense.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerDanceOfDefense });
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerDanceOfVictory.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerDanceOfVictory });
        }
    }
}
