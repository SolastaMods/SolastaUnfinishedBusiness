using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle
{
    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsablePowerBox_OnActivateCb
    {
        internal static bool Prefix(
            UsablePowerBox __instance,
            RulesetUsablePower ___usablePower,
            UsablePowerBox.PowerEngagedHandler ___powerEngaged,
            RulesetCharacter ___activator)
        {
            if (!Main.Settings.EnablePowersBundlePatch)
            {
                return true;
            }

            var masterPower = ___usablePower.PowerDefinition;

            if (PowerBundleContext.GetBundle(masterPower) == null)
            {
                return true;
            }

            if (___powerEngaged == null)
            {
                return true;
            }

            var masterSpell = PowerBundleContext.GetSpell(masterPower);
            var repertoire = new RulesetSpellRepertoire();
            var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
            var handler = new SpellsByLevelBox.SpellCastEngagedHandler(
                (spellRepertoire, spell, slotLevel) => PowerEngagedHandler(__instance, spell));

            repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

            subspellSelectionModalScreen.Bind(masterSpell, ___activator, repertoire, handler, 0, __instance.RectTransform);
            subspellSelectionModalScreen.Show();

            return false;
        }

        private static void PowerEngagedHandler(UsablePowerBox box, SpellDefinition spell)
        {
            var power = PowerBundleContext.GetPower(spell);
            var engagedHandler = box.GetField<UsablePowerBox.PowerEngagedHandler>("powerEngaged");
            var activator = box.GetField<RulesetCharacter>("activator");

            engagedHandler(UsablePowersProvider.Get(power, activator));
        }
    }
}
