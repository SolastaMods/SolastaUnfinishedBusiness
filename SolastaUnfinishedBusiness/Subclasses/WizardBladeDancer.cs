using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardBladeDancer : AbstractSubclass
{
    internal WizardBladeDancer()
    {
        var proficiencyBladeDancerLightArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerLightArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory)
            .AddToDB();

        var proficiencyBladeDancerMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerMartialWeapon")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var featureSetCasterBladeDancerFighting = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetCasterBladeDancerFighting")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyBladeDancerLightArmor, proficiencyBladeDancerMartialWeapon)
            .AddToDB();

        ConditionBladeDancerBladeDance = ConditionDefinitionBuilder
            .Create("ConditionBladeDancerBladeDance")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDancerBladeDance")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifierAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Intelligence)
                    .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
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
            .AddToDB();

        var effectBladeDance = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
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
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
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
                    .SetConsumeSpellSlotsReducedDamage(CharacterClassDefinitions.Wizard, 5)
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
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WizardBladeDancer", Resources.WizardBladeDancer, 256))
            .AddFeaturesAtLevel(2,
                featureSetCasterBladeDancerFighting,
                featureSetBladeDancerBladeDance)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                ReplaceAttackWithCantripCasterFighting)
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

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool IsBladeDanceValid(RulesetCharacter hero)
    {
        return !hero.IsWearingMediumArmor()
               && !hero.IsWearingHeavyArmor()
               && !hero.IsWearingShield()
               && !hero.IsWieldingTwoHandedWeapon();
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacter hero)
    {
        if (IsBladeDanceValid(hero))
        {
            return;
        }

        foreach (var rulesetCondition in hero.allConditions
                     .Where(x => x.ConditionDefinition == ConditionBladeDancerBladeDance ||
                                 x.ConditionDefinition == ConditionBladeDancerDanceOfDefense ||
                                 x.ConditionDefinition == ConditionBladeDancerDanceOfVictory)
                     .ToList())
        {
            hero.RemoveCondition(rulesetCondition);
        }
    }
}
