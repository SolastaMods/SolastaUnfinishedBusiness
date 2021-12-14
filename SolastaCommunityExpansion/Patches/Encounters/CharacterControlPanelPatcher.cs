using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    // this patch allows an "enemy" hero to use the inventory inspect button
    [HarmonyPatch(typeof(CharacterControlPanel), "OnInspectCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterControlPanel_OnInspectCb
    {
        private static RulesetCharacterHero Hero { get; set; }

        internal static void Prefix(CharacterControlPanel __instance)
        {
            if (Main.Settings.EnableEnemiesControlledByPlayer &&
                __instance?.GuiCharacter?.RulesetCharacter is RulesetCharacterHero &&
                __instance.GuiCharacter.RulesetCharacterHero.Side == RuleDefinitions.Side.Enemy)
            {
                Hero = __instance.GuiCharacter.RulesetCharacterHero;
                Hero.ChangeSide(RuleDefinitions.Side.Ally);
            }
        }

        internal static void Postfix()
        {
            if (Hero != null)
            {
                Hero.ChangeSide(RuleDefinitions.Side.Enemy);
                Hero = null;
            }
        }
    }
}
