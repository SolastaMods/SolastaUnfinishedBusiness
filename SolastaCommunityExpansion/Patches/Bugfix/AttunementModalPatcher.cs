using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(AttunementModal), "DoAttuneItemAndSwap")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttunementModal_DoAttuneItemAndSwap
    {
        public static bool Prefix(AttunementModal __instance, ref RulesetItem ___itemToAttune, ref RulesetItem ___itemToFadeIn)
        {
            if (!Main.Settings.BugFixAttunementUnknownCharacter)
            {
                return true;
            }

            // This is a work around for data corruption introduced by the CE character export feature.
            // Plus TA code shouldn't throw null-ref.

            var method = AccessTools.Method(typeof(AttunementModal), "FindAttunedCharacterOfItem");
            var attunedCharacterOfItem = (GuiCharacter)method.Invoke(__instance, new object[] { ___itemToAttune });
            Trace.AssertNotNull(attunedCharacterOfItem);
            attunedCharacterOfItem?.RulesetCharacterHero.UnattuneItem(___itemToAttune);

            ServiceRepository.GetService<IGameService>();
            ServiceRepository.GetService<IInventoryCommandService>().AttuneItem(__instance.AttuningCharacter.RulesetCharacterHero, ___itemToAttune, true);
            ___itemToFadeIn = ___itemToAttune;
            ___itemToAttune = null;

            return false;
        }
    }
}
