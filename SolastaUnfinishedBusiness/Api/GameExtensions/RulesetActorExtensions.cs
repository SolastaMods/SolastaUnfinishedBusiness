using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class RulesetActorExtensions
{
    #region Saving Throw Handlers

    private static void OnRollSavingThrowOath(
        RulesetCharacter caster,
        RulesetActor target,
        BaseDefinition sourceDefinition,
        string selfConditionName,
        ConditionDefinition conditionDefinitionEnemy)
    {
        if (caster == null ||
            caster.Side == target.Side ||
            !caster.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, selfConditionName))
        {
            return;
        }

        if (sourceDefinition is not SpellDefinition { castingTime: ActivationTime.Action } &&
            sourceDefinition is not FeatureDefinitionPower { RechargeRate: RechargeRate.ChannelDivinity } &&
            !caster.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Any(x => x.Name.Contains("Smite")))
        {
            return;
        }

        var gameLocationCaster = GameLocationCharacter.GetFromActor(caster);
        var gameLocationTarget = GameLocationCharacter.GetFromActor(target);

        if (gameLocationCaster == null ||
            gameLocationTarget == null ||
            !gameLocationCaster.IsWithinRange(gameLocationTarget, 2))
        {
            return;
        }

        target.InflictCondition(
            conditionDefinitionEnemy.Name,
            DurationType.Round,
            0,
            TurnOccurenceType.StartOfTurn,
            AttributeDefinitions.TagEffect,
            caster.guid,
            caster.CurrentFaction.Name,
            1,
            conditionDefinitionEnemy.Name,
            0,
            0,
            0);
    }

    // keep a tab on last SaveDC / SaveBonusAndRollModifier / SavingThrowAbility
    internal static int SaveDC { get; private set; }
    internal static int SaveBonusAndRollModifier { get; private set; }
    internal static string SavingThrowAbility { get; private set; }

    internal static void MyRollSavingThrow(
        this RulesetActor rulesetActorTarget,
        RulesetCharacter rulesetActorCaster,
        int saveBonus,
        string abilityScoreName,
        BaseDefinition sourceDefinition,
        List<TrendInfo> modifierTrends,
        List<TrendInfo> advantageTrends,
        int rollModifier,
        int saveDC,
        bool hasHitVisual,
        ref RollOutcome outcome,
        ref int outcomeDelta,
        List<EffectForm> effectForms)
    {
        //PATCH: supports Oath of Ancients / Oath of Dread
        OnRollSavingThrowOath(rulesetActorCaster, rulesetActorTarget, sourceDefinition,
            OathOfAncients.ConditionElderChampionName,
            OathOfAncients.ConditionElderChampionEnemy);
        OnRollSavingThrowOath(rulesetActorCaster, rulesetActorTarget, sourceDefinition,
            OathOfDread.ConditionAspectOfDreadName,
            OathOfDread.ConditionAspectOfDreadEnemy);

        var rulesetCharacterTarget = rulesetActorTarget as RulesetCharacter;

        //BEGIN PATCH
        if (rulesetCharacterTarget != null)
        {
            //PATCH: supports Path of The Savagery
            PathOfTheSavagery.OnRollSavingThrowFuriousDefense(rulesetCharacterTarget, ref abilityScoreName);

            //PATCH: supports `OnSavingThrowInitiated` interface
            foreach (var rollSavingThrowInitiated in rulesetCharacterTarget
                         .GetSubFeaturesByType<IRollSavingThrowInitiated>())
            {
                rollSavingThrowInitiated.OnSavingThrowInitiated(
                    rulesetActorCaster,
                    rulesetActorTarget,
                    ref saveBonus,
                    ref abilityScoreName,
                    sourceDefinition,
                    modifierTrends,
                    advantageTrends,
                    ref rollModifier,
                    ref saveDC,
                    ref hasHitVisual,
                    outcome,
                    outcomeDelta,
                    effectForms);
            }
        }
        //END PATCH

        // keep a tab on last SaveDC / SaveBonusAndRollModifier / SavingThrowAbility
        SaveDC = saveDC;
        SaveBonusAndRollModifier = saveBonus + rollModifier;
        SavingThrowAbility = abilityScoreName;

        var saveRoll = rulesetActorTarget.RollDie(
            DieType.D20, RollContext.SavingThrow, false, ComputeAdvantage(advantageTrends),
            out var firstRoll, out var secondRoll);

        var totalRoll = saveRoll + saveBonus + rollModifier;
        outcomeDelta = totalRoll - saveDC;
        outcome = totalRoll < saveDC ? RollOutcome.Failure : RollOutcome.Success;

        foreach (var modifierTrend in modifierTrends)
        {
            if (modifierTrend.dieFlag == TrendInfoDieFlag.None ||
                modifierTrend is not { value: > 0, dieType: > DieType.D1 })
            {
                continue;
            }

            var additionalSaveDieRolled = rulesetActorTarget.AdditionalSaveDieRolled;

            additionalSaveDieRolled?.Invoke(rulesetActorTarget, modifierTrend);
        }

        rulesetActorTarget.SaveRolled?.Invoke(rulesetActorTarget, abilityScoreName, sourceDefinition, outcome, saveDC,
            totalRoll,
            saveRoll, firstRoll, secondRoll, saveBonus + rollModifier, modifierTrends, advantageTrends, hasHitVisual);

        rulesetActorTarget.ProcessConditionsMatchingInterruption(ConditionInterruption.SavingThrow);

        //BEGIN PATCH
        if (rulesetCharacterTarget == null)
        {
            return;
        }

        //PATCH: supports `IRollSavingThrowFinished` interface
        foreach (var rollSavingThrowFinished in rulesetCharacterTarget.GetSubFeaturesByType<IRollSavingThrowFinished>())
        {
            rollSavingThrowFinished.OnSavingThrowFinished(
                rulesetActorCaster,
                rulesetActorTarget,
                saveBonus,
                abilityScoreName,
                sourceDefinition,
                modifierTrends,
                advantageTrends,
                rollModifier,
                saveDC,
                hasHitVisual,
                ref outcome,
                ref outcomeDelta,
                effectForms);
        }
        //END PATCH
    }

    #endregion

    internal static void ModifyAttributeAndMax(this RulesetActor hero, string attributeName, int amount)
    {
        var attribute = hero.GetAttribute(attributeName);

        attribute.BaseValue += amount;
        attribute.MaxValue += amount;
        attribute.MaxEditableValue += amount;
        attribute.Refresh();

        hero.AbilityScoreIncreased?.Invoke(hero, attributeName, amount, amount);
    }

    [NotNull]
    private static List<T> FeaturesByType<T>([CanBeNull] RulesetActor actor) where T : class
    {
        var list = new List<FeatureDefinition>();

        actor?.EnumerateFeaturesToBrowse<T>(list);

        // mainly because of Feature Sets granted as invocations (tabletop 2024)
        list.AddRange(list.OfType<FeatureDefinitionFeatureSet>().SelectMany(x => x.FeatureSet).ToArray());

        return list
            .OfType<T>()
            .ToList();
    }

    [NotNull]
    internal static List<T> GetFeaturesByType<T>(this RulesetActor actor) where T : class
    {
        return FeaturesByType<T>(actor);
    }

    [NotNull]
    // ReSharper disable once ReturnTypeCanBeEnumerable.Local
    private static List<BaseDefinition> AllActiveDefinitions([CanBeNull] RulesetActor actor)
    {
        var list = FeaturesByType<BaseDefinition>(actor);

        RulesetCharacterHero hero = null;

        switch (actor)
        {
            case RulesetCharacterHero rulesetCharacterHero:
                hero = rulesetCharacterHero;
                break;
            //WILDSHAPE: Original hero features
            case RulesetCharacterMonster { originalFormCharacter: RulesetCharacterHero rulesetCharacterHero }:
                hero = rulesetCharacterHero;
                list.AddRange(FeaturesByType<BaseDefinition>(hero)
                    .Where(f => !list.Contains(f)));
                break;
        }

        if (hero == null)
        {
            return list;
        }

        // metamagic are handled in other locations
        list.AddRange(hero.trainedFeats);
        list.AddRange(hero.trainedInvocations);
        list.AddRange(hero.trainedFightingStyles);

        return list;
    }

#if false
    internal static List<T> GetFeaturesByTypeAndTag<T>(this RulesetCharacterHero hero, string tag) where T : class
    {
        return hero.ActiveFeatures
            .Where(e => e.Key.Contains(tag))
            .SelectMany(e => e.Value)
            .SelectMany(Unfold)
            .OfType<T>()
            .ToList();
    }

    private static IEnumerable<FeatureDefinition> Unfold(FeatureDefinition feature)
    {
        return feature is FeatureDefinitionFeatureSet {Mode: FeatureSetMode.Union} set
            ? set.FeatureSet.SelectMany(Unfold)
            : new[] {feature};
    }
#endif

    internal static bool HasAnyFeature(this RulesetActor actor, params FeatureDefinition[] features)
    {
        return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
    }

#if false
    internal static bool HasAllFeatures(this RulesetActor actor, [NotNull] params FeatureDefinition[] features)
    {
        var all = FeaturesByType<FeatureDefinition>(actor);
        return FlattenFeatureList(features).All(f => all.Contains(f));
    }
#endif

    [NotNull]
    public static IEnumerable<FeatureDefinition> FlattenFeatureList([NotNull] IEnumerable<FeatureDefinition> features)
    {
        return features.SelectMany(f =>
            f is FeatureDefinitionFeatureSet set
                ? FlattenFeatureList(set.FeatureSet)
                : [f]);
    }

    [NotNull]
    internal static List<T> GetSubFeaturesByType<T>([CanBeNull] this RulesetActor actor, params Type[] typesToSkip)
        where T : class
    {
        var list = AllActiveDefinitions(actor)
            .Where(f => !typesToSkip.Contains(f.GetType()))
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .ToList();

        if (actor != null)
        {
            list.AddRange(actor.ConditionsByCategory
                .SelectMany(x => x.Value)
                .SelectMany(x => x.ConditionDefinition.GetAllSubFeaturesOfType<T>()));
        }

        return list;
    }

    internal static bool HasSubFeatureOfType<T>([CanBeNull] this RulesetActor actor, params Type[] typesToSkip)
        where T : class
    {
        if (AllActiveDefinitions(actor)
                .Where(f => !typesToSkip.Contains(f.GetType()))
                .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
                .FirstOrDefault() != null)
        {
            return true;
        }

        return actor?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .SelectMany(x => x.ConditionDefinition.GetAllSubFeaturesOfType<T>())
            .FirstOrDefault() != null;
    }

    internal static bool IsTouchingGround(this RulesetActor actor)
    {
        return !actor.HasConditionOfType(ConditionFlying) &&
               !actor.HasConditionOfType(ConditionLevitate) &&
               !(actor is RulesetCharacter character && character.MoveModes.ContainsKey((int)MoveMode.Fly));
    }

    internal static bool IsTemporarilyFlying(this RulesetActor actor)
    {
        return actor is RulesetCharacter character
               && character.HasTemporaryConditionOfType(ConditionFlying)
               && !character.HasConditionOfType(ConditionLevitate);
        /*
         * For future use, when can allow flying wildshape to temporarily walk
         *
        || (actor.HasConditionOfType(RuleDefinitions.ConditionWildShapeSubstituteForm)
                && actor is RulesetCharacterMonster monster
                && monster.MoveModes.ContainsKey((int)RuleDefinitions.MoveMode.Fly)
                && !actor.HasConditionOfType("ConditionFlightSuspended")

        );*/
    }

    internal static bool HasAnyConditionOfType(this RulesetActor actor, params string[] conditions)
    {
        return actor is RulesetCharacter && conditions.Any(actor.HasConditionOfType);
    }

    internal static bool HasAnyConditionOfTypeOrSubType(this RulesetActor actor, params string[] conditions)
    {
        return actor is RulesetCharacter && conditions.Any(actor.HasConditionOfTypeOrSubType);
    }


    internal static void RemoveAllConditionsOfType(this RulesetActor actor, params string[] conditions)
    {
        var conditionsToRemove = actor.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Where(x => conditions.Contains(x.ConditionDefinition.Name))
            .ToArray();

        foreach (var condition in conditionsToRemove)
        {
            actor.RemoveCondition(condition, false);
        }

        actor.RefreshAll();
    }
}
