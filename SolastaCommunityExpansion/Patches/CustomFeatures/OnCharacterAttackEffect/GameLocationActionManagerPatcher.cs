using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.OnCharacterAttackEffect
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationActionManager), "ExecuteActionAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationActionManager_ExecuteActionAsync
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            CharacterAction action)
        {
            Main.Logger.Log(action.ActionDefinition.Name);
            Global.CurrentAction = action;

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            Global.CurrentAction = null;
        }
    }
}
