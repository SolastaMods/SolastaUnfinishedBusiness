using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.VariableReplacement;

[HarmonyPatch(typeof(FunctorDisplayLore), "Execute")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FunctorDisplayLore_Execute
{
    internal static void Prefix(FunctorParametersDescription functorParameters)
    {
        functorParameters.stringParameter = DungeonMakerContext.ReplaceVariable(functorParameters.stringParameter);
    }
}
