using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheTempest : AbstractSubclass
{
    private const string Name = "WayOfTheTempest";

    internal WayOfTheTempest()
    {
        // LEVEL 03

        // Tempest's Swiftness

        var movementAffinityTempestSwiftness = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}TempestSwiftness")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .SetCustomSubFeatures(new OnAfterActionTempestSwiftness())
            .AddToDB();

        var combatAffinityTempestSwiftness = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}TempestSwiftness")
            .SetGuiPresentation($"Condition{Name}TempestSwiftness", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionTempestSwiftness = ConditionDefinitionBuilder
            .Create($"Condition{Name}TempestSwiftness")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityTempestSwiftness)
            .AddSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var additionalDamageTempestSwiftness = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}TempestSwiftness")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetTargetCondition(
                ConditionDefinitions.ConditionMonkFlurryOfBlowsUnarmedStrikeBonus,
                (AdditionalDamageTriggerCondition)ExtraAdditionalDamageTriggerCondition.SourceHasCondition)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionTempestSwiftness,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        var featureSetTempestSwiftness = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TempestSwiftness")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(movementAffinityTempestSwiftness, additionalDamageTempestSwiftness)
            .AddToDB();

        // LEVEL 06

        // Storm Surge

        var powerStormSurge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}StormSurge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerStormSurge.SetCustomSubFeatures(new CustomBehaviorStormSurge(powerStormSurge));

        // LEVEL 11

        // Tempest’s Fury

        var powerTempestFuryLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFuryLeap")
            .SetGuiPresentation($"Power{Name}TempestFury", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 0, TargetType.Individuals)
                    .SetDurationData(DurationType.Dispelled)
                    .SetSavingThrowData(true, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTempestSwiftness, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("TempestFury", Resources.PowerTempestFury, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfTurn)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionMonkFlurryOfBlowsUnarmedStrikeBonus,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionDisengaging,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(ValidatorsPowerUse.InCombat, new ValidatorsPowerUse(ValidatorsCharacter.HasAttacked))
            .AddToDB();

        powerTempestFury.SetCustomSubFeatures(new OnAfterActionTempestFury(powerTempestFury, powerTempestFuryLeap));

        // LEVEL 17

        // Unfettered Deluge

        var featureUnfetteredDeluge = FeatureDefinitionBuilder
            .Create($"Feature{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new CustomCodeUnfetteredDeluge())
            .AddToDB();

        var movementAffinityUnfetteredDeluge = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .SetCustomSubFeatures(new OnAfterActionTempestSwiftness())
            .AddToDB();

        var abilityCheckAffinityUnfetteredDeluge = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Strength, SkillDefinitions.Acrobatics))
            .AddToDB();

        var conditionAffinityUnfetteredDeluge = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetConditionType(ConditionDefinitions.ConditionSlowed)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .AddToDB();

        var featureSetUnfetteredDeluge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}UnfetteredDeluge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                featureUnfetteredDeluge,
                movementAffinityUnfetteredDeluge,
                abilityCheckAffinityUnfetteredDeluge,
                conditionAffinityUnfetteredDeluge)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheTempest, 256))
            .AddFeaturesAtLevel(3, featureSetTempestSwiftness, powerStormSurge, powerTempestFury)
            .AddFeaturesAtLevel(6, powerStormSurge)
            .AddFeaturesAtLevel(11, powerTempestFury)
            .AddFeaturesAtLevel(17, featureSetUnfetteredDeluge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Tempest Swiftness
    //

    private sealed class OnAfterActionTempestSwiftness : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != FeatureDefinitionPowers.PowerMonkFlurryOfBlows)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var conditionDisengaging = ConditionDefinitions.ConditionDisengaging;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                conditionDisengaging,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    //
    // Storm Surge
    //

    private class CustomBehaviorStormSurge : IReactToAttackOnMeFinished, IAfterAttackEffect
    {
        private const string StormSurge = "StormSurge";

        private static FeatureDefinitionPower _powerStormSurge;

        public CustomBehaviorStormSurge(FeatureDefinitionPower powerStormSurge)
        {
            _powerStormSurge = powerStormSurge;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (!attackMode.AttackTags.Contains(StormSurge))
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null)
            {
                return;
            }

            var modifierTrend = rulesetDefender.actionModifier.savingThrowModifierTrends;
            var advantageTrends = rulesetDefender.actionModifier.savingThrowAdvantageTrends;
            var attackerWisdomModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Wisdom));
            var attackerProficiencyBonus =
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var defenderDexterityModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Dexterity));

            rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, defenderDexterityModifier, 8 + attackerProficiencyBonus + attackerWisdomModifier,
                false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                defender.Guid,
                ConditionDefinitions.ConditionProne,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                attacker.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }

        public IEnumerator HandleReactToAttackOnMeFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetCharacter = me.RulesetCharacter;

            if (!rulesetCharacter.CanUsePower(_powerStormSurge))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            // do I need to check this as well?
            if (!battle.IsWithinBattleRange(me, attacker))
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);
            retaliationMode.AddAttackTagAsNeeded(StormSurge);

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity)
            {
                TargetCharacters = { attacker },
                ActionModifiers = { retaliationModifier },
                AttackMode = retaliationMode,
                StringParameter2 = $"Reaction/&ReactionAttack{StormSurge}Description"
            };

            var reactionRequest = new ReactionRequestReactionAttack(StormSurge, reactionParams)
            {
                Resource = new ReactionResourcePower(_powerStormSurge, Sprites.KiPointResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            // rulesetCharacter.UsePower(UsablePowersProvider.Get(_powerStormSurge, rulesetCharacter));
            rulesetCharacter.ForceKiPointConsumption(_powerStormSurge.CostPerUse);
        }
    }

    private sealed class OnAfterActionTempestFury : IOnAfterActionFeature
    {
        private readonly FeatureDefinitionPower _powerTempestFury;
        private readonly FeatureDefinitionPower _powerTempestFuryLeap;

        public OnAfterActionTempestFury(
            FeatureDefinitionPower powerTempestFury,
            FeatureDefinitionPower powerTempestFuryLeap)
        {
            _powerTempestFury = powerTempestFury;
            _powerTempestFuryLeap = powerTempestFuryLeap;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerTempestFury)
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerTempestFuryLeap, rulesetCharacter);

            foreach (var targetLocationCharacter in Gui.Battle.AllContenders
                         .Where(x =>
                             x.Side != actingCharacter.Side &&
                             gameLocationBattleService.IsWithin1Cell(actingCharacter, x)))
            {
                var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

                effectPower.ApplyEffectOnCharacter(
                    targetLocationCharacter.RulesetCharacter, true, targetLocationCharacter.LocationPosition);
            }
        }
    }

    //
    // Unfettered Deluge
    //

    private sealed class CustomCodeUnfetteredDeluge : IFeatureDefinitionCustomCode
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Dexterity, 2);

            hero.RefreshAll();
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Dexterity, -2);
        }

        private static void ModifyAttributeAndMax([NotNull] RulesetActor hero, string attributeName, int amount)
        {
            var attribute = hero.GetAttribute(attributeName);

            attribute.BaseValue += amount;
            attribute.MaxValue += amount;
            attribute.MaxEditableValue += amount;
            attribute.Refresh();

            hero.AbilityScoreIncreased?.Invoke(hero, attributeName, amount, amount);
        }
    }
}
