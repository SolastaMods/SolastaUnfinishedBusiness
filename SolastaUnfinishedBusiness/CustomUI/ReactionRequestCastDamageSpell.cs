namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestCastDamageSpell : ReactionRequestCastSpell
{
    public ReactionRequestCastDamageSpell(
        CharacterActionParams actionParams,
        GameLocationCharacter attacker, bool isCantrip)
        : base("CastSpellInRetribution", actionParams)
    {
        AttackerName = attacker.Name;
        if (!isCantrip) { BuildSlotSubOptions(); }
    }

    private string AttackerName { get; }

    public override string SuboptionTag => "SlotLevel";

    public override string FormatDescription()
    {
        var spellName = (ReactionParams.RulesetEffect as RulesetEffectSpell)?
            .SpellDefinition.GuiPresentation.Title;

        return Gui.Format(
            Gui.Localize("Reaction/&CastSpellInRetributionDescription"),
            AttackerName,
            Character.Name,
            spellName);
    }

    public override string FormatReactDescription()
    {
        var spellName = (ReactionParams.RulesetEffect as RulesetEffectSpell)?
            .SpellDefinition.GuiPresentation.Title;

        return Gui.Format("Reaction/&CastSpellInRetributionReactDescription", spellName);
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
