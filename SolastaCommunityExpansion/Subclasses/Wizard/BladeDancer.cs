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
    internal BladeDancer()
    {
        var lightArmorProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerLightArmor", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Armor, ArmorCategoryDefinitions.LightArmorCategory.Name)
            .AddToDB();

        var martialWeaponProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerMartialWeapon", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon, WeaponCategoryDefinitions.MartialWeaponCategory.Name)
            .AddToDB();

        var featureReplaceAttackWithCantrip = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripBladeDancer", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        ConditionBladeDance = ConditionDefinitionBuilder
            .Create("ConditionBladeDance", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .Configure(
                RuleDefinitions.DurationType.Minute,
                1,
                false,
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDance", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
                    .SetModifierAbilityScore(AttributeDefinitions.Intelligence, true)
                    .SetSituationalContext((RuleDefinitions.SituationalContext)
                        ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityIslandHalflingAcrobatics,
                        "AbilityCheckAffinityBladeDanceAcrobatics", DefinitionBuilder.CENamespaceGuid)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityBladeDanceConstitution", DefinitionBuilder.CENamespaceGuid)
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
            .Create("PowerBladeDance", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(
                "Feature/&FeatureBladeDanceTitle",
                "Condition/&ConditionBladeDanceDescription",
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
                new PowerUseValidity(IsBladeDanceValid))
            .AddToDB();

        ConditionDanceOfDefense = ConditionDefinitionBuilder
            .Create(ConditionBladeDance, "ConditionDanceOfDefense", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ConditionBladeDance", Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create("ReduceDamageDanceOfDefense", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetNotificationTag("DanceOfDefense")
                    .SetReducedDamage(-5)
                    .SetSourceType(RuleDefinitions.FeatureSourceType.CharacterFeature)
                    .SetSourceName("ReduceDamageDanceOfDefense")
                    .AddToDB())
            .AddToDB();

        var effectDanceOfDefense = EffectDescriptionBuilder
            .Create(effectBladeDance)
            .ClearEffectForms()
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDanceOfDefense, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create(powerBladeDance, "PowerDanceOfDefense", DefinitionBuilder.CENamespaceGuid)
            .SetEffectDescription(effectDanceOfDefense)
            .SetOverriddenPower(powerBladeDance)
            .AddToDB();

        ConditionDanceOfVictory = ConditionDefinitionBuilder
            .Create(ConditionDanceOfDefense, "ConditionDanceOfVictory", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ConditionBladeDance", Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierDanceOfVictory", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .Configure(
                        RuleDefinitions.AttackModifierMethod.None,
                        0,
                        String.Empty,
                        RuleDefinitions.AttackModifierMethod.FlatValue,
                        5)
                    .AddToDB())
            .AddToDB();

        var effectDanceOfVictory = EffectDescriptionBuilder
            .Create(effectBladeDance)
            .ClearEffectForms()
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDanceOfVictory, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerDanceOfVictory = FeatureDefinitionPowerBuilder
            .Create(powerBladeDance, "PowerDanceOfVictory", DefinitionBuilder.CENamespaceGuid)
            .SetEffectDescription(effectDanceOfVictory)
            .SetOverriddenPower(powerDanceOfDefense)
            .AddToDB();

        //
        // use sets for better descriptions on level up
        //

        var featureBladeDanceSet = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDance", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatureBladeDance", Category.Feature)
            .SetFeatureSet(powerBladeDance)
            .AddToDB();

        var featureDanceOfDefenseSet = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetReduceDamageDanceOfDefense", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ReduceDamageDanceOfDefense", Category.Feature)
            .SetFeatureSet(powerDanceOfDefense)
            .AddToDB();

        var featureDanceOfVictorySet = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDanceOfVictory", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("AttackModifierDanceOfVictory", Category.Feature)
            .SetFeatureSet(powerDanceOfVictory)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardBladeDancer", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, lightArmorProficiency, martialWeaponProficiency, featureBladeDanceSet)
            .AddFeatureAtLevel(featureReplaceAttackWithCantrip, 6)
            .AddFeatureAtLevel(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack, 6)
            .AddFeatureAtLevel(featureDanceOfDefenseSet, 10)
            .AddFeatureAtLevel(featureDanceOfVictorySet, 14)
            .AddToDB();
    }

    private static ConditionDefinition ConditionBladeDance { get; set; }

    private static ConditionDefinition ConditionDanceOfDefense { get; set; }

    private static ConditionDefinition ConditionDanceOfVictory { get; set; }

    private static CharacterSubclassDefinition Subclass { get; set; }

    private static bool IsBladeDanceValid(RulesetCharacter hero)
    {
        return !hero.IsWearingMediumArmor()
               && !hero.IsWearingHeavyArmor()
               && !hero.IsWearingShield()
               && !hero.IsWieldingTwoHandedWeapon();
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem _)
    {
        if (IsBladeDanceValid(hero))
        {
            return;
        }

        if (hero.HasConditionOfCategoryAndType("11Effect", ConditionBladeDance.Name))
        {
            hero.RemoveConditionOfCategory("11Effect",
                new RulesetCondition { conditionDefinition = ConditionBladeDance });
        }

        if (hero.HasConditionOfCategoryAndType("11Effect", ConditionDanceOfDefense.Name))
        {
            hero.RemoveConditionOfCategory("11Effect",
                new RulesetCondition { conditionDefinition = ConditionDanceOfDefense });
        }

        if (hero.HasConditionOfCategoryAndType("11Effect", ConditionDanceOfVictory.Name))
        {
            hero.RemoveConditionOfCategory("11Effect",
                new RulesetCondition { conditionDefinition = ConditionDanceOfVictory });
        }
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
