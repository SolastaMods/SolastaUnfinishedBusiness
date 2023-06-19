using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal class PerformanceFilterExtraData
{
    private const string MAIN_ATTACKS = "MAIN_ATTACKS";
    private const string BONUS_ATTACKS = "BONUS_ATTACKS";
    private static readonly Dictionary<ActionPerformanceFilter, PerformanceFilterExtraData> DataMap = new();
    private static readonly Stack<PerformanceFilterExtraData> Pool = new();

    public FeatureDefinition feature;
    private string name;
    public string origin;

    private static PerformanceFilterExtraData GetOrMakeData([NotNull] ActionPerformanceFilter filter)
    {
        if (!DataMap.ContainsKey(filter))
        {
            DataMap.Add(filter, Get());
        }

        return DataMap[filter];
    }

    private static PerformanceFilterExtraData Get()
    {
        return Pool.Empty() ? new PerformanceFilterExtraData() : Pool.Pop();
    }

    private static void Return(PerformanceFilterExtraData data)
    {
        data.Clear();
        if (Pool.Count < 30)
        {
            Pool.Push(data);
        }
    }

    internal static void ClearData(ActionPerformanceFilter filter)
    {
        if (!DataMap.TryGetValue(filter, out var data))
        {
            return;
        }

        DataMap.Remove(filter);
        Return(data);
    }

    internal static void AddData(ActionPerformanceFilter filter, FeatureDefinition feature, string origin)
    {
        var data = GetOrMakeData(filter);
        data.feature = feature;
        data.origin = origin;
        data.name = feature != null ? feature.Name : null;
    }

    private void Clear()
    {
        feature = null;
        origin = null;
        name = null;
    }

    public static PerformanceFilterExtraData GetData(ActionPerformanceFilter filter)
    {
        return DataMap.TryGetValue(filter, out var data) ? data : null;
    }

    private string Key(string type)
    {
        return $"PFD|{name}|{origin}|{type}";
    }

    public override string ToString()
    {
        return $"<{name}|{origin}>";
    }

    public void StoreAttacks(GameLocationCharacter character, ActionDefinitions.ActionType type, int? number = null)
    {
        if (type == ActionDefinitions.ActionType.Main)
        {
            Main.Log2(
                $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedMainAttacks} {ToString()}",
                true);
            character.UsedSpecialFeatures[Key(MAIN_ATTACKS)] = number ?? character.UsedMainAttacks;
        }
        else if (type == ActionDefinitions.ActionType.Bonus)
        {
            Main.Log2(
                $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedBonusAttacks} {ToString()}",
                true);
            character.UsedSpecialFeatures[Key(BONUS_ATTACKS)] = number ?? character.UsedBonusAttacks;
        }
    }

    public void LoadAttacks(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        int number;
        if (type == ActionDefinitions.ActionType.Main)
        {
            character.UsedMainAttacks = character.UsedSpecialFeatures.TryGetValue(Key(MAIN_ATTACKS), out number)
                ? number
                : 0;
            Main.Log2($"LoadAttacks [{character.Name}] type: {type} number: {character.UsedMainAttacks} {ToString()}",
                true);
        }
        else if (type == ActionDefinitions.ActionType.Bonus)
        {
            character.UsedBonusAttacks = character.UsedSpecialFeatures.TryGetValue(Key(BONUS_ATTACKS), out number)
                ? number
                : 0;
            Main.Log2($"LoadAttacks [{character.Name}] type: {type} number: {character.UsedBonusAttacks} {ToString()}",
                true);
        }
    }
}
