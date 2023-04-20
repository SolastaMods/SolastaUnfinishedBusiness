using System.Collections;
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
            .SetSpecialDuration(DurationType.Dispelled)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityTempestSwiftness)
            .AddSpecialInterruptions(ConditionInterruption.Attacked)
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

        // Storm Surge

        var powerStormSurge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}StormSurge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerStormSurge.SetCustomSubFeatures(new ReactToAttackOnMeFinishedStormSurge(powerStormSurge));

        // Tempest’s Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionMonkFlurryOfBlowsUnarmedStrikeBonus,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionDisengaging,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(ValidatorsCharacter.HasAttacked)
            .AddToDB();

        powerTempestFury.SetCustomSubFeatures(new OnAfterActionTempestFury(powerTempestFury));

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
            .AddFeaturesAtLevel(3, featureSetTempestSwiftness)
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
                0,
                TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private class ReactToAttackOnMeFinishedStormSurge : IReactToAttackOnMeFinished
    {
        private static FeatureDefinitionPower _powerStormSurge;

        public ReactToAttackOnMeFinishedStormSurge(FeatureDefinitionPower powerStormSurge)
        {
            _powerStormSurge = powerStormSurge;
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

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
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

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity)
            {
                TargetCharacters = { attacker },
                ActionModifiers = { retaliationModifier },
                AttackMode = retaliationMode
            };

            var reactionRequest = new ReactionRequestReactionAttack("StormSurge", reactionParams)
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

        public OnAfterActionTempestFury(FeatureDefinitionPower powerTempestFury)
        {
            _powerTempestFury = powerTempestFury;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerTempestFury)
            {
            }

            /*
             * immediately initiates one extra unarmed attack against each enemy creature within 5 feet of you upon its activation.
             * each of these extra attacks is a separate attack roll and applies Staggered condition upon hit.
             */
        }
    }

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
