using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;

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
    private const string MainAttacks = "MAIN_ATTACKS";
    private const string BonusAttacks = "BONUS_ATTACKS";
    private const string SpellFlags = "SPELL_FLAGS";
    private const string DefaultSpellFlags = "DEFAULT_SPELL_FLAGS";
    private const int MainSpell = 1; //001
    private const int MainCantrip = 2; //010
    private const int BonusSpell = 4; //100

    private static readonly Dictionary<ActionPerformanceFilter, PerformanceFilterExtraData> DataMap = new();
    private static readonly Stack<PerformanceFilterExtraData> Pool = new();

    private bool _customSpellcasting;
    private string _name;

    public FeatureDefinition Feature;
    public string Origin;

    private static PerformanceFilterExtraData GetOrMakeData([NotNull] ActionPerformanceFilter filter)
    {
        if (DataMap.TryGetValue(filter, out var value))
        {
            return value;
        }

        value = Get();
        DataMap.Add(filter, value);

        return value;
    }

    private static PerformanceFilterExtraData Get()
    {
        return Pool.Count == 0 ? new PerformanceFilterExtraData() : Pool.Pop();
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

        data.Feature = feature;
        data.Origin = origin;
#pragma warning disable IDE0031
        data._name = feature != null ? feature.Name : null;
#pragma warning restore IDE0031
        data._customSpellcasting = feature != null && feature.HasSubFeatureOfType<ActionWithCustomSpellTracking>();
    }

    private void Clear()
    {
        Feature = null;
        Origin = null;
        _name = null;
    }

    public static PerformanceFilterExtraData GetData(ActionPerformanceFilter filter)
    {
        return DataMap.TryGetValue(filter, out var data) ? data : null;
    }

    private string Key(string type)
    {
        return $"PFD|{_name}|{Origin}|{type}";
    }

    public override string ToString()
    {
        return $"<{_name}|{Origin}>";
    }

    public void StoreAttacks(GameLocationCharacter character, ActionDefinitions.ActionType type, int? number = null)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (type)
        {
            case ActionDefinitions.ActionType.Main:
                // ReSharper disable once InvocationIsSkipped
                Main.Log(
                    $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedMainAttacks} {ToString()}");
                character.UsedSpecialFeatures[Key(MainAttacks)] = number ?? character.UsedMainAttacks;
                break;
            case ActionDefinitions.ActionType.Bonus:
                // ReSharper disable once InvocationIsSkipped
                Main.Log(
                    $"StoreAttacks [{character.Name}] type: {type} number: {number ?? character.UsedBonusAttacks} {ToString()}");
                character.UsedSpecialFeatures[Key(BonusAttacks)] = number ?? character.UsedBonusAttacks;
                break;
        }
    }

    public void LoadAttacks(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        int number;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (type)
        {
            case ActionDefinitions.ActionType.Main:
                character.UsedMainAttacks = character.UsedSpecialFeatures.TryGetValue(Key(MainAttacks), out number)
                    ? number
                    : 0;
                // ReSharper disable once InvocationIsSkipped
                Main.Log(
                    $"LoadAttacks [{character.Name}] type: {type} number: {character.UsedMainAttacks} {ToString()}");
                break;
            case ActionDefinitions.ActionType.Bonus:
                character.UsedBonusAttacks = character.UsedSpecialFeatures.TryGetValue(Key(BonusAttacks), out number)
                    ? number
                    : 0;
                // ReSharper disable once InvocationIsSkipped
                Main.Log(
                    $"LoadAttacks [{character.Name}] type: {type} number: {character.UsedBonusAttacks} {ToString()}");
                break;
        }
    }

    public void StoreSpellcasting(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (type != ActionDefinitions.ActionType.Main)
        {
            return;
        }

        var key = _customSpellcasting ? Key(SpellFlags) : DefaultSpellFlags;

        character.UsedSpecialFeatures[key] = (character.UsedMainSpell ? MainSpell : 0)
                                             + (character.UsedMainCantrip ? MainCantrip : 0)
                                             + (character.UsedBonusSpell ? BonusSpell : 0);
        // ReSharper disable once InvocationIsSkipped
        Main.Log(
            $"StoreSpellcasting [{character.Name}] type: {type} '{key}' flags: {character.UsedSpecialFeatures[key]} ms: {character.UsedMainSpell}, mc: {character.UsedMainCantrip}, bs: {character.UsedBonusSpell} {ToString()}");
    }

    public void LoadSpellcasting(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        if (type != ActionDefinitions.ActionType.Main)
        {
            return;
        }

        var key = _customSpellcasting ? Key(SpellFlags) : DefaultSpellFlags;

        if (!character.UsedSpecialFeatures.TryGetValue(key, out var flags))
        {
            flags = 0;
        }

        character.usedMainSpell = (flags & MainSpell) > 0;
        character.usedMainCantrip = (flags & MainCantrip) > 0;
        character.usedBonusSpell = (flags & BonusSpell) > 0;

        // ReSharper disable once InvocationIsSkipped
        Main.Log(
            $"LoadSpellcasting [{character.Name}] type: {type} '{key}' flags: {flags} ms: {character.UsedMainSpell}, mc: {character.UsedMainCantrip}, bs: {character.UsedBonusSpell} {ToString()}");
    }

    public string FormatTitle()
    {
        if (Feature == null)
        {
            return null;
        }

        var title = Feature.FormatTitle();

        if (string.IsNullOrEmpty(title) || Feature.GuiPresentation.Title == GuiPresentationBuilder.EmptyString)
        {
            title = ToString() + " (NO TITLE!)";
        }

        return Gui.Format("UI/&AdditionalActionSource", title);
    }
}
