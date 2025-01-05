using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
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

    private static readonly ConditionDefinition ConditionWeaponMasteryNick = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasteryNick")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(
            FeatureDefinitionAdditionalActionBuilder
                .Create("AdditionalActionWeaponMasteryNick")
                .SetGuiPresentationNoContent(true)
                .SetActionType(ActionType.Bonus)
                .AddCustomSubFeatures(new PhysicalAttackFinishedByMeNickExtraAttack())
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryNickPreventBonusAttack =
        ConditionDefinitionBuilder
            .Create("ConditionWeaponMasteryNickPreventBonusAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWeaponMasteryNickPreventBonusAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(Id.AttackOff)
                    .AddToDB())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasteryNickPreventAllBonusExceptAttack =
        ConditionDefinitionBuilder
            .Create("ConditionWeaponMasteryNickPreventAllBonusExceptAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWeaponMasteryNickPreventAllBonusExceptAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(
                        Id.CastBonus, Id.DashBonus, Id.DisengageBonus, Id.HideBonus, Id.PowerBonus, Id.ShoveBonus,
                        Id.AssignTargetBonus, Id.UseItemBonus)
                    .AddToDB())
            .AddToDB();

    private static readonly ConditionDefinition ConditionWeaponMasterySap = ConditionDefinitionBuilder
        .Create("ConditionWeaponMasterySap")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByHunter)
        .SetConditionType(ConditionType.Detrimental)
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
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBullsStrength)
        .AddFeatures(
            FeatureDefinitionCombatAffinityBuilder
                .Create("CombatAffinityWeaponMasteryVex")
                .SetGuiPresentationNoContent(true)
                .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                .SetSituationalContext(SituationalContext.SourceHasCondition, ConditionWeaponMasteryVexSelf)
                .AddToDB())
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
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                        .Build())
                .Build())
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
        .AddCustomSubFeatures(new ModifyEffectDescriptionTopple())
        .AddToDB();

    private static readonly Dictionary<WeaponTypeDefinition, MasteryProperty> WeaponMasteryTable = new()
    {
        { CustomWeaponsContext.HalberdWeaponType, MasteryProperty.Cleave },
        { CustomWeaponsContext.HandXbowWeaponType, MasteryProperty.Vex },
        { CustomWeaponsContext.KatanaWeaponType, MasteryProperty.Slow },
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
        BuildCleave();

        foreach (var masteryProperty in Enum.GetValues(typeof(MasteryProperty))
                     .OfType<MasteryProperty>()
                     .Where(x => x != MasteryProperty.Cleave))
        {
            _ = FeatureDefinitionBuilder
                .Create($"FeatureWeaponMastery{masteryProperty}")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
        }

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityWeaponMasteryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryToggle)
            .AddToDB();

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
    }

    private static void BuildCleave()
    {
        var powerCleave = FeatureDefinitionPowerBuilder
            .Create("PowerWeaponMasteryCleave")
            .SetGuiPresentation("FeatureWeaponMasteryCleave", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .Build())
            .DelegatedToAction()
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create("WeaponMasteryCleave")
            .SetGuiPresentation(Category.Action, DatabaseHelper.ActionDefinitions.WhirlwindAttack)
            .SetActionId(ExtraActionId.WeaponMasteryCleave)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Large)
            .RequiresAuthorization()
            .OverrideClassName("UsePower")
            .SetActivatedPower(powerCleave)
            .AddToDB();

        _ = FeatureDefinitionActionAffinityBuilder
            .Create("FeatureWeaponMasteryCleave")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((Id)ExtraActionId.WeaponMasteryCleave)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(c => IsWeaponMasteryValid(
                    GameLocationCharacter.GetFromActor(c), c.GetMainWeapon(), MasteryProperty.Cleave)))
            .AddToDB();
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
        return attackMode.SourceDefinition is ItemDefinition { IsWeapon: true } itemDefinition &&
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
        return attacker.RulesetCharacter.IsToggleEnabled((Id)ExtraActionId.WeaponMasteryToggle) &&
               WeaponMasteryTable.TryGetValue(itemDefinition.WeaponDescription.WeaponTypeDefinition, out var value) &&
               value == property &&
               attacker.RulesetCharacter.Invocations.Any(x =>
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
            // Graze
            //

            if (attacker.GetSpecialFeatureUses("WeaponMasteryGraze") >= 0)
            {
                var abilityScore = attackMode.AbilityScore;
                var abilityScoreValue = rulesetAttacker.TryGetAttributeValue(abilityScore);
                var modifier = AttributeDefinitions.ComputeAbilityScoreModifier(abilityScoreValue);

                if (modifier <= 0)
                {
                    yield break;
                }

                var damageForm =
                    attackMode.EffectDescription.EffectForms.FirstOrDefault(x =>
                        x.FormType == EffectForm.EffectFormType.Damage)?.DamageForm;

                if (damageForm == null)
                {
                    yield break;
                }

                var effectForm = EffectFormBuilder.DamageForm(damageForm.DamageType, bonusDamage: modifier);
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetAttacker,
                    targetCharacter = rulesetDefender,
                    position = defender.LocationPosition
                };

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

            //
            // Nick - this is the only behavior that can trigger at same time with others
            //

            if ((rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                 action.ActionType == ActionType.Main &&
                 ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(rulesetAttacker) &&
                 IsWeaponMasteryValid(attacker, rulesetAttacker.GetMainWeapon(), MasteryProperty.Nick)) ||
                IsWeaponMasteryValid(attacker, rulesetAttacker.GetOffhandWeapon(), MasteryProperty.Nick))
            {
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

                if (!ValidatorsCharacter.HasBonusAttackAvailable(rulesetAttacker))
                {
                    rulesetAttacker.InflictCondition(
                        ConditionWeaponMasteryNickPreventAllBonusExceptAttack.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        ConditionWeaponMasteryNickPreventAllBonusExceptAttack.Name,
                        0,
                        0,
                        0);
                }
            }

            //
            // Push
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Push) &&
                rulesetDefender.SizeDefinition.MaxExtent.x <= 2)
            {
                var usablePower = PowerProvider.Get(PowerWeaponMasteryPush, rulesetAttacker);

                attacker.MyExecuteActionSpendPower(usablePower, defender);
            }

            //
            // Sap
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Sap))
            {
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

            //
            // Slow
            //

            if (damageAmount > 0 &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Slow))
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

            //
            // Topple
            //

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Topple))
            {
                var usablePower = PowerProvider.Get(PowerWeaponMasteryTopple, rulesetAttacker);
                var abilityScore = attackMode.AbilityScore;
                var abilityScoreIndex = Array.IndexOf(AttributeDefinitions.AbilityScoreNames, abilityScore);

                attacker.SetSpecialFeatureUses("WeaponMasteryTopple", abilityScoreIndex);
                attacker.MyExecuteActionSpendPower(usablePower, defender);
            }

            //
            // Vex
            //

            // ReSharper disable once InvertIf
            if (damageAmount > 0 &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Vex))
            {
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
            var rollOutcome = action.AttackRollOutcome;

            attacker.SetSpecialFeatureUses("WeaponMasteryGraze", -1);

            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure &&
                IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Graze))
            {
                attacker.SetSpecialFeatureUses("WeaponMasteryGraze", 0);
            }
            else
            {
                yield break;
            }
        }
    }

    //
    // Nick
    //

    private sealed class PhysicalAttackFinishedByMeNickExtraAttack : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var isOffHandAttack = action.ActionId == Id.AttackOff;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (isOffHandAttack &&
                !IsWeaponMasteryValid(attacker, attackMode, MasteryProperty.Nick) &&
                attacker.RulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionWeaponMasteryNick.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);

                yield break;
            }

            var condition = isOffHandAttack
                ? ConditionWeaponMasteryNickPreventBonusAttack
                : action.ActionType == ActionType.Bonus
                    ? ConditionWeaponMasteryNickPreventAllBonusExceptAttack
                    : null;

            if (!condition)
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    //
    // Topple
    //

    private sealed class ModifyEffectDescriptionTopple : IModifyEffectDescription
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
            var abilityScoreIndex = glc?.GetSpecialFeatureUses("WeaponMasteryTopple") ?? -1;

            if (abilityScoreIndex < 0)
            {
                return effectDescription;
            }

            var abilityScore = AttributeDefinitions.AbilityScoreNames[abilityScoreIndex];

            effectDescription.savingThrowDifficultyAbility = abilityScore;

            return effectDescription;
        }
    }
}
