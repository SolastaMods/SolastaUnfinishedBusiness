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
using static ActionDefinitions;
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
    #region Fencer

    internal static readonly FeatDefinition FeatFencer = FeatDefinitionBuilder
        .Create("FeatFencer")
        .SetGuiPresentation(Category.Feat)
        .AddCustomSubFeatures(
            new AddExtraMainHandAttack(
                ActionType.Bonus,
                ValidatorsCharacter.HasAttacked,
                ValidatorsCharacter.HasMeleeWeaponInMainHandAndFreeOffhand))
        .AddToDB();

    #endregion

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featAlwaysReady = BuildAlwaysReady();
        var featBladeMastery = BuildBladeMastery();
        var featCharger = BuildCharger();
        var featCleavingAttack = BuildCleavingAttack();
        var featCrusherStr = BuildCrusherStr();
        var featCrusherCon = BuildCrusherCon();
        var featDefensiveDuelist = BuildDefensiveDuelist();
        var featDevastatingStrikesDex = BuildDevastatingStrikesDex();
        var featDevastatingStrikesStr = BuildDevastatingStrikesStr();
        var featFellHanded = BuildFellHanded();
        var featGreatWeaponDefense = BuildGreatWeaponDefense();
        var featLongSwordFinesse = BuildLongswordFinesse();
        var featPiercerDex = BuildPiercerDex();
        var featPiercerStr = BuildPiercerStr();
        var featPowerAttack = BuildPowerAttack();
        var featRecklessAttack = BuildRecklessAttack();
        var featSavageAttack = BuildSavageAttack();
        var featSlasherStr = BuildSlasherStr();
        var featSlasherDex = BuildSlasherDex();
        var featSpearMastery = BuildSpearMastery();
        var featWhirlwindAttack = BuildWhirlWindAttack();

        feats.AddRange(
            featAlwaysReady,
            featBladeMastery,
            featCharger,
            featCleavingAttack,
            featCrusherCon,
            featCrusherStr,
            featDefensiveDuelist,
            featDevastatingStrikesDex,
            featDevastatingStrikesStr,
            featFellHanded,
            FeatFencer,
            featGreatWeaponDefense,
            featLongSwordFinesse,
            FeatOldTacticsDex,
            FeatOldTacticsStr,
            featPiercerDex,
            featPiercerStr,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSlasherDex,
            featSlasherStr,
            featSpearMastery,
            featWhirlwindAttack);

        var featGroupOldTactics = GroupFeats.MakeGroup("FeatGroupOldTactics", GroupFeats.OldTactics,
            FeatOldTacticsDex,
            FeatOldTacticsStr);

        var featGroupSlasher = GroupFeats.MakeGroup("FeatGroupSlasher", GroupFeats.Slasher,
            featSlasherDex,
            featSlasherStr);

        var featGroupDevastatingStrikes = GroupFeats.MakeGroup("FeatGroupDevastatingStrikes",
            GroupFeats.DevastatingStrikes,
            featDevastatingStrikesDex,
            featDevastatingStrikesStr);

        GroupFeats.FeatGroupCrusher.AddFeats(
            featCrusherStr,
            featCrusherCon);

        GroupFeats.FeatGroupPiercer.AddFeats(
            featPiercerDex,
            featPiercerStr);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featAlwaysReady,
            featDefensiveDuelist,
            featGreatWeaponDefense);

        GroupFeats.FeatGroupMeleeCombat.AddFeats(
            FeatFencer,
            featAlwaysReady,
            featBladeMastery,
            featCharger,
            featCleavingAttack,
            featDefensiveDuelist,
            featGroupDevastatingStrikes,
            featFellHanded,
            featLongSwordFinesse,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSpearMastery,
            featGroupOldTactics,
            featGroupSlasher,
            featWhirlwindAttack);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featGreatWeaponDefense);
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
                    .Create("DieRollModifierFeatSavageAttack")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll | MagicDamageValueRoll, 1, 0, 1,
                        "Feedback/&FeatSavageAttackReroll")
                    .AddToDB())
            .AddToDB();
    }

    #endregion

    #region Spear Mastery

    private static FeatDefinition BuildSpearMastery()
    {
        const string NAME = "FeatSpearMastery";
        const string REACH_CONDITION = $"Condition{NAME}Reach";

        var isSpear = ValidatorsWeapon.IsOfWeaponType(SpearType);

        var conditionFeatSpearMasteryReach = ConditionDefinitionBuilder
            .Create(REACH_CONDITION)
            .SetGuiPresentation($"Power{NAME}Reach", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .AddCustomSubFeatures(
                new IncreaseWeaponReach(1, isSpear, ValidatorsCharacter.HasAnyOfConditions(REACH_CONDITION)))
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
                            (OperationType.Set, !ranged && isSpear(mode, null, character))))
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
                WeaponValidator = isSpear,
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
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionFeatSpearMasteryCharge))
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
                            (OperationType.Set, !ranged && isSpear(mode, null, character))),
                        new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D8, DieType.D10), isSpear))
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

    #region Great Weapon Defense

    private static FeatDefinitionWithPrerequisites BuildGreatWeaponDefense()
    {
        const string NAME = "FeatGreatWeaponDefense";

        var attributeModifierArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .AddToDB();

        var conditionArmorClass = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ArmorClass")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(attributeModifierArmorClass)
            .AddToDB();

        var movementAffinity = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{NAME}")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(3)
            .AddToDB();

        var conditionMovement = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Movement")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFreedomOfMovement)
            .SetPossessive()
            .SetFeatures(movementAffinity)
            .AddToDB();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddCustomSubFeatures(new CustomBehaviorGreatWeaponDefense(conditionArmorClass, conditionMovement))
            .AddToDB();
    }

    private sealed class CustomBehaviorGreatWeaponDefense(
        ConditionDefinition conditionArmorClass,
        ConditionDefinition conditionMovement)
        : IPhysicalAttackFinishedByMe, IPhysicalAttackBeforeHitConfirmedOnEnemy, IOnReducedToZeroHpByMe,
            IOnItemEquipped
    {
        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (!HasFreeHandWithHeavyOrVersatileInMain(hero) &&
                hero.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionArmorClass.Name, out var activeCondition))
            {
                hero.RemoveCondition(activeCondition);
            }
        }

        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                conditionMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMovement.Name,
                0,
                0,
                0);
        }

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
            if (criticalHit)
            {
                yield return HandleReducedToZeroHpByMe(attacker, defender, attackMode, null);
            }
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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!HasFreeHandWithHeavyOrVersatileInMain(rulesetAttacker, attackMode) ||
                rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionArmorClass.Name))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                conditionArmorClass.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionArmorClass.Name,
                0,
                0,
                0);
        }

        private static bool HasFreeHandWithHeavyOrVersatileInMain(
            RulesetCharacter character,
            RulesetAttackMode attackMode = null)
        {
            var rulesetItem = character.GetMainWeapon();
            var itemDefinition = attackMode?.SourceDefinition as ItemDefinition ?? rulesetItem?.ItemDefinition;

            return
                ValidatorsCharacter.HasFreeHandConsiderGrapple(character) &&
                ((attackMode != null && ValidatorsWeapon.IsMelee(attackMode)) ||
                 (attackMode == null && ValidatorsWeapon.IsMelee(null, null, character))) &&
                ValidatorsWeapon.HasAnyWeaponTag(
                    itemDefinition, TagsDefinitions.WeaponTagHeavy, TagsDefinitions.WeaponTagVersatile);
        }
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
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .SetImpactParticleReference(FeatureDefinitionPowers.PowerRoguishHoodlumDirtyFighting)
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
                    ActionType.Bonus,
                    ValidatorsCharacter.HasMeleeWeaponInMainHandOrUnarmed,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionDashing)))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatCharger")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(featureExtraAttack, powerPool, powerAddDamage, powerShove)
            .AddToDB();
    }

    private sealed class PhysicalAttackBeforeHitConfirmedOnEnemyCharger(FeatureDefinitionPower powerPool)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IMoveStepStarted
    {
        private const string DirX = "DirectionX";
        private const string DirY = "DirectionY";
        private const string DirZ = "DirectionZ";
        private const string StraightLine = "StraightLine";

        private static readonly EffectForm ShoveForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
            .Build();

        public void MoveStepStarted(GameLocationCharacter mover, int3 source, int3 destination)
        {
            InitDirections(mover);

            var previousDirectionX = mover.UsedSpecialFeatures[DirX];
            var previousDirectionY = mover.UsedSpecialFeatures[DirY];
            var previousDirectionZ = mover.UsedSpecialFeatures[DirZ];

            var directionX = Math.Sign(source.x - destination.x);
            var directionY = Math.Sign(source.y - destination.y);
            var directionZ = Math.Sign(source.z - destination.z);

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
            var attackerPosition = attacker.LocationPosition;
            var defenderPosition = defender.LocationPosition;
            var attackDirectionX = Math.Sign(attackerPosition.x - defenderPosition.x);
            var attackDirectionY = Math.Sign(attackerPosition.y - defenderPosition.y);
            var attackDirectionZ = Math.Sign(attackerPosition.z - defenderPosition.z);

            InitDirections(attacker);

            if (!ValidatorsWeapon.IsMeleeOrUnarmed(attackMode) ||
                !attacker.OnceInMyTurnIsValid(powerPool.Name) ||
                attackDirectionX != attacker.UsedSpecialFeatures[DirX] ||
                attackDirectionY != attacker.UsedSpecialFeatures[DirY] ||
                attackDirectionZ != attacker.UsedSpecialFeatures[DirZ] ||
                attacker.UsedSpecialFeatures[StraightLine] < 2)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerPool, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerPool.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
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
        }

        private static void InitDirections(GameLocationCharacter mover)
        {
            mover.UsedSpecialFeatures.TryAdd(DirX, 0);
            mover.UsedSpecialFeatures.TryAdd(DirY, 0);
            mover.UsedSpecialFeatures.TryAdd(DirZ, 0);
            mover.UsedSpecialFeatures.TryAdd(StraightLine, 0);
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
                    .SetCasterEffectParameters(FeatureDefinitionPowers.PowerKnightLeadership)
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
        : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;

            if (action.AttackRollOutcome is not RollOutcome.Success ||
                helper != defender ||
                !helper.CanReact() ||
                (rulesetEffect != null && rulesetEffect.EffectDescription.RangeType is not RangeType.MeleeHit) ||
                (attackMode != null && !ValidatorsWeapon.IsMelee(attackMode)))
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var attackRoll = action.AttackRoll;
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

            var usablePower = PowerProvider.Get(powerDefensiveDuelist, rulesetHelper);

            yield return helper.MyReactToUsePower(
                Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "DefensiveDuelist",
                battleManager: battleManager);
        }
    }

    #endregion

    #region Old Tactics

    private static readonly FeatDefinition FeatOldTacticsStr = FeatDefinitionBuilder
        .Create("FeatOldTacticsStr")
        .SetGuiPresentation(Category.Feat)
        .SetFeatures(AttributeModifierCreed_Of_Einar)
        .SetFeatFamily(GroupFeats.OldTactics)
        .AddToDB();

    private static readonly FeatDefinition FeatOldTacticsDex = FeatDefinitionBuilder
        .Create("FeatOldTacticsDex")
        .SetGuiPresentation(Category.Feat)
        .SetFeatures(AttributeModifierCreed_Of_Misaye)
        .SetFeatFamily(GroupFeats.OldTactics)
        .AddToDB();

    internal static IEnumerator HandleFeatOldTactics(CharacterAction action)
    {
        var battleManager =
            ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!battleManager)
        {
            yield break;
        }

        var standingUpCharacter = action.ActingCharacter;

        if (Gui.Battle == null ||
            action.ActionId != Id.StandUp ||
            standingUpCharacter.Side == Side.Ally)
        {
            yield break;
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var ally in Gui.Battle.GetOpposingContenders(standingUpCharacter.Side))
        {
            if (ally.IsMyTurn() ||
                !ally.CanReact())
            {
                continue;
            }

            var rulesetAllyHero = ally.RulesetCharacter.GetOriginalHero();

            if (rulesetAllyHero == null ||
                !ally.IsWithinRange(standingUpCharacter, 1) ||
                (!rulesetAllyHero.TrainedFeats.Contains(FeatOldTacticsDex) &&
                 !rulesetAllyHero.TrainedFeats.Contains(FeatOldTacticsStr)))
            {
                continue;
            }

            var (retaliationMode, retaliationModifier) =
                ally.GetFirstMeleeModeThatCanAttack(standingUpCharacter, battleManager);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) =
                    ally.GetFirstRangedModeThatCanAttack(standingUpCharacter, battleManager);

                if (retaliationMode == null)
                {
                    continue;
                }
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            yield return ally.MyReactForOpportunityAttack(
                standingUpCharacter,
                standingUpCharacter,
                retaliationMode,
                retaliationModifier,
                "OldTactics",
                battleManager: battleManager);
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
            locationCharacter.ReadiedAction = ReadyActionType.Melee;
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
                !ValidatorsWeapon.IsMeleeOrUnarmed(attackMode))
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
            if (action.ActionType is ActionType.Reaction &&
                !attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag) &&
                ValidatorsWeapon.IsOfWeaponType(weaponTypeDefinition)(attackMode, null, null))
            {
                attackModifier.AttackAdvantageTrends.Add(
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

        var conditionCleavingAttackFinish = ConditionDefinitionBuilder
            .Create($"Condition{Name}Finish")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .AddCustomSubFeatures(new AddExtraMainHandAttack(ActionType.Bonus))
            .AddToDB();

        var actionAffinityCleavingAttackToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityCleavingAttackToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CleavingAttackToggle)
            .AddToDB();

        var featCleavingAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityCleavingAttackToggle)
            .AddToDB();

        featCleavingAttack.AddCustomSubFeatures(
            new CustomBehaviorCleaving(featCleavingAttack, conditionCleavingAttackFinish));

        return featCleavingAttack;
    }

    private sealed class CustomBehaviorCleaving(
        FeatDefinition featDefinition,
        ConditionDefinition conditionCleavingAttackFinish)
        : IOnReducedToZeroHpByMe, IPhysicalAttackFinishedByMe, IModifyWeaponAttackMode
    {
        private const int ToHit = -5;
        private const int ToDamage = +10;

        private readonly TrendInfo _attackTrendInfo =
            new(ToHit, FeatureSourceType.Feat, featDefinition.Name, featDefinition);

        private readonly TrendInfo _damageTrendInfo =
            new(ToDamage, FeatureSourceType.Feat, featDefinition.Name, featDefinition);

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!character.IsToggleEnabled((Id)ExtraActionId.CleavingAttackToggle))
            {
                return;
            }

            if (!ValidateCleavingAttack(character, attackMode, true))
            {
                return;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            attackMode.ToHitBonus += ToHit;
            attackMode.ToHitBonusTrends.Add(_attackTrendInfo);
            damage.BonusDamage += ToDamage;
            damage.DamageBonusTrends.Add(_damageTrendInfo);
        }

        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (!ValidateCleavingAttack(rulesetCharacter, attackMode))
            {
                yield break;
            }

            InflictCondition(rulesetCharacter);
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

            if (rollOutcome != RollOutcome.CriticalSuccess ||
                !ValidateCleavingAttack(rulesetCharacter, attackMode))
            {
                yield break;
            }

            InflictCondition(rulesetCharacter);
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

    private static bool ValidateCleavingAttack(
        RulesetCharacter character, RulesetAttackMode attackMode, bool validateHeavy = false)
    {
        if (attackMode?.SourceObject is not RulesetItem rulesetItem)
        {
            return false;
        }

        // don't use IsMelee(attackMode) in IModifyWeaponAttackMode as it will always fail
        return ValidatorsWeapon.IsMelee(null, rulesetItem, character) &&
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
                CampaignsContext.ActionAffinityFeatCrusherToggle)
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
                CampaignsContext.ActionAffinityFeatCrusherToggle)
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

            if (!ValidatorsWeapon.IsMeleeOrUnarmed(attackMode) ||
                attackMode.EffectDescription.FindFirstDamageForm()?.DamageType != DamageTypeBludgeoning)
            {
                yield break;
            }

            if (criticalHit)
            {
                actualEffectForms.Add(criticalEffectForm);
            }

            if (!attacker.OnceInMyTurnIsValid(SpecialFeatureName) ||
                !rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.FeatCrusherToggle))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(SpecialFeatureName, 1);
            actualEffectForms.Add(pushEffectForm);
        }
    }

    #endregion

    #region Devastating Strikes

    private static readonly FeatureDefinitionAdditionalDamage AdditionalDamageFeatDevastatingStrikes =
        FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageFeatDevastatingStrikes")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DevastatingStrikes")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
                    (OperationType.Set, ValidatorsWeapon.IsMeleeOrUnarmed(mode))),
                new PhysicalAttackBeforeHitConfirmedOnEnemyDevastatingStrikes(
                    ConditionDefinitionBuilder
                        .Create("ConditionDevastatingStrikes")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .AddCustomSubFeatures(new ModifyDamageAffinityDevastatingStrikes())
                        .AddToDB()))
            .AddToDB();

    private static FeatDefinition BuildDevastatingStrikesDex()
    {
        const string NAME = "FeatDevastatingStrikes";

        // kept name for backward compatibility
        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation($"{NAME}Dex", Category.Feat)
            .AddFeatures(AttributeModifierCreed_Of_Misaye, AdditionalDamageFeatDevastatingStrikes)
            .SetFeatFamily(GroupFeats.DevastatingStrikes)
            .AddToDB();

        return feat;
    }

    private static FeatDefinition BuildDevastatingStrikesStr()
    {
        const string NAME = "FeatDevastatingStrikesStr";

        var feat = FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(AttributeModifierCreed_Of_Einar, AdditionalDamageFeatDevastatingStrikes)
            .SetFeatFamily(GroupFeats.DevastatingStrikes)
            .AddToDB();

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
        PhysicalAttackBeforeHitConfirmedOnEnemyDevastatingStrikes(ConditionDefinition conditionBypassResistance)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
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
            if (!criticalHit ||
                !ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionBypassResistance.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionBypassResistance.Name,
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
            .SetExplicitAbilityScore(AttributeDefinitions.Strength)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm()
                            .Build())
                    .Build())
            .AddToDB();

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

    private sealed class PhysicalAttackFinishedByMeFeatFellHanded(
        FeatureDefinitionPower powerAdvantage,
        FeatureDefinitionPower powerDisadvantage,
        params WeaponTypeDefinition[] weaponTypeDefinition) : IPhysicalAttackFinishedByMe
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
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
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

            var attackModifier = action.ActionParams.ActionModifiers[0];
            var modifier = attackMode.ToHitBonus + attackModifier.AttackRollModifier;
            var advantageType = ComputeAdvantage(attackModifier.AttackAdvantageTrends);
            var attackRoll = 0;

            FeatureDefinitionPower power = null;

            switch (advantageType)
            {
                case AdvantageType.Advantage when rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess:
                    attacker.UsedSpecialFeatures.TryGetValue("LowestAttackRoll", out attackRoll);
                    power = powerAdvantage;

                    break;
                case AdvantageType.Disadvantage when rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure:
                    attacker.UsedSpecialFeatures.TryGetValue("HighestAttackRoll", out attackRoll);
                    power = powerDisadvantage;

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
                powerAdvantage,
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

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }
    }

    #endregion

    #region Piercer

    private static readonly FeatureDefinition FeatureFeatPiercer =
        FeatureDefinitionDieRollModifierBuilder
            .Create("FeatureFeatPiercer")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(AttackDamageValueRoll, 1, 0, 1, "Feedback/&FeatPiercerReroll")
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
        : CustomAdditionalDamage(provider), IValidateDieRollModifier, IAllowRerollDice
    {
        public bool IsValid(RulesetActor rulesetActor, bool attackModeDamage, DamageForm damageForm)
        {
            return damageForm.DamageType == DamageTypePiercing;
        }

        public bool CanModifyRoll(
            RulesetCharacter character, List<FeatureDefinition> features, List<string> damageTypes)
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

        var actionAffinityPowerAttackToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityPowerAttackToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.PowerAttackToggle)
            .AddToDB();

        var featPowerAttack = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityPowerAttackToggle)
            .AddToDB();

        featPowerAttack.AddCustomSubFeatures(new ModifyWeaponAttackModeFeatPowerAttack(featPowerAttack));

        return featPowerAttack;
    }

    private sealed class ModifyWeaponAttackModeFeatPowerAttack(FeatDefinition featDefinition) : IModifyWeaponAttackMode
    {
        private const int ToHit = 3;

        private readonly TrendInfo _attackTrendInfo =
            new(-ToHit, FeatureSourceType.Feat, featDefinition.Name, featDefinition);

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!character.IsToggleEnabled((Id)ExtraActionId.PowerAttackToggle))
            {
                return;
            }

            // don't use IsMelee(attackMode) in IModifyWeaponAttackMode as it will always fail
            if (!ValidatorsWeapon.IsMelee(null, attackMode.SourceObject as RulesetItem, character) &&
                !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                return;
            }

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var proficiency = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var toDamage = ToHit + proficiency;

            attackMode.ToHitBonus -= ToHit;
            attackMode.ToHitBonusTrends.Add(_attackTrendInfo);
            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(
                new TrendInfo(toDamage, FeatureSourceType.Feat, featDefinition.Name, featDefinition));
        }
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

    private sealed class PhysicalAttackAfterDamageFeatSlasher(
        ConditionDefinition conditionDefinition,
        ConditionDefinition criticalConditionDefinition,
        string damageType) : IPhysicalAttackFinishedByMe
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
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != damageType)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetDefender.InflictCondition(
                    conditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionDefinition.Name,
                    0,
                    0,
                    0);
            }

            if (rollOutcome is not RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                criticalConditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                criticalConditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Whirlwind Attack

    private static FeatDefinition BuildWhirlWindAttack()
    {
        const string NAME = "WhirlWindAttack";

        var powerWhirlWindAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation($"Feat{NAME}", Category.Feat,
                Sprites.GetSprite($"Power{NAME}", Resources.PowerWhirlWindAttack, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        powerWhirlWindAttack.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new ValidatorsValidatePowerUse(
                c => GameLocationCharacter.GetFromActor(c)?.OncePerTurnIsValid("PowerWhirlWindAttack") == true,
                ValidatorsCharacter.HasMainHandWeaponType(GreatswordType, MaulType, GreataxeType)),
            new CustomBehaviorWhirlWindAttack(powerWhirlWindAttack));

        var featureExtraBonusAttack = FeatureDefinitionBuilder
            .Create($"Feature{NAME}ExtraBonusAttack")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new AddWhirlWindFollowUpAttack(GreatswordType),
                new AddWhirlWindFollowUpAttack(MaulType),
                new AddWhirlWindFollowUpAttack(GreataxeType))
            .AddToDB();

        // kept name for backward compatibility
        return FeatDefinitionBuilder
            .Create("FeatWhirlWindAttackDex")
            .SetGuiPresentation($"Feat{NAME}", Category.Feat)
            .SetFeatures(powerWhirlWindAttack, featureExtraBonusAttack)
            .AddToDB();
    }

    private sealed class CustomBehaviorWhirlWindAttack(FeatureDefinitionPower powerWhirlWindAttack)
        : IPowerOrSpellFinishedByMe, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerWhirlWindAttack;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var actingCharacter = GameLocationCharacter.GetFromActor(character);

            if (actingCharacter == null)
            {
                return effectDescription;
            }

            var attackMode = actingCharacter.FindActionAttackMode(Id.AttackMain);
            var pb = actingCharacter.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            effectDescription.targetParameter = 1 + (pb / 2);
            effectDescription.rangeParameter = attackMode.ReachRange;

            return effectDescription;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var attackMode = actingCharacter.FindActionAttackMode(Id.AttackMain);

            actingCharacter.BurnOneMainAttack();
            actingCharacter.SetSpecialFeatureUses("PowerWhirlWindAttack", 0);

            foreach (var target in action.ActionParams.TargetCharacters)
            {
                actingCharacter.MyExecuteActionAttack(
                    Id.AttackFree,
                    target,
                    attackMode,
                    new ActionModifier());
            }
        }
    }

    #endregion
}
