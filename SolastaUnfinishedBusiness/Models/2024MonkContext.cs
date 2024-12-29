using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static readonly ConditionDefinition ConditionStunningStrikeMark = ConditionDefinitionBuilder
        .Create("ConditionStunningStrikeMark")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    private static readonly ConditionDefinition ConditionGrappleNoCost = ConditionDefinitionBuilder
        .Create("ConditionGrappleNoCost")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    internal static readonly ConditionDefinition ConditionGrappleNoCostUsed = ConditionDefinitionBuilder
        .Create("ConditionGrappleNoCostUsed")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddCustomSubFeatures(new OnConditionAddedOrRemovedGrappleNoCostUsed())
        .AddToDB();

    private static readonly FeatureDefinitionActionAffinity FeatureMonkHeightenedFocus =
        FeatureDefinitionActionAffinityBuilder
            .Create("FeatureMonkHeightenedFocus")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((Id)ExtraActionId.GrappleNoCost)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAnyOfConditions(ConditionGrappleNoCost.Name),
                    ValidatorsCharacter.HasNoneOfConditions(ConditionGrappleNoCostUsed.Name)),
                new CustomBehaviorHeightenedFocus(
                    ConditionDefinitionBuilder
                        .Create("ConditionMonkFlurryOfBlowsHeightenedFocus")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(
                            FeatureDefinitionAttackModifierBuilder
                                .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
                                    "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusHeightenedFocus")
                                .SetUnarmedStrike(3)
                                .AddToDB())
                        .AddToDB(),
                    ConditionDefinitionBuilder
                        .Create("ConditionMonkFlurryOfBlowsFreedomHeightenedFocus")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(
                            FeatureDefinitionAttackModifierBuilder
                                .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom,
                                    "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedomHeightenedFocus")
                                .SetUnarmedStrike(4)
                                .AddToDB())
                        .AddToDB()))
            .AddToDB();

    private static readonly FeatureDefinition FeatureMonkPerfectFocus =
        FeatureDefinitionBuilder
            .Create("FeatureMonkPerfectFocus")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkReturnAttacks = FeatureDefinitionPowerBuilder
        .Create("PowerMonkReturnAttacks")
        .SetGuiPresentation(Category.Feature, hidden: true)
        .SetShowCasting(false)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
        .SetExplicitAbilityScore(AttributeDefinitions.Dexterity)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 8)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetDamageForm(diceNumber: 2)
                        .Build())
                .SetImpactEffectParameters(PowerRoguishHoodlumDirtyFighting)
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorMonkDeflectAttacks())
        .AddToDB();

    private static readonly ConditionDefinition ConditionMonkReturnAttacks = ConditionDefinitionBuilder
        .Create("ConditionMonkReturnAttacks")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFixedAmount(0)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkSuperiorDefense = FeatureDefinitionPowerBuilder
        .Create("PowerMonkSuperiorDefense")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerMonkSuperiorDefense", Resources.PowerMonkSuperiorDefense, 256, 128))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3, 3)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionMonkSuperiorDefense")
                                .SetGuiPresentation(Category.Condition, ConditionAuraOfProtection)
                                .SetPossessive()
                                .AddFeatures(
                                    DamageAffinityAcidResistance,
                                    DamageAffinityBludgeoningResistanceTrue,
                                    DamageAffinityColdResistance,
                                    DamageAffinityFireResistance,
                                    DamageAffinityLightningResistance,
                                    DamageAffinityNecroticResistance,
                                    DamageAffinityPiercingResistanceTrue,
                                    DamageAffinityPoisonResistance,
                                    DamageAffinityPsychicResistance,
                                    DamageAffinityRadiantResistance,
                                    DamageAffinitySlashingResistanceTrue,
                                    DamageAffinityThunderResistance)
                                .SetConditionParticleReference(ConditionHolyAura)
                                .SetCancellingConditions(
                                    DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                                        x.IsSubtypeOf(RuleDefinitions.ConditionIncapacitated)).ToArray())
                                .AddCancellingConditions(ConditionCharmedByHypnoticPattern)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinition FeatureMonkBodyAndMind = FeatureDefinitionBuilder
        .Create("FeatureMonkBodyAndMind")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomLevelUpLogicMonkBodyAndMind())
        .AddToDB();

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank =
        [.. AttackModifierMonkMartialArtsImprovedDamage.DieTypeByRankTable];

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank2024 =
    [
        new() { dieType = DieType.D6, rank = 1 },
        new() { dieType = DieType.D6, rank = 2 },
        new() { dieType = DieType.D6, rank = 3 },
        new() { dieType = DieType.D6, rank = 4 },
        new() { dieType = DieType.D8, rank = 5 },
        new() { dieType = DieType.D8, rank = 6 },
        new() { dieType = DieType.D8, rank = 7 },
        new() { dieType = DieType.D8, rank = 8 },
        new() { dieType = DieType.D8, rank = 9 },
        new() { dieType = DieType.D8, rank = 10 },
        new() { dieType = DieType.D10, rank = 11 },
        new() { dieType = DieType.D10, rank = 12 },
        new() { dieType = DieType.D10, rank = 13 },
        new() { dieType = DieType.D10, rank = 14 },
        new() { dieType = DieType.D10, rank = 15 },
        new() { dieType = DieType.D10, rank = 16 },
        new() { dieType = DieType.D12, rank = 17 },
        new() { dieType = DieType.D12, rank = 18 },
        new() { dieType = DieType.D12, rank = 19 },
        new() { dieType = DieType.D12, rank = 20 }
    ];

    private static readonly FeatureDefinitionPower PowerMonkUncannyMetabolism = FeatureDefinitionPowerBuilder
        .Create("PowerMonkUncannyMetabolism")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetShowCasting(false)
        .AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new InitiativeEndListenerUncannyMetabolism())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024AtWill = FeatureDefinitionPowerBuilder
        .Create(PowerMonkPatientDefense, "PowerMonkPatientDefense2024AtWill")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerPatientDefenseNoCost", Resources.PowerPatientDefenseNoCost, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(PowerMonkPatientDefense)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging))
                .SetCasterEffectParameters(SpellDefinitions.Command)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024Survival3AtWill =
        FeatureDefinitionPowerBuilder
            .Create(PowerMonkPatientDefenseSurvival3, "PowerMonkPatientDefense2024Survival3AtWill")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetOverriddenPower(PowerMonkPatientDefense2024AtWill)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkPatientDefenseSurvival3)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging),
                        EffectFormBuilder.ConditionForm(ConditionTraditionSurvivalDefensiveStance))
                    .SetCasterEffectParameters(SpellDefinitions.Command)
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024Survival6AtWill =
        FeatureDefinitionPowerBuilder
            .Create(PowerMonkPatientDefenseSurvival6, "PowerMonkPatientDefense2024Survival6AtWill")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetOverriddenPower(PowerMonkPatientDefense2024Survival3AtWill)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkPatientDefenseSurvival6)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging),
                        EffectFormBuilder.ConditionForm(ConditionTraditionSurvivalDefensiveStance),
                        EffectFormBuilder.ConditionForm(
                            ConditionTraditionSurvivalUnbreakableBodyPatientDefenseImproved))
                    .SetCasterEffectParameters(SpellDefinitions.Command)
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkStepOfTheWind2024AtWill = FeatureDefinitionPowerBuilder
        .Create(PowerMonkStepOfTheWindDash, "PowerMonkStepOfTheWind2024AtWill")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerStepOfTheWindNoCost", Resources.PowerStepOfTheWindNoCost, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(PowerMonkStepOfTheWindDash)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDashingBonusStepOfTheWind))
                .SetCasterEffectParameters(SpellDefinitions.Command)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024Ki = FeatureDefinitionPowerBuilder
        .Create(PowerMonkPatientDefense, "PowerMonkPatientDefense2024Ki")
        .SetOrUpdateGuiPresentation(Category.Feature)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(PowerMonkPatientDefense)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging),
                    EffectFormBuilder.ConditionForm(ConditionDodgingPatientDefense))
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024Survival3Ki =
        FeatureDefinitionPowerBuilder
            .Create(PowerMonkPatientDefenseSurvival3, "PowerMonkPatientDefense2024Survival3Ki")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerMonkPatientDefense2024Ki)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkPatientDefenseSurvival3)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging),
                        EffectFormBuilder.ConditionForm(ConditionDodgingPatientDefense),
                        EffectFormBuilder.ConditionForm(ConditionTraditionSurvivalDefensiveStance))
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkPatientDefense2024Survival6Ki =
        FeatureDefinitionPowerBuilder
            .Create(PowerMonkPatientDefenseSurvival6, "PowerMonkPatientDefense2024Survival6Ki")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerMonkPatientDefense2024Survival3Ki)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkPatientDefenseSurvival6)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging),
                        EffectFormBuilder.ConditionForm(ConditionDodgingPatientDefense),
                        EffectFormBuilder.ConditionForm(ConditionTraditionSurvivalDefensiveStance),
                        EffectFormBuilder.ConditionForm(
                            ConditionTraditionSurvivalUnbreakableBodyPatientDefenseImproved))
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkStepOfTheWind2024Ki = FeatureDefinitionPowerBuilder
        .Create(PowerMonkStepOfTheWindDash, "PowerMonkStepOfTheWind2024Ki")
        .SetOrUpdateGuiPresentation(Category.Feature)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(PowerMonkStepOfTheWindDash)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionDashingBonusStepOfTheWind),
                    EffectFormBuilder.ConditionForm(ConditionDisengagingStepOfTheWind))
                .Build())
        .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetMonkPatientDefense =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMonkPatientDefense")
            .SetGuiPresentation("PatientDefense", Category.Feature)
            .SetFeatureSet(PowerMonkPatientDefense)
            .AddToDB();

    private static readonly ConditionDefinition ConditionStunningStrikeHalfMove = ConditionDefinitionBuilder
        .Create("ConditionStunningStrikeHalfMove")
        .SetGuiPresentation(Category.Condition, ConditionSlowed)
        .SetConditionType(ConditionType.Detrimental)
        .SetFeatures(MovementAffinityConditionSlowed)
        .AddToDB();

    private static readonly ConditionDefinition ConditionStunningStrikeAdvantage = ConditionDefinitionBuilder
        .Create("ConditionStunningStrikeAdvantage")
        .SetGuiPresentation(Category.Condition, ConditionPossessed)
        .SetConditionType(ConditionType.Detrimental)
        .SetFeatures(CombatAffinityStunnedAdvantage)
        .SetSpecialInterruptions(ConditionInterruption.Attacked)
        .AddToDB();

    private static readonly FeatureDefinition FeatureMonkSelfRestoration = FeatureDefinitionBuilder
        .Create("FeatureMonkSelfRestoration")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CharacterBeforeTurnEndListenerSelfRestoration())
        .AddToDB();

    private static void LoadMonkFocus()
    {
        Monk.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerMonkPatientDefense);
        Monk.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetMonkPatientDefense, 2));
    }

    private static void LoadMonkStunningStrike()
    {
        PowerMonkStunningStrike.AddCustomSubFeatures(new MagicEffectFinishedByMeStunningStrike());
    }

    internal static void SwitchMonkDeflectAttacks()
    {
        if (Main.Settings.EnableMonkDeflectAttacks2024)
        {
            FeatureSetMonkDeflectMissiles.GuiPresentation.Title =
                "Feature/&FeatureSetMonkAlternateDeflectMissilesTitle";
            FeatureSetMonkDeflectMissiles.GuiPresentation.Description =
                "Feature/&FeatureSetMonkAlternateDeflectMissilesDescription";
            FeatureSetMonkDeflectMissiles.FeatureSet.SetRange(PowerMonkReturnAttacks);

            FeatureSetMonkTongueSunMoon.GuiPresentation.Title =
                "Feature/&FeatureSetMonkAlternateTongueSunMoonTitle";
            FeatureSetMonkTongueSunMoon.GuiPresentation.Description =
                "Feature/&FeatureSetMonkAlternateTongueSunMoonDescription";
            FeatureSetMonkTongueSunMoon.FeatureSet.Clear();
        }
        else
        {
            FeatureSetMonkDeflectMissiles.GuiPresentation.Title =
                "Feature/&FeatureSetMonkDeflectMissilesTitle";
            FeatureSetMonkDeflectMissiles.GuiPresentation.Description =
                "Feature/&FeatureSetMonkDeflectMissilesDescription";
            FeatureSetMonkDeflectMissiles.FeatureSet.SetRange(
                ActionAffinityMonkDeflectMissiles, PowerMonkReturnMissile);

            FeatureSetMonkTongueSunMoon.GuiPresentation.Title =
                "Feature/&FeatureSetMonkTongueSunMoonTitle";
            FeatureSetMonkTongueSunMoon.GuiPresentation.Description =
                "Feature/&FeatureSetMonkTongueSunMoonDescription";
            FeatureSetMonkTongueSunMoon.FeatureSet.SetRange(ProficiencyAllLanguagesButCode);
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkFocus()
    {
        if (Main.Settings.EnableMonkFocus2024)
        {
            FeatureSetMonkFlurryOfBlows.GuiPresentation.description =
                "Feature/&FeatureSetAlternateMonkFlurryOfBlowsDescription";

            FeatureSetMonkPatientDefense.GuiPresentation.description =
                "Feature/&AlternatePatientDefenseDescription";
            FeatureSetMonkPatientDefense.FeatureSet.SetRange(
                PowerMonkPatientDefense2024AtWill,
                PowerMonkPatientDefense2024Ki);

            FeatureSetTraditionSurvivalDefensiveStance.FeatureSet.SetRange(
                AttributeModifierTraditionSurvivalDefensiveStance,
                PowerMonkPatientDefense2024Survival3AtWill,
                PowerMonkPatientDefense2024Survival3Ki);

            FeatureSetTraditionSurvivalUnbreakableBody.FeatureSet.SetRange(
                PowerTraditionSurvivalUnbreakableBody,
                PowerMonkPatientDefense2024Survival6AtWill,
                PowerMonkPatientDefense2024Survival6Ki);

            FeatureSetMonkStepOfTheWind.GuiPresentation.description =
                "Feature/&AlternateFeatureSetMonkStepOfTheWindDescription";
            FeatureSetMonkStepOfTheWind.FeatureSet.SetRange(
                PowerMonkStepOfTheWind2024AtWill,
                PowerMonkStepOfTheWind2024Ki);
        }
        else
        {
            FeatureSetMonkFlurryOfBlows.GuiPresentation.description =
                "Feature/&FeatureSetMonkFlurryOfBlowsDescription";

            FeatureSetMonkPatientDefense.GuiPresentation.description =
                "Feature/&PatientDefenseDescription";
            FeatureSetMonkPatientDefense.FeatureSet.SetRange(
                PowerMonkPatientDefense);

            FeatureSetTraditionSurvivalDefensiveStance.FeatureSet.SetRange(
                AttributeModifierTraditionSurvivalDefensiveStance,
                PowerMonkPatientDefenseSurvival3);

            FeatureSetTraditionSurvivalUnbreakableBody.FeatureSet.SetRange(
                PowerTraditionSurvivalUnbreakableBody,
                PowerMonkPatientDefenseSurvival6);

            FeatureSetMonkStepOfTheWind.GuiPresentation.description =
                "Feature/&FeatureSetMonkStepOfTheWindDescription";
            FeatureSetMonkStepOfTheWind.FeatureSet.SetRange(
                PowerMonkStepOfTheWindDash,
                PowerMonkStepOftheWindDisengage);
        }
    }

    internal static void SwitchMonkMartialArts()
    {
        if (Main.Settings.EnableMonkMartialArts2024)
        {
            AttackModifierMonkMartialArtsImprovedDamage.dieTypeByRankTable = MonkUnarmedDieTypeByRank2024;
            AttackModifierMonkMartialArtsImprovedDamage.GuiPresentation.Description =
                "Feature/&AttackModifierMonkMartialArtsExtendedDescription";

            PowerMonkMartialArts.GuiPresentation.description =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusDescription";
            PowerMonkMartialArts.GuiPresentation.title =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = true;
            PowerMonkMartialArts.activationTime = ActivationTime.NoCost;
        }
        else
        {
            AttackModifierMonkMartialArtsImprovedDamage.dieTypeByRankTable = MonkUnarmedDieTypeByRank;
            AttackModifierMonkMartialArtsImprovedDamage.GuiPresentation.Description =
                "Feature/&AttackModifierMonkMartialArtsDescription";

            PowerMonkMartialArts.GuiPresentation.description = "Action/&MartialArtsDescription";
            PowerMonkMartialArts.GuiPresentation.title = "Action/&MartialArtsTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = false;
            PowerMonkMartialArts.activationTime = ActivationTime.OnAttackHitMartialArts;
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkHeightenedFocus()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureMonkHeightenedFocus ||
            x.FeatureDefinition == FeatureMonkPerfectFocus ||
            x.FeatureDefinition == FeatureSetMonkPurityOfBody ||
            x.FeatureDefinition == FeatureSetMonkTimelessBody);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkHeightenedFocus2024
                ? new FeatureUnlockByLevel(FeatureMonkHeightenedFocus, 10)
                : new FeatureUnlockByLevel(FeatureSetMonkPurityOfBody, 10));

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkHeightenedFocus2024
                ? new FeatureUnlockByLevel(FeatureMonkPerfectFocus, 15)
                : new FeatureUnlockByLevel(FeatureSetMonkTimelessBody, 15));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkSelfRestoration()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureMonkSelfRestoration ||
            x.FeatureDefinition == FeatureSetMonkStillnessOfMind);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkHeightenedFocus2024
                ? new FeatureUnlockByLevel(FeatureMonkSelfRestoration, 10)
                : new FeatureUnlockByLevel(FeatureSetMonkStillnessOfMind, 7));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkSuperiorDefense()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.PowerMonkEmptyBody ||
            x.FeatureDefinition == PowerMonkSuperiorDefense);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkSuperiorDefense2024
                ? new FeatureUnlockByLevel(PowerMonkSuperiorDefense, 18)
                : new FeatureUnlockByLevel(Level20Context.PowerMonkEmptyBody, 18));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkUncannyMetabolism()
    {
        Monk.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerMonkUncannyMetabolism);

        if (Main.Settings.EnableMonkUncannyMetabolism2024)
        {
            Monk.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerMonkUncannyMetabolism, 2));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkStunningStrike()
    {
        PowerMonkStunningStrike.GuiPresentation.Description = Main.Settings.EnableMonkStunningStrike2024
            ? "Feature/&MonkAlternateStunningStrikeDescription"
            : "Feature/&MonkStunningStrikeDescription";
    }

    internal static void SwitchMonkBodyAndMind()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.FeatureMonkPerfectSelf ||
            x.FeatureDefinition == FeatureMonkBodyAndMind);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkBodyAndMind2024
                ? new FeatureUnlockByLevel(FeatureMonkBodyAndMind, 20)
                : new FeatureUnlockByLevel(Level20Context.FeatureMonkPerfectSelf, 20));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class CustomBehaviorMonkDeflectAttacks
        : IPhysicalAttackBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe,
            IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private static readonly string[] AllDamages =
            DatabaseRepository.GetDatabase<DamageDefinition>().Select(x => x.Name).ToArray();

        private static readonly string[] AllowedDamages =
            [DamageTypeBludgeoning, DamageTypePiercing, DamageTypeSlashing];

        public IEnumerator OnMagicEffectFinishedByMe(CharacterAction action, GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (action.ActionParams.RulesetEffect.SourceDefinition != PowerMonkReturnAttacks ||
                !rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionMonkReturnAttacks.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetCharacter.RemoveCondition(activeCondition);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerMonkReturnAttacks;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.EffectForms[0].DamageForm;

            damageForm.dieType = character.GetMonkDieType();

            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionMonkReturnAttacks.Name, out var activeCondition))
            {
                return effectDescription;
            }

            damageForm.damageType = AllDamages[activeCondition.Amount];

            return effectDescription;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var classLevel = defender.RulesetCharacter.GetClassLevel(Monk);
            var damageForm = actualEffectForms.FirstOrDefault(x =>
                x.FormType == EffectForm.EffectFormType.Damage &&
                (classLevel >= 13 || AllowedDamages.Contains(x.DamageForm.DamageType)));

            if (damageForm == null || !defender.CanReact())
            {
                yield break;
            }

            yield return defender.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "DeflectAttacks",
                "CustomReactionDeflectAttacksDescription".Formatted(Category.Reaction, attacker.Name, defender.Name),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var rulesetDefender = defender.RulesetCharacter;
                var damageIndex = Array.IndexOf(AllDamages, damageForm.DamageForm.DamageType);
                var dexterity = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Dexterity);
                var reductionAmount =
                    RollDie(DieType.D10, AdvantageType.None, out _, out _) +
                    AttributeDefinitions.ComputeAbilityScoreModifier(dexterity) +
                    rulesetDefender.GetClassLevel(Monk);

                EffectHelpers.StartVisualEffect(defender, attacker, PowerMonkReturnMissile,
                    EffectHelpers.EffectType.Caster);
                actionModifier.DamageRollReduction += reductionAmount;
                rulesetDefender.DamageReduced.Invoke(rulesetDefender, FeatureSetMonkDeflectMissiles, reductionAmount);
                rulesetDefender.InflictCondition(
                    ConditionMonkReturnAttacks.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetDefender.guid,
                    rulesetDefender.CurrentFaction.Name,
                    1,
                    ConditionMonkReturnAttacks.Name,
                    damageIndex,
                    0,
                    0);
            }
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (damageAmount > 0 ||
                rulesetDefender.RemainingKiPoints == 0 ||
                !defender.CanAct() ||
                !defender.CanPerceiveTarget(attacker) ||
                !rulesetDefender.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionMonkReturnAttacks.Name))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(PowerMonkReturnAttacks, rulesetDefender);

            yield return defender.MyReactToUsePower(
                Id.PowerNoCost,
                usablePower,
                [attacker],
                attacker,
                "ReturnAttacks",
                "UseReturnAttacksDescription".Formatted(Category.Reaction, attacker.Name, defender.Name));
        }
    }

    private sealed class OnConditionAddedOrRemovedGrappleNoCostUsed : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            GameLocationCharacter.GetFromActor(target).BreakGrapple();
        }
    }

    private sealed class MagicEffectFinishedByMeStunningStrike : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            if (!Main.Settings.EnableMonkStunningStrike2024 ||
                action.ActionParams.RulesetEffect?.SourceDefinition != PowerMonkStunningStrike ||
                action.SaveOutcome == RollOutcome.Failure)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var rulesetDefender = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            rulesetDefender.InflictCondition(
                ConditionStunningStrikeHalfMove.Name,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionStunningStrikeHalfMove.Name,
                0,
                0,
                0);

            rulesetDefender.InflictCondition(
                ConditionStunningStrikeAdvantage.Name,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionStunningStrikeAdvantage.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorHeightenedFocus(
        ConditionDefinition conditionFlurryOfBlowsHeightenedFocus,
        ConditionDefinition conditionFlurryOfBlowsFreedomHeightenedFocus)
        : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private readonly EffectForm _effectForm =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsHeightenedFocus);

        private readonly EffectForm _effectFormFreedom =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsFreedomHeightenedFocus);

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var definition = action.ActionParams.activeEffect.SourceDefinition;
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (definition.Name.StartsWith("PowerMonkPatientDefense"))
            {
                var tempHp = rulesetCharacter.RollDiceAndSum(
                    rulesetCharacter.GetMonkDieType(), RollContext.HealValueRoll, 2, []);

                rulesetCharacter.ReceiveTemporaryHitPoints(
                    tempHp, DurationType.Round, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
            }
            else if (definition.Name.StartsWith("PowerMonkStepOfTheWind"))
            {
                rulesetCharacter.InflictCondition(
                    ConditionGrappleNoCost.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    ConditionGrappleNoCost.Name,
                    0,
                    0,
                    0);
            }

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableMonkHeightenedFocus2024 &&
                   character.GetClassLevel(Monk) >= 10 &&
                   (definition == PowerMonkFlurryOfBlows ||
                    definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement ||
                    definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition == PowerMonkFlurryOfBlows ||
                definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectFormFreedom);
            }

            return effectDescription;
        }
    }

    private sealed class CustomLevelUpLogicMonkBodyAndMind : ICustomLevelUpLogic
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, 4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, 4);
            hero.RefreshAll();
        }

        public void RemoveFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, -4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, -4);
            hero.RefreshAll();
        }
    }

    private sealed class InitiativeEndListenerUncannyMetabolism : IInitiativeEndListener
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerMonkUncannyMetabolism, rulesetCharacter);

            if ((rulesetCharacter.UsedKiPoints == 0 && rulesetCharacter.MissingHitPoints == 0) ||
                rulesetCharacter.GetRemainingUsesOfPower(usablePower) == 0)
            {
                HandlePerfectFocus();

                yield break;
            }

            yield return character.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                character,
                "UncannyMetabolism",
                "CustomReactionUncannyMetabolismDescription"
                    .Formatted(Category.Reaction, rulesetCharacter.UsedKiPoints),
                ReactionValidated,
                HandlePerfectFocus);

            yield break;

            void ReactionValidated()
            {
                var classLevel = rulesetCharacter.GetClassLevel(Monk);
                var healing = classLevel + rulesetCharacter.RollDiceAndSum(
                    rulesetCharacter.GetMonkDieType(), RollContext.HealValueRoll, 1, []);

                // be silent on combat log
                usablePower.remainingUses--;
                rulesetCharacter.UsedKiPoints = 0;
                rulesetCharacter.KiPointsAltered?.Invoke(rulesetCharacter, rulesetCharacter.RemainingKiPoints);
                rulesetCharacter.ReceiveHealing(healing, true, rulesetCharacter.Guid);
                EffectHelpers.StartVisualEffect(
                    character, character, PowerTraditionOpenHandWholenessOfBody, EffectHelpers.EffectType.Caster);
            }

            void HandlePerfectFocus()
            {
                if (!Main.Settings.EnableMonkHeightenedFocus2024 ||
                    rulesetCharacter.GetClassLevel(Monk) < 15 ||
                    rulesetCharacter.RemainingKiPoints >= 4)
                {
                    return;
                }

                rulesetCharacter.UsedKiPoints -= 4 - rulesetCharacter.RemainingKiPoints;
                rulesetCharacter.KiPointsAltered?.Invoke(rulesetCharacter, rulesetCharacter.RemainingKiPoints);
                rulesetCharacter.LogCharacterUsedFeature(FeatureMonkPerfectFocus);
            }
        }
    }

    private sealed class CharacterBeforeTurnEndListenerSelfRestoration : ICharacterBeforeTurnEndListener
    {
        // doing one-by-one so we can prioritize remove charmed first instead of getting them randomly
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var conditionsToRemove = new[]
            {
                RuleDefinitions.ConditionCharmed, //
                RuleDefinitions.ConditionFrightened, //
                RuleDefinitions.ConditionPoisoned //
            };

            foreach (var conditionToRemove in conditionsToRemove)
            {
                var activeCondition = rulesetCharacter.AllConditions.FirstOrDefault(x =>
                    x.ConditionDefinition.IsSubtypeOf(conditionToRemove));

                if (activeCondition == null)
                {
                    continue;
                }

                rulesetCharacter.RemoveCondition(activeCondition);

                break;
            }
        }
    }
}
