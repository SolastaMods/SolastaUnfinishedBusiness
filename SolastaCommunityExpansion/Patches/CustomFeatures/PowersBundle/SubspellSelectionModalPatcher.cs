using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        internal static bool Prefix(
            SubspellSelectionModal __instance,
            int index,
            RulesetSpellRepertoire ___spellRepertoire,
            SpellDefinition ___masterSpell,
            SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged,
            int ___slotLevel,
            UsableDeviceFunctionBox.DeviceFunctionEngagedHandler ___deviceFunctionEngaged,
            GuiCharacter ___guiCharacter,
            RulesetItemDevice ___rulesetItemDevice,
            RulesetDeviceFunction ___rulesetDeviceFunction
            )
        {
            if (!Main.Settings.EnablePowersBundlePatch)
            {
                return true;
            }

            var masterPower = PowerBundleContext.GetPower(___masterSpell);

            if (masterPower == null)
            {
                return true;
            }

            if (___spellCastEngaged != null)
            {
                ___spellCastEngaged(___spellRepertoire, ___spellRepertoire.KnownSpells[index], ___slotLevel);
            }
            else
            {
                ___deviceFunctionEngaged?.Invoke(
                    ___guiCharacter,
                    ___rulesetItemDevice,
                    ___rulesetDeviceFunction,
                    0, index
                );
            }

            __instance.Hide();
            return false;
        }
    }
}
