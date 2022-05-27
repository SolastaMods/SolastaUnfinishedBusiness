using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey;

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
            if (Main.Settings.EnableUpcastConjureElementalAndFey
                && SubspellSelectionModal_Bind.FilteredSubspells != null
                && SubspellSelectionModal_Bind.FilteredSubspells.Count > 0)
            {
                var subspells = SubspellSelectionModal_Bind.FilteredSubspells;

                if (subspells.Count > index)
                {
                    ___spellCastEngaged?.Invoke(___spellRepertoire,
                        SubspellSelectionModal_Bind.FilteredSubspells[index], ___slotLevel);

                    // If a device had the summon function, implement here

                    //else if (this.deviceFunctionEngaged != null)
                    //    this.deviceFunctionEngaged(this.guiCharacter, this.rulesetItemDevice, this.rulesetDeviceFunction, 0, index);

                    __instance.Hide();

                    subspells.Clear();

                    return false;
                }
            }

            if (Main.Settings.EnablePowersBundlePatch)
            {
                var masterPower = PowerBundleContext.GetPower(___masterSpell);

                if (masterPower != null)
                {
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

            return true;
        }
    }
}
