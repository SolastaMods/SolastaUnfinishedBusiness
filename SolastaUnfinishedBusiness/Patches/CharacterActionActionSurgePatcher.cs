using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionActionSurgePatcher
{
    //BUGFIX: use a power no cost on execution for better interaction with replace attack with cantrips
    [HarmonyPatch(typeof(CharacterActionActionSurge), nameof(CharacterActionActionSurge.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionActionSurge __instance)
        {
            __result = Process(__instance);

            return false;
        }

        private static IEnumerator Process(CharacterAction action)
        {
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.PowerNoCost];
            //directly instantiate UsePower action instead of using CharacterAction.InstantiateAction - that one seems to fail here for some reason
            action.ResultingActions.Add(new CharacterActionUsePower(actionParams));

            yield break;
        }
    }
}
