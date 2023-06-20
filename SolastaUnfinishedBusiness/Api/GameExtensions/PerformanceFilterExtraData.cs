using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal class ActionWithCustomSpellTracking
{
    private ActionWithCustomSpellTracking()
    {
    }

    public static ActionWithCustomSpellTracking Mark { get; } = new();
}

internal class PerformanceFilterExtraData
{
    private const string MAIN_ATTACKS = "MAIN_ATTACKS";
    private const string BONUS_ATTACKS = "BONUS_ATTACKS";
    private const string SPELL_FLAGS = "SPELL_FLAGS";
    private const string DEFAULT_SPELL_FLAGS = "DEFAULT_SPELL_FLAGS";
    private const int MAIN_SPELL = 1; //001
    private const int MAIN_CANTRIP = 2; //010
    private const int BONUS_SPELL = 4; //100

    private static readonly Dictionary<ActionPerformanceFilter, PerformanceFilterExtraData> DataMap = new();
    private static readonly Stack<PerformanceFilterExtraData> Pool = new();

    public FeatureDefinition feature;
    private string name;
    public string origin;
    private bool customSpellcasting;

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
        data.customSpellcasting = feature != null && feature.HasSubFeatureOfType<ActionWithCustomSpellTracking>();
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
            Main.Info(
                $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedMainAttacks} {ToString()}");
            character.UsedSpecialFeatures[Key(MAIN_ATTACKS)] = number ?? character.UsedMainAttacks;
        }
        else if (type == ActionDefinitions.ActionType.Bonus)
        {
            Main.Info(
                $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedBonusAttacks} {ToString()}");
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
            Main.Info($"LoadAttacks [{character.Name}] type: {type} number: {character.UsedMainAttacks} {ToString()}");
        }
        else if (type == ActionDefinitions.ActionType.Bonus)
        {
            character.UsedBonusAttacks = character.UsedSpecialFeatures.TryGetValue(Key(BONUS_ATTACKS), out number)
                ? number
                : 0;
            Main.Info($"LoadAttacks [{character.Name}] type: {type} number: {character.UsedBonusAttacks} {ToString()}");
        }
    }

    public void StoreSpellcasting(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (type != ActionDefinitions.ActionType.Main)
        {
            return;
        }

        var key = customSpellcasting ? Key(SPELL_FLAGS) : DEFAULT_SPELL_FLAGS;
        character.UsedSpecialFeatures[key] = (character.UsedMainSpell ? MAIN_SPELL : 0)
                                             + (character.UsedMainCantrip ? MAIN_CANTRIP : 0)
                                             + (character.UsedBonusSpell ? BONUS_SPELL : 0);
        Main.Info(
            $"StoreSpellcasting [{character.Name}] type: {type} '{key}' flags: {character.UsedSpecialFeatures[key]} ms: {character.UsedMainSpell}, mc: {character.UsedMainCantrip}, bs: {character.UsedBonusSpell} {ToString()}");
    }

    public void LoadSpellcasting(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (type != ActionDefinitions.ActionType.Main)
        {
            return;
        }

        var key = customSpellcasting ? Key(SPELL_FLAGS) : DEFAULT_SPELL_FLAGS;
        if (!character.UsedSpecialFeatures.TryGetValue(key, out var flags))
        {
            flags = 0;
        }

        character.usedMainSpell = (flags & MAIN_SPELL) > 0;
        character.usedMainCantrip = (flags & MAIN_CANTRIP) > 0;
        character.usedBonusSpell = (flags & BONUS_SPELL) > 0;

        Main.Info(
            $"LoadSpellcasting [{character.Name}] type: {type} '{key}' flags: {flags} ms: {character.UsedMainSpell}, mc: {character.UsedMainCantrip}, bs: {character.UsedBonusSpell} {ToString()}");
    }

    public string FormatTitle()
    {
        if (feature == null)
        {
            return null;
        }

        var title = feature.FormatTitle();
        if (string.IsNullOrEmpty(title))
        {
            title = feature.Name + " (NO TITLE!)";
        }

        return Gui.Format("UI/&AdditionalActionSource", title);
    }
}
