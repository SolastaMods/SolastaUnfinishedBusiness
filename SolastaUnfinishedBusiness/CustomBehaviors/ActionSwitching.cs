using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public static class ActionSwitching
{
    internal static void Load()
    {
    }

    private static void EnumerateFeaturesHierarchicaly<T>(List<(FeatureDefinition feature, string origin)> features,
        List<FeatureDefinition> parentList, string origin)
    {
        foreach (var feature in parentList)
        {
            if (!feature.AllowsDuplicate && features.Any(x => x.feature == feature))
            {
                continue;
            }

            if (feature is FeatureDefinitionFeatureSet {Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union} set)
            {
                EnumerateFeaturesHierarchicaly<T>(features, set.FeatureSet, origin);
            }
            else if (feature is T)
            {
                features.Add((feature, origin));
            }
        }
    }

    private static List<FeatureDefinition> GetConditionFeatures<T>(ConditionDefinition condition,
        List<FeatureDefinition> list = null)
    {
        list ??= new List<FeatureDefinition>();

        if (condition.parentCondition != null)
        {
            GetConditionFeatures<T>(condition.parentCondition, list);
        }

        foreach (var f in condition.features)
        {
            if (f is T)
            {
                list.Add(f);
            }
        }

        return list;
    }

    private static List<(FeatureDefinition feature, string origin)> EnumerateHeroFeatures<T>(RulesetCharacterHero hero)
    {
        List<(FeatureDefinition feature, string origin)> features = new();

        //Character features
        foreach (var activeFeature in hero.activeFeatures)
        {
            EnumerateFeaturesHierarchicaly<T>(features, activeFeature.Value, activeFeature.Key);
        }

        //Equipment
        foreach (var activeItemFeature in hero.activeItemFeatures)
        {
            EnumerateFeaturesHierarchicaly<T>(features, activeItemFeature.Value, activeItemFeature.Key.Name);
        }

        //Fighting Styles
        foreach (var activeFightingStyle in hero.activeFightingStyles)
        {
            EnumerateFeaturesHierarchicaly<T>(features, activeFightingStyle.Features, activeFightingStyle.Name);
        }

        //Feats
        foreach (var trainedFeat in hero.trainedFeats)
        {
            EnumerateFeaturesHierarchicaly<T>(features, trainedFeat.Features, trainedFeat.Name);
        }

        //Conditions
        hero.GetAllConditions(hero.AllConditionsForEnumeration);
        foreach (var rulesetCondition in hero.AllConditionsForEnumeration)
        {
            EnumerateFeaturesHierarchicaly<T>(features, GetConditionFeatures<T>(rulesetCondition.ConditionDefinition),
                $"{rulesetCondition.Name}:{rulesetCondition.Guid}");
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
            if (grantedFeature != null && grantedFeature is T)
            {
                features.Add((grantedFeature, invocation.InvocationDefinition.Name));
            }
            else if (grantedFeature != null &&
                     grantedFeature is FeatureDefinitionFeatureSet set)
            {
                EnumerateFeaturesHierarchicaly<T>(features, set.FeatureSet, invocation.InvocationDefinition.Name);
            }
        }

        return features;
    }

    internal static void PrirotizeAction(GameLocationCharacter character, ActionDefinitions.ActionType type, int index)
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

        list.RemoveAt(index);
        list.Insert(rank, k);

        SaveIndexes(map, type, list);

        character.RefreshActionPerformances();

        //Load new action attacks, do not reuse `filters` list, because it is changed after refresh
        data = PerformanceFilterExtraData.GetData(character.ActionPerformancesByType[type][rank]);
        data?.LoadAttacks(character, type);
    }

    internal static void CheckIfActionSwitched(GameLocationCharacter __instance, 
        CharacterActionParams actionParams, 
        ActionDefinitions.ActionScope scope,
        int mainRank, int mainAttacks, int bonusRank, int bonusAttacks)
    {
        if (!Main.Settings.EnableActionSwitching)
        {
            return;
        }
            
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        var type = actionParams.ActionDefinition.ActionType;
        if (type == ActionDefinitions.ActionType.Main)
        {
            CheckIfActionSwitched(__instance, type, mainRank, mainAttacks);
        }
        else if(type == ActionDefinitions.ActionType.Bonus)
        {
            CheckIfActionSwitched(__instance, type, bonusRank, bonusAttacks);
        }
    }
    
    private static void CheckIfActionSwitched(GameLocationCharacter character, ActionDefinitions.ActionType type,
        int wasRank, int wasAttacks)
    {
        if (character.Side != RuleDefinitions.Side.Ally)
        {
            return;
        }

        var rank = character.CurrentActionRankByType[type];
        var newData = PerformanceFilterExtraData.GetData(character.ActionPerformancesByType[type][rank]);
        if (rank == wasRank)
        {
            newData?.StoreAttacks(character, type);
            return;
        }

        var wasData = PerformanceFilterExtraData.GetData(character.ActionPerformancesByType[type][wasRank]);

        wasData?.StoreAttacks(character, type, wasAttacks + 1);
        newData?.LoadAttacks(character, type);
    }

    private static List<int> LoadIndexes(Dictionary<string, int> map, ActionDefinitions.ActionType type, int max)
    {
        var list = new List<int>();
        for (var i = 0; i < max; i++)
        {
            list.Add(map.TryGetValue($"ActionIndex{type}:{i}", out var k) ? k : i);
        }

        return list;
    }

    private static void SaveIndexes(Dictionary<string, int> map, ActionDefinitions.ActionType type, List<int> list)
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

        if (character.Side != RuleDefinitions.Side.Ally)
        {
            return;
        }

        if (character.RulesetCharacter is not (RulesetCharacterHero or RulesetCharacterMonster))
        {
            return;
        }

        List<(FeatureDefinition feature, string origin)> features = null;
        if (character.RulesetCharacter is RulesetCharacterHero hero)
        {
            features = EnumerateHeroFeatures<IAdditionalActionsProvider>(hero);
        }

        if (features == null)
        {
            return;
        }

        var onKill = features.FindAll(x => x.feature is IAdditionalActionsProvider
        {
            TriggerCondition: RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy
        });

        if (!onKill.Empty())
        {
            features.RemoveAll(x => onKill.Contains(x));
            features.AddRange(onKill);
        }

        features.RemoveAll(x =>
        {
            var validator = x.feature.GetFirstSubFeatureOfType<IDefinitionApplicationValidator>();

            return validator != null && !validator.IsValid(x.feature, character.RulesetCharacter);
        });

        //remove non-triggered features to have only one place that requires those checks
        if (character.enemiesDownedByAttack <= 0)
        {
            features.RemoveAll(x =>
                (x.feature as IAdditionalActionsProvider)?.TriggerCondition ==
                RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy);
        }

        Main.Log2(
            $"ACTIONS: [{character.Name}] {string.Join(", ", features.Select(x => $"<{x.feature.Name}:{x.origin}>"))}",
            true);

        var type = ActionDefinitions.ActionType.Main;

        var filtered = features.Where(x => x.feature is IAdditionalActionsProvider f && f.ActionType == type).ToList();
        var filters = character.ActionPerformancesByType[type];
        Main.Log2($"FILTERS: {filters.Count} datas: {filtered.Count}", true);
        for (var i = 0; i < filters.Count; i++)
        {
            if (i == 0)
            {
                PerformanceFilterExtraData.AddData(filters[i], null, null);
            }
            else
            {
                var x = filtered[i - 1];
                PerformanceFilterExtraData.AddData(filters[i], x.feature, x.origin);
            }
        }

        var max = filters.Count;
        var sorted = new List<ActionPerformanceFilter>();
        var list = LoadIndexes(character.UsedSpecialFeatures, type, max);

        Main.Log2($"ResortPerformances [{character.Name}] : [{string.Join(", ", list)}]", true);

        foreach (var k in list)
        {
            sorted.Add(filters[k]);
        }

        character.ActionPerformancesByType[type] = sorted;
        
        character.dirtyActions = true;
        character.ActionsRefreshed?.Invoke(character);
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
            Main.Log2($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO LOC CHAR", true);
            return;
        }

        var conditionFeatures = new List<FeatureDefinition>();
        condition.ConditionDefinition.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(conditionFeatures);
        if (conditionFeatures.Empty())
        {
            Main.Log2($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO ACTIONS", true);
            return;
        }

        List<(FeatureDefinition feature, string origin)> features = null;
        if (character is RulesetCharacterHero hero)
        {
            features = EnumerateHeroFeatures<IAdditionalActionsProvider>(hero);
        }

        if (features == null)
        {
            Main.Log2($"AccountRemovedCondition [{character.Name}] '{condition.Name}' NO FEATURES", true);
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
            Main.Log2($"AccountRemovedCondition [{character.Name}] '{conditionFeature.Name}' from '{origin}'", true);
            Main.Log2($"AccountRemovedCondition [{character.Name}] WAS: [{string.Join(", ", list)}]", true);

            var k = -1;
            for (var i = 0; i < max; i++)
            {
                var filter = filters[i];
                var data = PerformanceFilterExtraData.GetData(filter);
                if (data == null) { continue; }

                if (data.feature != conditionFeature || data.origin != origin)
                {
                    continue;
                }

                k = i;
                break;
            }

            if (k < 0) { continue; }

            var rank = locCharacter.CurrentActionRankByType[type];
            Main.Log2($"AccountRemovedCondition [{character.Name}] remove at: {k} rank was: {rank}", true);

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

            Main.Log2($"AccountRemovedCondition [{character.Name}] BECAME: [{string.Join(", ", list)}]", true);

            SaveIndexes(locCharacter.UsedSpecialFeatures, type, list);
        }
    }
}
