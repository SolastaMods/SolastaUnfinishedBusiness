namespace SolastaCommunityExpansion.PatchCode.CustomUI;

internal static class Tooltips
{
    
    internal static void AddContextToRecoveredFeature(RecoveredFeatureItem item, RulesetCharacterHero character)
    {
        item.GuiTooltip.Context = character;
    }
    
    internal static void AddContextToPowerBoxTooltip(UsablePowerBox box)
    {
        CharacterControlPanel panel = box.GetComponentInParent<CharacterControlPanelExploration>();
        panel ??= box.GetComponentInParent<CharacterControlPanelBattle>();

        if (panel != null)
        {
            box.GuiTooltip.Context = panel.GuiCharacter?.RulesetCharacter;
        }
    }
}