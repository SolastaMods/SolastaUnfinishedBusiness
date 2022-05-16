using System.Linq;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomUI
{
    public class ReactionRequestSpendSpellSlotExtended : ReactionRequest
    {
        private readonly GuiCharacter _guiCharacter;

        public ReactionRequestSpendSpellSlotExtended(CharacterActionParams actionParams)
            : base("SpendSpellSlot", actionParams)
        {
            SubOptionsAvailability.Clear();

            var hero = actionParams.ActingCharacter.RulesetCharacter as RulesetCharacterHero;
            var spellRepertoire = ReactionParams.SpellRepertoire;

            var selected = MulticlassGameUiContext.AddAvailableSubLevels(SubOptionsAvailability, hero, spellRepertoire, 1);
            if (selected >= 0)
            {
                SelectSubOption(selected);
            }
            _guiCharacter = new GuiCharacter(Character);
        }

        public override int SelectedSubOption => SubOptionsAvailability.Keys.ToList().Find(v => v == ReactionParams.IntParameter);

        public override string SuboptionTag => ReactionParams.StringParameter;

        public override void SelectSubOption(int option) => ReactionParams.IntParameter = SubOptionsAvailability.Keys.ToList()[option];

        public override string FormatTitle() => Gui.Localize(string.Format(
            DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).GuiPresentation.Title,
            ReactionParams.StringParameter));

        public override string FormatDescription() => Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).GuiPresentation
                    .Description, ReactionParams.StringParameter), _guiCharacter.Name);

        public override string FormatReactTitle() => Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).ReactTitle,
                ReactionParams.StringParameter), _guiCharacter.Name);

        public override string FormatReactDescription() => Gui.Format(
            string.Format(
                DatabaseRepository.GetDatabase<ReactionDefinition>().GetElement(DefinitionName).ReactDescription,
                ReactionParams.StringParameter), _guiCharacter.Name);
    }
}
