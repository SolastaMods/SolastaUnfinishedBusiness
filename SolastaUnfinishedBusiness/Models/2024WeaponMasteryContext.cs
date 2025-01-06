using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Patches;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private const string Stage = "WeaponMasteryRelearn";
    private const string IndexUnlearn = "WeaponMasteryUnlearn";
    private const string IndexLearn = "WeaponMasteryLearn";
    private const int StageUnlearn = -1;
    private const int StageLearn = 1;

    private const string WeaponMasteryCleave = "WeaponMasteryCleave";
    private const string WeaponMasteryGraze = "WeaponMasteryGraze";
    private const string WeaponMasteryNick = "WeaponMasteryNick";
    private const string WeaponMasteryTopple = "WeaponMasteryTopple";

    private static readonly FeatureDefinitionPower PowerWeaponMasteryRelearnPool = FeatureDefinitionPowerBuilder
        .Create("PowerWeaponMasteryRelearnPool")
        .SetGuiPresentationNoContent(true)
        .SetShowCasting(false)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddCustomSubFeatures(new MagicEffectFinishedByMeRelearn())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerWeaponMasteryRelearn = FeatureDefinitionPowerBuilder
        .Create("PowerWeaponMasteryRelearn")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerWeaponMasteryRelearn", Resources.PowerWeaponMasteryRelearn, 256, 128))
        .SetShowCasting(false)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddCustomSubFeatures(
            ValidatorsValidatePowerUse.NotInCombat,
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(PowerWeaponMasteryRelearnPool) > 0),
            new PowerOrSpellFinishedByMeRelearn(),
            new CustomBehaviorWeaponMastery())
        .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetFeatWeaponMasteryLearn1 =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetFeatWeaponMasteryLearn1")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .SetFeatureSet(
                PowerWeaponMasteryRelearnPool,
                PowerWeaponMasteryRelearn,
                CustomInvocationPoolDefinitionBuilder
                    .Create("InvocationPoolFeatWeaponMasteryLearn1")
                    .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
                    .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization)
                    .AddToDB())
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetWeaponMasteryLearn1 =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWeaponMasteryLearn1")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .SetFeatureSet(
                PowerWeaponMasteryRelearnPool,
                PowerWeaponMasteryRelearn,
                CustomInvocationPoolDefinitionBuilder
                    .Create("InvocationPoolWeaponMasteryLearn1")
                    .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
                    .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization)
                    .AddToDB())
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetWeaponMasteryLearn2 =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWeaponMasteryLearn2")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .SetFeatureSet(
                PowerWeaponMasteryRelearnPool,
                PowerWeaponMasteryRelearn,
                CustomInvocationPoolDefinitionBuilder
                    .Create("InvocationPoolWeaponMasteryLearn2")
                    .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
                    .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization, 2)
                    .AddToDB())
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetWeaponMasteryLearn3 =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWeaponMasteryLearn3")
            .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
            .SetFeatureSet(
                PowerWeaponMasteryRelearnPool,
                PowerWeaponMasteryRelearn,
                CustomInvocationPoolDefinitionBuilder
                    .Create("InvocationPoolWeaponMasteryLearn3")
                    .SetGuiPresentation("InvocationPoolWeaponMasteryLearn", Category.Feature)
                    .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization, 3)
                    .AddToDB())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryCleave =
        ConditionDefinitionBuilder
            .Create("ConditionWeaponMasteryCleave")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWeaponMasteryCleave")
                    .SetGuiPresentationNoContent(true)
                    .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryCleave)
                    .AddCustomSubFeatures(new ValidateDefinitionApplication(c =>
                    {
                        var glc = GameLocationCharacter.GetFromActor(c);

                        return IsWeaponMasteryValid(glc, c.GetMainWeapon(), MasteryProperty.Cleave);
                    }))
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorConditionCleave())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryNick =
        ConditionDefinitionBuilder
            .Create("ConditionWeaponMasteryNick")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWeaponMasteryNick")
                    .SetGuiPresentationNoContent(true)
                    .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryNick)
                    .AddCustomSubFeatures(new ValidateDefinitionApplication(c =>
                    {
                        var glc = GameLocationCharacter.GetFromActor(c);

                        // the condition gets removed on all other scenarios
                        // right after AttackOff or WeaponMasteryNick executes

                        return IsWeaponMasteryValid(glc, c.GetMainWeapon(), MasteryProperty.Nick) ||
                               IsWeaponMasteryValid(glc, c.GetOffhandWeapon(), MasteryProperty.Nick);
                    }))
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorConditionNick())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryNickPreventAttackOff =
        ConditionDefinitionBuilder
            .Create("ConditionWeaponMasteryNickPreventAttackOff")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWeaponMasteryNickPreventBonusAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(Id.AttackOff)
                    .AddToDB())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasterySap = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasterySap")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionCursed)
        .SetConditionType(ConditionType.Detrimental)
        .SetSilent(Silent.WhenAdded)
        .AddFeatures(
            FeatureDefinitionCombatAffinityBuilder
                .Create("CombatAffinityWeaponMasterySap")
                .SetGuiPresentationNoContent(true)
                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                .AddToDB())
        .SetSpecialInterruptions(ConditionInterruption.Attacks)
        .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasterySlow = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasterySlow")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionSlowed)
        .SetConditionType(ConditionType.Detrimental)
        .SetSilent(Silent.WhenAdded)
        .AddFeatures(
            FeatureDefinitionMovementAffinityBuilder
                .Create("MovementAffinityWeaponMasterySlow")
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(-2)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryVexSelf = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasteryVexSelf")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryVex = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasteryVex")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByHunter)
        .SetConditionType(ConditionType.Detrimental)
        .SetSilent(Silent.WhenAdded)
        .AddFeatures(
            FeatureDefinitionCombatAffinityBuilder
                .Create("CombatAffinityWeaponMasteryVex")
                .SetGuiPresentationNoContent(true)
                .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                .SetSituationalContext(SituationalContext.SourceHasCondition, ConditionWeaponMasteryVexSelf)
                .AddToDB())
        .AddCustomSubFeatures(new CustomBehaviorVex())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerWeaponMasteryPush = FeatureDefinitionPowerBuilder
        .Create("PowerWeaponMasteryPush")
        .SetGuiPresentation("FeatureWeaponMasteryPush", Category.Feature, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                        .Build())
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorPush())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerWeaponMasteryTopple = FeatureDefinitionPowerBuilder
        .Create("PowerWeaponMasteryTopple")
        .SetGuiPresentation("FeatureWeaponMasteryTopple", Category.Feature, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetMotionForm(MotionForm.MotionType.FallProne)
                        .Build())
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorTopple())
        .AddToDB();

    private static readonly Dictionary<WeaponTypeDefinition, MasteryProperty> WeaponMasteryTable = new()
    {
        { CustomWeaponsContext.HalberdWeaponType, MasteryProperty.Cleave },
        { CustomWeaponsContext.HandXbowWeaponType, MasteryProperty.Vex },
        { CustomWeaponsContext.KatanaWeaponType, MasteryProperty.Sap },
        { CustomWeaponsContext.LongMaceWeaponType, MasteryProperty.Sap },
        { CustomWeaponsContext.PikeWeaponType, MasteryProperty.Push },
        { WeaponTypeDefinitions.BattleaxeType, MasteryProperty.Topple },
        { WeaponTypeDefinitions.ClubType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.DaggerType, MasteryProperty.Nick },
        { WeaponTypeDefinitions.DartType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.GreataxeType, MasteryProperty.Cleave },
        { WeaponTypeDefinitions.GreatswordType, MasteryProperty.Graze },
        { WeaponTypeDefinitions.HandaxeType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.HeavyCrossbowType, MasteryProperty.Push },
        { WeaponTypeDefinitions.JavelinType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.LightCrossbowType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.LongbowType, MasteryProperty.Slow },
        { WeaponTypeDefinitions.LongswordType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.MaceType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.MaulType, MasteryProperty.Topple },
        { WeaponTypeDefinitions.MorningstarType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.QuarterstaffType, MasteryProperty.Topple },
        { WeaponTypeDefinitions.RapierType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.ScimitarType, MasteryProperty.Nick },
        { WeaponTypeDefinitions.ShortbowType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.ShortswordType, MasteryProperty.Vex },
        { WeaponTypeDefinitions.SpearType, MasteryProperty.Sap },
        { WeaponTypeDefinitions.WarhammerType, MasteryProperty.Push }
    };

    private static void LoadWeaponMastery()
    {
        // Cleave

        var powerCleave = FeatureDefinitionPowerBuilder
            .Create("PowerWeaponMasteryCleave")
            .SetGuiPresentation("FeatureWeaponMasteryCleave", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .Build())
            .DelegatedToAction()
            .AddCustomSubFeatures(new CustomBehaviorCleave())
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(WeaponMasteryCleave)
            .SetGuiPresentation(Category.Action, DatabaseHelper.ActionDefinitions.WhirlwindAttack)
            .SetActionId(ExtraActionId.WeaponMasteryCleave)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Large)
            .RequiresAuthorization()
            .OverrideClassName("UsePower")
            .SetActivatedPower(powerCleave)
            .AddToDB();

        // Nick

        var powerNick = FeatureDefinitionPowerBuilder
            .Create("PowerWeaponMasteryNick")
            .SetGuiPresentation("FeatureWeaponMasteryNick", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .Build())
            .DelegatedToAction()
            .AddCustomSubFeatures(new CustomBehaviorNick())
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(WeaponMasteryNick)
            .SetGuiPresentation(Category.Action, Sprites.GetSprite("ActionNick", Resources.ActionNick, 144))
            .SetActionId(ExtraActionId.WeaponMasteryNick)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Large)
            .RequiresAuthorization()
            .OverrideClassName("UsePower")
            .SetActivatedPower(powerNick)
            .AddToDB();

        // create a feature for every mastery property

        foreach (MasteryProperty masteryProperty in Enum.GetValues(typeof(MasteryProperty)))
        {
            _ = FeatureDefinitionBuilder
                .Create($"FeatureWeaponMastery{masteryProperty}")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }

        // master toggle

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityWeaponMasteryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryToggle)
            .AddCustomSubFeatures(new ValidateDefinitionApplication(c =>
                !c.IsToggleEnabled((Id)ExtraActionId.TacticalMasterToggle)))
            .AddToDB();

        // level up custom invocations and re-learn powers

        var powers = new List<FeatureDefinitionPower>();

        foreach (var kvp in WeaponMasteryTable)
        {
            var weaponTypeDefinition = kvp.Key;
            var weaponTypeName = weaponTypeDefinition.Name;
            var masteryProperty = kvp.Value;
            var featureSpecialization = GetDefinition<FeatureDefinition>($"FeatureWeaponMastery{masteryProperty}");
            var featureSet = FeatureDefinitionFeatureSetBuilder
                .Create($"FeatureSetWeaponMastery{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetFeatureSet(actionAffinityToggle, featureSpecialization)
                .AddToDB();

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocationWeaponMastery{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    featureSpecialization.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.WeaponMasterySpecialization)
                .SetGrantedFeature(featureSet)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();

            var powerRelearnWeapon = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerWeaponMasteryRelearn{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.FormatTitle(), featureSpecialization.GuiPresentation.Description)
                .SetShowCasting(false)
                .SetSharedPool(ActivationTime.NoCost, PowerWeaponMasteryRelearnPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .Build())
                .AddToDB();

            powerRelearnWeapon.GuiPresentation.hidden = true;
            powers.Add(powerRelearnWeapon);
        }

        PowerBundle.RegisterPowerBundle(PowerWeaponMasteryRelearnPool, false, powers);

        if (!Main.Settings.UseWeaponMasterySystem || !Main.Settings.UseWeaponMasterySystemAddWeaponTag)
        {
            return;
        }

        foreach (var weapon in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon))
        {
            var weaponType = weapon.WeaponDescription.WeaponTypeDefinition;

            if (WeaponMasteryTable.TryGetValue(weaponType, out var masteryProperty))
            {
                weapon.WeaponDescription.WeaponTags.Add($"{masteryProperty}");
            }
        }
    }

    internal static void SwitchWeaponMastery()
    {
        var klasses = new[] { Barbarian, Fighter, Paladin, Ranger, Rogue };

        foreach (var klass in klasses)
        {
            klass.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == FeatureSetWeaponMasteryLearn1 ||
                x.FeatureDefinition == FeatureSetWeaponMasteryLearn2 ||
                x.FeatureDefinition == FeatureSetWeaponMasteryLearn3);
        }

        if (!Main.Settings.UseWeaponMasterySystem)
        {
            return;
        }

        Barbarian.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn2, 1),
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn1, 4),
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn1, 10));
        Fighter.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn3, 1),
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn1, 4),
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn1, 10),
            new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn1, 16));
        Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn2, 1));
        Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn2, 1));
        Rogue.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetWeaponMasteryLearn2, 1));
    }

    private static bool IsWeaponMasteryValid(
        GameLocationCharacter attacker, RulesetAttackMode attackMode, MasteryProperty property)
    {
        return attackMode?.SourceDefinition is ItemDefinition { IsWeapon: true } itemDefinition &&
               IsWeaponMasteryValid(attacker, itemDefinition, property);
    }

    private static bool IsWeaponMasteryValid(
        GameLocationCharacter attacker, RulesetItem rulesetItem, MasteryProperty property)
    {
        return rulesetItem is { ItemDefinition: { IsWeapon: true } itemDefinition } &&
               IsWeaponMasteryValid(attacker, itemDefinition, property);
    }

    private static bool IsWeaponMasteryValid(
        GameLocationCharacter attacker, ItemDefinition itemDefinition, MasteryProperty property)
    {
        var rulesetCharacter = attacker.RulesetCharacter;

        return (rulesetCharacter.IsToggleEnabled((Id)ExtraActionId.WeaponMasteryToggle) ||
                rulesetCharacter.IsToggleEnabled((Id)ExtraActionId.TacticalMasterToggle)) &&
               WeaponMasteryTable.TryGetValue(itemDefinition.WeaponDescription.WeaponTypeDefinition, out var value) &&
               value == property &&
               rulesetCharacter.Invocations.Any(x =>
                   x.InvocationDefinition.Name ==
                   $"CustomInvocationWeaponMastery{itemDefinition.WeaponDescription.WeaponTypeDefinition.Name}");
    }

    private enum MasteryProperty
    {
        Cleave,
        Graze,
        Nick,
        Push,
        Sap,
        Slow,
        Topple,
        Vex
    }

    //
    // Weapon Mastery
    //

    private sealed class CustomBehaviorWeaponMastery
        : IPhysicalAttackInitiatedByMe, ITryAlterOutcomeAttack, IPhysicalAttackFinishedByMe
    {
        private const string WeaponMasteryGuard = "WeaponMasteryGuard";

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            // avoid double dip if MC any 2 classes with mastery or has feat weapon mastery
            if (attacker.GetSpecialFeatureUses(WeaponMasteryGuard) >= 0)
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(WeaponMasteryGuard, 0);

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            //
            // Nick
            //

            if (action.ActionType == ActionType.Main &&
                ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(rulesetAttacker) &&
                (rulesetAttacker.UsedBonusAttacks == 0 ||
                 ValidatorsCharacter.HasAvailableBonusAction(rulesetAttacker)) &&
                (IsWeaponMasteryValid(attacker, rulesetAttacker.GetMainWeapon(), MasteryProperty.Nick) ||
                 IsWeaponMasteryValid(attacker, rulesetAttacker.GetOffhandWeapon(), MasteryProperty.Nick)) &&
                attacker.OnceInMyTurnIsValid(WeaponMasteryNick))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Nick);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoNick(attacker);
                }

                attacker.SetSpecialFeatureUses(WeaponMasteryNick, 0);

                // remove this to allow Nick to trigger with others
                yield break;
            }

            //
            // Cleave - here to only support Tactical Master
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Cleave) &&
                attacker.OnceInMyTurnIsValid(WeaponMasteryCleave))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Cleave);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoCleave(attacker, defender);
                }

                attacker.SetSpecialFeatureUses(WeaponMasteryCleave, 0);

                yield break;
            }

            //
            // Graze
            //

            if (attacker.GetSpecialFeatureUses(WeaponMasteryGraze) >= 0)
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Graze);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoGraze(attacker, defender, attackMode);
                }

                yield break;
            }

            //
            // Push
            //

            if (rulesetDefender.SizeDefinition.MaxExtent.x <= 2 &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Push))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Push);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoPush(attacker, defender);
                }

                yield break;
            }

            //
            // Sap
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Sap))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Sap);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoSap(attacker, defender);
                }

                yield break;
            }

            //
            // Slow
            //

            if (damageAmount > 0 &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Slow))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Slow);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoSlow(attacker, defender);
                }

                yield break;
            }

            //
            // Topple
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Topple))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Topple);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoTopple(attacker, defender, attackMode);
                }

                yield break;
            }

            //
            // Vex
            //

            // ReSharper disable once InvertIf
            if (damageAmount > 0 &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Vex))
            {
                yield return HandleFighterTacticalMaster(action, attacker, defender, MasteryProperty.Vex);

                if (attacker.GetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name) < 0)
                {
                    DoVex(attacker, defender);
                }
            }
        }

        //
        // prevent this behavior to trigger more than once on MC / Feat Mastery integration scenarios
        //

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            attacker.SetSpecialFeatureUses(WeaponMasteryGuard, -1);

            yield break;
        }

        //
        // Graze
        //

        public int HandlerPriority => -1;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var isValid = action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure &&
                          IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Graze);

            attacker.SetSpecialFeatureUses(WeaponMasteryGraze, isValid ? 0 : -1);

            yield break;
        }

        //
        // Fighter Tactical Master
        //

        private static IEnumerator HandleFighterTacticalMaster(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            MasteryProperty masteryToReplace)
        {
            attacker.SetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name, -1);

            if (!Main.Settings.EnableFighterTacticalMaster2024 ||
                attacker.RulesetCharacter.GetClassLevel(Fighter) < 9)
            {
                yield break;
            }

            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerFighterTacticalMasterPool, rulesetCharacter);
            var usablePowers = new[] { MasteryProperty.Push, MasteryProperty.Sap, MasteryProperty.Slow }
                .Where(x => x != masteryToReplace)
                .Select(mastery =>
                    PowerProvider.Get(GetDefinition<FeatureDefinitionPower>($"PowerFighterTacticalMaster{mastery}")))
                .ToArray();

            rulesetCharacter.UsablePowers.AddRange(usablePowers);

            yield return character.MyReactToSpendPowerBundle(
                usablePower,
                [character],
                character,
                FeatureSetFighterTacticalMaster.Name,
                $"ReactionSpendPowerBundle{FeatureSetFighterTacticalMaster.Name}Description"
                    .Formatted(Category.Reaction, $"FeatureWeaponMastery{masteryToReplace}Title"
                        .Localized(Category.Feature)),
                ReactionValidated);

            usablePowers.Do(x => rulesetCharacter.UsablePowers.Remove(x));

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                attacker.SetSpecialFeatureUses(FeatureSetFighterTacticalMaster.Name, 0);

                switch (reactionRequest.SelectedSubOption)
                {
                    case 0:
                        DoPush(attacker, defender);
                        break;
                    case 1:
                        DoSap(attacker, defender);
                        break;
                    case 2:
                        DoSlow(attacker, defender);
                        break;
                }
            }
        }

        #region Behaviors

        private static void DoCleave(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                ConditionWeaponMasteryCleave.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                ConditionWeaponMasteryCleave.Name,
                0,
                0,
                0);
        }

        private static void DoGraze(
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackMode)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;
            var abilityScore = attackMode.AbilityScore;
            var abilityScoreValue = rulesetAttacker.TryGetAttributeValue(abilityScore);
            var modifier = AttributeDefinitions.ComputeAbilityScoreModifier(abilityScoreValue);

            if (modifier <= 0)
            {
                return;
            }

            var damageForm =
                attackMode.EffectDescription.EffectForms.FirstOrDefault(x =>
                    x.FormType == EffectForm.EffectFormType.Damage)?.DamageForm;

            if (damageForm == null)
            {
                return;
            }

            var effectForm = EffectFormBuilder.DamageForm(damageForm.DamageType, bonusDamage: modifier);
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            rulesetAttacker.LogCharacterUsedFeature(GetDefinition<FeatureDefinition>("FeatureWeaponMasteryGraze"));

            RulesetActor.InflictDamage(
                modifier,
                effectForm.DamageForm,
                effectForm.DamageForm.DamageType,
                applyFormsParams,
                rulesetDefender,
                false,
                attacker.Guid,
                false,
                [],
                new RollInfo(DieType.D1, [], modifier),
                false,
                out _);
        }

        private static void DoNick(GameLocationCharacter attacker)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                ConditionWeaponMasteryNick.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWeaponMasteryNick.Name,
                0,
                0,
                0);
        }

        private static void DoPush(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerWeaponMasteryPush, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }

        private static void DoSap(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            rulesetAttacker.LogCharacterUsedFeature(GetDefinition<FeatureDefinition>("FeatureWeaponMasterySap"));

            rulesetDefender.InflictCondition(
                ConditionWeaponMasterySap.Name,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWeaponMasterySap.Name,
                0,
                0,
                0);
        }

        private static void DoSlow(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            rulesetAttacker.LogCharacterUsedFeature(GetDefinition<FeatureDefinition>("FeatureWeaponMasterySlow"));

            if (!rulesetDefender.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasterySlow.Name))
            {
                rulesetDefender.InflictCondition(
                    ConditionWeaponMasterySlow.Name,
                    DurationType.Round,
                    1,
                    (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    ConditionWeaponMasterySlow.Name,
                    0,
                    0,
                    0);
            }
        }

        private static void DoTopple(
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackMode)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerWeaponMasteryTopple, rulesetAttacker);
            var abilityScore = attackMode.AbilityScore;
            var abilityScoreIndex = Array.IndexOf(AttributeDefinitions.AbilityScoreNames, abilityScore);

            attacker.SetSpecialFeatureUses(WeaponMasteryTopple, abilityScoreIndex);
            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }

        private static void DoVex(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            rulesetAttacker.LogCharacterUsedFeature(GetDefinition<FeatureDefinition>("FeatureWeaponMasteryVex"));

            rulesetAttacker.InflictCondition(
                ConditionWeaponMasteryVexSelf.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWeaponMasteryVexSelf.Name,
                0,
                0,
                0);

            rulesetDefender.InflictCondition(
                ConditionWeaponMasteryVex.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWeaponMasteryVex.Name,
                0,
                0,
                0);
        }

        #endregion
    }

    //
    // Cleave
    //

    private sealed class CustomBehaviorCleave : IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var attacker = __instance.ActionParams.ActingCharacter;
            var attackMode = attacker.FindActionAttackMode(Id.AttackMain);

            if (!attacker.RulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasteryCleave.Name, out var activeCondition))
            {
                __instance.actionModifier.FailureFlags.Add(Gui.Localize("Failure/&CannotAttackTarget"));

                return false;
            }

            var rulesetFirstTarget = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);
            var firstTarget = GameLocationCharacter.GetFromActor(rulesetFirstTarget);

            if (!attacker.IsWithinRange(target, attackMode.reachRange) ||
                target == firstTarget)
            {
                __instance.actionModifier.FailureFlags.Add(Gui.Localize("Failure/&CannotAttackTarget"));

                return false;
            }

            if (firstTarget.IsWithinRange(target, 1))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Failure/&SecondTargetNotWithinRange");

            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var attackMode = attacker.FindActionAttackMode(Id.AttackMain);

            attacker.SetSpecialFeatureUses(WeaponMasteryCleave, 0);
            attackMode.AddAttackTagAsNeeded(WeaponMasteryCleave);

            var target = action.actionParams.TargetCharacters[0];

            var actionAttack = new CharacterActionAttack(
                new CharacterActionParams(
                    attacker,
                    Id.AttackFree,
                    attackMode,
                    target,
                    new ActionModifier()));

            yield return CharacterActionAttackPatcher.ExecuteImpl_Patch.ExecuteImpl(actionAttack);
        }
    }

    private sealed class CustomBehaviorConditionCleave : IModifyWeaponAttackMode, IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasteryCleave.Name, out var activeCondition))
            {
                yield break;
            }

            if (activeCondition.Amount == 1)
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            activeCondition.Amount = 1;
        }

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!attackMode.AttackTags.Contains(WeaponMasteryCleave))
            {
                return;
            }

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();
            var attributeDamage =
                damageForm.DamageBonusTrends.FirstOrDefault(x => x.sourceType == FeatureSourceType.AbilityScore);

            damageForm.BonusDamage -= attributeDamage.value;
            damageForm.DamageBonusTrends.RemoveAll(x => x.sourceType == FeatureSourceType.AbilityScore);
        }
    }

    //
    // Nick
    //

    private sealed class CustomBehaviorNick : IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var attacker = __instance.ActionParams.ActingCharacter;
            var attackMode = attacker.FindActionAttackMode(Id.AttackOff);

            //TODO: allow thrown scenarios with Nick
            if (attacker.IsWithinRange(target, attackMode.ReachRange))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add(Gui.Localize("Failure/&CannotAttackTarget"));

            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var attackMode = attacker.FindActionAttackMode(Id.AttackOff);

            attacker.SetSpecialFeatureUses(WeaponMasteryNick, 0);
            attackMode.AddAttackTagAsNeeded(TwoWeaponCombatFeats.DualFlurryTriggerMark);

            var target = action.actionParams.TargetCharacters[0];

            var actionAttack = new CharacterActionAttack(
                new CharacterActionParams(
                    attacker,
                    Id.AttackFree,
                    attackMode,
                    target,
                    new ActionModifier()));

            yield return CharacterActionAttackPatcher.ExecuteImpl_Patch.ExecuteImpl(actionAttack);

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                ConditionWeaponMasteryNickPreventAttackOff.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWeaponMasteryNickPreventAttackOff.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorConditionNick : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (action.ActionId is Id.AttackOff or (Id)ExtraActionId.WeaponMasteryNick &&
                rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasteryNick.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    //
    // Push
    //

    private sealed class CustomBehaviorPush : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerWeaponMasteryTopple;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.UseWeaponMasterySystemPushSave)
            {
                effectDescription.hasSavingThrow = false;
                effectDescription.EffectForms[0].hasSavingThrow = false;

                return effectDescription;
            }

            var glc = GameLocationCharacter.GetFromActor(character);
            var abilityScoreIndex = glc?.GetSpecialFeatureUses(WeaponMasteryTopple) ?? -1;

            if (abilityScoreIndex < 0)
            {
                return effectDescription;
            }

            var abilityScore = AttributeDefinitions.AbilityScoreNames[abilityScoreIndex];

            effectDescription.savingThrowDifficultyAbility = abilityScore;

            return effectDescription;
        }
    }

    //
    // Topple
    //

    private sealed class CustomBehaviorTopple : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerWeaponMasteryTopple;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);
            var abilityScoreIndex = glc?.GetSpecialFeatureUses(WeaponMasteryTopple) ?? -1;

            if (abilityScoreIndex < 0)
            {
                return effectDescription;
            }

            var abilityScore = AttributeDefinitions.AbilityScoreNames[abilityScoreIndex];

            effectDescription.savingThrowDifficultyAbility = abilityScore;

            return effectDescription;
        }
    }

    //
    // Vex
    //

    private sealed class CustomBehaviorVex : IPhysicalAttackFinishedOnMe
    {
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

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasteryVex.Name, out var activeCondition) ||
                activeCondition.SourceGuid != attacker.Guid)
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);
        }
    }

    #region Relearn

    //
    // Relearn
    //

    private sealed class PowerOrSpellFinishedByMeRelearn : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            const string InvocationNamePrefix = "CustomInvocationWeaponMastery";
            var character = action.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;
            var aborted = false;
            var usablePowers = new List<RulesetUsablePower>();
            var usablePower = PowerProvider.Get(PowerWeaponMasteryRelearnPool, rulesetCharacter);

            //
            // UNLEARN
            //

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var invocation in rulesetCharacter.Invocations
                         .Where(x => x.InvocationDefinition.Name.StartsWith(InvocationNamePrefix)))
            {
                var weaponTypeName = invocation.InvocationDefinition.Name.Replace(InvocationNamePrefix, string.Empty);
                var power = GetDefinition<FeatureDefinitionPower>($"PowerWeaponMasteryRelearn{weaponTypeName}");

                usablePowers.Add(PowerProvider.Get(power, rulesetCharacter));
            }

            rulesetCharacter.UsablePowers.AddRange(usablePowers);
            character.SetSpecialFeatureUses(Stage, StageUnlearn);

            yield return character.MyReactToSpendPowerBundle(
                usablePower,
                [character],
                character,
                "WeaponMasteryRelearn",
                "ReactionSpendPowerBundleWeaponMasteryUnlearnDescription".Localized(Category.Reaction),
                ReactionValidatedUnlearn,
                ReactionNotValidated);

            usablePowers.Do(x => rulesetCharacter.UsablePowers.Remove(x));

            if (aborted)
            {
                yield break;
            }

            //
            // LEARN
            //

            usablePowers.Clear();

            var weaponTypes = rulesetCharacter.Invocations.Where(x =>
                    x.InvocationDefinition.Name.StartsWith(InvocationNamePrefix)).Select(x =>
                    x.InvocationDefinition.Name.Replace(InvocationNamePrefix, string.Empty))
                .ToList();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var weaponTypeDefinition in WeaponMasteryTable.Keys
                         .Where(x => !weaponTypes.Contains(x.Name)))
            {
                var weaponTypeName = weaponTypeDefinition.Name;
                var power = GetDefinition<FeatureDefinitionPower>($"PowerWeaponMasteryRelearn{weaponTypeName}");

                usablePowers.Add(PowerProvider.Get(power, rulesetCharacter));
            }

            rulesetCharacter.UsablePowers.AddRange(usablePowers);
            character.SetSpecialFeatureUses(Stage, StageLearn);

            yield return character.MyReactToSpendPowerBundle(
                usablePower,
                [character],
                character,
                "WeaponMasteryRelearn",
                "ReactionSpendPowerBundleWeaponMasteryLearnDescription".Localized(Category.Reaction),
                ReactionValidatedLearn,
                ReactionNotValidated);

            usablePowers.Do(x => rulesetCharacter.UsablePowers.Remove(x));

            yield break;

            void ReactionValidatedUnlearn(ReactionRequestSpendBundlePower reactionRequest)
            {
                character.SetSpecialFeatureUses(IndexUnlearn, reactionRequest.SelectedSubOption);
            }

            void ReactionValidatedLearn(ReactionRequestSpendBundlePower reactionRequest)
            {
                character.SetSpecialFeatureUses(IndexLearn, reactionRequest.SelectedSubOption);
            }

            void ReactionNotValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                aborted = true;
                usablePower.remainingUses++;
            }
        }
    }

    private sealed class MagicEffectFinishedByMeRelearn : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            if (!action.ActionParams.RulesetEffect.SourceDefinition.Name.StartsWith("PowerWeaponMasteryRelearn"))
            {
                yield break;
            }

            if (attacker.GetSpecialFeatureUses(Stage) < 0)
            {
                yield break;
            }

            var indexUnlearn = attacker.GetSpecialFeatureUses(IndexUnlearn);
            var weaponTypeUnlearnName = WeaponMasteryTable.Keys.ToArray()[indexUnlearn].Name;
            var invocationToUnlearn =
                GetDefinition<InvocationDefinition>($"CustomInvocationWeaponMastery{weaponTypeUnlearnName}");

            var indexLearn = attacker.GetSpecialFeatureUses(IndexLearn);
            var weaponTypeLearnName = WeaponMasteryTable.Keys.ToArray()[indexLearn].Name;
            var invocationToLearn =
                GetDefinition<InvocationDefinition>($"CustomInvocationWeaponMastery{weaponTypeLearnName}");

            var hero = attacker.RulesetCharacter.GetOriginalHero();

            hero!.TrainedInvocations.Remove(invocationToUnlearn);
            hero.TrainedInvocations.Add(invocationToLearn);
            hero.GrantInvocations();
        }
    }

    #endregion
}
