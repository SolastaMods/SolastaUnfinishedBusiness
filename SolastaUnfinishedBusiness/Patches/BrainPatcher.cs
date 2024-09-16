using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using TA.AI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class BrainPatcher
{
    //BUGFIX: potential null exception on vanilla code
    [HarmonyPatch(typeof(Brain), nameof(Brain.UpdateContextElementsIfNecessary))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateContextElementsIfNecessary_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(out IEnumerator __result, Brain __instance, ContextType contextType)
        {
            __result = UpdateContextElementsIfNecessary(__instance, contextType);

            return false;
        }

        private static IEnumerator UpdateContextElementsIfNecessary(Brain brain, ContextType contextType)
        {
            List<DecisionContext> decisionContexts = [];

            //BEGIN PATCH
            //moved this off the IF block to ensure ELSE block won't fail if contexts doesn't have context type
            //END PATCH

            brain.contexts.TryAdd(contextType, decisionContexts);

            if (!brain.contextUpToDateStatus.TryGetValue(contextType, out var isUpToDate))
            {
                brain.contextUpToDateStatus.Add(contextType, true);
            }

            decisionContexts = brain.contexts[contextType];

            if (isUpToDate)
            {
                yield break;
            }

            decisionContexts.Clear();

            yield return brain.BuildContextElements(contextType, decisionContexts);

            brain.contextUpToDateStatus[contextType] = true;
        }
    }
}
