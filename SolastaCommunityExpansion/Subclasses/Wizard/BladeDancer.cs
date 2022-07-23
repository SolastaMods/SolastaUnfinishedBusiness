using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class BladeDancer : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("081d41b8-ed38-4d44-8af1-3879efc99aa1");

    private static ConditionDefinition ConditionBladeDance { get; set; }

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem _)
    {
        if (!hero.IsWearingShield()
            && !hero.IsWearingMediumArmor()
            && !hero.IsWearingHeavyArmor()
            && !hero.IsWieldingTwoHandedWeapon())
        {
            return;
        }

        hero.RemoveConditionOfCategory("11Effect", new RulesetCondition()
        {
            conditionDefinition = ConditionBladeDance
        });
    }

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
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDance", SubclassNamespace)
                    .SetGuiPresentation(Category.Feature)
                    .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
                    .SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus)
                    .SetModifierAbilityScore(AttributeDefinitions.Intelligence)
                    .SetSituationalContext((RuleDefinitions.SituationalContext)
                        ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                 FeatureDefinitionAbilityCheckAffinityBuilder
                     .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityIslandHalflingAcrobatics,
                         "AbilityCheckBladeDanceAcrobatics", SubclassNamespace)
                     .AddToDB(),
                // TODO: Make this add INT bonus to concentration checks
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                        "AbilityCheckBladeDanceConstitution", SubclassNamespace)
                    .AddToDB(),
                FeatureDefinitionMoveModes.MoveModeMove7)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetTerminateWhenRemoved(true)
            .SetSilent(Silent.None)
            .SetAllowMultipleInstances(false)
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
                    !character.IsWearingMediumArmor() && !character.IsWearingHeavyArmor() && !character.IsWearingShield()))
            .AddToDB();

        // TODO: Not sure why this isn't triggering
        var powerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create("PowerDanceOfDefense", SubclassNamespace)
            .SetGuiPresentation(Category.Power)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Reaction,
                1,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Intelligence,
                new EffectDescriptionBuilder()
                    .SetTargetingData(
                        RuleDefinitions.Side.Ally,
                        RuleDefinitions.RangeType.Touch,
                        1,
                        RuleDefinitions.TargetType.Self
                    )
                    .SetEffectAdvancement(
                        RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1
                    )
                    .SetEffectForms(new EffectFormBuilder()
                        .SetHealingForm(
                            RuleDefinitions.HealingComputation.Dice,
                            0,
                            RuleDefinitions.DieType.D1,
                            5,
                            false,
                            RuleDefinitions.HealingCap.MaximumHitPoints
                        )
                        .Build()
                    )
                    .Build()
            )
            .SetCustomSubFeatures(
                new PowerUseValidity(character =>
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
            // TODO: remove powerDanceOfDefense after tests
            .AddFeaturesAtLevel(2, lightArmorProficiency, martialWeaponProficiency, powerBladeDance, powerDanceOfDefense)
            // TODO: allow cantrips as one of the attacks
            .AddFeatureAtLevel(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack, 6)
            .AddFeatureAtLevel(powerDanceOfDefense, 10)
            .AddFeatureAtLevel(featureDanceOfVictory, 14)
            .AddToDB();
    }

    private static CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
