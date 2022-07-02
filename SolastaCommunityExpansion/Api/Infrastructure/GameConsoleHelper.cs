using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class GameConsoleHelper
{
    private const string DefaultUseText = "Feedback/&ActivatePowerLine";

    public static void LogCharacterUsedPower(RulesetCharacter character, [NotNull] FeatureDefinitionPower power,
        string text = DefaultUseText, bool indent = false)
    {
        var abilityName = string.IsNullOrEmpty(power.ShortTitleOverride)
            ? power.GuiPresentation.Title
            : power.ShortTitleOverride;

        LogCharacterActivatesAbility(character, abilityName, text, indent);
    }

    public static void LogCharacterActivatesAbility([NotNull] RulesetCharacter character, string abilityName,
        string text = DefaultUseText, bool indent = false)
    {
        var console = Gui.Game.GameConsole;
        var characterName = character is RulesetCharacterHero hero ? hero.DisplayName : character.Name;
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) {Indent = indent};

        entry.AddParameter(ConsoleStyleDuplet.ParameterType.Player, characterName);
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AttackSpellPower, abilityName);
        console.AddEntry(entry);
    }

    public static void LogCharacterAffectsTarget(RulesetCharacter character, RulesetCharacter target,
        string notificationTag, bool indent = false)
    {
        var console = Gui.Game.GameConsole;
        var text = $"Feedback/&NotifyEffect{notificationTag}Line";
        var entry = new GameConsoleEntry(text, console.consoleTableDefinition) {Indent = indent};

        console.AddCharacterEntry(character, entry);
        console.AddCharacterEntry(target, entry);
        console.AddEntry(entry);
    }
}
