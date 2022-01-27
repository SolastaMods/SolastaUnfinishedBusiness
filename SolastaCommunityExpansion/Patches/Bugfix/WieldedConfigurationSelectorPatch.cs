using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    /// <summary>
    /// Issue: WieldedConfigurationSelector.Bind passes character=null to mainHandSlotBox.Bind and offHandSlotBox.Bind
    /// </summary>
    [HarmonyPatch(typeof(WieldedConfigurationSelector), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WieldedConfigurationSelector_Bind
    {
        private static GuiCharacter _character;

        public static void Prefix(GuiCharacter character)
        {
            if (!Main.Settings.BugFixWieldedConfigurationSelector)
            {
                return;
            }

            // grab the character being passed to WieldedConfigurationSelector.Bind
            _character = character;
        }

        public static void Postfix()
        {
            // release
            _character = null;
        }

        internal static GuiCharacter GuiCharacter => _character;
    }

    [HarmonyPatch(typeof(InventorySlotBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventorySlotBox_Bind
    {
        public static void Prefix(ref GuiCharacter guiCharacter)
        {
            // If the character being passed in to the InventorySlotBox is null,
            // and we're in WieldedConfigurationSelector.Bind then use the character from there.
            if (guiCharacter == null)
            {
                guiCharacter = WieldedConfigurationSelector_Bind.GuiCharacter;
            }
        }
    }
}
