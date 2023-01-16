using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ProficienciesPanelPatcher
{
    [HarmonyPatch(typeof(ProficienciesPanel), nameof(ProficienciesPanel.RuntimeLoaded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RuntimeLoaded_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ProficienciesPanel __instance)
        {
            //PATCH: support for custom invocations
            //adds separate sub-panels for each custom invocation type
            CustomInvocationSubPanel.AddCustomSubPanels(__instance);
        }
    }

    //TODO: Don't know when this is called - was unable to trigger this method 
#if false
    [HarmonyPatch(typeof(ProficienciesPanel), nameof(ProficienciesPanel.Unload))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Unload_Patch
    {
        [UsedImplicitly] public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            ProficienciesPanel __instance)
        {
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

            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }
#endif
}
