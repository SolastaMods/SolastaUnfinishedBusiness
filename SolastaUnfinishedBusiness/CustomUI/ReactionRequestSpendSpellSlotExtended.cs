using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses.Builders;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class ReactionRequestSpendSpellSlotExtended : ReactionRequest
{
    private readonly GuiCharacter _guiCharacter;

    internal ReactionRequestSpendSpellSlotExtended(CharacterActionParams actionParams)
        : base("SpendSpellSlot", actionParams)
    {
        SubOptionsAvailability.Clear();

        var hero = actionParams.ActingCharacter.RulesetCharacter.GetOriginalHero();
        var spellRepertoire = ReactionParams.SpellRepertoire;
        int selected;

        if (actionParams.StringParameter == InvocationsBuilders.EldritchSmiteTag)
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
                DatabaseHelper.GetDefinition<ReactionDefinition>(DefinitionName).GuiPresentation
                    .Description, ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatReactDescription()
    {
        return Gui.Format(
            string.Format(
                DatabaseHelper.GetDefinition<ReactionDefinition>(DefinitionName).ReactDescription,
                ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatReactTitle()
    {
        return Gui.Format(
            string.Format(
                DatabaseHelper.GetDefinition<ReactionDefinition>(DefinitionName).ReactTitle,
                ReactionParams.StringParameter), _guiCharacter.Name);
    }

    public override string FormatTitle()
    {
        return Gui.Localize(string.Format(
            DatabaseHelper.GetDefinition<ReactionDefinition>(DefinitionName).GuiPresentation.Title,
            ReactionParams.StringParameter));
    }

    public override void SelectSubOption(int option)
    {
        ReactionParams.IntParameter = SubOptionsAvailability.Keys.ToList()[option];
    }
}
