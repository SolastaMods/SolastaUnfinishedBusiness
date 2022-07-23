using System;
using System.Collections.Generic;
using System.Linq;
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

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem item)
    {
        if (!hero.IsWearingShield() && !hero.IsWearingMediumArmor() && !hero.IsWearingHeavyArmor())
        {
            return;
        }

        var conditionBladeDance02 = DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement("ConditionBladeDance02");

        hero.RemoveConditionOfCategory("11Effect", new RulesetCondition()
        {
            conditionDefinition = conditionBladeDance02
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

        var oneHandedMeleeWeaponProficiency = FeatureDefinitionFeatureSetBuilder
            .Create("OneHandedMeleeWeaponProficiency", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create("BladeDancerDaggerProficiency", SubclassNamespace)
                    .SetGuiPresentation(WeaponTypeDefinitions.DaggerType.GuiPresentation)
                    .SetProficiencies(
                        RuleDefinitions.ProficiencyType.Weapon, WeaponTypeDefinitions.DaggerType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("BladeDancerLongSwordProficiency", SubclassNamespace)
                    .SetGuiPresentation(WeaponTypeDefinitions.LongswordType.GuiPresentation)
                    .SetProficiencies(
                        RuleDefinitions.ProficiencyType.Weapon, WeaponTypeDefinitions.LongswordType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("BladeDancerRapierProficiency", SubclassNamespace)
                    .SetGuiPresentation(WeaponTypeDefinitions.RapierType.GuiPresentation)
                    .SetProficiencies(
                        RuleDefinitions.ProficiencyType.Weapon, WeaponTypeDefinitions.RapierType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("BladeDancerScimitarProficiency", SubclassNamespace)
                    .SetGuiPresentation(WeaponTypeDefinitions.ScimitarType.GuiPresentation)
                    .SetProficiencies(
                        RuleDefinitions.ProficiencyType.Weapon, WeaponTypeDefinitions.ScimitarType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create("BladeDancerShortSwordProficiency", SubclassNamespace)
                    .SetGuiPresentation(WeaponTypeDefinitions.ShortswordType.GuiPresentation)
                    .SetProficiencies(
                        RuleDefinitions.ProficiencyType.Weapon, WeaponTypeDefinitions.ShortswordType.Name)
                    .AddToDB())
            .AddToDB();

        var conditionBladeDance02 = ConditionDefinitionBuilder
            .Create("ConditionBladeDance02", SubclassNamespace)
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

        var conditionBladeDance10 = ConditionDefinitionBuilder
            .Create(conditionBladeDance02, "ConditionBladeDance10", SubclassNamespace)
            .SetGuiPresentation(
                "PowerBladeDance", Category.Power,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(FeatureDefinitionPowers.PowerFeatRaiseShield)
            .AddToDB();

        var conditionBladeDance14 = ConditionDefinitionBuilder
            .Create(conditionBladeDance10, "ConditionBladeDance14", SubclassNamespace)
            .SetGuiPresentation(
                "PowerBladeDance", Category.Power,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierBladeDance", SubclassNamespace)
                    .SetGuiPresentation("PowerDanceOfVictory", Category.Power)
                    .Configure(
                        RuleDefinitions.AttackModifierMethod.AddAbilityScoreBonus,
                        0,
                        AttributeDefinitions.Intelligence)
                    .AddToDB())
            .AddToDB();

        var effectBladeDance02 = EffectDescriptionBuilder
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
                    .SetConditionForm(conditionBladeDance02, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var effectBladeDance10 = EffectDescriptionBuilder
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
                    .SetConditionForm(conditionBladeDance10, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var effectBladeDance14 = EffectDescriptionBuilder
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
                    .SetConditionForm(conditionBladeDance14, ConditionForm.ConditionOperation.Add)
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
                effectBladeDance02,
                true)
            .SetCustomSubFeatures(
                new PowerUseValidity(
                    (character) =>
                    {
                        if (character is not RulesetCharacterHero hero)
                        {
                            return false;
                        }

                        return !hero.IsWearingMediumArmor() && !hero.IsWearingHeavyArmor() && !hero.IsWearingShield();
                    }))
            .AddToDB();

        var powerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create("PowerDanceOfDefense", SubclassNamespace)
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
                effectBladeDance10,
                true)
            .SetOverriddenPower(powerBladeDance)
            .SetCustomSubFeatures(
                new PowerUseValidity(
                    (character) =>
                    {
                        if (character is not RulesetCharacterHero hero)
                        {
                            return false;
                        }

                        return !hero.IsWearingMediumArmor() && !hero.IsWearingHeavyArmor() && !hero.IsWearingShield();
                    }))
            .AddToDB();

        var powerDanceOfVictory = FeatureDefinitionPowerBuilder
            .Create("PowerDanceOfVictory", SubclassNamespace)
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
                effectBladeDance14,
                true)
            .SetOverriddenPower(powerDanceOfDefense)
            .SetCustomSubFeatures(
                new PowerUseValidity(
                    (character) =>
                    {
                        if (character is not RulesetCharacterHero hero)
                        {
                            return false;
                        }

                        return !hero.IsWearingMediumArmor() && !hero.IsWearingHeavyArmor() && !hero.IsWearingShield();
                    }))
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardBladeDancer", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, lightArmorProficiency, oneHandedMeleeWeaponProficiency, powerBladeDance)
            .AddFeatureAtLevel(FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack, 6) // TODO: allow cantrips as one of the attacks
            .AddFeatureAtLevel(powerDanceOfDefense, 10)
            .AddFeatureAtLevel(powerDanceOfVictory, 14)
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
