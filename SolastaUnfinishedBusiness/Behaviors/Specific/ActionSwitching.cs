using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

public static class ActionSwitching
{
    internal static readonly TutorialStepDefinition Tutorial = TutorialStepDefinitionBuilder
        .Create("TutorialActionSwitching")
        .SetGuiPresentation(Category.Tutorial, Sprites.TutorialActionSwitching)
        .AddToDB();

    internal static void LateLoad()
    {
        //TA's implementation of Rage Start spends Bonus Action twice
        //not a big problem in vanilla, but breaks action switching code
        //use our custom rage start class that doesn't have this issue
        DatabaseHelper.ActionDefinitions.RageStart.classNameOverride = "CombatRageStart";

        DatabaseHelper.ActionDefinitions.GrantBardicInspiration.classNameOverride = "UsePower";

        //Mark Action Surge to track spell flags separately
        FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain
            .AddCustomSubFeatures(ActionWithCustomSpellTracking.Mark);

        //Make Horde Breaker add condition instead of using trigger
        var hordeBreaker = FeatureDefinitionAdditionalActions.AdditionalActionHunterHordeBreaker;

        hordeBreaker.AddCustomSubFeatures(
            new OnReducedToZeroHpByMeHordeBreaker(
                ConditionDefinitionBuilder
                    .Create("ConditionHunterHordeBreaker")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAdditionalActionBuilder
                            .Create("AdditionalActionHunterHordeBreaker2")
                            .SetGuiPresentation(hordeBreaker.GuiPresentation)
                            .SetActionType(ActionDefinitions.ActionType.Main)
                            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                            .SetMaxAttacksNumber(1)
                            .AddToDB())
                    .AddToDB()));
    }

    private static void EnumerateFeaturesHierarchically<T>(
        ICollection<(FeatureDefinition feature, string origin)> features,
        IEnumerable<FeatureDefinition> parentList, string origin)
    {
        // extra null checks required as some custom campaigns NPCs might produce a null here
        foreach (var feature in parentList
                     .Where(feature =>
                         (feature && feature.AllowsDuplicate) ||
                         (features != null && features.All(x => x.feature != feature))))
        {
            switch (feature)
            {
                case FeatureDefinitionFeatureSet { Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union } set:
                    EnumerateFeaturesHierarchically<T>(features, set.FeatureSet, origin);
                    break;
                case T:
                    features.Add((feature, origin));
                    break;
            }
        }
    }

    private static List<FeatureDefinition> GetConditionFeatures<T>(
        ConditionDefinition condition,
        List<FeatureDefinition> list = null)
    {
        list ??= [];

        if (condition.parentCondition)
        {
            GetConditionFeatures<T>(condition.parentCondition, list);
        }

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var f in condition.features)
        {
            if (f is T)
            {
                list.Add(f);
            }
        }

        return list;
    }

    internal static List<(FeatureDefinition feature, string origin)> EnumerateActorFeatures<T>(RulesetActor actor)
    {
        return actor switch
        {
            RulesetCharacterHero hero => EnumerateHeroFeatures<T>(hero),
            RulesetCharacterMonster monster => EnumerateMonsterFeatures<T>(monster),
            _ => null
        };
    }

    private static List<(FeatureDefinition feature, string origin)> EnumerateHeroFeatures<T>(
        RulesetCharacterHero hero,
        bool skipConditions = false)
    {
        List<(FeatureDefinition feature, string origin)> features = [];

        //Character features
        foreach (var activeFeature in hero.activeFeatures)
        {
            EnumerateFeaturesHierarchically<T>(features, activeFeature.Value, activeFeature.Key);
        }

        //Equipment
        foreach (var activeItemFeature in hero.activeItemFeatures)
        {
            EnumerateFeaturesHierarchically<T>(features, activeItemFeature.Value, activeItemFeature.Key.Name);
        }

        //Fighting Styles
        foreach (var activeFightingStyle in hero.activeFightingStyles)
        {
            EnumerateFeaturesHierarchically<T>(features, activeFightingStyle.Features, activeFightingStyle.Name);
        }

        //Feats
        foreach (var trainedFeat in hero.trainedFeats)
        {
            EnumerateFeaturesHierarchically<T>(features, trainedFeat.Features, trainedFeat.Name);
        }

        //Conditions
        if (!skipConditions)
        {
            hero.GetAllConditions(hero.AllConditionsForEnumeration);
            hero.AllConditionsForEnumeration.Sort((a, b) => a.Guid.CompareTo(b.Guid));

            foreach (var rulesetCondition in hero.AllConditionsForEnumeration)
            {
                EnumerateFeaturesHierarchically<T>(features,
                    GetConditionFeatures<T>(rulesetCondition.ConditionDefinition),
                    $"{rulesetCondition.Name}:{rulesetCondition.Guid}");
            }
        }

        //Metamagic
        features.AddRange(hero.MetamagicFeatures.Where(f => f.Value is T).Select(x => (x.Value, x.Key.Name)));

        foreach (var invocation in hero.Invocations)
        {
            if (!invocation.Active)
            {
                continue;
            }

            var grantedFeature = invocation.InvocationDefinition.GrantedFeature;

            if (grantedFeature && grantedFeature is T)
            {
                features.Add((grantedFeature, invocation.InvocationDefinition.Name));
            }
            else if (grantedFeature &&
                     grantedFeature is FeatureDefinitionFeatureSet set)
            {
                EnumerateFeaturesHierarchically<T>(features, set.FeatureSet, invocation.InvocationDefinition.Name);
            }
        }

        return features;
    }

    private static List<(FeatureDefinition feature, string origin)> EnumerateMonsterFeatures<T>(
        RulesetCharacterMonster monster)
    {
        List<(FeatureDefinition feature, string origin)> features = [];

        //Monster features
        EnumerateFeaturesHierarchically<T>(features, monster.activeFeatures, monster.monsterDefinition.Name);

        //WILDSHAPE: Original hero features
        if (monster.originalFormCharacter is RulesetCharacterHero hero)
        {
            features.AddRange(EnumerateHeroFeatures<T>(hero, true));
        }

        //Conditions
        monster.GetAllConditions(monster.AllConditionsForEnumeration);
        monster.AllConditionsForEnumeration.Sort((a, b) => a.Guid.CompareTo(b.Guid));

        foreach (var rulesetCondition in monster.AllConditionsForEnumeration)
        {
            EnumerateFeaturesHierarchically<T>(features, GetConditionFeatures<T>(rulesetCondition.ConditionDefinition),
                $"{rulesetCondition.Name}:{rulesetCondition.Guid}");
        }

        return features;
    }

    internal static void PrioritizeAction(GameLocationCharacter character, ActionDefinitions.ActionType type, int index)
    {
        var actionParams = new CharacterActionParams(character, (ActionDefinitions.Id)ExtraActionId.PrioritizeAction)
        {
            IntParameter = (int)type, IntParameter2 = index
        };

        ServiceRepository.GetService<IGameLocationActionService>().ExecuteAction(actionParams, null, true);
    }

    internal static void DoPrioritizeAction(
        GameLocationCharacter character,
        ActionDefinitions.ActionType type,
        int index)
    {
        var rank = character.CurrentActionRankByType[type];

        if (index <= rank)
        {
            return;
        }

        var map = character.UsedSpecialFeatures;
        var filters = character.ActionPerformancesByType[type];
        var list = LoadIndexes(map, type, filters.Count);
        var k = list[index];

        //Store current action attacks
        var data = PerformanceFilterExtraData.GetData(filters[rank]);

        data?.StoreAttacks(character, type);
        data?.StoreSpellcasting(character, type);

        list.RemoveAt(index);
        list.Insert(rank, k);

        SaveIndexes(map, type, list);

        character.RefreshActionPerformances();

        data = PerformanceFilterExtraData.GetData(filters[rank]);
        data?.LoadAttacks(character, type);
        data?.LoadSpellcasting(character, type, true);

        character.RulesetCharacter?.RefreshAttackModes();
    }

    internal static void CheckIfActionSwitched(
        GameLocationCharacter character,
        CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope,
        int mainRank, int mainAttacks, int bonusRank, int bonusAttacks)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }

        if (character.Side != RuleDefinitions.Side.Ally)
        {
            return;
        }

        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        var type = actionParams.ActionDefinition.ActionType;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (type == ActionDefinitions.ActionType.Main)
        {
            CheckIfActionSwitched(character, type, mainRank, mainAttacks);
        }
        else if (type == ActionDefinitions.ActionType.Bonus)
        {
            CheckIfActionSwitched(character, type, bonusRank, bonusAttacks);
        }
    }

    private static void CheckIfActionSwitched(
        GameLocationCharacter character,
        ActionDefinitions.ActionType type,
        int wasRank, int wasAttacks)
    {
        var rank = character.CurrentActionRankByType[type];
        var filters = character.ActionPerformancesByType[type];
        var newData = rank < filters.Count ? PerformanceFilterExtraData.GetData(filters[rank]) : null;

        if (rank == wasRank)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"CheckIfActionSwitched [{character.Name}] {type} rank: {rank} - NO CHANGE");
            newData?.StoreAttacks(character, type);
            newData?.StoreSpellcasting(character, type);

            return;
        }

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"CheckIfActionSwitched [{character.Name}] {type} was: {wasRank} new: {rank}");

        if (wasRank >= filters.Count)
        {
            return;
        }

        var wasData = PerformanceFilterExtraData.GetData(filters[wasRank]);

        //TODO: has potential to spend extra attack for sorcerers who initiate Flexible Casting (slot-sorcerer point conversion) and then cancel it.
        wasData?.StoreAttacks(character, type, wasAttacks + 1);
        wasData?.StoreSpellcasting(character, type);
        newData?.LoadAttacks(character, type);
        newData?.LoadSpellcasting(character, type, true);
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static List<int> LoadIndexes(
        Dictionary<string, int> map, ActionDefinitions.ActionType type, int max)
    {
        var list = new List<int>();

        for (var i = 0; i < max; i++)
        {
            list.Add(map.TryGetValue($"ActionIndex{type}:{i}", out var k) ? k : i);
        }

        return list;
    }

    private static void SaveIndexes(
        IDictionary<string, int> map,
        ActionDefinitions.ActionType type,
        List<int> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            map.AddOrReplace($"ActionIndex{type}:{i}", list[i]);
        }
    }

    public static void ResortPerformances(GameLocationCharacter character)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }

        if (character.IsOppositeSide(RuleDefinitions.Side.Ally))
        {
            return;
        }

        if (character.RulesetCharacter is not (RulesetCharacterHero or RulesetCharacterMonster))
        {
            return;
        }

        var features = EnumerateActorFeatures<IAdditionalActionsProvider>(character.RulesetCharacter);

        if (features == null)
        {
            return;
        }

        var onKill = features.FindAll(x => x.feature is IAdditionalActionsProvider
        {
            TriggerCondition: RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy
        });

        if (onKill.Count != 0)
        {
            features.RemoveAll(x => onKill.Contains(x));
            features.AddRange(onKill);
        }

        features.RemoveAll(x =>
        {
            var validator = x.feature.GetFirstSubFeatureOfType<IValidateDefinitionApplication>();

            return validator != null && !validator.IsValid(x.feature, character.RulesetCharacter);
        });

        //remove triggered features - all such features are reworked to grant conditions
        features.RemoveAll(x =>
            x.feature is IAdditionalActionsProvider
            {
                TriggerCondition: RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy
            });

        ResortPerformancesOfType(character, features, ActionDefinitions.ActionType.Main);
        ResortPerformancesOfType(character, features, ActionDefinitions.ActionType.Bonus);

        character.dirtyActions = true;
        character.ActionsRefreshed?.Invoke(character);
    }

    private static void ResortPerformancesOfType(GameLocationCharacter character,
        IEnumerable<(FeatureDefinition feature, string origin)> allFeatures, ActionDefinitions.ActionType type)
    {
        var features = allFeatures
            .Where(x => x.feature is IAdditionalActionsProvider f && f.ActionType == type)
            .ToArray();
        var filters = character.ActionPerformancesByType[type];
        var filtersCount = filters.Count;

        // ReSharper disable once InvocationIsSkipped
        Main.Log(
            $"ResortPerformancesOfType [{character.Name}] {type} filters: {filtersCount} features: [{string.Join(", ", features.Select(x => $"<{x.feature.Name}|{x.origin}>"))}]");

        for (var i = 0; i < filtersCount; i++)
        {
            if (i == 0)
            {
                PerformanceFilterExtraData.AddData(filters[i], null, null);
            }
            else
            {
                var (feature, origin) = features[i - 1];

                PerformanceFilterExtraData.AddData(filters[i], feature, origin);
            }
        }

        var rank = character.CurrentActionRankByType[type];
        var list = LoadIndexes(character.UsedSpecialFeatures, type, filtersCount);

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"ResortPerformancesOfType [{character.Name}] {type} : [{string.Join(", ", list)}] rank: {rank}");

        var sorted = list.Select(k => filters[k]).ToArray();

        if (rank < sorted.Length)
        {
            var data = PerformanceFilterExtraData.GetData(sorted[rank]);

            data?.LoadSpellcasting(character, type, false);
        }

        filters.SetRange(sorted);
    }

    public static void AccountRemovedCondition(RulesetCharacter character, RulesetCondition condition)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }

        var locCharacter = GameLocationCharacter.GetFromActor(character);

        if (locCharacter == null)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO LOC CHAR");
            return;
        }

        var conditionFeatures = new List<FeatureDefinition>();

        condition.ConditionDefinition.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(conditionFeatures);

        if (conditionFeatures.Count == 0)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO ACTIONS");
            return;
        }

        var features = EnumerateActorFeatures<IAdditionalActionsProvider>(character);

        if (features == null)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO FEATURES");
            return;
        }

        var origin = $"{condition.Name}:{condition.Guid}";

        foreach (var conditionFeature in conditionFeatures)
        {
            if (conditionFeature is not IAdditionalActionsProvider provider)
            {
                //Shouldn't happen, in place so Rider won't complain
                continue;
            }

            var type = provider.ActionType;
            var filters = locCharacter.ActionPerformancesByType[type];
            var max = filters.Count;
            var list = LoadIndexes(locCharacter.UsedSpecialFeatures, type, max);

            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] '{conditionFeature.Name}' from '{origin}'");
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] WAS: [{string.Join(", ", list)}]");

            var k = -1;

            for (var i = 0; i < max; i++)
            {
                var filter = filters[i];
                var data = PerformanceFilterExtraData.GetData(filter);

                if (data == null) { continue; }

                if (data.Feature != conditionFeature || data.Origin != origin)
                {
                    continue;
                }

                k = i;
                break;
            }

            if (k < 0) { continue; }

            var rank = locCharacter.CurrentActionRankByType[type];

            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] remove at: {k} rank was: {rank}");

            if (rank > k)
            {
                locCharacter.CurrentActionRankByType[type]--;
            }

            for (var i = max - 1; i >= 0; i--)
            {
                if (list[i] == k)
                {
                    list.RemoveAt(i);
                }
                else if (list[i] > k)
                {
                    list[i]--;
                }
            }

            // ReSharper disable once InvocationIsSkipped
            Main.Log($"AccountRemovedCondition [{character.Name}] BECAME: [{string.Join(", ", list)}]");

            SaveIndexes(locCharacter.UsedSpecialFeatures, type, list);
        }
    }

    public static void SpendActionType(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }

        if (type is not (ActionDefinitions.ActionType.Main or ActionDefinitions.ActionType.Bonus))
        {
            return;
        }

        var filters = character.actionPerformancesByType[type];
        var rank = character.currentActionRankByType[type];

        if (rank >= filters.Count)
        {
            return;
        }

        var data = PerformanceFilterExtraData.GetData(filters[rank]);

        if (data == null)
        {
            return;
        }

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"SpendActionType [{character.Name}] {type} rank: {rank}, filters: {filters.Count}");

        data.StoreAttacks(character, type);
        data.StoreSpellcasting(character, type);

        if (rank + 1 >= filters.Count)
        {
            return;
        }

        data = PerformanceFilterExtraData.GetData(filters[rank + 1]);

        if (data == null)
        {
            return;
        }

        data.LoadAttacks(character, type);
        data.LoadSpellcasting(character, type, true);
    }

    public static void RefundActionUse(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }

        if (type is not (ActionDefinitions.ActionType.Main or ActionDefinitions.ActionType.Bonus))
        {
            return;
        }

        var rank = character.currentActionRankByType[type];

        if (rank <= 0)
        {
            return;
        }

        var filters = character.actionPerformancesByType[type];

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"RefundActionUse [{character.Name}] {type} rank: {rank}, filters: {filters.Count}");

        //PATCH: fixed action switching interaction with actions that offer further selections in a modal and get cancelled by player
        if (filters.Count <= rank)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"RefundActionUse ABORTED [{character.Name}] {type} rank: {rank}, filters: {filters.Count}");

            return;
        }

        var data = PerformanceFilterExtraData.GetData(filters[rank]);

        if (data != null)
        {
            data.StoreAttacks(character, type);
            data.StoreSpellcasting(character, type);
        }

        data = PerformanceFilterExtraData.GetData(filters[rank - 1]);

        if (data == null)
        {
            return;
        }

        data.LoadAttacks(character, type);
        data.LoadSpellcasting(character, type, true);
    }

    internal static void CheckSpellcastingAvailability(
        GameLocationCharacter character,
        ActionDefinitions.Id actionId,
        ActionDefinitions.ActionScope scope,
        ref ActionDefinitions.ActionStatus result)
    {
        if (scope != ActionDefinitions.ActionScope.Battle || result != ActionDefinitions.ActionStatus.Available)
        {
            return;
        }

        ActionDefinitions.ActionType type;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (actionId == ActionDefinitions.Id.CastMain)
        {
            type = ActionDefinitions.ActionType.Main;
        }
        else if (actionId == ActionDefinitions.Id.CastBonus)
        {
            type = ActionDefinitions.ActionType.Bonus;
        }
        else
        {
            return;
        }

        if (!character.actionPerformancesByType.TryGetValue(type, out var filters))
        {
            return;
        }

        var onlyCantrips = filters
            .Select(PerformanceFilterExtraData.GetData)
            .Any(filter => filter != null && filter.CantripsOnly(character, type));

        var rulesetCharacter = character.RulesetCharacter;

        if (!rulesetCharacter.CanCastSpellOfActionType(type, onlyCantrips))
        {
            result = ActionDefinitions.ActionStatus.Unavailable;
        }
    }

    public static void CheckSpellcastingCantrips(
        GameLocationCharacter character,
        ActionDefinitions.ActionType actionType,
        ref bool cantripsOnly)
    {
        if (Gui.Battle == null ||
            actionType is not (ActionDefinitions.ActionType.Main or ActionDefinitions.ActionType.Bonus) ||
            !character.actionPerformancesByType.TryGetValue(actionType, out var filters))
        {
            return;
        }

        cantripsOnly = cantripsOnly || filters.Select(PerformanceFilterExtraData.GetData)
            .Any(filter => filter != null && filter.CantripsOnly(character, actionType));
    }

    private sealed class OnReducedToZeroHpByMeHordeBreaker(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!Main.Settings.EnableActionSwitching)
            {
                yield break;
            }

            if (attacker.RulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, condition.Name))
            {
                yield break;
            }

            if (!attacker.IsMyTurn())
            {
                yield break;
            }

            attacker.RulesetCharacter.InflictCondition(
                condition.Name,
                RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                attacker.RulesetCharacter.guid,
                attacker.RulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }
}
