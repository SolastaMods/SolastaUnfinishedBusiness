namespace SolastaCommunityExpansion.CustomUI;

public class ReactionRequestCastDamageSpell : ReactionRequestCastSpell
{
    public ReactionRequestCastDamageSpell(
        CharacterActionParams actionParams,
        GameLocationCharacter attacker, bool isCanrip)
        : base("CastSpellInRetribution", actionParams)
    {
        attackerName = attacker.Name;
        if (!isCanrip) { BuildSlotSubOptions(); }
    }

    private string attackerName { get; }

    public override string SuboptionTag => "SlotLevel";

    public override string FormatDescription()
    {
        var spellName = (ReactionParams.RulesetEffect as RulesetEffectSpell)?.SpellDefinition.GuiPresentation
            .Title;
        return Gui.Format(Gui.Localize("Reaction/&CastSpellInRetributionDescription"), attackerName,
            Character.Name,
            spellName);
    }

    public override string FormatReactDescription()
    {
        var spellName = (ReactionParams.RulesetEffect as RulesetEffectSpell)?.SpellDefinition.GuiPresentation
            .Title;
        return Gui.Format(Gui.Localize("Reaction/&CastSpellInRetributionReactDescription"), spellName);
    }

    public override string FormatTitle()
    {
        return Gui.Localize("Reaction/&CastSpellInRetributionTitle");
    }

    public override string FormatReactTitle()
    {
        return Gui.Localize("Reaction/&CastSpellInRetributionReactTitle");
    }
}
