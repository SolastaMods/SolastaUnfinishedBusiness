using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public class ProficienciesPanelPatcher
{
    [HarmonyPatch(typeof(ProficienciesPanel), "RuntimeLoaded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RuntimeLoaded_Patch
    {
        public static void Prefix(ProficienciesPanel __instance)
        {
            //PATCH: support for custom invocations
            //adds separate sub-panels for each custom invocation type
            CustomInvocationSubPanel.AddCustomSubPanels(__instance);
        }
    }

    [HarmonyPatch(typeof(ProficienciesPanel), "Unload")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Unload_Patch
    {
        public static IEnumerator Postfix(
            [NotNull] IEnumerator __result,
            ProficienciesPanel __instance)
        {
            //TODO: Don't know when this is called - was unable to trigger this method 
            //PATCH: support for custom invocations
            //unloads all custom sub-panels
            if (__instance.toggleGroup != null)
            {
                var childCount = __instance.toggleGroup.childCount;

                for (var i = (int)ProficienciesPanel.ProficiencyType.Max; i < childCount; i++)
                {
                    var proficiencyToggle =
                        __instance.toggleGroup.GetChild(i).GetComponent<CharacterInspectionToggle>();

                    proficiencyToggle.Unbind();

                    yield return __instance.subPanels[i].Unload();
                }
            }

            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
        }
    }
}
