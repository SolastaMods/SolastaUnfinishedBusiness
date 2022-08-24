using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle;

[HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SubspellSelectionModal_OnActivate
{
    internal static bool Prefix(SubspellSelectionModal __instance, int index)
    {
        if (Main.Settings.EnableUpcastConjureElementalAndFey
            && SubspellSelectionModal_Bind.FilteredSubspells is {Count: > 0})
        {
            var subspells = SubspellSelectionModal_Bind.FilteredSubspells;

            if (subspells.Count > index)
            {
                __instance.spellCastEngaged?.Invoke(__instance.spellRepertoire,
                    SubspellSelectionModal_Bind.FilteredSubspells[index], __instance.slotLevel);

                // If a device had the summon function, implement here

                //else if (this.deviceFunctionEngaged != null)
                //    this.deviceFunctionEngaged(this.guiCharacter, this.rulesetItemDevice, this.rulesetDeviceFunction, 0, index);

                __instance.Hide();

                subspells.Clear();

                return false;
            }
        }

        var masterPower = PowerBundleContext.GetPower(__instance.masterSpell);

        if (masterPower == null)
        {
            return true;
        }

        if (__instance.spellCastEngaged != null)
        {
            __instance.spellCastEngaged(__instance.spellRepertoire,
                __instance.spellRepertoire.KnownSpells[index], __instance.slotLevel);
        }
        else
        {
            __instance.deviceFunctionEngaged?.Invoke(
                __instance.guiCharacter,
                __instance.rulesetItemDevice,
                __instance.rulesetDeviceFunction,
                0, index
            );
        }

        __instance.Hide();

        return false;
    }
}
