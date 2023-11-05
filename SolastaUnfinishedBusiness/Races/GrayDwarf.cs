using System;
using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceGrayDwarfBuilder
{
    internal static CharacterRaceDefinition SubraceGrayDwarf { get; } = BuildGrayDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildGrayDwarf()
    {
        var grayDwarfSpriteReference = Sprites.GetSprite("GrayDwarf", Resources.GrayDwarf, 1024, 512);

        var attributeModifierGrayDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGrayDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var lightAffinityGrayDwarfLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(CustomConditionsContext.LightSensitivity.Name, Category.Condition)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = CustomConditionsContext.LightSensitivity
                })
            .AddToDB();

        var conditionAffinityGrayDwarfCharm = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityElfFeyAncestryCharm, "ConditionAffinityGrayDwarfCharm")
            .AddToDB();

        var conditionAffinityGrayDwarfCharmedByHypnoticPattern = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern,
                "ConditionAffinityGrayDwarfCharmedByHypnoticPattern")
            .AddToDB();

        var conditionAffinityGrayDwarfParalyzedAdvantage = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityHalflingBrave, "ConditionAffinityGrayDwarfParalyzedAdvantage")
            .SetConditionType(ConditionDefinitions.ConditionParalyzed)
            .AddToDB();

        var savingThrowAffinityGrayDwarfIllusion = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityGemIllusion, "SavingThrowAffinityGrayDwarfIllusion")
            .AddToDB();

        for (var i = 0; i < 6; i++)
        {
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].affinity = CharacterSavingThrowAffinity.Advantage;
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].savingThrowModifierDiceNumber = 0;
        }

        var featureSetGrayDwarfAncestry = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetGrayDwarfAncestry")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                conditionAffinityGrayDwarfCharm,
                conditionAffinityGrayDwarfCharmedByHypnoticPattern,
                conditionAffinityGrayDwarfParalyzedAdvantage,
                savingThrowAffinityGrayDwarfIllusion)
            .AddToDB();

        var abilityCheckAffinityGrayDwarfStoneStrength = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(AbilityCheckAffinityConditionBullsStrength, "AbilityCheckAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var savingThrowAffinityGrayDwarfStoneStrength = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityConditionRaging, "SavingThrowAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var conditionGrayDwarfStoneStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBullsStrength, "ConditionGrayDwarfStoneStrength")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionStoneResilience)
            .SetFeatures(
                abilityCheckAffinityGrayDwarfStoneStrength,
                savingThrowAffinityGrayDwarfStoneStrength)
            .AddCustomSubFeatures(new AdditionalDamageGrayDwarfStoneStrength())
            .AddToDB();

        var powerGrayDwarfStoneStrength = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfStoneStrength")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Stoneskin)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionGrayDwarfStoneStrength, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerGrayDwarfInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Invisibility.EffectDescription)
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        var grayDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        grayDwarfRacePresentation.needBeard = false;
        grayDwarfRacePresentation.MaleBeardShapeOptions.Add("BeardShape_None");
        grayDwarfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        grayDwarfRacePresentation.preferedHairColors = new RangedInt(35, 41);

        grayDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        grayDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;

        var raceGrayDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceGrayDwarf")
            .SetGuiPresentation(Category.Race, grayDwarfSpriteReference)
            .SetRacePresentation(grayDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierGrayDwarfStrengthAbilityScoreIncrease,
                featureSetGrayDwarfAncestry,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                lightAffinityGrayDwarfLightSensitivity,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages)
            .AddFeaturesAtLevel(3,
                powerGrayDwarfStoneStrength)
            .AddFeaturesAtLevel(5,
                powerGrayDwarfInvisibility)
            .AddToDB();

        Dwarf.SubRaces.Add(raceGrayDwarf);

        return raceGrayDwarf;
    }

    private sealed class AdditionalDamageGrayDwarfStoneStrength : IModifyWeaponAttackMode, IPhysicalAttackInitiatedByMe
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode?.abilityScore != AttributeDefinitions.Strength ||
                (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode)))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            if (k < 0 || damage == null)
            {
                return;
            }

            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(damage.damageType, 1, DieType.D4)
                .Build();

            additionalDice.dcModifier = Int32.MinValue; // mark this damage form if we need to remove later
            effectDescription.EffectForms.Insert(k + 1, additionalDice);
        }

        // this is required to handle thrown scenarios
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var isStrength = attackMode.abilityScore == AttributeDefinitions.Strength;
            var isMelee = ValidatorsWeapon.IsMelee(attackMode);
            var isUnarmed = ValidatorsWeapon.IsUnarmed(attackMode);
            var isStoneStrengthValid = isStrength && (isMelee || isUnarmed);

            if (isStoneStrengthValid)
            {
                yield break;
            }

            // remove marked damage form as it's a thrown attack
            attackMode.EffectDescription.EffectForms.RemoveAll(x => x.dcModifier == Int32.MinValue);
        }
    }
}
