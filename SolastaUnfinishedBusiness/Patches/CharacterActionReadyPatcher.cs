using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionReadyPatcher
{
    [HarmonyPatch(typeof(CharacterActionReady), nameof(CharacterActionReady.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionReady __instance)
        {
            //PATCH: Adds support for DontEndTurnAfterReady setting 
            if (!Main.Settings.DontEndTurnAfterReady)
            {
                return true;
            }

            __result = Execute(__instance);

            return false;
        }

        private static IEnumerator Execute(CharacterActionReady action)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            action.ActingCharacter.ReadiedAction = action.readyActionType;
        }
    }
}
