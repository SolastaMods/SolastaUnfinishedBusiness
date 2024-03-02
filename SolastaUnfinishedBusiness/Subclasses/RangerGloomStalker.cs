using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
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
                BuildSpellGroup(2, SpellsContext.GoneWithTheWind),
                BuildSpellGroup(5, PassWithoutTrace),
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
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetImpactParticleReference(ArcaneSword.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var actionAffinityDreadAmbusherMainAttack = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}DreadAmbusherMain")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
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
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
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

        conditionDreadAmbusher.GuiPresentation.description = Gui.NoLocalization;

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

        featureShadowyDodge.AddCustomSubFeatures(new PhysicalAttackInitiatedOnMeShadowyDodge(featureShadowyDodge));

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
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
        ConditionDefinition conditionDreadAmbusherBonusAttack) : IInitiativeEndListener, IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
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

            yield break;
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

            if (!attacker.OnceInMyTurnIsValid("DreadAmbusher"))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd("DreadAmbusher", 1);

            var condition = action.ActionType == ActionDefinitions.ActionType.Main
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
        private static readonly List<SenseMode.Type> Senses = [SenseMode.Type.Darkvision];

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
            return Senses;
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

            attacker.UsedSpecialFeatures.TryAdd(featureStalkersFlurry.Name, 1);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.AttackFree];

            attacker.RulesetCharacter.LogCharacterUsedFeature(featureStalkersFlurry);
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    //
    // Shadowy Dodge
    //

    private sealed class PhysicalAttackInitiatedOnMeShadowyDodge(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureShadowyDodge) : IMagicalAttackInitiatedOnMe, IPhysicalAttackInitiatedOnMe
    {
        public IEnumerator OnMagicalAttackInitiatedOnMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter target,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            yield return HandleReaction(action.ActionParams.ActingCharacter, target, attackModifier);
        }

        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            yield return HandleReaction(attacker, defender, attackModifier);
        }

        private IEnumerator HandleReaction(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier)
        {
            var advantageType = ComputeAdvantage(attackModifier.AttackAdvantageTrends);

            if (advantageType == AdvantageType.Advantage ||
                !attacker.CanReact())
            {
                yield break;
            }

            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionManager == null ||
                gameLocationBattleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionShadowyDodgeDescription"
                };
            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ShadowyDodge", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleManager.WaitForReactions(defender, gameLocationActionManager,
                previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            attackModifier.AttackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.CharacterFeature, featureShadowyDodge.Name, featureShadowyDodge));
        }
    }
}
