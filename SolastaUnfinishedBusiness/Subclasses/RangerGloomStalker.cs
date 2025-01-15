using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Patches;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerGloomStalker : AbstractSubclass
{
    private const string Name = "RangerGloomStalker";

    public RangerGloomStalker()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, DetectEvilAndGood),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, Fear),
                BuildSpellGroup(13, GreaterInvisibility),
                BuildSpellGroup(17, SpellsContext.FarStep))
            .AddToDB();

        // Dread Ambusher

        var additionalDamageDreadAmbusher = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DreadAmbusher")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DreadAmbusher")
            .SetDamageDice(DieType.D8, 1)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetImpactParticleReference(ArcaneSword.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var actionAffinityDreadAmbusherMainAttack = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}DreadAmbusherMain")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionType.Main)
            .SetRestrictedActions(Id.AttackMain)
            .SetMaxAttacksNumber(1)
            .AddToDB();

        var conditionDreadAmbusherMainAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}DreadAmbusherMainAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(actionAffinityDreadAmbusherMainAttack, additionalDamageDreadAmbusher)
            .AddToDB();

        var actionAffinityDreadAmbusherBonusAttack = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}DreadAmbusherBonus")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionType.Bonus)
            .SetRestrictedActions(Id.AttackOff)
            .SetMaxAttacksNumber(1)
            .AddToDB();

        var conditionDreadAmbusherBonusAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}DreadAmbusherBonusAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(actionAffinityDreadAmbusherBonusAttack, additionalDamageDreadAmbusher)
            .AddToDB();

        var movementAffinityDreadAmbusher = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}DreadAmbusher")
            .SetGuiPresentation($"AttributeModifier{Name}DreadAmbusher", Category.Feature, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var conditionDreadAmbusher = ConditionDefinitionBuilder
            .Create($"Condition{Name}DreadAmbusher")
            .SetGuiPresentation($"AttributeModifier{Name}DreadAmbusher", Category.Feature,
                ConditionDefinitions.ConditionFreedomOfMovement)
            .SetPossessive()
            .SetFeatures(movementAffinityDreadAmbusher)
            .AddToDB();

        conditionDreadAmbusher.GuiPresentation.description = Gui.EmptyContent;

        var attributeModifierDreadAmbusher = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DreadAmbusher")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Wisdom)
            .AddCustomSubFeatures(
                new CustomBehaviorDreadAmbusher(
                    conditionDreadAmbusher,
                    conditionDreadAmbusherMainAttack,
                    conditionDreadAmbusherBonusAttack))
            .AddToDB();

        // Umbral Sight

        var senseDarkvision18 = FeatureDefinitionSenseBuilder
            .Create(FeatureDefinitionSenses.SenseDarkvision, "SenseDarkvision6")
            .SetGuiPresentationNoContent(true)
            .SetSense(SenseMode.Type.Darkvision, 18, 9)
            .AddToDB();

        var featureUmbralSight = FeatureDefinitionBuilder
            .Create($"Feature{Name}UmbralSight")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorUmbralSight(senseDarkvision18))
            .AddToDB();

        //
        // LEVEL 07
        //

        // Iron Mind

        var featureSetIronMind = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}IronMind")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfPakri,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfMaraike,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta)
            .AddToDB();

        //
        // LEVEL 11
        //

        // Stalker's Flurry

        var featureStalkersFlurry = FeatureDefinitionBuilder
            .Create($"Feature{Name}StalkersFlurry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureStalkersFlurry.AddCustomSubFeatures(new PhysicalAttackFinishedByMeStalkersFlurry(featureStalkersFlurry));

        //
        // LEVEL 15
        //

        // Shadowy Dodge

        var featureShadowyDodge = FeatureDefinitionBuilder
            .Create($"Feature{Name}ShadowyDodge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureShadowyDodge.AddCustomSubFeatures(new TryAlterOutcomeAttackShadowyDodge(featureShadowyDodge));

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerGloomStalker, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells, attributeModifierDreadAmbusher, featureUmbralSight)
            .AddFeaturesAtLevel(7, featureSetIronMind)
            .AddFeaturesAtLevel(11, featureStalkersFlurry)
            .AddFeaturesAtLevel(15, featureShadowyDodge)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Dread Ambusher
    //

    private sealed class CustomBehaviorDreadAmbusher(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDreadAmbusher,
        ConditionDefinition conditionDreadAmbusherMainAttack,
        ConditionDefinition conditionDreadAmbusherBonusAttack)
        : ICharacterBattleStartedListener, IPhysicalAttackFinishedByMe
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionDreadAmbusher.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDreadAmbusher.Name,
                0,
                0,
                0);
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

            if (battleManager.Battle.CurrentRound > 1 ||
                !attacker.OnceInMyTurnIsValid(conditionDreadAmbusher.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(conditionDreadAmbusher.Name, 1);

            var condition = action.ActionType == ActionType.Main
                ? conditionDreadAmbusherMainAttack
                : conditionDreadAmbusherBonusAttack;

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
    // Umbral Sight
    //

    private sealed class CustomBehaviorUmbralSight(FeatureDefinitionSense senseDarkvision18)
        : ICustomLevelUpLogic, IPreventEnemySenseMode
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            hero.ActiveFeatures[tag]
                .TryAdd(hero.GetFeaturesByType<FeatureDefinitionSense>()
                    .Any(x => x.SenseType == SenseMode.Type.Darkvision)
                    ? senseDarkvision18
                    : FeatureDefinitionSenses.SenseDarkvision);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }

        public List<SenseMode.Type> PreventedSenseModes(GameLocationCharacter attacker, RulesetCharacter defender)
        {
            return Main.Settings.AllowAlliesToPerceiveRangerGloomStalkerInNaturalDarkness &&
                   attacker.Side == defender.Side
                ? []
                : [SenseMode.Type.Darkvision];
        }
    }

    //
    // Stalker's Flurry
    //

    private sealed class PhysicalAttackFinishedByMeStalkersFlurry(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureStalkersFlurry) : IPhysicalAttackFinishedByMe
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
            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                !attacker.OnceInMyTurnIsValid(featureStalkersFlurry.Name))
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(featureStalkersFlurry.Name, 0);
            attacker.RulesetCharacter.LogCharacterUsedFeature(featureStalkersFlurry);

            if (action.ActionId == Id.AttackOpportunity)
            {
                var actionParams = new CharacterActionParams(
                    attacker,
                    Id.AttackOpportunity,
                    attackMode,
                    defender,
                    new ActionModifier());
                var actionAttack = new CharacterActionAttack(actionParams);

                yield return CharacterActionAttackPatcher.ExecuteImpl_Patch.ExecuteImpl(actionAttack);
            }
            else
            {
                var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

                attackModeCopy.Copy(attackMode);

                if (action.ActionId == Id.AttackOff)
                {
                    attackModeCopy.AddAttackTagAsNeeded(TwoWeaponCombatFeats.DualFlurryTriggerMark);
                }

                var actionModifier = new ActionModifier
                {
                    proximity = attacker.IsWithinRange(defender, attackMode.reachRange)
                        ? AttackProximity.Melee
                        : AttackProximity.Range
                };

                attacker.MyExecuteActionAttack(
                    Id.AttackFree,
                    defender,
                    attackModeCopy,
                    actionModifier);
            }
        }
    }

    //
    // Shadowy Dodge
    //

    private sealed class TryAlterOutcomeAttackShadowyDodge(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureShadowyDodge) : ITryAlterOutcomeAttack
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
            if (action.AttackRollOutcome != RollOutcome.Success ||
                helper != defender ||
                !helper.CanReact() ||
                !helper.CanPerceiveTarget(attacker))
            {
                yield break;
            }

            yield return defender.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "ShadowyDodge",
                "CustomReactionShadowyDodgeDescription".Localized(Category.Reaction),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var attackRoll = action.AttackRoll;
                var outcome = action.AttackRollOutcome;
                var rollCaption = outcome == RollOutcome.CriticalSuccess
                    ? "Feedback/&RollAttackCriticalSuccessTitle"
                    : "Feedback/&RollAttackSuccessTitle";
                var rulesetAttacker = attacker.RulesetCharacter;

                int roll;
                int toHitBonus;
                int successDelta;

                actionModifier.AttackAdvantageTrends.SetRange(new List<TrendInfo>
                {
                    new(-1, FeatureSourceType.CharacterFeature, featureShadowyDodge.Name, featureShadowyDodge)
                });

                if (attackMode != null)
                {
                    toHitBonus = attackMode.ToHitBonus + actionModifier.AttackRollModifier;
                    roll = rulesetAttacker.RollAttack(
                        toHitBonus,
                        defender.RulesetActor,
                        attackMode.SourceDefinition,
                        attackMode.ToHitBonusTrends,
                        false,
                        actionModifier.AttackAdvantageTrends,
                        attackMode.Ranged,
                        false,
                        actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        -1,
                        true);
                }
                else if (rulesetEffect != null)
                {
                    toHitBonus = rulesetEffect.MagicAttackBonus + actionModifier.AttackRollModifier;
                    roll = rulesetAttacker.RollMagicAttack(
                        rulesetEffect,
                        defender.RulesetActor,
                        rulesetEffect.GetEffectSource(),
                        actionModifier.AttacktoHitTrends,
                        actionModifier.AttackAdvantageTrends,
                        false,
                        actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        -1,
                        true);
                }
                // should never happen
                else
                {
                    return;
                }

                var rulesetDefender = defender.RulesetCharacter;
                var sign = toHitBonus >= 0 ? "+" : string.Empty;

                rulesetDefender.LogCharacterUsedFeature(
                    featureShadowyDodge,
                    "Feedback/&TriggerRerollLine",
                    false,
                    (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}{sign}{toHitBonus}"),
                    (ConsoleStyleDuplet.ParameterType.FailedRoll,
                        Gui.Format(rollCaption, $"{attackRoll + toHitBonus}")));

                action.AttackRollOutcome = outcome;
                action.AttackSuccessDelta = successDelta;
                action.AttackRoll = roll;
            }
        }
    }
}
