using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class CharacterContext
{
    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceBarbarian =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceBarbarian")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                // BlindFighting
                // Crippling
                // Defense
                "Dueling",
                // Executioner
                "GreatWeapon",
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceMonk =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceMonk")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                // BlindFighting
                // Crippling
                // Defense
                "Dueling",
                // Executioner
                // GreatWeapon
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceRogue =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceRogue")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                // BlindFighting
                // Crippling
                "Defense",
                // Dueling
                // Executioner
                // GreatWeapon
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    #region Barbarian

    private const string BrutalStrike = "BarbarianBrutalStrike";

    private static ConditionDefinition _conditionBrutalStrike;
    private static ConditionDefinition _conditionHamstringBlow;
    private static ConditionDefinition _conditionStaggeringBlow;
    private static ConditionDefinition _conditionStaggeringBlowAoO;
    private static ConditionDefinition _conditionSunderingBlow;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrike;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement13;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement17;

    private static void BuildBarbarianBrutalStrike()
    {
        const string BrutalStrikeImprovement13 = "BarbarianBrutalStrikeImprovement13";
        const string BrutalStrikeImprovement17 = "BarbarianBrutalStrikeImprovement17";

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageBrutalStrike")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("BrutalStrike")
                .SetDamageDice(DieType.D10, 1)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 9)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    ClassHolder.Barbarian,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, _, _) => (OperationType.Set,
                        character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.BrutalStrikeToggle))))
                .AddToDB();

        _conditionBrutalStrike = ConditionDefinitionBuilder
            .Create($"Condition{BrutalStrike}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageBrutalStrike)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new CustomBehaviorBrutalStrike(powerPool));

        // Forceful Blow

        var powerForcefulBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}ForcefulBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Hamstring Blow

        var powerHamstringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}HamstringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionHamstringBlow = ConditionDefinitionBuilder
            .Create("ConditionHamstringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature,
                ConditionDefinitions.ConditionHindered)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityHamstringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(-3)
                    .AddToDB())
            .CopyParticleReferences(ConditionDefinitions.ConditionSlowed)
            .AddToDB();

        _conditionHamstringBlow.GuiPresentation.description = Gui.EmptyContent;

        // Staggering Blow

        var powerStaggeringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}StaggeringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionStaggeringBlow = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature,
                ConditionDefinitions.ConditionDazzled)
            .SetSilent(Silent.WhenRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create("SavingThrowAffinityStaggeringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.SavingThrow)
            .CopyParticleReferences(ConditionDefinitions.ConditionDazzled)
            .AddToDB();

        _conditionStaggeringBlow.GuiPresentation.description = Gui.EmptyContent;

        _conditionStaggeringBlowAoO = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlowAoO")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(SrdAndHouseRulesContext.ActionAffinityConditionBlind)
            .AddToDB();

        // Sundering Blow

        var additionalDamageSunderingBlow = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{BrutalStrike}SunderingBlow")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("SunderingBlow")
            .SetDamageDice(DieType.D10, 1)
            .AddToDB();

        var conditionSunderingBlowAlly = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlowAlly")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageSunderingBlow)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerSunderingBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}SunderingBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionSunderingBlow = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlow")
            .SetGuiPresentation($"Power{BrutalStrike}SunderingBlow", Category.Feature,
                ConditionDefinitions.ConditionTargetedByGuidingBolt)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddCustomSubFeatures(new CustomBehaviorSunderingBlow(powerSunderingBlow, conditionSunderingBlowAlly))
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttackedNotBySource)
            .CopyParticleReferences(ConditionDefinitions.ConditionLeadByExampleMarked)
            .AddToDB();

        // MAIN

        PowerBundle.RegisterPowerBundle(powerPool, true,
            powerForcefulBlow, powerHamstringBlow, powerStaggeringBlow, powerSunderingBlow);

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityBrutalStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.BrutalStrikeToggle)
            .AddToDB();

        _featureSetBarbarianBrutalStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPool, actionAffinityToggle, powerForcefulBlow, powerHamstringBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement13 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement13}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerStaggeringBlow, powerSunderingBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement17 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement17}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private sealed class CustomBehaviorBrutalStrike(FeatureDefinitionPower powerBarbarianBrutalStrike)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private static readonly EffectForm ForcefulBlowForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
            .Build();

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid(BrutalStrike) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                _conditionBrutalStrike.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionBrutalStrike.Name,
                0,
                0,
                0);

            var usablePower = PowerProvider.Get(powerBarbarianBrutalStrike, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerBarbarianBrutalStrike.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                // determine selected power to collect cost
                var option = reactionRequest.SelectedSubOption;
                var subPowers = powerBarbarianBrutalStrike.GetBundle()?.SubPowers;

                if (subPowers == null)
                {
                    return;
                }

                var selectedPower = subPowers[option];

                switch (selectedPower.Name)
                {
                    case $"Power{BrutalStrike}ForcefulBlow":
                        actualEffectForms.Add(ForcefulBlowForm);
                        break;
                    case $"Power{BrutalStrike}HamstringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionHamstringBlow.Name);
                        break;
                    case $"Power{BrutalStrike}StaggeringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlow.Name);
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlowAoO.Name);
                        break;
                    case $"Power{BrutalStrike}SunderingBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionSunderingBlow.Name);
                        break;
                }
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackMode,
            RollOutcome rollOutcome, int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(BrutalStrike, 0);
        }

        private static void InflictCondition(
            RulesetCharacter rulesetAttacker, RulesetCharacter rulesetDefender, string conditionName)
        {
            rulesetDefender.InflictCondition(
                conditionName,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorSunderingBlow(
        FeatureDefinitionPower powerSunderingBlow,
        ConditionDefinition conditionSunderingBlowAlly) : IPhysicalAttackInitiatedOnMe, IMagicEffectAttackInitiatedOnMe
    {
        public IEnumerator OnMagicEffectAttackInitiatedOnMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            var damageType = activeEffect.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var damageType = attackMode.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        private void AddBonusAttackAndDamageRoll(
            RulesetCharacter rulesetAttacker,
            RulesetActor rulesetDefender,
            ActionModifier actionModifier)
        {
            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _conditionSunderingBlow.Name, out var activeCondition))
            {
                return;
            }

            var rulesetSource = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetAttacker == rulesetSource)
            {
                return;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var bonusAttackRoll =
                rulesetAttacker.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);

            actionModifier.AttackRollModifier += bonusAttackRoll;
            actionModifier.AttacktoHitTrends.Add(new TrendInfo(
                bonusAttackRoll, FeatureSourceType.CharacterFeature, powerSunderingBlow.Name, powerSunderingBlow));

            rulesetAttacker.InflictCondition(
                conditionSunderingBlowAlly.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                FactionDefinitions.Party.Name,
                1,
                conditionSunderingBlowAlly.Name,
                0,
                0,
                0);
        }
    }

    internal static void SwitchBarbarianBrutalStrike()
    {
        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrike, 9));
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement13, 13));
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement17, 17));
        }
        else
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 9 && x.FeatureDefinition == _featureSetBarbarianBrutalStrike);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 13 && x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement13);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 17 && x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement17);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchBarbarianBrutalCritical()
    {
        if (Main.Settings.DisableBarbarianBrutalCritical)
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 9 && x.FeatureDefinition == FeatureSetBarbarianBrutalCritical);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 13 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 17 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);
        }
        else
        {
            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 9 && x.FeatureDefinition == FeatureSetBarbarianBrutalCritical))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(FeatureSetBarbarianBrutalCritical, 9));
            }

            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 13 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 13));
            }

            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 17 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 17));
            }
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchBarbarianRecklessSameBuffDebuffDuration()
    {
        RecklessAttack.GuiPresentation.description = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration
            ? "Action/&RecklessAttackExtendedDescription"
            : "Action/&RecklessAttackDescription";
    }

    internal static void SwitchBarbarianRegainOneRageAtShortRest()
    {
        FeatureSetBarbarianRage.GuiPresentation.description = Main.Settings.EnableBarbarianRegainOneRageAtShortRest
            ? "Feature/&FeatureSetRageExtendedDescription"
            : "Feature/&FeatureSetRageDescription";
    }

    internal static void SwitchBarbarianFightingStyle()
    {
        if (Main.Settings.EnableBarbarianFightingStyle)
        {
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceBarbarian, 2));
        }
        else
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 2 && x.FeatureDefinition == FightingStyleChoiceBarbarian);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    #endregion

    #region Monk

    private static readonly FeatureDefinition FeatureMonkHeightenedMetabolism = FeatureDefinitionBuilder
        .Create("FeatureMonkHeightenedMetabolism")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(
            new CustomBehaviorHeightenedMetabolism(
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusHeightenedMetabolism")
                            .SetUnarmedStrike(3)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsFreedomHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedomHeightenedMetabolism")
                            .SetUnarmedStrike(4)
                            .AddToDB())
                    .AddToDB()))
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkStepOfTheWindHeightenedMetabolism =
        FeatureDefinitionPowerBuilder
            .Create("PowerMonkStepOfTheWindHeightenedMetabolism")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerStepOfTheWind", Resources.PowerStepOfTheWind, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkStepOfTheWindDash)
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .AddEffectForms(PowerMonkStepOftheWindDisengage.EffectDescription.EffectForms[0])
                    .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                    .Build())
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
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAuraOfProtection)
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
                                .SetConditionParticleReference(ConditionDefinitions.ConditionHolyAura)
                                .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
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

    private static void LoadMonkHeightenedMetabolism()
    {
        var validatePower = new ValidatorsValidatePowerUse(c =>
            !Main.Settings.EnableMonkHeightenedMetabolism || c.GetClassLevel(Monk) < 10);

        PowerMonkStepOfTheWindDash.AddCustomSubFeatures(validatePower);
        PowerMonkStepOftheWindDisengage.AddCustomSubFeatures(validatePower);
    }

    private static void LoadMonkWeaponSpecialization()
    {
        var weaponTypeDefinitions = new List<WeaponTypeDefinition>
        {
            WeaponTypeDefinitions.BattleaxeType,
            WeaponTypeDefinitions.LightCrossbowType,
            WeaponTypeDefinitions.LongbowType,
            WeaponTypeDefinitions.LongswordType,
            WeaponTypeDefinitions.MorningstarType,
            WeaponTypeDefinitions.RapierType,
            WeaponTypeDefinitions.ScimitarType,
            WeaponTypeDefinitions.ShortbowType,
            WeaponTypeDefinitions.WarhammerType,
            CustomWeaponsContext.HandXbowWeaponType
        };

        foreach (var weaponTypeDefinition in weaponTypeDefinitions)
        {
            var weaponTypeName = weaponTypeDefinition.Name;

            var featureMonkWeaponSpecialization = FeatureDefinitionProficiencyBuilder
                .Create($"FeatureMonkWeaponSpecialization{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(ProficiencyType.Weapon, weaponTypeName)
                .AddCustomSubFeatures(
                    new MonkWeaponSpecialization { WeaponType = weaponTypeDefinition })
                .AddToDB();

            if (!weaponTypeDefinition.IsBow && !weaponTypeDefinition.IsCrossbow)
            {
                featureMonkWeaponSpecialization.AddCustomSubFeatures(
                    new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important,
                        ValidatorsWeapon.IsOfWeaponType(weaponTypeDefinition))
                );
            }

            // ensure we get dice upgrade on these
            AttackModifierMonkMartialArtsImprovedDamage.AddCustomSubFeatures(
                new MonkWeaponSpecializationDiceUpgrade(weaponTypeDefinition));

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocationMonkWeaponSpecialization{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    weaponTypeDefinition.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization)
                .SetGrantedFeature(featureMonkWeaponSpecialization)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }
    }

    internal static void SwitchMonkAbundantKi()
    {
        if (Main.Settings.EnableMonkAbundantKi)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(AttributeModifierMonkAbundantKi, 2));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 2 &&
                                x.FeatureDefinition == AttributeModifierMonkAbundantKi);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkFightingStyle()
    {
        if (Main.Settings.EnableMonkFightingStyle)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceMonk, 2));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 2 &&
                                x.FeatureDefinition == FightingStyleChoiceMonk);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack()
    {
        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            PowerMonkMartialArts.GuiPresentation.description =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusDescription";
            PowerMonkMartialArts.GuiPresentation.title =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = true;
            PowerMonkMartialArts.activationTime = ActivationTime.NoCost;
        }
        else
        {
            PowerMonkMartialArts.GuiPresentation.description = "Action/&MartialArtsDescription";
            PowerMonkMartialArts.GuiPresentation.title = "Action/&MartialArtsTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = false;
            PowerMonkMartialArts.activationTime = ActivationTime.OnAttackHitMartialArts;
        }

        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkDoNotRequireAttackActionForFlurry()
    {
        FeatureSetMonkFlurryOfBlows.GuiPresentation.description =
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry
                ? "Feature/&FeatureSetAlternateMonkFlurryOfBlowsDescription"
                : "Feature/&FeatureSetMonkFlurryOfBlowsDescription";
    }

    internal static void SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall()
    {
        if (Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall)
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkAlternateUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkAlternateUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = true;
        }
        else
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = false;
        }
    }

    internal static void SwitchMonkHeightenedMetabolism()
    {
        if (Main.Settings.EnableMonkHeightenedMetabolism)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FeatureMonkHeightenedMetabolism, 10));
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(PowerMonkStepOfTheWindHeightenedMetabolism, 10));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 10 &&
                                (x.FeatureDefinition == FeatureMonkHeightenedMetabolism ||
                                 x.FeatureDefinition == PowerMonkStepOfTheWindHeightenedMetabolism));
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkSuperiorDefenseToReplaceEmptyBody()
    {
        Monk.FeatureUnlocks
            .RemoveAll(x => x.level == 18 &&
                            (x.FeatureDefinition == Level20Context.PowerMonkEmptyBody ||
                             x.FeatureDefinition == PowerMonkSuperiorDefense));

        Monk.FeatureUnlocks.TryAdd(
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody
                ? new FeatureUnlockByLevel(PowerMonkSuperiorDefense, 18)
                : new FeatureUnlockByLevel(Level20Context.PowerMonkEmptyBody, 18));

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkBodyAndMindToReplacePerfectSelf()
    {
        Monk.FeatureUnlocks
            .RemoveAll(x => x.level == 20 &&
                            (x.FeatureDefinition == Level20Context.FeatureMonkPerfectSelf ||
                             x.FeatureDefinition == FeatureMonkBodyAndMind));

        Monk.FeatureUnlocks.TryAdd(
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf
                ? new FeatureUnlockByLevel(FeatureMonkBodyAndMind, 20)
                : new FeatureUnlockByLevel(Level20Context.FeatureMonkPerfectSelf, 20));

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkWeaponSpecialization()
    {
        var levels = new[] { 2, 11 };

        if (Main.Settings.EnableMonkWeaponSpecialization)
        {
            foreach (var level in levels)
            {
                Monk.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(InvocationPoolMonkWeaponSpecialization, level));
            }
        }
        else
        {
            foreach (var level in levels)
            {
                Monk.FeatureUnlocks
                    .RemoveAll(x => x.level == level &&
                                    x.FeatureDefinition == InvocationPoolMonkWeaponSpecialization);
            }
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    private sealed class CustomBehaviorHeightenedMetabolism(
        ConditionDefinition conditionFlurryOfBlowsHeightenedMetabolism,
        ConditionDefinition conditionFlurryOfBlowsFreedomHeightenedMetabolism)
        : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private readonly EffectForm _effectForm =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsHeightenedMetabolism);

        private readonly EffectForm _effectFormFreedom =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsFreedomHeightenedMetabolism);

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var definition = action.ActionParams.activeEffect.SourceDefinition;

            if (definition != PowerMonkPatientDefense &&
                definition != PowerMonkPatientDefenseSurvival3 &&
                definition != PowerMonkPatientDefenseSurvival6)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var dieType = rulesetCharacter.GetMonkDieType();
            var tempHp = rulesetCharacter.RollDiceAndSum(dieType, RollContext.HealValueRoll, 2, []);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                tempHp, DurationType.Round, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableMonkHeightenedMetabolism &&
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
            if (definition == PowerMonkFlurryOfBlows)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement)
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

    internal sealed class MonkWeaponSpecialization
    {
        internal WeaponTypeDefinition WeaponType { get; set; }
    }

    private sealed class MonkWeaponSpecializationDiceUpgrade : IValidateContextInsteadOfRestrictedProperty
    {
        private readonly WeaponTypeDefinition _weaponTypeDefinition;

        internal MonkWeaponSpecializationDiceUpgrade(WeaponTypeDefinition weaponTypeDefinition)
        {
            _weaponTypeDefinition = weaponTypeDefinition;
        }

        public (OperationType, bool) ValidateContext(
            BaseDefinition definition,
            IRestrictedContextProvider provider,
            RulesetCharacter character,
            ItemDefinition itemDefinition,
            bool rangedAttack, RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var attackModeWeaponType =
                (attackMode?.SourceDefinition as ItemDefinition)?.WeaponDescription.WeaponTypeDefinition;

            return (OperationType.Or,
                character.GetSubFeaturesByType<MonkWeaponSpecializationDiceUpgrade>().Exists(
                    x => x._weaponTypeDefinition == attackModeWeaponType));
        }
    }

    #endregion

    #region Ranger

    internal static void SwitchRangerHumanoidFavoredEnemy()
    {
        if (Main.Settings.AddHumanoidFavoredEnemyToRanger)
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Add(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }
        else
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Remove(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }

        AdditionalDamageRangerFavoredEnemyChoice.FeatureSet.Sort((x, y) =>
            string.Compare(x.FormatTitle(), y.FormatTitle(), StringComparison.CurrentCulture));
    }

    internal static void SwitchRangerNatureShroud()
    {
        if (Main.Settings.EnableRangerNatureShroudAt10)
        {
            Ranger.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 10));
        }
        else
        {
            Ranger.FeatureUnlocks
                .RemoveAll(x => x.level == 10
                                && x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    #endregion

    #region Rogue

    private const string FeatSteadyAim = "FeatSteadyAim";

    private static readonly FeatureDefinitionPower PowerFeatSteadyAim = FeatureDefinitionPowerBuilder
        .Create($"Power{FeatSteadyAim}")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite(FeatSteadyAim, Resources.PowerSteadyAim, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Advantage")
                                .SetGuiPresentation(Category.Condition,
                                    ConditionDefinitions.ConditionGuided)
                                .SetPossessive()
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                .AddFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create($"CombatAffinity{FeatSteadyAim}")
                                        .SetGuiPresentation($"Power{FeatSteadyAim}", Category.Feature,
                                            Gui.NoLocalization)
                                        .SetMyAttackAdvantage(AdvantageType.Advantage)
                                        .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Restrained")
                                .SetGuiPresentation(Category.Condition)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddFeatures(MovementAffinityConditionRestrained)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetParticleEffectParameters(PowerFunctionWandFearCommand)
                .SetImpactEffectParameters(new AssetReference())
                .Build())
        .AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => GameLocationCharacter.GetFromActor(c) is { UsedTacticalMoves: 0 }))
        .AddToDB();

    internal static readonly ConditionDefinition ConditionReduceSneakDice = ConditionDefinitionBuilder
        .Create("ConditionReduceSneakDice")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetConditionType(ConditionType.Detrimental)
        .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
        .SetSpecialInterruptions(ConditionInterruption.Attacks)
        .AddToDB();

    private static FeatureDefinitionFeatureSet _featureSetRogueCunningStrike;
    private static FeatureDefinitionFeatureSet _featureSetRogueDeviousStrike;
    private static readonly char[] Separator = ['\t'];

    private static void BuildRogueCunningStrike()
    {
        const string Cunning = "RogueCunningStrike";
        const string Devious = "RogueDeviousStrike";

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Cunning}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        // Disarm

        var combatAffinityDisarmed = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Cunning}Disarmed")
            .SetGuiPresentation($"Condition{Cunning}Disarmed", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionDisarmed = ConditionDefinitionBuilder
            .Create($"Condition{Cunning}Disarmed")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionDefinitions.ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityDisarmed)
            .AddToDB();

        var powerDisarm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Disarm")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDisarmed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Poison

        var powerPoison = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Poison")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionPoisoned, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Trip

        var powerTrip = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Trip")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Withdraw

        var powerWithdraw = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Withdraw")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .Build())
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                PowerUsesSneakDiceTooltipModifier.Instance,
                new CustomBehaviorWithdraw())
            .AddToDB();

        //
        // DEVIOUS STRIKES - LEVEL 14
        //

        // Dazed

        var actionAffinityDazedOnlyMovement = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Devious}DazedOnlyMovement")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(false, false, freeOnce: false, reaction: false, noCost: false)
            .AddToDB();

        var conditionDazedOnlyMovement = ConditionDefinitionBuilder
            .Create($"Condition{Devious}DazedOnlyMovement")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(actionAffinityDazedOnlyMovement)
            .AddToDB();

        var actionAffinityDazed = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Devious}Dazed")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(reaction: false, bonus: false)
            .AddToDB();

        var conditionDazed = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionDazzled, $"Condition{Devious}Dazed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(actionAffinityDazed)
            .AddCustomSubFeatures(new ActionFinishedByMeDazed(conditionDazedOnlyMovement))
            .AddToDB();

        var powerDaze = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}Daze")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 2)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDazed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Knock Out

        var conditionKnockOut = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionIncapacitated, $"Condition{Devious}KnockOut")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionDefinitions.ConditionAsleep)
            .SetParentCondition(ConditionDefinitions.ConditionIncapacitated)
            .SetFeatures()
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerKnockOut = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}KnockOut")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 5)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        var powerKnockOutApply = FeatureDefinitionPowerBuilder
            .Create($"Power{Devious}KnockOutApply")
            .SetGuiPresentation($"Power{Devious}KnockOut", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionKnockOut, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // Obscure

        var powerObscure = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}Obscure")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 3)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionBlinded,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // MAIN

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorCunningStrike(powerPool, powerKnockOut, powerKnockOutApply, powerWithdraw));

        PowerBundle.RegisterPowerBundle(powerPool, true,
            powerDisarm, powerPoison, powerTrip, powerWithdraw, powerDaze, powerKnockOut, powerObscure);

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityCunningStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CunningStrikeToggle)
            .AddToDB();

        _featureSetRogueCunningStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Cunning}")
            .SetGuiPresentation($"Power{Cunning}", Category.Feature)
            .AddFeatureSet(powerPool, actionAffinityToggle, powerDisarm, powerPoison, powerTrip, powerWithdraw)
            .AddToDB();

        _featureSetRogueDeviousStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Devious}")
            .SetGuiPresentation($"Power{Devious}", Category.Feature)
            .AddFeatureSet(powerDaze, powerKnockOut, powerObscure)
            .AddToDB();
    }

    internal static bool IsSneakAttackValid(
        ActionModifier attackModifier,
        GameLocationCharacter attacker,
        GameLocationCharacter defender)
    {
        // only trigger if it hasn't used sneak attack yet
        if (!attacker.OncePerTurnIsValid("AdditionalDamageRogueSneakAttack") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishHoodlumNonFinesseSneakAttack") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishDuelistDaringDuel") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishUmbralStalkerDeadlyShadows"))
        {
            return false;
        }

        var advantageType = ComputeAdvantage(attackModifier.AttackAdvantageTrends);

        return advantageType switch
        {
            AdvantageType.Advantage => true,
            AdvantageType.Disadvantage => false,
            _ =>
                // it's an attack with a nearby enemy (standard sneak attack)
                ServiceRepository.GetService<IGameLocationBattleService>()
                    .IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker) ||
                // it's a Duelist and target is dueling with him
                RoguishDuelist.TargetIsDuelingWithRoguishDuelist(attacker, defender, advantageType) ||
                // it's an Umbral Stalker and source or target are in dim light or darkness
                RoguishUmbralStalker.SourceOrTargetAreNotBright(attacker, defender, advantageType)
        };
    }

    private sealed class CustomBehaviorCunningStrike(
        FeatureDefinitionPower powerRogueCunningStrike,
        FeatureDefinitionPower powerKnockOut,
        FeatureDefinitionPower powerKnockOutApply,
        FeatureDefinitionPower powerWithdraw)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private FeatureDefinitionPower _selectedPower;

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
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
            _selectedPower = null;

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.CunningStrikeToggle) ||
                !IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerRogueCunningStrike, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerRogueCunningStrike.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                // determine selected power to collect cost
                var option = reactionRequest.SelectedSubOption;
                var subPowers = powerRogueCunningStrike.GetBundle()?.SubPowers;

                if (subPowers == null)
                {
                    return;
                }

                _selectedPower = subPowers[option];

                // inflict condition passing power cost on amount to be deducted later on from sneak dice
                rulesetAttacker.InflictCondition(
                    ConditionReduceSneakDice.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    ConditionReduceSneakDice.Name,
                    _selectedPower.CostPerUse,
                    0,
                    0);
            }
        }

        // handle Knock Out exception which should apply condition after attack
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (_selectedPower == powerKnockOut)
            {
                yield return HandleKnockOut(attacker, defender);
            }
            else if (_selectedPower == powerWithdraw)
            {
                yield return HandleWithdraw(action, attacker);
            }

            _selectedPower = null;
        }

        private IEnumerator HandleWithdraw(CharacterAction action, GameLocationCharacter attacker)
        {
            yield return GameUiContext.SelectPosition(action, powerWithdraw);

            var rulesetAttacker = attacker.RulesetCharacter;
            var position = action.ActionParams.Positions[0];
            var distance = int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= (int)distance;

            if (attacker.UsedTacticalMoves < 0)
            {
                attacker.UsedTacticalMoves = 0;
            }

            attacker.UsedTacticalMovesChanged?.Invoke(attacker);

            rulesetAttacker.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionDisengaging,
                0,
                0,
                0);

            attacker.MyExecuteActionTacticalMove(position);
        }

        private IEnumerator HandleKnockOut(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerKnockOutApply, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }
    }

    private sealed class ActionFinishedByMeDazed(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDazedOnlyMovement) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionMove)
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionDazedOnlyMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDazedOnlyMovement.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorWithdraw : IFilterTargetingPosition, IIgnoreInvisibilityInterruptionCheck
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

            var halfMaxTacticalMoves = (actingCharacter.MaxTacticalMoves + 1) / 2; // half-rounded up
            var boxInt = new BoxInt(actingCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(halfMaxTacticalMoves, 0, halfMaxTacticalMoves);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }
    }

    internal static void SwitchRogueCunningStrike()
    {
        if (Main.Settings.EnableRogueCunningStrike)
        {
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(_featureSetRogueCunningStrike, 5));
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(_featureSetRogueDeviousStrike, 14));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 5 && x.FeatureDefinition == _featureSetRogueCunningStrike);
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 14 && x.FeatureDefinition == _featureSetRogueDeviousStrike);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchRogueFightingStyle()
    {
        if (Main.Settings.EnableRogueFightingStyle)
        {
            Rogue.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceRogue, 2));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 2 && x.FeatureDefinition == FightingStyleChoiceRogue);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchRogueSteadyAim()
    {
        if (Main.Settings.EnableRogueSteadyAim)
        {
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(PowerFeatSteadyAim, 3));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x =>
                x.level == 3 && x.FeatureDefinition == PowerFeatSteadyAim);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    private static void SwitchRogueStrSaving()
    {
        var powerNames = new List<string>
        {
            "PowerRogueCunningStrikeDisarm",
            //"PowerRogueCunningStrikePoison",
            "PowerRogueCunningStrikeTrip",
            //"PowerRogueCunningStrikeWithdraw",
            //"PowerRogueDeviousStrikeDaze",
            //"PowerRogueDeviousStrikeKnockOut",
            "PowerRogueDeviousStrikeObscure",
            "PowerRoguishOpportunistDebilitatingStrike",
            "PowerRoguishOpportunistImprovedDebilitatingStrike",
            "PowerRoguishBladeCallerHailOfBlades"
        };

        foreach (var power in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Where(x => powerNames.Contains(x.Name)))
        {
            power.AddCustomSubFeatures(new ModifyEffectDescriptionSavingThrowRogue(power));
        }
    }

    #endregion
}
