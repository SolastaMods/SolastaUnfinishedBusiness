using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class GameConsoleHelper
{
    private const string DefaultUseText = "Feedback/&ActivatePowerLine";
    private const string TriggerFeature = "Feedback/&TriggerFeatureLine";

    internal static void LogCharacterUsedPower(
        [NotNull] RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power,
        string text = DefaultUseText,
        bool indent = false,
        params (ConsoleStyleDuplet.ParameterType type, string value)[] extra)
    {
        var abilityName = string.IsNullOrEmpty(power.ShortTitleOverride)
            ? power.GuiPresentation.Title
            : power.ShortTitleOverride;

        LogCharacterActivatesAbility(character, abilityName, text, indent, power.Name, "PowerDefinition", extra);
    }

    internal static void LogCharacterUsedFeature(
        [NotNull] RulesetCharacter character,
        [NotNull] FeatureDefinition feature,
        string text = TriggerFeature,
        bool indent = false,
        params (ConsoleStyleDuplet.ParameterType type, string value)[] extra)
    {
        var abilityName = feature.GuiPresentation.Title;

        LogCharacterActivatesAbility(character, abilityName, text, indent, feature.FormatDescription(), extra: extra);
    }

    internal static void LogCharacterActivatesAbility(
        [NotNull] RulesetCharacter character,
        string abilityName,
        string text = DefaultUseText,
        bool indent = false,
        string tooltipContent = null,
        string tooltipClass = null,
        params (ConsoleStyleDuplet.ParameterType type, string value)[] extra)
    {
        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) { Indent = indent };

        console.AddCharacterEntry(character, entry);
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AttackSpellPower, abilityName,
            tooltipContent: tooltipContent, tooltipClass: tooltipClass);
        foreach (var (type, value) in extra)
        {
            entry.AddParameter(type, value);
        }

        console.AddEntry(entry);
    }

    internal static void LogCharacterAffectsTarget(
        [NotNull] RulesetCharacter character,
        [NotNull] RulesetCharacter target,
        string notificationTag,
        bool indent = false)
    {
        var console = Gui.Game.GameConsole;
        var text = $"Feedback/&NotifyEffect{notificationTag}Line";
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) { Indent = indent };

        console.AddCharacterEntry(character, entry);
        console.AddCharacterEntry(target, entry);
        console.AddEntry(entry);
    }
    
    internal static void LogCharacterAffectsTarget(
        [NotNull] RulesetCharacter character,
        [NotNull] RulesetCharacter target,
        string abilityName,
        string text = DefaultUseText,
        bool indent = false,
        string tooltipContent = null,
        string tooltipClass = null,
        params (ConsoleStyleDuplet.ParameterType type, string value)[] extra)
    {
        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) { Indent = indent };

        console.AddCharacterEntry(character, entry);
        console.AddCharacterEntry(target, entry);
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AttackSpellPower, abilityName,
            tooltipContent: tooltipContent, tooltipClass: tooltipClass);
        foreach (var (type, value) in extra)
        {
            entry.AddParameter(type, value);
        }

        console.AddEntry(entry);
    }

    internal static void LogCharacterAffectedByCondition(
        [NotNull] RulesetCharacter character,
        [NotNull] ConditionDefinition condition)
    {
        var console = Gui.Game.GameConsole;
        var text = condition.Possessive ? GameConsole.ConditionAddedHasLine : GameConsole.ConditionAddedLine;
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) {Indent = true};

        ConsoleStyleDuplet.ParameterType type;
        switch (condition.ConditionType)
        {
            case RuleDefinitions.ConditionType.Beneficial:
                type = ConsoleStyleDuplet.ParameterType.Positive;
                break;
            case RuleDefinitions.ConditionType.Detrimental:
                type = ConsoleStyleDuplet.ParameterType.Negative;
                break;
            default:
                type = ConsoleStyleDuplet.ParameterType.AbilityInfo;
                break;
        }

        console.AddCharacterEntry(character, entry);
        entry.AddParameter(type, condition.FormatTitle(), tooltipContent: condition.FormatDescription());

        console.AddEntry(entry);
    }

    internal static void LogCharacterConversationLine(string character, string line, bool npc)
    {
        var console = Gui.Game.GameConsole;
        var type = npc ? ConsoleStyleDuplet.ParameterType.Enemy : ConsoleStyleDuplet.ParameterType.Player;
        var entry = new GameConsoleEntry($"{{0}}: {line}", console.consoleTableDefinition,
            baseType: ConsoleStyleDuplet.ParameterType.Banter);

        entry.AddParameter(type, character);
        console.AddEntry(entry);
    }
}
