using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Feats.FeatHelpers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class MeleeCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featAlwaysReady = BuildAlwaysReady();
        var featBladeMastery = BuildBladeMastery();
        var featCleavingAttack = BuildCleavingAttack();
        var featCrusherStr = BuildCrusherStr();
        var featCrusherCon = BuildCrusherCon();
        var featDefensiveDuelist = BuildDefensiveDuelist();
        var featDevastatingStrikes = BuildDevastatingStrikes();
        var featFellHanded = BuildFellHanded();
        var featFencer = BuildFencer();
        var featHammerThePoint = BuildHammerThePoint();
        var featLongSwordFinesse = BuildLongswordFinesse();
        var featOldTacticsDex = BuildOldTacticsDex();
        var featOldTacticsStr = BuildOldTacticsStr();
        var featPiercerDex = BuildPiercerDex();
        var featPiercerStr = BuildPiercerStr();
        var featPowerAttack = BuildPowerAttack();
        var featRecklessAttack = BuildRecklessAttack();
        var featSavageAttack = BuildSavageAttack();
        var featSlasherStr = BuildSlasherStr();
        var featSlasherDex = BuildSlasherDex();
        var featSpearMastery = BuildSpearMastery();

        feats.AddRange(
            featAlwaysReady,
            featBladeMastery,
            featCleavingAttack,
            featCrusherCon,
            featCrusherStr,
            featDefensiveDuelist,
            featDevastatingStrikes,
            featFellHanded,
            featFencer,
            featHammerThePoint,
            featLongSwordFinesse,
            featOldTacticsDex,
            featOldTacticsStr,
            featPiercerDex,
            featPiercerStr,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSlasherDex,
            featSlasherStr,
            featSpearMastery);

        var featGroupCrusher = GroupFeats.MakeGroup("FeatGroupCrusher", GroupFeats.Crusher,
            featCrusherStr,
            featCrusherCon);

        var featGroupOldTactics = GroupFeats.MakeGroup("FeatGroupOldTactics", GroupFeats.OldTactics,
            featOldTacticsDex,
            featOldTacticsStr);

        var featGroupSlasher = GroupFeats.MakeGroup("FeatGroupSlasher", GroupFeats.Slasher,
            featSlasherDex,
            featSlasherStr);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featAlwaysReady,
            featDefensiveDuelist);

        GroupFeats.FeatGroupPiercer.AddFeats(
            featPiercerDex,
            featPiercerStr);

        GroupFeats.FeatGroupUnarmoredCombat.AddFeats(
            featGroupCrusher);

        GroupFeats.MakeGroup("FeatGroupMeleeCombat", null,
            GroupFeats.FeatGroupElementalTouch,
            GroupFeats.FeatGroupPiercer,
            FeatDefinitions.DauntingPush,
            FeatDefinitions.DistractingGambit,
            FeatDefinitions.TripAttack,
            featAlwaysReady,
            featBladeMastery,
            featCleavingAttack,
            featDefensiveDuelist,
            featDevastatingStrikes,
            featFellHanded,
            featFencer,
            featHammerThePoint,
            featLongSwordFinesse,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSpearMastery,
            featGroupCrusher,
            featGroupOldTactics,
            featGroupSlasher);
    }

    #region Defensive Duelist

    private static FeatDefinition BuildDefensiveDuelist()
    {
        const string NAME = "FeatDefensiveDuelist";

        var conditionDefensiveDuelist = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                    AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDefensiveDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionDefensiveDuelist,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .SetCustomSubFeatures(
                new RestrictedContextValidator((_, _, _, _, _, mode, _) =>
                    (OperationType.Set,
                        ValidatorsWeapon.HasAnyWeaponTag(mode?.SourceDefinition as ItemDefinition,
                            TagsDefinitions.WeaponTagFinesse))))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerDefensiveDuelist)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    #endregion

    #region Reckless Attack

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRecklessAttack")
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(ActionAffinityBarbarianRecklessAttack)
            .SetValidators(ValidatorsFeat.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    #endregion

    #region Savage Attack

    private static FeatDefinition BuildSavageAttack()
    {
        return FeatDefinitionBuilder
            .Create("FeatSavageAttack")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttackNonMagic")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackReroll")
                    .AddToDB(),
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttackMagic")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(MagicDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackReroll")
                    .AddToDB())
            .AddToDB();
    }

    #endregion

    #region Spear Mastery

    private static FeatDefinition BuildSpearMastery()
    {
        const string NAME = "FeatSpearMastery";
        const string REACH_CONDITION = $"Condition{NAME}Reach";

        var validWeapon = ValidatorsWeapon.IsOfWeaponTypeWithoutAttackTag("Polearm", SpearType);

        var conditionFeatSpearMasteryReach = ConditionDefinitionBuilder
            .Create(REACH_CONDITION)
            .SetGuiPresentation($"Power{NAME}Reach", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(FeatureDefinitionBuilder
                .Create($"Feature{NAME}Reach")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new IncreaseWeaponReach(1, validWeapon,
                    ValidatorsCharacter.HasAnyOfConditions(REACH_CONDITION)))
                .AddToDB())
            .AddToDB();

        var powerFeatSpearMasteryReach = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Reach")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite($"Power{NAME}Reach", Resources.SpearMasteryReach, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.Shield)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(
                        conditionFeatSpearMasteryReach,
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true)
                    .Build())
                .UseQuickAnimations()
                .Build())
            .AddToDB();

        var conditionDamage = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Damage")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("SpearMastery")
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                //Adding any property so that custom restricted context would trigger
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .SetCustomSubFeatures(new RestrictedContextValidator((_, _, character, _, ranged, mode, _) =>
                    (OperationType.Set, !ranged && validWeapon(mode, null, character))))
                .SetIgnoreCriticalDoubleDice(true)
                .AddToDB())
            .AddToDB();

        IEnumerator AddCondition(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationBattleManager manager,
            GameLocationActionManager actionManager,
            ReactionRequest request)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionDamage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);

            yield break;
        }

        IEnumerator RemoveCondition(GameLocationCharacter attacker, GameLocationCharacter defender,
            GameLocationBattleManager manager, GameLocationActionManager actionManager, ReactionRequest request)
        {
            attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat,
                conditionDamage.Name);

            yield break;
        }

        var conditionFeatSpearMasteryCharge = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Charge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetFeatures(FeatureDefinitionBuilder
                .Create($"Feature{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new CanMakeAoOOnReachEntered
                {
                    AllowRange = false,
                    AccountAoOImmunity = true,
                    WeaponValidator = validWeapon,
                    BeforeReaction = AddCondition,
                    AfterReaction = RemoveCondition
                })
                .AddToDB())
            .AddToDB();

        var powerFeatSpearMasteryCharge = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Charge")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite($"Power{NAME}Charge", Resources.SpearMasteryCharge, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(conditionFeatSpearMasteryCharge,
                        ConditionForm.ConditionOperation.Add, true)
                    .Build())
                .UseQuickAnimations()
                .Build())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerFeatSpearMasteryReach,
                powerFeatSpearMasteryCharge,
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{NAME}")
                    .SetGuiPresentation(Category.Feature)
                    .SetAttackRollModifier(1)
                    .SetCustomSubFeatures(
                        new RestrictedContextValidator((_, _, character, _, ranged, mode, _) =>
                            (OperationType.Set, !ranged && validWeapon(mode, null, character))),
                        new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D8, DieType.D10), validWeapon))
                    .AddToDB())
            .AddToDB();
    }

    #endregion

    #region Longsword Finesse

    private static FeatDefinition BuildLongswordFinesse()
    {
        const string Name = "FeatLongswordFinesse";

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(LongswordType);

        var attributeModifierArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasLongswordInHands)
            .AddToDB();

        var modifyAttackModeFinesse = FeatureDefinitionBuilder
            .Create($"ModifyAttackMode{Name}Finesse")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                attributeModifierArmorClass,
                modifyAttackModeFinesse)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    #endregion

    #region Fencer

    private static FeatDefinition BuildFencer()
    {
        const string NAME = "FeatFencer";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(
                new AddExtraMainHandAttack(
                    ActionDefinitions.ActionType.Bonus,
                    ValidatorsCharacter.HasAttacked,
                    ValidatorsCharacter.HasFreeHand,
                    ValidatorsCharacter.HasMeleeWeaponInMainHand))
            .AddToDB();
    }

    #endregion

    #region Hammer the Point

    private static FeatDefinition BuildHammerThePoint()
    {
        const string Name = "FeatHammerThePoint";

        var conditionHammerThePoint = ConditionDefinitionBuilder
            .Create($"Condition{Name}HammerThePoint")
            .SetGuiPresentationNoContent(true)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AllowMultipleInstances()
            .AddToDB();

        var additionalDamageHammerThePoint = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}HammerThePoint")
            .SetGuiPresentationNoContent(true)
            .SetAttackModeOnly()
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionHammerThePoint)
            .AddToDB();

        var featHammerThePoint = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(additionalDamageHammerThePoint)
            .AddToDB();

        additionalDamageHammerThePoint.SetCustomSubFeatures(
            new PhysicalAttackInitiatedByMeFeatHammerThePoint(conditionHammerThePoint, featHammerThePoint));

        return featHammerThePoint;
    }

    private sealed class PhysicalAttackInitiatedByMeFeatHammerThePoint : IPhysicalAttackInitiatedByMe
    {
        private readonly ConditionDefinition _conditionHammerThePoint;
        private readonly FeatDefinition _featHammerThePoint;

        public PhysicalAttackInitiatedByMeFeatHammerThePoint(
            ConditionDefinition conditionHammerThePoint,
            FeatDefinition featHammerThePoint)
        {
            _conditionHammerThePoint = conditionHammerThePoint;
            _featHammerThePoint = featHammerThePoint;
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var battle = Gui.Battle;
            var rulesetDefender = defender.RulesetCharacter;

            if (battle == null || rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var attackedThisTurnCount = rulesetDefender.AllConditions
                .Count(x => x.ConditionDefinition == _conditionHammerThePoint);

            if (attackedThisTurnCount == 0)
            {
                yield break;
            }

            var trendInfo = new TrendInfo(
                attackedThisTurnCount, FeatureSourceType.Feat, _featHammerThePoint.Name, _featHammerThePoint);

            attackModifier.AttackRollModifier += attackedThisTurnCount;
            attackModifier.AttacktoHitTrends.Add(trendInfo);

            var damage = attackerAttackMode?.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                yield break;
            }

            damage.BonusDamage += attackedThisTurnCount;
            damage.DamageBonusTrends.Add(trendInfo);
        }
    }

    #endregion

    #region Old Tactics

    private static FeatDefinition BuildOldTacticsStr()
    {
        const string Name = "FeatOldTacticsStr";

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar)
            .SetCustomSubFeatures(new ActionFinishedByEnemyOldTactics())
            .AddToDB();
    }

    private static FeatDefinition BuildOldTacticsDex()
    {
        const string Name = "FeatOldTacticsDex";

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye)
            .SetCustomSubFeatures(new ActionFinishedByEnemyOldTactics())
            .AddToDB();
    }

    private sealed class ActionFinishedByEnemyOldTactics : IActionFinishedByEnemy
    {
        public ActionDefinition ActionDefinition => DatabaseHelper.ActionDefinitions.StandUp;

        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle.ActiveContenderIgnoringLegendary == target)
            {
                yield break;
            }

            if (!target.CanReact())
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var enemy = characterAction.ActingCharacter;

            if (!battle.IsWithin1Cell(target, enemy))
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = target.GetFirstMeleeModeThatCanAttack(enemy);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = target.GetFirstRangedModeThatCanAttack(enemy);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(target, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(enemy);
            reactionParams.StringParameter = target.Name;
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("OldTactics", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(target, manager, previousReactionCount);
        }
    }

    #endregion

    #region Always Ready

    private static FeatDefinition BuildAlwaysReady()
    {
        var conditionAlwaysReady = ConditionDefinitionBuilder
            .Create("ConditionAlwaysReady")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var featureAlwaysReady = FeatureDefinitionBuilder
            .Create("FeatureAlwaysReady")
            .SetGuiPresentation("FeatAlwaysReady", Category.Feat)
            .AddToDB();

        featureAlwaysReady.SetCustomSubFeatures(new CustomBehaviorAlwaysReady(conditionAlwaysReady,
            featureAlwaysReady));

        return FeatDefinitionBuilder
            .Create("FeatAlwaysReady")
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(featureAlwaysReady)
            .AddToDB();
    }

    private sealed class CustomBehaviorAlwaysReady : IPhysicalAttackAfterDamage, ICharacterTurnEndListener
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _featureDefinition;

        public CustomBehaviorAlwaysReady(ConditionDefinition conditionDefinition, FeatureDefinition featureDefinition)
        {
            _conditionDefinition = conditionDefinition;
            _featureDefinition = featureDefinition;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.HasAnyConditionOfType(_conditionDefinition.Name))
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedFeature(_featureDefinition);
            locationCharacter.ReadiedAction = ActionDefinitions.ReadyActionType.Melee;
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                !(ValidatorsWeapon.IsMelee(attackMode) || ValidatorsWeapon.IsUnarmed(rulesetCharacter, attackMode)))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                _conditionDefinition.Name,
                _conditionDefinition.durationType,
                _conditionDefinition.durationParameter,
                _conditionDefinition.turnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Blade Mastery

    private static FeatDefinition BuildBladeMastery()
    {
        const string NAME = "FeatBladeMastery";

        var weaponTypes = new[] { ShortswordType, LongswordType, ScimitarType, RapierType, GreatswordType };

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetModifier(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.ArmorClass, 1)
                    .AddToDB())
            .AddToDB();

        feat.SetCustomSubFeatures(
            new AttackComputeModifierFeatBladeMastery(feat, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class AttackComputeModifierFeatBladeMastery : IPhysicalAttackInitiatedByMe
    {
        private readonly FeatDefinition _featDefinition;
        private readonly WeaponTypeDefinition[] _weaponTypeDefinition;

        public AttackComputeModifierFeatBladeMastery(FeatDefinition featDefinition,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _featDefinition = featDefinition;
            _weaponTypeDefinition = weaponTypeDefinition;
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            if ((action.ActionId == ActionDefinitions.Id.SwiftRetaliation ||
                 action.ActionType == ActionDefinitions.ActionType.Reaction) &&
                ValidatorsWeapon.IsOfWeaponType(_weaponTypeDefinition)(attackerAttackMode, null, null))
            {
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Feat, _featDefinition.Name, _featDefinition));
            }

            yield break;
        }
    }

    #endregion

    #region Cleaving Attack

    private static FeatDefinition BuildCleavingAttack()
    {
        const string Name = "FeatCleavingAttack";

        var concentrationProvider = new StopPowerConcentrationProvider(
            Name,
            "Tooltip/&CleavingAttackConcentration",
            Sprites.GetSprite(nameof(Resources.PowerAttackConcentrationIcon), Resources.PowerAttackConcentrationIcon,
                64, 64));

        var modifyAttackModeForWeapon = FeatureDefinitionBuilder
            .Create("ModifyAttackModeForWeaponFeatCleavingAttack")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionCleavingAttackFinish = ConditionDefinitionBuilder
            .Create($"Condition{Name}Finish")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create($"AdditionalAction{Name}Finish")
                    .SetGuiPresentation($"Condition{Name}Finish", Category.Condition, Gui.NoLocalization)
                    .SetCustomSubFeatures(AdditionalActionAttackValidator.MeleeOnly)
                    .SetActionType(ActionDefinitions.ActionType.Main)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB();

        var conditionCleavingAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(modifyAttackModeForWeapon)
            .AddToDB();

        var powerCleavingAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite(nameof(Resources.PowerAttackIcon), Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionCleavingAttack, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(
                    ValidatorsCharacter.HasNoneOfConditions(conditionCleavingAttack.Name)))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerCleavingAttack);

        var powerTurnOffCleavingAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionCleavingAttack, ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffCleavingAttack);

        var featCleavingAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerCleavingAttack,
                powerTurnOffCleavingAttack,
                FeatureDefinitionBuilder
                    .Create($"Feature{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(
                        new AddExtraPhysicalAttackFeatCleavingPhysicalAttack(conditionCleavingAttackFinish))
                    .AddToDB())
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffCleavingAttack;
        modifyAttackModeForWeapon
            .SetCustomSubFeatures(
                concentrationProvider,
                new ModifyWeaponAttackModeFeatCleavingAttack(featCleavingAttack));

        return featCleavingAttack;
    }

    private sealed class AddExtraPhysicalAttackFeatCleavingPhysicalAttack : IPhysicalAttackAfterDamage,
        IOnTargetReducedToZeroHp
    {
        private readonly ConditionDefinition _conditionCleavingAttackFinish;

        public AddExtraPhysicalAttackFeatCleavingPhysicalAttack(ConditionDefinition conditionCleavingAttackFinish)
        {
            _conditionCleavingAttackFinish = conditionCleavingAttackFinish;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (activeEffect != null || !ValidateCleavingAttack(attackMode))
            {
                yield break;
            }

            InflictCondition(attacker.RulesetCharacter);
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome != RollOutcome.CriticalSuccess || !ValidateCleavingAttack(attackMode))
            {
                return;
            }

            InflictCondition(attacker.RulesetCharacter);
        }

        private void InflictCondition(RulesetCharacter rulesetCharacter)
        {
            rulesetCharacter.InflictCondition(
                _conditionCleavingAttackFinish.Name,
                _conditionCleavingAttackFinish.DurationType,
                _conditionCleavingAttackFinish.DurationParameter,
                _conditionCleavingAttackFinish.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyWeaponAttackModeFeatCleavingAttack : IModifyWeaponAttackMode
    {
        private readonly FeatDefinition _featDefinition;

        public ModifyWeaponAttackModeFeatCleavingAttack(FeatDefinition featDefinition)
        {
            _featDefinition = featDefinition;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidateCleavingAttack(attackMode, true))
            {
                return;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            const int TO_HIT = -5;
            const int TO_DAMAGE = +10;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT, FeatureSourceType.Feat,
                _featDefinition.Name, _featDefinition));

            damage.BonusDamage += TO_DAMAGE;
            damage.DamageBonusTrends.Add(new TrendInfo(TO_DAMAGE, FeatureSourceType.Feat,
                _featDefinition.Name, _featDefinition));
        }
    }

    private static bool ValidateCleavingAttack(RulesetAttackMode attackMode, bool validateHeavy = false)
    {
        return !attackMode.Ranged &&
               ValidatorsWeapon.IsMelee(attackMode) &&
               (!validateHeavy ||
                ValidatorsWeapon.HasAnyWeaponTag(attackMode.SourceDefinition as ItemDefinition,
                    TagsDefinitions.WeaponTagHeavy));
    }

    #endregion

    #region Crusher

    private static readonly FeatureDefinition FeatureFeatCrusher = FeatureDefinitionBuilder
        .Create("FeatureFeatCrusher")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(new PhysicalAttackFinishedByMeCrusher(
            ConditionDefinitionBuilder
                .Create("ConditionFeatCrusherCriticalHit")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
                .SetSpecialDuration(DurationType.Round, 1)
                .SetConditionType(ConditionType.Detrimental)
                .SetFeatures(
                    FeatureDefinitionCombatAffinityBuilder
                        .Create("CombatAffinityFeatCrusher")
                        .SetGuiPresentation("ConditionFeatCrusherCriticalHit", Category.Condition, Gui.NoLocalization)
                        .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                        .AddToDB())
                .AddToDB()))
        .AddToDB();

    private static FeatDefinition BuildCrusherStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatCrusherStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                FeatureFeatCrusher,
                GameUiContext.ActionAffinityFeatCrusherToggle)
            .SetFeatFamily(GroupFeats.Crusher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildCrusherCon()
    {
        return FeatDefinitionBuilder
            .Create("FeatCrusherCon")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Arun,
                FeatureFeatCrusher,
                GameUiContext.ActionAffinityFeatCrusherToggle)
            .SetFeatFamily(GroupFeats.Crusher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
            .AddToDB();
    }

    private sealed class PhysicalAttackFinishedByMeCrusher : IPhysicalAttackFinishedByMe
    {
        private const string SpecialFeatureName = "FeatureCrusher";

        private readonly ConditionDefinition _criticalConditionDefinition;

        public PhysicalAttackFinishedByMeCrusher(ConditionDefinition conditionDefinition)
        {
            _criticalConditionDefinition = conditionDefinition;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (attackRollOutcome is RollOutcome.CriticalSuccess)
            {
                rulesetDefender.InflictCondition(
                    _criticalConditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagCombat,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }

            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!attacker.OncePerTurnIsValid(SpecialFeatureName) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.FeatCrusherToggle))
            {
                yield break;
            }

            var actionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionService == null || !battleManager.IsBattleInProgress)
            {
                yield break;
            }

            if (attackerAttackMode.ranged ||
                !ValidatorsWeapon.IsOfDamageType(DamageTypeBludgeoning)(attackerAttackMode, null, null))
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = "Reaction/&CustomReactionCrusherDescription"
                };
            var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("Crusher", reactionParams);

            actionService.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(
                attacker, actionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            implementationService.ApplyEffectForms(
                new List<EffectForm>
                {
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                        .Build()
                },
                applyFormsParams,
                new List<string>(),
                out _,
                out _);

            attacker.UsedSpecialFeatures.TryAdd(SpecialFeatureName, 1);
        }
    }

    #endregion

    #region Devastating Strikes

    private static FeatDefinition BuildDevastatingStrikes()
    {
        const string NAME = "FeatDevastatingStrikes";

        var weaponTypes = new[] { GreatswordType, GreataxeType, MaulType };

        var featureDevastatingStrikes = FeatureDefinitionBuilder
            .Create("FeatureDevastatingStrikes")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new IgnoreDamageAffinityDevastatingStrikes())
            .AddToDB();

        var conditionDevastatingStrikes = ConditionDefinitionBuilder
            .Create("ConditionDevastatingStrikes")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureDevastatingStrikes)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        feat.SetCustomSubFeatures(
            new CustomBehaviorFeatDevastatingStrikes(conditionDevastatingStrikes, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class IgnoreDamageAffinityDevastatingStrikes : IIgnoreDamageAffinity
    {
        public bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, RulesetActor rulesetActor)
        {
            return provider.DamageAffinityType == DamageAffinityType.Resistance;
        }
    }

    private sealed class CustomBehaviorFeatDevastatingStrikes :
        IPhysicalAttackAfterDamage, IAttackBeforeHitConfirmedOnEnemy
    {
        private const string DevastatingStrikesDescription = "Feat/&FeatDevastatingStrikesDescription";
        private const string DevastatingStrikesTitle = "Feat/&FeatDevastatingStrikesTitle";
        private readonly ConditionDefinition _conditionBypassResistance;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public CustomBehaviorFeatDevastatingStrikes(
            ConditionDefinition conditionBypassResistance,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
            _conditionBypassResistance = conditionBypassResistance;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            if (!criticalHit)
            {
                yield break;
            }

            rulesetCharacter.InflictCondition(
                _conditionBypassResistance.Name,
                _conditionBypassResistance.DurationType,
                _conditionBypassResistance.DurationParameter,
                _conditionBypassResistance.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null || outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                return;
            }

            var originalDamageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (originalDamageForm == null)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            var bonusDamage = 0;

            if (attackModifier.AttackAdvantageTrend > 0)
            {
                var modifier = attackMode.ToHitBonus + attackModifier.AttackRollModifier;
                var lowerRoll = Math.Min(Global.FirstAttackRoll, Global.SecondAttackRoll);
                var lowOutcome = GameLocationBattleManagerTweaks.GetAttackResult(lowerRoll, modifier, rulesetDefender);

                Gui.Game.GameConsole.AttackRolled(
                    rulesetAttacker,
                    rulesetDefender,
                    attackMode.SourceDefinition,
                    lowOutcome,
                    lowerRoll + modifier,
                    lowerRoll,
                    modifier,
                    attackModifier.AttacktoHitTrends,
                    new List<TrendInfo>());

                if (lowOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    var strength = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Strength);
                    var strengthMod = AttributeDefinitions.ComputeAbilityScoreModifier(strength);
                    var dexterity = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Dexterity);
                    var dexterityMod = AttributeDefinitions.ComputeAbilityScoreModifier(dexterity);

                    if (strengthMod > 0 || dexterityMod > 0)
                    {
                        bonusDamage = Math.Max(strengthMod, dexterityMod);
                    }
                }
            }

            if (bonusDamage == 0 && outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            var dieRoll = 0;
            var rolls = new List<int>();
            var damage = new DamageForm
            {
                DamageType = originalDamageForm.DamageType,
                DieType = originalDamageForm.DieType,
                DiceNumber = 0,
                BonusDamage = bonusDamage
            };

            if (outcome is RollOutcome.CriticalSuccess)
            {
                var advantageType = attackModifier.AttackAdvantageTrend switch
                {
                    > 0 => AdvantageType.Advantage,
                    < 0 => AdvantageType.Disadvantage,
                    _ => AdvantageType.None
                };

                dieRoll = RollDie(originalDamageForm.DieType, advantageType, out _, out _);
                damage.DiceNumber = 1;
                rolls.Add(dieRoll);
            }

            rulesetAttacker.LogCharacterAffectsTarget(rulesetDefender,
                DevastatingStrikesTitle,
                "Feedback/&FeatFeatFellHandedDisadvantage",
                tooltipContent: DevastatingStrikesDescription);
            RulesetActor.InflictDamage(
                dieRoll + bonusDamage,
                damage,
                damage.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                attacker.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(damage.DieType, rolls, bonusDamage),
                true,
                out _);
        }
    }

    #endregion

    #region Fell Handed

    private static FeatDefinition BuildFellHanded()
    {
        const string NAME = "FeatFellHanded";

        var weaponTypes = new[] { BattleaxeType, GreataxeType, HandaxeType, MaulType, WarhammerType };

        var fellHandedAdvantage = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Advantage")
            .SetGuiPresentation(NAME, Category.Feat, $"Feature/&Power{NAME}AdvantageDescription", hidden: true)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne)
                    .Build())
                .Build())
            .AddToDB();

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        feat.SetCustomSubFeatures(
            new PhysicalAttackAfterDamageFeatFellHanded(fellHandedAdvantage, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class PhysicalAttackAfterDamageFeatFellHanded : IPhysicalAttackAfterDamage
    {
        private const string SuretyText = "Feedback/&FeatFeatFellHandedDisadvantage";
        private const string SuretyTitle = "Feat/&FeatFellHandedTitle";
        private const string SuretyDescription = "Feature/&PowerFeatFellHandedDisadvantageDescription";
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();
        private readonly DamageForm damage;
        private readonly FeatureDefinitionPower power;

        public PhysicalAttackAfterDamageFeatFellHanded(FeatureDefinitionPower power,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            this.power = power;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);

            damage = new DamageForm
            {
                DamageType = DamageTypeBludgeoning, DieType = DieType.D1, DiceNumber = 0, BonusDamage = 0
            };
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var modifier = attackMode.ToHitBonus + attackModifier.AttackRollModifier;

            switch (attackModifier.AttackAdvantageTrend)
            {
                case > 0 when outcome is RollOutcome.Success or RollOutcome.CriticalSuccess:
                    var lowerRoll = Math.Min(Global.FirstAttackRoll, Global.SecondAttackRoll);

                    var lowOutcome =
                        GameLocationBattleManagerTweaks.GetAttackResult(lowerRoll, modifier, rulesetDefender);

                    Gui.Game.GameConsole.AttackRolled(
                        rulesetAttacker,
                        rulesetDefender,
                        power,
                        lowOutcome,
                        lowerRoll + modifier,
                        lowerRoll,
                        modifier,
                        attackModifier.AttacktoHitTrends,
                        new List<TrendInfo>());

                    if (lowOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                    {
                        var usablePower = UsablePowersProvider.Get(power, rulesetAttacker);
                        ServiceRepository.GetService<IRulesetImplementationService>()
                            .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                            .AddAsActivePowerToSource()
                            .ApplyEffectOnCharacter(rulesetDefender, true, defender.LocationPosition);

                        rulesetDefender.LogCharacterAffectedByCondition(ConditionDefinitions.ConditionProne);
                    }

                    break;
                case < 0 when outcome is RollOutcome.Failure or RollOutcome.CriticalFailure:
                    var higherRoll = Math.Max(Global.FirstAttackRoll, Global.SecondAttackRoll);

                    var strength = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Strength);
                    var strengthMod = AttributeDefinitions.ComputeAbilityScoreModifier(strength);

                    if (strengthMod <= 0)
                    {
                        break;
                    }

                    var higherOutcome =
                        GameLocationBattleManagerTweaks.GetAttackResult(higherRoll, modifier, rulesetDefender);

                    if (higherOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
                    {
                        break;
                    }

                    rulesetAttacker.LogCharacterAffectsTarget(rulesetDefender,
                        SuretyTitle, SuretyText, tooltipContent: SuretyDescription);

                    damage.BonusDamage = strengthMod;
                    RulesetActor.InflictDamage(
                        strengthMod,
                        damage,
                        DamageTypeBludgeoning,
                        new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                        rulesetDefender,
                        false,
                        attacker.Guid,
                        false,
                        attackMode.AttackTags,
                        new RollInfo(DieType.D1, new List<int>(), strengthMod),
                        true,
                        out _);

                    break;
            }
        }
    }

    #endregion

    #region Piercer

    private static readonly FeatureDefinition FeatureFeatPiercer = FeatureDefinitionBuilder
        .Create("FeatureFeatPiercer")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(
            new PhysicalAttackInitiatedByMeFeatPiercer(
                ConditionDefinitionBuilder
                    .Create("ConditionFeatPiercerNonMagic")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
                    .SetFeatures(
                        FeatureDefinitionDieRollModifierBuilder
                            .Create("DieRollModifierFeatPiercerNonMagic")
                            .SetGuiPresentation("ConditionFeatPiercerNonMagic", Category.Condition)
                            .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatPiercerReroll")
                            .AddToDB())
                    .AddToDB(),
                DamageTypePiercing),
            new CustomAdditionalDamageFeatPiercer(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageFeatPiercer")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag(GroupFeats.Piercer)
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                    .SetIgnoreCriticalDoubleDice(true)
                    .AddToDB(),
                DamageTypePiercing))
        .AddToDB();

    private static FeatDefinition BuildPiercerDex()
    {
        return FeatDefinitionBuilder
            .Create("FeatPiercerDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                FeatureFeatPiercer)
            .SetFeatFamily(GroupFeats.Piercer)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildPiercerStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatPiercerStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                FeatureFeatPiercer)
            .SetFeatFamily(GroupFeats.Piercer)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private sealed class PhysicalAttackInitiatedByMeFeatPiercer : IPhysicalAttackInitiatedByMe
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly string _damageType;

        internal PhysicalAttackInitiatedByMeFeatPiercer(ConditionDefinition conditionDefinition, string damageType)
        {
            _conditionDefinition = conditionDefinition;
            _damageType = damageType;
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class CustomAdditionalDamageFeatPiercer : CustomAdditionalDamage
    {
        private readonly string _damageType;

        public CustomAdditionalDamageFeatPiercer(IAdditionalDamageProvider provider, string damageType) : base(provider)
        {
            _damageType = damageType;
        }

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            return criticalHit && damage != null && damage.DamageType == _damageType;
        }
    }

    #endregion

    #region Power Attack

    private static FeatDefinition BuildPowerAttack()
    {
        const string Name = "FeatPowerAttack";

        var concentrationProvider = new StopPowerConcentrationProvider("PowerAttack",
            "Tooltip/&PowerAttackConcentration",
            Sprites.GetSprite("PowerAttackConcentrationIcon", Resources.PowerAttackConcentrationIcon, 64, 64));

        var modifyAttackModeForWeapon = FeatureDefinitionBuilder
            .Create($"ModifyAttackModeForWeapon{Name}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionPowerAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(modifyAttackModeForWeapon)
            .AddToDB();

        var powerAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(ValidatorsCharacter.HasNoneOfConditions(conditionPowerAttack.Name)))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerAttack);

        var powerTurnOffPowerAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffPowerAttack);

        var featPowerAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttack,
                powerTurnOffPowerAttack
            )
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffPowerAttack;
        modifyAttackModeForWeapon
            .SetCustomSubFeatures(
                concentrationProvider,
                new ModifyWeaponAttackModeFeatPowerAttack(featPowerAttack));

        return featPowerAttack;
    }

    private sealed class ModifyWeaponAttackModeFeatPowerAttack : IModifyWeaponAttackMode
    {
        private readonly FeatDefinition _featDefinition;

        public ModifyWeaponAttackModeFeatPowerAttack(FeatDefinition featDefinition)
        {
            _featDefinition = featDefinition;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(character, attackMode))
            {
                return;
            }

            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            const int TO_HIT = -3;
            var proficiency = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var toDamage = 3 + proficiency;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT, FeatureSourceType.Feat, _featDefinition.Name,
                _featDefinition));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage, FeatureSourceType.Feat, _featDefinition.Name,
                _featDefinition));
        }
    }

    #endregion

    #region Slasher

    private static readonly FeatureDefinition FeatureFeatSlasher = FeatureDefinitionBuilder
        .Create("FeatureFeatSlasher")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(
            new PhysicalAttackAfterDamageFeatSlasher(
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherHit")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionMovementAffinityBuilder
                            .Create("MovementAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherHit", Category.Condition)
                            .SetBaseSpeedAdditiveModifier(-2)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherCriticalHit")
                    .SetGuiPresentation(Category.Condition)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionCombatAffinityBuilder
                            .Create("CombatAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherCriticalHit", Category.Condition)
                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                            .AddToDB())
                    .AddToDB(),
                DamageTypeSlashing))
        .AddToDB();

    private static FeatDefinition BuildSlasherDex()
    {
        return FeatDefinitionBuilder
            .Create("FeatSlasherDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                FeatureFeatSlasher)
            .SetFeatFamily(GroupFeats.Slasher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildSlasherStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatSlasherStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                FeatureFeatSlasher)
            .SetFeatFamily(GroupFeats.Slasher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private sealed class PhysicalAttackAfterDamageFeatSlasher : IPhysicalAttackAfterDamage
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly ConditionDefinition _criticalConditionDefinition;
        private readonly string _damageType;

        internal PhysicalAttackAfterDamageFeatSlasher(
            ConditionDefinition conditionDefinition,
            ConditionDefinition criticalConditionDefinition,
            string damageType)
        {
            _conditionDefinition = conditionDefinition;
            _criticalConditionDefinition = criticalConditionDefinition;
            _damageType = damageType;
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetDefender.InflictCondition(
                    _conditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagCombat,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }

            if (outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            rulesetDefender.InflictCondition(
                _criticalConditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    #endregion
}
