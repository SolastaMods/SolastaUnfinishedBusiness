using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeSequenceBuilderPatcher
{
    //PATCH: Ensure all roles are filled if party has less than 4 heroes
    [HarmonyPatch(typeof(NarrativeSequenceBuilder), nameof(NarrativeSequenceBuilder.BuildBaseSequence))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildBaseSequence_Patch
    {
        [UsedImplicitly]
        public static void Prefix(NarrativeSequence narrativeSequence)
        {
            var top = narrativeSequence.PlayerActors.Count - 1;
            var i = 0;

            // this should never happen on custom campaigns
            if (top < 0)
            {
                return;
            }

            while (narrativeSequence.PlayerActors.Count < ToolsContext.GamePartySize)
            {
                narrativeSequence.PlayerActors.Add(narrativeSequence.PlayerActors.ElementAt(i));

                // cannot use mod if by any chance denominator equals 1
                i = i == top ? 0 : i + 1;
            }
        }
    }
}
