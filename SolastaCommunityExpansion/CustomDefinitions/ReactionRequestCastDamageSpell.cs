namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class ReactionRequestCastDamageSpell : ReactionRequestCastSpell
    {
        private string attackerName { get; set; }

        public ReactionRequestCastDamageSpell(
            CharacterActionParams actionParams,
            GameLocationCharacter attacker, bool isCanrip)
            : base("CastSpellInRetribution", actionParams)
        {
            this.attackerName = attacker.Name;
            if (!isCanrip) { this.BuildSlotSubOptions(); }
        }

        public override string SuboptionTag { get => "SlotLevel"; }

        public override string FormatDescription()
        {
            var spellName = (this.ReactionParams.RulesetEffect as RulesetEffectSpell)?.SpellDefinition.GuiPresentation
                .Title;
            return Gui.Format(Gui.Localize("Reaction/CastSpellInRetributionDescription"), this.attackerName,
                this.Character.Name,
                spellName);
        }

        public override string FormatReactDescription()
        {
            var spellName = (this.ReactionParams.RulesetEffect as RulesetEffectSpell)?.SpellDefinition.GuiPresentation
                .Title;
            return Gui.Format(Gui.Localize("Reaction/CastSpellInRetributionReactDescription"), spellName);
        }

        public override string FormatTitle()
        {
            return Gui.Localize("Reaction/CastSpellInRetributionTitle");
        }

        public override string FormatReactTitle()
        {
            return Gui.Localize("Reaction/CastSpellInRetributionReactTitle");
        }
    }
}
