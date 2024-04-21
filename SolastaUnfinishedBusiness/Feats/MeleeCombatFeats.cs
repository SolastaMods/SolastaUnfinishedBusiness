using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Feats.FeatHelpers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class MeleeCombatFeats
{
    internal static FeatDefinition FeatFencer { get; private set; }

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        FeatFencer = BuildFencer();

        var featAlwaysReady = BuildAlwaysReady();
        var featBladeMastery = BuildBladeMastery();
        var featCharger = BuildCharger();
        var featCleavingAttack = BuildCleavingAttack();
        var featCrusherStr = BuildCrusherStr();
        var featCrusherCon = BuildCrusherCon();
        var featDefensiveDuelist = BuildDefensiveDuelist();
        var featDevastatingStrikes = BuildDevastatingStrikes();
        var featFellHanded = BuildFellHanded();
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
            FeatFencer,
            featAlwaysReady,
            featBladeMastery,
            featCharger,
            featCleavingAttack,
            featCrusherCon,
            featCrusherStr,
            featDefensiveDuelist,
            featDevastatingStrikes,
            featFellHanded,
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

        var featGroupOldTactics = GroupFeats.MakeGroup("FeatGroupOldTactics", GroupFeats.OldTactics,
            featOldTacticsDex,
            featOldTacticsStr);

        var featGroupSlasher = GroupFeats.MakeGroup("FeatGroupSlasher", GroupFeats.Slasher,
            featSlasherDex,
            featSlasherStr);

        GroupFeats.FeatGroupCrusher.AddFeats(
            featCrusherStr,
            featCrusherCon);

        GroupFeats.FeatGroupPiercer.AddFeats(
            featPiercerDex,
            featPiercerStr);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featAlwaysReady,
            featDefensiveDuelist);

        GroupFeats.FeatGroupMeleeCombat.AddFeats(
            FeatFencer,
            featAlwaysReady,
            featBladeMastery,
            featCharger,
            featCleavingAttack,
            featDefensiveDuelist,
            featDevastatingStrikes,
            featFellHanded,
            featHammerThePoint,
            featLongSwordFinesse,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSpearMastery,
            featGroupOldTactics,
            featGroupSlasher);
    }

    #region Reckless Attack

    private static FeatDefinitionWithPrerequisites BuildRecklessAttack()
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
            .AddCustomSubFeatures(
                new IncreaseWeaponReach(1, validWeapon, ValidatorsCharacter.HasAnyOfConditions(REACH_CONDITION)))
            .AddToDB();

        var powerFeatSpearMasteryReach = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Reach")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("SpearMasteryReach", Resources.SpearMasteryReach, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(SpellDefinitions.Shield)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionFeatSpearMasteryReach))
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        var conditionDamage = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Damage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("SpearMastery")
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                    //Adding any property so that custom restricted context would trigger
                    .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                    .AddCustomSubFeatures(new ValidateContextInsteadOfRestrictedProperty(
                        (_, _, character, _, ranged, mode, _) =>
                            (OperationType.Set, !ranged && validWeapon(mode, null, character))))
                    .SetIgnoreCriticalDoubleDice(true)
                    .AddToDB())
            .AddToDB();

        var conditionFeatSpearMasteryCharge = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Charge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .AddCustomSubFeatures(new CanMakeAoOOnReachEntered
            {
                AllowRange = false,
                AccountAoOImmunity = true,
                WeaponValidator = validWeapon,
                BeforeReaction = AddCondition,
                AfterReaction = RemoveCondition
            })
            .AddToDB();

        var powerFeatSpearMasteryCharge = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Charge")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite($"Power{NAME}Charge", Resources.SpearMasteryCharge, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
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
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, ranged, mode, _) =>
                            (OperationType.Set, !ranged && validWeapon(mode, null, character))),
                        new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D8, DieType.D10), validWeapon))
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
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDamage.Name,
                0,
                0,
                0);

            yield break;
        }

        IEnumerator RemoveCondition(GameLocationCharacter attacker, GameLocationCharacter defender,
            GameLocationBattleManager manager, GameLocationActionManager actionManager, ReactionRequest request)
        {
            attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionDamage.Name);

            yield break;
        }
    }

    #endregion

    #region Longsword Finesse

    private static FeatDefinitionWithPrerequisites BuildLongswordFinesse()
    {
        const string Name = "FeatLongswordFinesse";

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(LongswordType);

        var attributeModifierArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasLongswordInHands)
            .AddCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                attributeModifierArmorClass)
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
            .AddCustomSubFeatures(
                new AddExtraMainHandAttack(
                    ActionDefinitions.ActionType.Bonus,
                    ValidatorsCharacter.HasAttacked,
                    ValidatorsCharacter.HasFreeHandWithoutTwoHandedInMain,
                    ValidatorsCharacter.HasMeleeWeaponInMainHand))
            .AddToDB();
    }

    #endregion

    #region Charger

    private static FeatDefinition BuildCharger()
    {
        const string Name = "FeatCharger";

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .AddToDB();

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new PhysicalAttackBeforeHitConfirmedOnEnemyCharger(powerPool));

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Charger")
            .SetDamageDice(DieType.D8, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetImpactParticleReference(FeatureDefinitionPowers.PowerRoguishHoodlumDirtyFighting)
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
                    (OperationType.Set, mode != null)))
            .AddToDB();

        var conditionAdditionalDamage = ConditionDefinitionBuilder
            .Create($"Condition{Name}AdditionalDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamage)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerAddDamage = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}AddDamage")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAdditionalDamage))
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerShove = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}Shove")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPool, true, powerAddDamage, powerShove);

        var featureExtraAttack = FeatureDefinitionBuilder
            .Create($"Feature{Name}ExtraAttack")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new AddExtraMainHandAttack(
                    ActionDefinitions.ActionType.Bonus,
                    ValidatorsCharacter.HasMeleeWeaponOrUnarmedInMainHand,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionDashing)))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatCharger")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(featureExtraAttack, powerPool, powerAddDamage, powerShove)
            .AddToDB();
    }

    internal sealed class PhysicalAttackBeforeHitConfirmedOnEnemyCharger(FeatureDefinitionPower powerPool)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private const string DirX = "DirectionX";
        private const string DirY = "DirectionY";
        private const string DirZ = "DirectionZ";
        private const string StraightLine = "StraightLine";

        private static readonly EffectForm ShoveForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
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
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager ||
                battleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            var attackerPosition = attacker.LocationPosition;
            var defenderPosition = defender.LocationPosition;

            var attackDirectionX = Math.Sign(attackerPosition.x - defenderPosition.x);
            var attackDirectionY = Math.Sign(attackerPosition.y - defenderPosition.y);
            var attackDirectionZ = Math.Sign(attackerPosition.z - defenderPosition.z);

            InitDirections(attacker);

            if ((!ValidatorsWeapon.IsMelee(attackMode) &&
                 !ValidatorsWeapon.IsUnarmed(attackMode)) ||
                !attacker.OncePerTurnIsValid(powerPool.Name) ||
                attackDirectionX != attacker.UsedSpecialFeatures[DirX] ||
                attackDirectionY != attacker.UsedSpecialFeatures[DirY] ||
                attackDirectionZ != attacker.UsedSpecialFeatures[DirZ] ||
                attacker.UsedSpecialFeatures[StraightLine] < 2)
            {
                yield break;
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPool, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { actionModifier },
                StringParameter = powerPool.Name,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(powerPool.Name, 0);

            // add the shove form direct to the attack
            var option = reactionRequest.SelectedSubOption;
            var subPowers = powerPool.GetBundle()?.SubPowers;

            if (subPowers != null &&
                subPowers[option].Name == "PowerFeatChargerShove")
            {
                actualEffectForms.Add(ShoveForm);
            }
        }

        private static void InitDirections(GameLocationCharacter mover)
        {
            mover.UsedSpecialFeatures.TryAdd(DirX, 0);
            mover.UsedSpecialFeatures.TryAdd(DirY, 0);
            mover.UsedSpecialFeatures.TryAdd(DirZ, 0);
            mover.UsedSpecialFeatures.TryAdd(StraightLine, 0);
        }

        internal static void RecordStraightLine(GameLocationCharacter mover, int3 destination)
        {
            InitDirections(mover);

            var origin = mover.LocationPosition;

            var previousDirectionX = mover.UsedSpecialFeatures[DirX];
            var previousDirectionY = mover.UsedSpecialFeatures[DirY];
            var previousDirectionZ = mover.UsedSpecialFeatures[DirZ];

            var directionX = Math.Sign(origin.x - destination.x);
            var directionY = Math.Sign(origin.y - destination.y);
            var directionZ = Math.Sign(origin.z - destination.z);

            mover.UsedSpecialFeatures[DirX] = directionX;
            mover.UsedSpecialFeatures[DirY] = directionY;
            mover.UsedSpecialFeatures[DirZ] = directionZ;

            mover.UsedSpecialFeatures[StraightLine] =
                previousDirectionX == directionX &&
                previousDirectionY == directionY &&
                previousDirectionZ == directionZ
                    ? mover.UsedSpecialFeatures[StraightLine] + 1
                    : 1;
        }
    }

    #endregion

    #region Defensive Duelist

    private static FeatDefinition BuildDefensiveDuelist()
    {
        const string NAME = "FeatDefensiveDuelist";

        var conditionDefensiveDuelist = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetModifier(
                        AttributeModifierOperation.AddProficiencyBonus,
                        AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        var powerDefensiveDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDefensiveDuelist))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerKnightLeadership)
                    .Build())
            .AddToDB();

        powerDefensiveDuelist.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitPossibleOnMeOrAllyDefensiveDuelist(powerDefensiveDuelist));

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerDefensiveDuelist)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private class AttackBeforeHitPossibleOnMeOrAllyDefensiveDuelist(FeatureDefinitionPower powerDefensiveDuelist)
        : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            if ((rulesetEffect != null &&
                 rulesetEffect.EffectDescription.RangeType is not RangeType.MeleeHit) ||
                !ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (helper != defender ||
                !defender.CanReact() ||
                !ValidatorsWeapon.HasAnyWeaponTag(rulesetHelper.GetMainWeapon(), TagsDefinitions.WeaponTagFinesse))
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var totalAttack =
                attackRoll +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0) +
                actionModifier.AttackRollModifier;

            if (armorClass > totalAttack)
            {
                yield break;
            }

            var pb = rulesetHelper.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            if (armorClass + pb <= totalAttack)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerDefensiveDuelist, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "DefensiveDuelist",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }

    #endregion

    #region Hammer the Point

    private static FeatDefinition BuildHammerThePoint()
    {
        const string Name = "FeatHammerThePoint";

        var conditionHammerThePoint = ConditionDefinitionBuilder
            .Create($"Condition{Name}HammerThePoint")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
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

        additionalDamageHammerThePoint.AddCustomSubFeatures(
            new PhysicalAttackInitiatedByMeFeatHammerThePoint(conditionHammerThePoint, featHammerThePoint));

        return featHammerThePoint;
    }

    private sealed class PhysicalAttackInitiatedByMeFeatHammerThePoint(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionHammerThePoint,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatDefinition featHammerThePoint)
        : IPhysicalAttackInitiatedByMe
    {
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var attackedThisTurnCount = rulesetDefender.AllConditions
                .Count(x => x.ConditionDefinition == conditionHammerThePoint);

            if (attackedThisTurnCount == 0)
            {
                yield break;
            }

            var trendInfo = new TrendInfo(
                attackedThisTurnCount, FeatureSourceType.Feat, featHammerThePoint.Name, featHammerThePoint);

            attackModifier.AttackRollModifier += attackedThisTurnCount;
            attackModifier.AttacktoHitTrends.Add(trendInfo);

            var damage = attackMode?.EffectDescription.FindFirstDamageForm();

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
            .AddCustomSubFeatures(new ActionFinishedByEnemyOldTactics())
            .SetFeatFamily(GroupFeats.OldTactics)
            .AddToDB();
    }

    private static FeatDefinition BuildOldTacticsDex()
    {
        const string Name = "FeatOldTacticsDex";

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye)
            .AddCustomSubFeatures(new ActionFinishedByEnemyOldTactics())
            .SetFeatFamily(GroupFeats.OldTactics)
            .AddToDB();
    }

    private sealed class ActionFinishedByEnemyOldTactics : IActionFinishedByEnemy
    {
        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!actionManager ||
                battleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (characterAction.ActionId != ActionDefinitions.Id.StandUp)
            {
                yield break;
            }

            if (target.IsMyTurn() ||
                !target.CanReact())
            {
                yield break;
            }

            var enemy = characterAction.ActingCharacter;

            if (!target.IsWithinRange(enemy, 1))
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

            var actionParams = new CharacterActionParams(target, ActionDefinitions.Id.AttackOpportunity)
            {
                StringParameter = target.Name,
                ActionModifiers = { retaliationModifier },
                AttackMode = retaliationMode,
                TargetCharacters = { enemy }
            };
            var reactionRequest = new ReactionRequestReactionAttack("OldTactics", actionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(enemy, actionManager, count);
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
            .AddToDB();

        var featureAlwaysReady = FeatureDefinitionBuilder
            .Create("FeatureAlwaysReady")
            .SetGuiPresentation("FeatAlwaysReady", Category.Feat)
            .AddToDB();

        featureAlwaysReady.AddCustomSubFeatures(
            new CustomBehaviorAlwaysReady(conditionAlwaysReady, featureAlwaysReady));

        return FeatDefinitionBuilder
            .Create("FeatAlwaysReady")
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(featureAlwaysReady)
            .AddToDB();
    }

    private sealed class CustomBehaviorAlwaysReady(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : IPhysicalAttackFinishedByMe, ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDefinition.Name))
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedFeature(featureDefinition);
            locationCharacter.ReadiedAction = ActionDefinitions.ReadyActionType.Melee;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode)))
            {
                yield break;
            }

            rulesetCharacter.InflictCondition(
                conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
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

        var weaponTypes = new[] { DaggerType, ShortswordType, LongswordType, ScimitarType, RapierType, GreatswordType };

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetModifier(
                        AttributeModifierOperation.Additive,
                        AttributeDefinitions.ArmorClass, 1)
                    .SetSituationalContext(ExtraSituationalContext.HasBladeMasteryWeaponTypesInHands)
                    .AddToDB())
            .AddToDB();

        feat.AddCustomSubFeatures(
            new AttackComputeModifierFeatBladeMastery(feat, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class AttackComputeModifierFeatBladeMastery(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatDefinition featDefinition,
        params WeaponTypeDefinition[] weaponTypeDefinition)
        : IPhysicalAttackInitiatedByMe
    {
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            if (attackMode is { ActionType: ActionDefinitions.ActionType.Reaction } &&
                !attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag) &&
                ValidatorsWeapon.IsOfWeaponType(weaponTypeDefinition)(attackMode, null, null))
            {
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Feat, featDefinition.Name, featDefinition));
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

        var conditionCleavingAttackFinish = ConditionDefinitionBuilder
            .Create($"Condition{Name}Finish")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create($"Feature{Name}Finish")
                    .SetGuiPresentation($"Condition{Name}Finish", Category.Condition, Gui.NoLocalization)
                    .AddCustomSubFeatures(
                        ValidateAdditionalActionAttack.MeleeOnly,
                        new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus))
                    .AddToDB())
            .AddToDB();

        var conditionCleavingAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerCleavingAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite(nameof(Resources.PowerAttackIcon), Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCleavingAttack, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                IgnoreInvisibilityInterruptionCheck.Marker,
                new ValidatorsValidatePowerUse(
                    ValidatorsCharacter.HasNoneOfConditions(conditionCleavingAttack.Name)))
            .AddToDB();

        var powerTurnOffCleavingAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCleavingAttack, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(IgnoreInvisibilityInterruptionCheck.Marker)
            .AddToDB();

        var featCleavingAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerCleavingAttack,
                powerTurnOffCleavingAttack,
                FeatureDefinitionBuilder
                    .Create($"Feature{Name}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(new CustomBehaviorCleaving(conditionCleavingAttackFinish))
                    .AddToDB())
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffCleavingAttack;
        conditionCleavingAttack
            .AddCustomSubFeatures(
                concentrationProvider,
                new ModifyWeaponAttackModeFeatCleavingAttack(featCleavingAttack));

        return featCleavingAttack;
    }

    private sealed class CustomBehaviorCleaving(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionCleavingAttackFinish) : IOnReducedToZeroHpByMe, IPhysicalAttackFinishedByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!ValidateCleavingAttack(attackMode))
            {
                yield break;
            }

            InflictCondition(attacker.RulesetCharacter);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome != RollOutcome.CriticalSuccess ||
                !ValidateCleavingAttack(attackMode))
            {
                yield break;
            }

            InflictCondition(attacker.RulesetCharacter);
        }

        private void InflictCondition(RulesetCharacter rulesetCharacter)
        {
            rulesetCharacter.InflictCondition(
                conditionCleavingAttackFinish.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionCleavingAttackFinish.Name,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyWeaponAttackModeFeatCleavingAttack(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatDefinition featDefinition)
        : IModifyWeaponAttackMode
    {
        private const int ToHit = -5;
        private const int ToDamage = +10;

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidateCleavingAttack(attackMode, true))
            {
                return;
            }

            attackMode.ToHitBonus += ToHit;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(ToHit, FeatureSourceType.Feat,
                featDefinition.Name, featDefinition));
            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.BonusDamage += ToDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(ToDamage, FeatureSourceType.Feat,
                featDefinition.Name, featDefinition));
        }
    }

    private static bool ValidateCleavingAttack(RulesetAttackMode attackMode, bool validateHeavy = false)
    {
        return ValidatorsWeapon.IsMelee(attackMode) &&
               (!validateHeavy ||
                ValidatorsWeapon.HasAnyWeaponTag(
                    attackMode.SourceDefinition as ItemDefinition, TagsDefinitions.WeaponTagHeavy));
    }

    #endregion

    #region Crusher

    private static readonly FeatureDefinition FeatureFeatCrusher = FeatureDefinitionBuilder
        .Create("FeatureFeatCrusher")
        .SetGuiPresentationNoContent(true)
        .AddCustomSubFeatures(
            new PhysicalPhysicalAttackFinishedByMeCrusher(
                EffectFormBuilder.ConditionForm(
                    ConditionDefinitionBuilder
                        .Create("ConditionFeatCrusherCriticalHit")
                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
                        .SetConditionType(ConditionType.Detrimental)
                        .SetSpecialDuration(DurationType.Round, 1,
                            (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                        .SetFeatures(
                            FeatureDefinitionCombatAffinityBuilder
                                .Create("CombatAffinityFeatCrusher")
                                .SetGuiPresentation("ConditionFeatCrusherCriticalHit", Category.Condition,
                                    Gui.NoLocalization)
                                .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                                .AddToDB())
                        .AddToDB()),
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                    .Build()))
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

    private sealed class PhysicalPhysicalAttackFinishedByMeCrusher(
        EffectForm criticalEffectForm,
        EffectForm pushEffectForm)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private const string SpecialFeatureName = "FeatureCrusher";

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

            if (!ValidatorsWeapon.IsMelee(attackMode) ||
                !ValidatorsWeapon.IsOfDamageType(DamageTypeBludgeoning)(attackMode, null, null))
            {
                yield break;
            }

            if (criticalHit)
            {
                actualEffectForms.Add(criticalEffectForm);
            }

            if (!attacker.OncePerTurnIsValid(SpecialFeatureName) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.FeatCrusherToggle))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(SpecialFeatureName, 1);
            actualEffectForms.Add(pushEffectForm);
        }
    }

    #endregion

    #region Devastating Strikes

    private static FeatDefinition BuildDevastatingStrikes()
    {
        const string NAME = "FeatDevastatingStrikes";

        var weaponTypes = new[] { GreatswordType, GreataxeType, MaulType };

        var additionalDamageSunderingBlow = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DevastatingStrikes")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) => (OperationType.Set,
                    ValidatorsWeapon.IsOfWeaponType(weaponTypes)(mode, null, null))))
            .AddToDB();

        var conditionDevastatingStrikes = ConditionDefinitionBuilder
            .Create("ConditionDevastatingStrikes")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddCustomSubFeatures(new ModifyDamageAffinityDevastatingStrikes())
            .AddToDB();

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(additionalDamageSunderingBlow)
            .AddToDB();

        feat.AddCustomSubFeatures(
            new PhysicalAttackBeforeHitConfirmedOnEnemyDevastatingStrikes(conditionDevastatingStrikes, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class ModifyDamageAffinityDevastatingStrikes : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Resistance });
        }
    }

    private sealed class
        PhysicalAttackBeforeHitConfirmedOnEnemyDevastatingStrikes : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private readonly ConditionDefinition _conditionBypassResistance;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = [];

        public PhysicalAttackBeforeHitConfirmedOnEnemyDevastatingStrikes(
            ConditionDefinition conditionBypassResistance,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
            _conditionBypassResistance = conditionBypassResistance;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(GameLocationBattleManager battleManager,
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
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _conditionBypassResistance.Name,
                0,
                0,
                0);

            var damageForm = actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (damageForm == null)
            {
                yield break;
            }

            var newDamageForm = EffectFormBuilder
                .Create()
                .SetDamageForm(damageForm.DamageForm.DamageType, 1, damageForm.DamageForm.DieType)
                .Build();

            newDamageForm.DamageForm.IgnoreCriticalDoubleDice = true;

            actualEffectForms.Add(newDamageForm);
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
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddToDB();

        var fellHandedDisadvantage = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Disadvantage")
            .SetGuiPresentation(NAME, Category.Feat, $"Feature/&Power{NAME}DisadvantageDescription", hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm())
                    .Build())
            .AddToDB();

        fellHandedDisadvantage.AddCustomSubFeatures(
            new ModifyEffectDescriptionPowerDisadvantage(fellHandedDisadvantage));

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(fellHandedAdvantage)
            .AddToDB();

        fellHandedAdvantage.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeFeatFellHanded(fellHandedAdvantage, fellHandedDisadvantage, weaponTypes),
            new ModifyWeaponAttackModeTypeFilter(feat, weaponTypes));

        return feat;
    }

    private sealed class PhysicalAttackFinishedByMeFeatFellHanded : IPhysicalAttackFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerAdvantage;
        private readonly FeatureDefinitionPower _powerDisadvantage;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = [];

        public PhysicalAttackFinishedByMeFeatFellHanded(
            FeatureDefinitionPower powerAdvantage,
            FeatureDefinitionPower powerDisadvantage,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _powerAdvantage = powerAdvantage;
            _powerDisadvantage = powerDisadvantage;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var attackModifier = action.ActionParams.ActionModifiers[0];
            var modifier = attackMode.ToHitBonus + attackModifier.AttackRollModifier;
            var advantageType = ComputeAdvantage(attackModifier.attackAdvantageTrends);
            var attackRoll = 0;

            FeatureDefinitionPower power = null;

            switch (advantageType)
            {
                case AdvantageType.Advantage when rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess:
                    attacker.UsedSpecialFeatures.TryGetValue("LowestAttackRoll", out attackRoll);
                    power = _powerAdvantage;

                    break;
                case AdvantageType.Disadvantage when rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure:
                    attacker.UsedSpecialFeatures.TryGetValue("HighestAttackRoll", out attackRoll);
                    power = _powerDisadvantage;

                    break;
                case AdvantageType.None:
                default:
                    break;
            }

            if (!power || attackRoll == 0)
            {
                yield break;
            }

            var outcome = GLBM.GetAttackResult(attackRoll, modifier, rulesetDefender);

            Gui.Game.GameConsole.AttackRolled(
                rulesetAttacker,
                rulesetDefender,
                _powerAdvantage,
                outcome,
                attackRoll + modifier,
                attackRoll,
                modifier,
                attackModifier.AttacktoHitTrends,
                []);

            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    private sealed class ModifyEffectDescriptionPowerDisadvantage(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition powerDisadvantage) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDisadvantage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var strength = character.TryGetAttributeValue(AttributeDefinitions.Strength);
            var strMod = Math.Max(1, AttributeDefinitions.ComputeAbilityScoreModifier(strength));
            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.BonusDamage = strMod;

            return effectDescription;
        }
    }

    #endregion

    #region Piercer

    private static readonly FeatureDefinition FeatureFeatPiercer =
        FeatureDefinitionDieRollModifierBuilder
            .Create("FeatureFeatPiercer")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatPiercerReroll")
            .AddCustomSubFeatures(
                new CustomAdditionalDamageFeatPiercer(
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create("AdditionalDamageFeatPiercer")
                        .SetGuiPresentationNoContent(true)
                        .SetNotificationTag(GroupFeats.Piercer)
                        .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                        .SetIgnoreCriticalDoubleDice(true)
                        .AddToDB()))
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

    private sealed class CustomAdditionalDamageFeatPiercer(IAdditionalDamageProvider provider)
        : CustomAdditionalDamage(provider), IValidateDieRollModifier
    {
        public bool CanModifyRoll(RulesetCharacter character, List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return damageTypes.Contains(DamageTypePiercing);
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

            return criticalHit && damage is { DamageType: DamageTypePiercing };
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

        var conditionPowerAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                IgnoreInvisibilityInterruptionCheck.Marker,
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(conditionPowerAttack.Name)))
            .AddToDB();

        var powerTurnOffPowerAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(IgnoreInvisibilityInterruptionCheck.Marker)
            .AddToDB();

        var featPowerAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttack,
                powerTurnOffPowerAttack)
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffPowerAttack;
        conditionPowerAttack.AddCustomSubFeatures(
            concentrationProvider,
            new ModifyWeaponAttackModeFeatPowerAttack(featPowerAttack));

        return featPowerAttack;
    }

    private sealed class ModifyWeaponAttackModeFeatPowerAttack(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatDefinition featDefinition) : IModifyWeaponAttackMode
    // thrown is allowed on power attack
    //, IPhysicalAttackInitiatedByMe
    {
        private const int ToHit = 3;

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            var proficiency = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var toDamage = ToHit + proficiency;

            attackMode.ToHitBonus -= ToHit;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(-ToHit, FeatureSourceType.Feat, featDefinition.Name,
                featDefinition));

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage, FeatureSourceType.Feat, featDefinition.Name,
                featDefinition));
        }

// thrown is allowed on power attack
#if false
        // this is required to handle thrown scenarios
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var isMelee = ValidatorsWeapon.IsMelee(attackMode);
            var isUnarmed = ValidatorsWeapon.IsUnarmed(attackMode);
            var isPowerAttackValid = isMelee || isUnarmed;

            if (isPowerAttackValid)
            {
                yield break;
            }

            attackModifier.AttacktoHitTrends.RemoveAll(x => x.sourceName == _featDefinition.Name);
            attackMode.ToHitBonusTrends.RemoveAll(x => x.sourceName == _featDefinition.Name);
            attackMode.ToHitBonus += ToHit;

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                yield break;
            }

            var proficiency = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var toDamage = ToHit + proficiency;

            damageForm.DamageBonusTrends.RemoveAll(x => x.sourceName == _featDefinition.Name);
            damageForm.BonusDamage -= toDamage;
        }
#endif
    }

    #endregion

    #region Slasher

    private static readonly FeatureDefinition FeatureFeatSlasher = FeatureDefinitionBuilder
        .Create("FeatureFeatSlasher")
        .SetGuiPresentationNoContent(true)
        .AddCustomSubFeatures(
            new PhysicalAttackAfterDamageFeatSlasher(
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherHit")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionMovementAffinityBuilder
                            .Create("MovementAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherHit", Category.Condition, Gui.NoLocalization)
                            .SetBaseSpeedAdditiveModifier(-2)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherCriticalHit")
                    .SetGuiPresentation(Category.Condition)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionCombatAffinityBuilder
                            .Create("CombatAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherCriticalHit", Category.Condition,
                                Gui.NoLocalization)
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

    private sealed class PhysicalAttackAfterDamageFeatSlasher : IPhysicalAttackFinishedByMe
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

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetDefender.InflictCondition(
                    _conditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    _conditionDefinition.Name,
                    0,
                    0,
                    0);
            }

            if (rollOutcome is not RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _criticalConditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _criticalConditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion
}
