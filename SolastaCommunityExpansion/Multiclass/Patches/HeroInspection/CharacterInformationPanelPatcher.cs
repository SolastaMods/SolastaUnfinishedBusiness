using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class CharacterInformationPanelPatcher
    {
        // overrides the selected class search term with the one determined by the hotkeys / enumerate class badges logic
        [HarmonyPatch(typeof(CharacterInformationPanel), "Refresh")]
        internal static class CharacterInformationPanelRefresh
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var containsMethod = typeof(string).GetMethod("Contains");
                var getSelectedClassSearchTermMethod = typeof(Models.InspectionPanelContext).GetMethod("GetSelectedClassSearchTerm");
                var enumerateClassBadgesMethod = typeof(CharacterInformationPanel).GetMethod("EnumerateClassBadges", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var myEnumerateClassBadgesMethod = typeof(Models.InspectionPanelContext).GetMethod("EnumerateClassBadges");
                var found = 0;

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(containsMethod))
                    {
                        found++;

                        if (found == 2 || found == 3)
                        {
                            yield return new CodeInstruction(OpCodes.Call, getSelectedClassSearchTermMethod);
                        }

                        yield return instruction;
                    }
                    else if (instruction.Calls(enumerateClassBadgesMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myEnumerateClassBadgesMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
