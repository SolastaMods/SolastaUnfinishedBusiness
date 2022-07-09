using System.Linq;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomUI;

public sealed class ReactionRequestSpendSpellSlotExtended : ReactionRequest
{
    private readonly GuiCharacter _guiCharacter;

    public ReactionRequestSpendSpellSlotExtended(CharacterActionParams actionParams)
        : base("SpendSpellSlot", actionParams)
    {
        SubOptionsAvailability.Clear();

        var hero = actionParams.ActingCharacter.RulesetCharacter as RulesetCharacterHero;
        var spellRepertoire = ReactionParams.SpellRepertoire;
        int selected;

        if (actionParams.StringParameter == "EldritchSmite")
        {
            var minLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

            selected = MulticlassGameUiContext.AddAvailableSubLevels(SubOptionsAvailability, hero, spellRepertoire,
                minLevel, minLevel);
        }
        else
        {
            selected = MulticlassGameUiContext.AddAvailableSubLevels(SubOptionsAvailability, hero, spellRepertoire);
        }

        if (selected >= 0)
        {
            SelectSubOption(selected);
        }

        _guiCharacter = new GuiCharacter(Character);
    }

    public override int SelectedSubOption =>
        SubOptionsAvailability.Keys.ToList().FindIndex(v => v == ReactionParams.IntParameter);

    public override string SuboptionTag => ReactionParams.StringParameter;

    public override string FormatDescription()
    {
        return Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).GuiPresentation
                    .Description, ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatReactDescription()
    {
        return Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).ReactDescription,
                ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatReactTitle()
    {
        return Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).ReactTitle,
                ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatTitle()
    {
        return Gui.Localize(string.Format(
            DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).GuiPresentation.Title,
            ReactionParams.StringParameter));
    }

    public override void SelectSubOption(int option)
    {
        Main.Logger.Log(
            $"SelectSubOption: character: '{ReactionParams.ActingCharacter?.Name}', option: {option}, items: [{string.Join(", ", SubOptionsAvailability.Select(e => $"{e.Key}:{e.Value}"))}]");
        ReactionParams.IntParameter = SubOptionsAvailability.Keys.ToList()[option];
    }
}
