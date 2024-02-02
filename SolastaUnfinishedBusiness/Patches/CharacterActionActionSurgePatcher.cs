using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionActionSurgePatcher
{
    [HarmonyPatch(typeof(CharacterActionActionSurge), nameof(CharacterActionActionSurge.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] CharacterActionActionSurge __instance)
        {
            //PATCH: support for action switching
            yield return Process(__instance, values);
        }

        private static IEnumerator Process(CharacterAction action, IEnumerator values)
        {
            if (!Main.Settings.EnableActionSwitching)
            {
                yield return values;
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.PowerNoCost];
            //directly instantiate UsePower action instead of using CharacterAction.InstantiateAction - that one seems to fail here for some reason
            action.ResultingActions.Add(new CharacterActionUsePower(actionParams));
        }
    }
}
