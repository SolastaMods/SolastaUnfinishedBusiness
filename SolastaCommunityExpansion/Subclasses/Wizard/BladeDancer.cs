using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class BladeDancer : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("081d41b8-ed38-4d44-8af1-3879efc99aa1");

    internal BladeDancer()
    {
        var lightArmorProficiency = FeatureDefinitionProficiencyBuilder
            .Create("BladeDancerLightArmorProficiency", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Armor, ArmorCategoryDefinitions.LightArmorCategory.Name)
            .AddToDB();

        var martialWeaponProficiency = FeatureDefinitionProficiencyBuilder
            .Create("BladeDancerMartialWeaponProficiency", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon, WeaponCategoryDefinitions.MartialWeaponCategory.Name)
            .AddToDB();

        ConditionBladeDance = ConditionDefinitionBuilder
            .Create("ConditionBladeDance", SubclassNamespace)
            .SetGuiPresentation(
                "PowerBladeDance", Category.Power,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .Configure(
                RuleDefinitions.DurationType.Minute,
                1,
                false,
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDance", SubclassNamespace)
                    .SetGuiPresentation(Category.Feature)
                    .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
                    .SetModifierType2(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus)
                    .SetModifierAbilityScore(AttributeDefinitions.Intelligence)
                    .SetSituationalContext((RuleDefinitions.SituationalContext)
                        ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityIslandHalflingAcrobatics,
                        "AbilityCheckBladeDanceAcrobatics", SubclassNamespace)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckBladeDanceConstitution", SubclassNamespace)
                    .BuildAndSetAffinityGroups(
                        RuleDefinitions.CharacterAbilityCheckAffinity.None,
                        RuleDefinitions.DieType.D1,
                        4,
                        (AttributeDefinitions.Constitution, string.Empty))
                    .AddToDB())
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetTerminateWhenRemoved(true)
            .AddToDB();

        var effectBladeDance = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(
                RuleDefinitions.DurationType.Minute, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionBladeDance, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerBladeDance = FeatureDefinitionPowerBuilder
            .Create("PowerBladeDance", SubclassNamespace)
            .SetGuiPresentation(
                Category.Power,
                FeatureDefinitionPowers.PowerClericDivineInterventionWizard.GuiPresentation.SpriteReference)
            .Configure(
                0, RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                effectBladeDance,
                true)
            .SetCustomSubFeatures(
                new PowerUseValidity(character =>
                    !character.IsWearingMediumArmor() && !character.IsWearingHeavyArmor() &&
                    !character.IsWearingShield()))
            .AddToDB();

        var featureReplaceAttackWithCantrip = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("FeatureReplaceAttackWithCantrip", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureDanceOfDefense = FeatureDefinitionReduceDamageBuilder
            .Create("FeatureDanceOfDefense", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DanceOfDefense")
            .SetReducedDamage(-5)
            .SetSourceType(RuleDefinitions.FeatureSourceType.CharacterFeature)
            .SetSourceName("FeatureDanceOfDefense")
            .SetCustomSubFeatures(
                new FeatureApplicationValidator(character =>
                    character.HasConditionOfCategoryAndType("11Effect", "ConditionBladeDance")))
            .AddToDB();

        var featureDanceOfVictory = FeatureDefinitionAttackModifierBuilder
            .Create("FeatureDanceOfVictory", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .Configure(
                RuleDefinitions.AttackModifierMethod.AddAbilityScoreBonus,
                0,
                AttributeDefinitions.Intelligence)
            .SetCustomSubFeatures(
                new FeatureApplicationValidator(character =>
                    character.HasConditionOfCategoryAndType("11Effect", "ConditionBladeDance")))
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardBladeDancer", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, lightArmorProficiency, martialWeaponProficiency, powerBladeDance)
            .AddFeatureAtLevel(featureReplaceAttackWithCantrip, 6)
            .AddFeatureAtLevel(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack, 6)
            .AddFeatureAtLevel(featureDanceOfDefense, 10)
            .AddFeatureAtLevel(featureDanceOfVictory, 14)
            .AddToDB();
    }

    private static ConditionDefinition ConditionBladeDance { get; set; }

    private static CharacterSubclassDefinition Subclass { get; set; }

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem _)
    {
        if ((!hero.IsWearingShield()
             && !hero.IsWearingMediumArmor()
             && !hero.IsWearingHeavyArmor()
             && !hero.IsWieldingTwoHandedWeapon())
            || !hero.HasConditionOfCategoryAndType("11Effect", ConditionBladeDance.Name))
        {
            return;
        }

        hero.RemoveConditionOfCategory("11Effect", new RulesetCondition {conditionDefinition = ConditionBladeDance});
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
