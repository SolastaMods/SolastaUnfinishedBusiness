using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.BehaviorsGeneric;

namespace SolastaUnfinishedBusiness.Patches;

public static class MetamagicSelectionPanelPatcher
{
    [HarmonyPatch(typeof(MetamagicSelectionPanel), nameof(MetamagicSelectionPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActions_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for `ReplaceMetamagicOption`
            return ReplaceMetamagicOption.PatchMetamagicGetter(instructions, "MetamagicSelectionPanel.Bind");
        }
    }
}
