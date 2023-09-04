using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class GameConsoleHelper
{
    private const string DefaultUseText = "Feedback/&ActivatePowerLine";
    private const string TriggerFeature = "Feedback/&TriggerFeatureLine";

    internal static void LogCharacterUsedPower(
        [NotNull] this RulesetCharacter character,
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
        [NotNull] this RulesetCharacter character,
        [NotNull] BaseDefinition feature,
        string text = TriggerFeature,
        bool indent = false,
        params (ConsoleStyleDuplet.ParameterType type, string value)[] extra)
    {
        var abilityName = feature.GuiPresentation.Title;

        LogCharacterActivatesAbility(character, abilityName, text, indent, feature.FormatDescription(), extra: extra);
    }

    internal static void LogCharacterActivatesAbility(
        [NotNull] this RulesetCharacter character,
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
        [NotNull] this RulesetCharacter character,
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
        [NotNull] this RulesetCharacter character,
        [NotNull] ConditionDefinition condition)
    {
        var console = Gui.Game.GameConsole;
        var text = condition.Possessive ? GameConsole.ConditionAddedHasLine : GameConsole.ConditionAddedLine;
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) { Indent = true };

        var type = condition.ConditionType switch
        {
            RuleDefinitions.ConditionType.Beneficial => ConsoleStyleDuplet.ParameterType.Positive,
            RuleDefinitions.ConditionType.Detrimental => ConsoleStyleDuplet.ParameterType.Negative,
            _ => ConsoleStyleDuplet.ParameterType.AbilityInfo
        };

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

    internal static void LogConcentrationCheckRoll(this RulesetCharacter character,
        RulesetEffect effect,
        RuleDefinitions.RollOutcome outcome,
        int totalRoll,
        int rawRoll,
        int modifier,
        int saveDC,
        List<RuleDefinitions.TrendInfo> modifierTrends,
        List<RuleDefinitions.TrendInfo> advantageTrends)
    {
        const string RolledAnyAdvantage = "Feedback/&ConcentrationEffectCheckRolledAnyAdvantageLine";
        const string Rolled = "Feedback/&ConcentrationEffectCheckRolledLine";

        var advantage = RuleDefinitions.ComputeAdvantage(advantageTrends);
        var hasAnyAdvantage = advantage != RuleDefinitions.AdvantageType.None;

        var console = Gui.Game.GameConsole;
        var entry =
            new GameConsoleEntry(hasAnyAdvantage ? RolledAnyAdvantage : Rolled, console.consoleTableDefinition)
            {
                Indent = true
            };

        console.AddCharacterEntry(character, entry);
        var gui = effect.SourceDefinition.GuiPresentation;
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AttackSpellPower, gui.Title,
            tooltipContent: gui.Description);

        if (hasAnyAdvantage)
        {
            var type = advantage == RuleDefinitions.AdvantageType.Advantage
                ? ConsoleStyleDuplet.ParameterType.Advantage
                : ConsoleStyleDuplet.ParameterType.Disadvantage;
            var localizedAdvantage = Gui.Localize(GameConsole.AdvantageRollDescription);
            var localizedTrends = Gui.FormatAdvantageTrends(advantageTrends);

            entry.AddParameter(type, GameConsole.AdvantageRollOutcome,
                tooltipContent: $"{localizedAdvantage}\n{localizedTrends}");
        }

        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo, saveDC.ToString());
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.Base, $"{rawRoll}{modifier:+0;-#}",
            tooltipContent: Gui.FormatSavingThrowTrends(modifier, modifierTrends));

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (outcome)
        {
            case RuleDefinitions.RollOutcome.CriticalSuccess:
            case RuleDefinitions.RollOutcome.Success:
                entry.AddParameter(ConsoleStyleDuplet.ParameterType.SuccessfulRoll,
                    Gui.Format(GameConsole.SaveSuccessOutcome, totalRoll.ToString()));
                break;
            case RuleDefinitions.RollOutcome.CriticalFailure:
            case RuleDefinitions.RollOutcome.Failure:
                entry.AddParameter(ConsoleStyleDuplet.ParameterType.FailedRoll,
                    Gui.Format(GameConsole.SaveFailureOutcome, totalRoll.ToString()));
                break;
        }

        console.AddEntry(entry);
    }
}
